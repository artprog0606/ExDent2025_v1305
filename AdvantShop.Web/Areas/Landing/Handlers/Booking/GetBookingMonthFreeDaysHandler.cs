using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class GetBookingMonthFreeDaysHandler : ICommandHandler<GetBookingMonthDaysResult>
    {
        private readonly GetBookingByTimeMonthDaysDto _model;

        public GetBookingMonthFreeDaysHandler(GetBookingByTimeMonthDaysDto model)
        {
            _model = model;
        }

        public GetBookingMonthDaysResult Execute()
        {
            if (!_model.LoadPrevMonth &&
                !_model.LoadCurrentMonth &&
                !_model.LoadNextMonth)
                return null;

            var affiliate = AffiliateService.Get(_model.AffiliateId);
            if (affiliate == null)
                throw new BlException("Филиал не найден");

            var resource = ReservationResourceService.Get(_model.ResourceId);
            if (resource == null)
                throw new BlException("Ресурс не найден");

            var dateFrom = new DateTime(_model.Year, _model.Month, 1).Date;
            var dateTo = dateFrom.AddMonths(1);

            // не загружать предыдущий месяц, если он младше текущего (эти данные не понадобятся)
            if (_model.LoadPrevMonth && dateFrom.AddMonths(-1) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date)
                _model.LoadPrevMonth = false;

            if (_model.LoadPrevMonth)
                dateFrom = dateFrom.AddMonths(-1);

            if (_model.LoadNextMonth)
                dateTo = dateTo.AddMonths(1);

            var affiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_model.AffiliateId)
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var affiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_model.AffiliateId, dateFrom, dateTo)
                .GroupBy(x => x.StartTime.Date)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var resourceTimesOfBooking = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                ReservationResourceTimeOfBookingService.GetBy(_model.AffiliateId, _model.ResourceId)
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var resourceAdditionalTimes = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(_model.AffiliateId, _model.ResourceId, dateFrom, dateTo)
                .GroupBy(x => x.StartTime.Date)
                .ToDictionary(x => x.Key, x => x.ToList()));

            return GetBookingMonthDaysResult(affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, dateFrom, dateTo);
        }

        public GetBookingMonthDaysResult GetBookingMonthDaysResult(SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBooking, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> resourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> resourceTimesOfBooking, DateTime dateFrom, DateTime dateTo)
        {
            var toDay = DateTime.Today;
            return new GetBookingMonthDaysResult
            {
                CurrentMonth = !_model.LoadCurrentMonth ? null : GetWeekendDays(new DateTime(_model.Year, _model.Month, 1).Date, new DateTime(_model.Year, _model.Month, 1).AddMonths(1).Date,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking)
                    .Where(x => x >= toDay)
                    .Select(x => x.Day).ToList(),
                PrevMonth = !_model.LoadPrevMonth ? null : GetWeekendDays(dateFrom, dateFrom.AddMonths(1),
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking)
                    .Where(x => x >= toDay)
                    .Select(x => x.Day).ToList(),
                NextMonth = !_model.LoadNextMonth ? null : GetWeekendDays(dateTo.AddMonths(-1), dateTo,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking)
                    .Where(x => x >= toDay)
                    .Select(x => x.Day).ToList(),
            };
        }

        private List<DateTime> GetWeekendDays(DateTime dateFrom, DateTime dateTo,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> resourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> resourceTimesOfBooking)
        {
            var monthTimes = new List<DateTime>();
            var day = dateFrom;
            do
            {
                var isWork = true;

                if (resourceAdditionalTimes.ContainsKey(day))
                {
                    if (!resourceAdditionalTimes[day][0].IsWork)
                        isWork = false;
                }
                else if (!resourceTimesOfBooking.ContainsKey(day.DayOfWeek))
                    isWork = false;

                if (affiliateAdditionalTimes.ContainsKey(day))
                {
                    if (!affiliateAdditionalTimes[day][0].IsWork)
                        isWork = false;
                }
                else if (!affiliateTimesOfBooking.ContainsKey(day.DayOfWeek))
                    isWork = false;

                if (!isWork)
                    monthTimes.Add(day);

                day = day.AddDays(1);
            } while (day < dateTo);

            return monthTimes;
        }
    }
}
