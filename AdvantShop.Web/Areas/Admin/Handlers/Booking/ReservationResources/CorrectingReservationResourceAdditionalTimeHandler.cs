using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    /// <summary>
    /// Удаляет время у сотрудников не рабочее для филиала
    /// </summary>
    public class CorrectingReservationResourceAdditionalTimeHandler
    {
        private readonly int _affiliateId;
        private readonly ReservationResource _reservationResource;
        private SortedDictionary<DateTime, List<AffiliateAdditionalTime>> _dictionaryAffiliateAdditionalTimes;
        private SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> _dictionaryAffiliateTimesOfBooking;
        private readonly DateTime? _date;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateBy;

        #region Constructors

        private CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource)
        {
            _reservationResource = reservationResource;
            _affiliateId = affiliateId;
        }

        public CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking)
            : this(affiliateId, reservationResource)
        {
            _dictionaryAffiliateAdditionalTimes = dictionaryAffiliateAdditionalTimes;
            _dictionaryAffiliateTimesOfBooking = dictionaryAffiliateTimesOfBooking;
        }

        public CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource, DateTime date)
            : this(affiliateId, reservationResource)
        {
            _date = date;
        }

        public CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource, DateTime date,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking)
            : this(affiliateId, reservationResource)
        {
            _date = date;
            _dictionaryAffiliateAdditionalTimes = dictionaryAffiliateAdditionalTimes;
            _dictionaryAffiliateTimesOfBooking = dictionaryAffiliateTimesOfBooking;
        }

        public CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource, DateTime dateFrom, DateTime? dateBy)
            : this(affiliateId, reservationResource)
        {
            _dateFrom = dateFrom;
            _dateBy = dateBy;
        }

        public CorrectingReservationResourceAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource, DateTime dateFrom, DateTime? dateBy,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking)
            : this(affiliateId, reservationResource)
        {
            _dateFrom = dateFrom;
            _dateBy = dateBy;
            _dictionaryAffiliateAdditionalTimes = dictionaryAffiliateAdditionalTimes;
            _dictionaryAffiliateTimesOfBooking = dictionaryAffiliateTimesOfBooking;
        }

        #endregion

        public void Execute()
        {
            var dictionaryReservationResourceAdditionalTimes = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                (_date.HasValue || _dateFrom.HasValue // есть временные промежутки
                    ? _date.HasValue // указан конкретный день
                        ? ReservationResourceAdditionalTimeService.GetByDate(_affiliateId, _reservationResource.Id, _date.Value)
                        : _dateBy.HasValue // указаны дата от и по
                            ? ReservationResourceAdditionalTimeService.GetByDateFromTo(_affiliateId, _reservationResource.Id, _dateFrom.Value.Date,
                                _dateBy.Value.Date.AddDays(1))
                            : ReservationResourceAdditionalTimeService.GetByDateFrom(_affiliateId, _reservationResource.Id, _dateFrom.Value.Date)
                    : ReservationResourceAdditionalTimeService.GetBy(_affiliateId, _reservationResource.Id)
                    ).GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList()));

            if (dictionaryReservationResourceAdditionalTimes.Count > 0 &&
                (_dictionaryAffiliateAdditionalTimes == null || _dictionaryAffiliateTimesOfBooking == null))
                LoadAffiliateTimes();

            foreach (var kvReservationResourceAdditionalTimes in dictionaryReservationResourceAdditionalTimes)
            {
                var affiliateAdditionalTimes =
                    _dictionaryAffiliateAdditionalTimes.ContainsKey(kvReservationResourceAdditionalTimes.Key)
                        ? _dictionaryAffiliateAdditionalTimes[kvReservationResourceAdditionalTimes.Key]
                        : new List<AffiliateAdditionalTime>();

                var affiliateTimesOfBookingDayOfWeek =
                    _dictionaryAffiliateTimesOfBooking.ContainsKey(kvReservationResourceAdditionalTimes.Key.DayOfWeek)
                        ? _dictionaryAffiliateTimesOfBooking[kvReservationResourceAdditionalTimes.Key.DayOfWeek]
                        : new List<AffiliateTimeOfBooking>();

                /* 
                 * Правила нерабочих дней не обрабатываем, т.к. нет в этом смысла.
                 * Правило нерабочего дня всегда будет корректно,
                 * т.к. не важно работает или нет в этот день филиал.
                */
                if (kvReservationResourceAdditionalTimes.Value[0].IsWork)
                {
                    var isWork = false;

                    if (affiliateAdditionalTimes.Count > 0)
                    {
                        if (affiliateAdditionalTimes[0].IsWork)
                            isWork = true;
                    }
                    else if (affiliateTimesOfBookingDayOfWeek.Count > 0)
                        isWork = true;


                    if (isWork)
                    {
                        var reservationResourceBookingIntervalMinutes =
                            ReservationResourceService.GetBookingIntervalMinutesForAffiliate(_affiliateId,
                                _reservationResource.Id);

                        // если в этот день филиал работает корректируем данные (удаляем не рабочее время)


                        if (reservationResourceBookingIntervalMinutes == null)
                            foreach (var reservationResourceAdditionalTime in kvReservationResourceAdditionalTimes.Value)
                                if (!AffiliateService.ExistDateRangeInTimeOfBooking(
                                    reservationResourceAdditionalTime.StartTime,
                                    reservationResourceAdditionalTime.EndTime,
                                    affiliateAdditionalTimes,
                                    affiliateTimesOfBookingDayOfWeek))
                                {
                                    ReservationResourceAdditionalTimeService.Delete(reservationResourceAdditionalTime.Id);
                                }

                        if (reservationResourceBookingIntervalMinutes != null)
                            foreach (var reservationResourceAdditionalTime in kvReservationResourceAdditionalTimes.Value)
                                if (!AffiliateService.CheckDateRangeIsWork(
                                    reservationResourceAdditionalTime.StartTime,
                                    reservationResourceAdditionalTime.EndTime,
                                    _dictionaryAffiliateAdditionalTimes,
                                    _dictionaryAffiliateTimesOfBooking))
                                {
                                    ReservationResourceAdditionalTimeService.Delete(reservationResourceAdditionalTime.Id);
                                }
                    }
                    else
                        ReservationResourceAdditionalTimeService.DeleteByDate(_affiliateId, _reservationResource.Id,
                            kvReservationResourceAdditionalTimes.Key);
                }
            }
        }

        private void LoadAffiliateTimes()
        {
            _dictionaryAffiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                (!_date.HasValue
                    ? AffiliateTimeOfBookingService.GetByAffiliate(_affiliateId)
                    : AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_affiliateId, _date.Value.DayOfWeek)
                    ).GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList()));

            _dictionaryAffiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                (_date.HasValue || _dateFrom.HasValue // есть временные промежутки
                    ? _date.HasValue // указан конкретный день
                        ? AffiliateAdditionalTimeService.GetByAffiliateAndDate(_affiliateId, _date.Value)
                        : _dateBy.HasValue // указаны дата от и по
                            ? AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(
                                _affiliateId,
                                _dateFrom.Value.Date,
                                _dateBy.Value.Date.AddDays(1))
                            : AffiliateAdditionalTimeService.GetByAffiliateAndDateFrom(_affiliateId, _dateFrom.Value.Date)
                    : AffiliateAdditionalTimeService.GetByAffiliate(_affiliateId)
                    ).GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList()));
        }
    }
}
