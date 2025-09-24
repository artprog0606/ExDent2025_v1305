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
    public class GetBookingByDaysStartDayHandler : ICommandHandler<string>
    {
        private const int CountMonthNext = 3;
        private readonly GetBookingByDaysStartDayDto _model;

        public GetBookingByDaysStartDayHandler(GetBookingByDaysStartDayDto model)
        {
            _model = model;
        }

        public string Execute()
        {
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
                timeEnd += new TimeSpan(1, 0, 0, 0);

            var dateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
            var dateTo = dateFrom.AddMonths(CountMonthNext);

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

            for (int i = 0; i < CountMonthNext; i++)
            {
                var handlerBookingMonthStartDays = new GetBookingByDaysMonthStartDaysHandler(
                    new GetBookingByDaysMonthStartDaysDto()
                    {
                        AffiliateId = _model.AffiliateId,
                        ResourceId = _model.ResourceId,
                        LoadCurrentMonth = true,
                        LoadPrevMonth = false,
                        LoadNextMonth = false,
                        Year = dateFrom.Year,
                        Month = dateFrom.Month,
                        SelectedServices = _model.SelectedServices,
                        TimeFrom = _model.TimeFrom,
                        TimeEnd = _model.TimeEnd,
                        TimeEndAtNextDay = _model.TimeEndAtNextDay
                    });

                var startDaysResult = handlerBookingMonthStartDays.GetBookingMonthDaysResult(affiliateAdditionalTimes,
                    affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking, bookings, dateFrom, dateFrom.AddMonths(1),
                    timeFrom, timeEnd);

                if (startDaysResult.CurrentMonth != null && startDaysResult.CurrentMonth.Count > 0)
                    return new DateTime(dateFrom.Year, dateFrom.Month, startDaysResult.CurrentMonth[0]).ToString("yyyy-MM-dd");

                dateFrom = dateFrom.AddMonths(1);
            }

            return null;
        }
    }
}
