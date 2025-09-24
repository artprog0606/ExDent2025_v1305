using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    /// <summary>
    /// Удаляет время у сотрудников не рабочее для филиала
    /// </summary>
    public class CorrectingReservationResourcesAdditionalTimeHandler
    {
        private readonly int _affiliateId;
        private readonly DateTime? _date;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateBy;
        private readonly TypeCorrectingReservationResourcesTimes _typeCorrecting;

        private CorrectingReservationResourcesAdditionalTimeHandler(int affiliateId, TypeCorrectingReservationResourcesTimes typeCorrecting)
        {
            _affiliateId = affiliateId;
            _typeCorrecting = typeCorrecting;
        }

        public CorrectingReservationResourcesAdditionalTimeHandler(int affiliateId, DateTime date, TypeCorrectingReservationResourcesTimes typeCorrecting) : this(affiliateId, typeCorrecting)
        {
            _date = date;
        }

        public CorrectingReservationResourcesAdditionalTimeHandler(int affiliateId, DateTime dateFrom, DateTime? dateBy, TypeCorrectingReservationResourcesTimes typeCorrecting) : this(affiliateId, typeCorrecting)
        {
            _dateFrom = dateFrom;
            _dateBy = dateBy;
        }

        public void Execute()
        {
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking = null;
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes = null;

            var reservationResources = ReservationResourceService.GetByAffiliate(_affiliateId);

            if (reservationResources.Count > 0)
            {
                dictionaryAffiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                    (!_date.HasValue
                        ? AffiliateTimeOfBookingService.GetByAffiliate(_affiliateId)
                        : AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_affiliateId, _date.Value.DayOfWeek)
                    ).GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList()));

                dictionaryAffiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                    (_date.HasValue || _dateFrom.HasValue // есть временные промежутки
                        ? _date.HasValue // указан конкретный день
                            ? AffiliateAdditionalTimeService.GetByAffiliateAndDate(_affiliateId, _date.Value)
                            : _dateBy.HasValue // указаны дата от и по
                                ? AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_affiliateId, _dateFrom.Value.Date, _dateBy.Value.Date.AddDays(1))
                                : AffiliateAdditionalTimeService.GetByAffiliateAndDateFrom(_affiliateId, _dateFrom.Value.Date)
                        : AffiliateAdditionalTimeService.GetByAffiliate(_affiliateId)
                    ).GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList()));
            }

            foreach (var reservationResource in reservationResources)
            {
                var reservationResourceBookingIntervalMinutes =
                    ReservationResourceService.GetBookingIntervalMinutesForAffiliate(_affiliateId,
                        reservationResource.Id);

                var isDefaultIntervalMinutes =
                    reservationResourceBookingIntervalMinutes == null &&
                    (_typeCorrecting == TypeCorrectingReservationResourcesTimes.All ||
                     _typeCorrecting == TypeCorrectingReservationResourcesTimes.WithDefaultInterval);

                var isIndividualIntervalMinutes =
                    reservationResourceBookingIntervalMinutes != null &&
                    (_typeCorrecting == TypeCorrectingReservationResourcesTimes.All ||
                     _typeCorrecting == TypeCorrectingReservationResourcesTimes.WithIndividualInterval);

                if (isDefaultIntervalMinutes == false && isIndividualIntervalMinutes == false)
                    continue;

                if (_date.HasValue || _dateFrom.HasValue)
                {
                    if (_date.HasValue)
                        new CorrectingReservationResourceAdditionalTimeHandler(_affiliateId, reservationResource, _date.Value,
                            dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking).Execute();
                    else
                        new CorrectingReservationResourceAdditionalTimeHandler(_affiliateId, reservationResource, _dateFrom.Value, _dateBy,
                            dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking).Execute();
                }
                else
                    new CorrectingReservationResourceAdditionalTimeHandler(_affiliateId, reservationResource, dictionaryAffiliateAdditionalTimes,
                        dictionaryAffiliateTimesOfBooking).Execute();
            }
        }
    }
}
