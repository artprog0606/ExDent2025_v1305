using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetMonthFreeDaysHandler
    {
        private readonly GetMonthFreeDaysModel _model;

        public List<string> Errors { get; set; }

        public GetMonthFreeDaysHandler(GetMonthFreeDaysModel model)
        {
            _model = model;
            Errors = new List<string>();
        }


        public MonthsFreeDaysModel Execute()
        {
            if (!_model.LoadPrevMonth &&
                !_model.LoadCurrentMonth &&
                !_model.LoadNextMonth)
                return new MonthsFreeDaysModel();

            var affiliate = AffiliateService.Get(_model.AffiliateId);
            if (affiliate == null)
            {
                Errors.Add("Указанный филиал не найден");
                return null;
            }

            var dateMonthFrom = new DateTime(_model.Year, _model.Month, 1).Date;
            var dateMonthTo = dateMonthFrom.AddMonths(1);

            if (_model.LoadPrevMonth)
                dateMonthFrom = dateMonthFrom.AddMonths(-1);

            if (_model.LoadNextMonth)
                dateMonthTo = dateMonthTo.AddMonths(1);

            var dictionaryAffiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_model.AffiliateId)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList()));

            var dictionaryAffiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_model.AffiliateId, dateMonthFrom.Date, dateMonthTo.Date)
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList()));

            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking = null;
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes = null;

            if (_model.SelectedReservationResourceId.HasValue)
            {
                var reservationResource = ReservationResourceService.Get(_model.SelectedReservationResourceId.Value);

                dictionaryReservationResourceTimesOfBooking = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                    ReservationResourceTimeOfBookingService.GetBy(_model.AffiliateId, reservationResource.Id)
                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList()));

                dictionaryReservationResourceAdditionalTimes = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                    ReservationResourceAdditionalTimeService.GetByDateFromTo(_model.AffiliateId, reservationResource.Id, dateMonthFrom, dateMonthTo)
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList()));

            }

            var result = new MonthsFreeDaysModel();

            if (_model.LoadCurrentMonth)
                result.CurrentMonth = GetWeekendDays(
                    new DateTime(_model.Year, _model.Month, 1).Date,
                    new DateTime(_model.Year, _model.Month, 1).Date.AddMonths(1),
                    dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking,
                    dictionaryReservationResourceAdditionalTimes,
                    dictionaryReservationResourceTimesOfBooking).Select(x => x.Day).ToList();

            if (_model.LoadPrevMonth)
                result.PrevMonth = GetWeekendDays(
                    dateMonthFrom,
                    dateMonthFrom.AddMonths(1),
                    dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking, dictionaryReservationResourceAdditionalTimes,
                    dictionaryReservationResourceTimesOfBooking).Select(x => x.Day).ToList();

            if (_model.LoadNextMonth)
                result.NextMonth = GetWeekendDays(
                    dateMonthTo.AddMonths(-1),
                    dateMonthTo,
                    dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking, dictionaryReservationResourceAdditionalTimes,
                    dictionaryReservationResourceTimesOfBooking).Select(x => x.Day).ToList();

            return result;
        }

        private List<DateTime> GetWeekendDays(DateTime dateMonthFrom, DateTime dateMonthTo,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking)
        {
            var monthTimes = new List<DateTime>();
            var day = dateMonthFrom;
            do
            {
                var isWork = true;

                if (_model.SelectedReservationResourceId.HasValue)
                {
                    if (dictionaryReservationResourceAdditionalTimes.ContainsKey(day))
                    {
                        if (!dictionaryReservationResourceAdditionalTimes[day][0].IsWork)
                            isWork = false;
                    }
                    else if (!dictionaryReservationResourceTimesOfBooking.ContainsKey(day.DayOfWeek))
                        isWork = false;
                }


                if (dictionaryAffiliateAdditionalTimes.ContainsKey(day))
                {
                    if (!dictionaryAffiliateAdditionalTimes[day][0].IsWork)
                        isWork = false;
                }
                else if (!dictionaryAffiliateTimesOfBooking.ContainsKey(day.DayOfWeek))
                    isWork = false;

                if (!isWork)
                    monthTimes.Add(day);

                day = day.AddDays(1);
            } while (day < dateMonthTo);
            return monthTimes;
        }
    }
}
