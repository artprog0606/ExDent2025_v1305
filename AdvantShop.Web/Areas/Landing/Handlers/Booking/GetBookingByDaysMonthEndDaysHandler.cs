using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class GetBookingByDaysMonthEndDaysHandler : ICommandHandler<GetBookingMonthDaysResult>
    {
        private readonly GetBookingByDaysMonthEndDaysDto _model;
        private readonly TimeSpan oneDayTime = new TimeSpan(1, 0, 0, 0);

        public GetBookingByDaysMonthEndDaysHandler(GetBookingByDaysMonthEndDaysDto model)
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

            if (_model.TimeFrom.IsNullOrEmpty() || _model.TimeEnd.IsNullOrEmpty())
                throw new BlException("Не заданы время начала и окончания");

            TimeSpan timeFrom, timeEnd;
            if (!TimeSpan.TryParseExact(_model.TimeFrom, @"hh\:mm", CultureInfo.InvariantCulture, out timeFrom) ||
                !TimeSpan.TryParseExact(_model.TimeEnd, @"hh\:mm", CultureInfo.InvariantCulture, out timeEnd))
                throw new BlException("Не корректные время начала и окончания");

            if (_model.TimeEndAtNextDay)
                timeEnd += oneDayTime;

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

            var bookings = BookingService.GetListByDateFromToAndReservationResource(_model.AffiliateId, dateFrom, dateTo, _model.ResourceId);

            var handlerBookingMonthDays = new GetBookingMonthFreeDaysHandler(
                new GetBookingByTimeMonthDaysDto()
                {
                    AffiliateId = _model.AffiliateId,
                    ResourceId = _model.ResourceId,
                    LoadCurrentMonth = _model.LoadCurrentMonth,
                    LoadPrevMonth = _model.LoadPrevMonth,
                    LoadNextMonth = _model.LoadNextMonth,
                    Year = _model.Year,
                    Month = _model.Month,
                    SelectedServices = _model.SelectedServices
                });

            var freeDaysResult = handlerBookingMonthDays.GetBookingMonthDaysResult(affiliateAdditionalTimes,
                affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, dateFrom, dateTo);

            if (freeDaysResult.CurrentMonth != null)
                freeDaysResult.CurrentMonth = GetEndDays(freeDaysResult.CurrentMonth, new DateTime(_model.Year, _model.Month, 1), _model.StartDate, timeFrom, timeEnd,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, bookings);
            if (freeDaysResult.PrevMonth != null)
                freeDaysResult.PrevMonth = GetEndDays(freeDaysResult.PrevMonth, dateFrom, _model.StartDate, timeFrom, timeEnd,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, bookings);
            if (freeDaysResult.NextMonth != null)
                freeDaysResult.NextMonth = GetEndDays(freeDaysResult.NextMonth, dateTo.AddMonths(-1), _model.StartDate, timeFrom, timeEnd,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, bookings);

            return freeDaysResult;
        }

        private List<int> GetEndDays(List<int> weekendsOfMonth, DateTime monthDate, DateTime startDate, TimeSpan timeFrom, TimeSpan timeEnd,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> resourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> resourceTimesOfBooking,
            List<Core.Services.Booking.Booking> bookings)
        {
            var days = new List<int>();
            var toDay = DateTime.Today;
            var tempTimeEnd = new TimeSpan(timeEnd.Hours, timeEnd.Minutes, timeEnd.Seconds);
            DateTime beginDate, endDate;
            DateTime day = new DateTime(monthDate.Year, monthDate.Month, 1);

            do
            {
                if (!weekendsOfMonth.Contains(day.Day) && day >= toDay && day >= startDate.Date)
                {
                    beginDate = startDate.Date + timeFrom;
                    endDate = day + tempTimeEnd;

                    if ((endDate - beginDate) >= (timeEnd - timeFrom) &&
                        !bookings.Any(x => beginDate < x.EndDate && x.BeginDate < endDate)//!BookingService.Exist(_model.AffiliateId, _model.ResourceId, beginDate, endDate)
                        && ReservationResourceService.CheckDateRangeIsWork(beginDate, endDate,
                            resourceAdditionalTimes, resourceTimesOfBooking, affiliateAdditionalTimes, affiliateTimesOfBooking))
                        days.Add(day.Day);
                }

                day = day.AddDays(1);
            } while (day.Month == monthDate.Month);

            return days;
        }
    }
}
