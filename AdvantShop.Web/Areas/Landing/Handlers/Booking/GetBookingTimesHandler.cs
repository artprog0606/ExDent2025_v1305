using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class GetBookingTimesHandler : ICommandHandler<object>
    {
        private readonly int _resourceId;
        private readonly int _affiliateId;
        private readonly DateTime _selectedDate;
        private readonly TimeSpan _servicesDuration;

        public GetBookingTimesHandler(int resourceId, int affiliateId, DateTime selectedDate, List<int> selectedServices)
        {
            _resourceId = resourceId;
            _affiliateId = affiliateId;
            _selectedDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day);
            if (selectedServices != null)
            {
                var services = selectedServices.Select(serviceId => ServiceService.Get(serviceId)).Where(x => x != null && x.Enabled).ToList();
                _servicesDuration = TimeSpan.FromSeconds(services.Where(x => x.Duration.HasValue).Sum(x => x.Duration.Value.TotalSeconds));
            }
        }

        public object Execute()
        {
            var affiliate = AffiliateService.Get(_affiliateId);
            if (affiliate == null)
                throw new BlException("Филиал не найден");

            var resource = ReservationResourceService.Get(_resourceId);
            if (resource == null)
                throw new BlException("Ресурс не найден");

            var affiliateAdditionalTimes = AffiliateAdditionalTimeService.GetByAffiliateAndDate(_affiliateId, _selectedDate);
            var affiliateTimesOfBooking = AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_affiliateId, _selectedDate.DayOfWeek);
            var resourceAdditionalTimes = ReservationResourceAdditionalTimeService.GetByDate(_affiliateId, _resourceId, _selectedDate);
            var resourceTimesOfBooking = ReservationResourceTimeOfBookingService.GetByDayOfWeek(_affiliateId, _resourceId, _selectedDate.DayOfWeek);
            var bookings = BookingService.GetListByDateAndReservationResource(_affiliateId, _selectedDate.Date, _resourceId);

            var times = GetBookingTimes(
                _selectedDate,
                affiliateAdditionalTimes,
                affiliateTimesOfBooking,
                resourceAdditionalTimes,
                resourceTimesOfBooking,
                bookings
                );

            return new
            {
                Times = times.Select(time => new
                {
                    From = time.Item1.ToString("hh\\:mm"),
                    To = time.Item2.ToString("hh\\:mm")
                }).ToList(),
                HasFreeTime = times.Any()
            };
        }

        public List<Tuple<TimeSpan, TimeSpan>> GetBookingTimes(
            DateTime date,
            List<AffiliateAdditionalTime> affiliateAdditionalTimes,
            List<AffiliateTimeOfBooking> affiliateTimesOfBooking,
            List<ReservationResourceAdditionalTime> resourceAdditionalTimes,
            List<ReservationResourceTimeOfBooking> resourceTimesOfBooking,
            List<Core.Services.Booking.Booking> bookings
            )
        {
            var times = new List<Tuple<TimeSpan, TimeSpan>>();

            var nullTime = new TimeSpan(0, 0, 0);
            var oneDayTime = new TimeSpan(1, 0, 0, 0);

            var affiliateIsWork = (affiliateAdditionalTimes.Count > 0 && affiliateAdditionalTimes[0].IsWork) ||
                                    (affiliateAdditionalTimes.Count == 0 && affiliateTimesOfBooking.Count > 0);

            if (affiliateIsWork)
            {

                if (resourceAdditionalTimes.Count > 0)
                {
                    if (resourceAdditionalTimes[0].IsWork)
                        times.AddRange(resourceAdditionalTimes
                            .Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime.TimeOfDay, x.EndTime.TimeOfDay == nullTime ? oneDayTime : x.EndTime.TimeOfDay)));
                }
                else
                {
                    times.AddRange(resourceTimesOfBooking.Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime, x.EndTime)));
                }

                bool disablePastTime = date.Date <= DateTime.Now.Date;

                var reservationResourceAdditionalTimeDictionary = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>() { { date.Date, resourceAdditionalTimes } };
                var reservationResourceTimesOfBookingDayOfWeekDictionary = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>() { { date.DayOfWeek, resourceTimesOfBooking } };
                var affiliateAdditionalTimeDictionary = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { { date.Date, affiliateAdditionalTimes } };
                var affiliateTimesOfBookingDayOfWeekDictionary = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { { date.DayOfWeek, affiliateTimesOfBooking } };

                if (times.Count > 0 && _servicesDuration.TotalSeconds > 0)
                {

                    var timesTemp = new List<Tuple<TimeSpan, TimeSpan>>();
                    foreach (var time in times)
                    {
                        var timeFrom = time.Item1;
                        var timeServicesDone = time.Item1 + _servicesDuration;
                        var timeEnd = times.FirstOrDefault(x => x.Item1 < timeServicesDone && x.Item2 >= timeServicesDone);
                        if (timeEnd == null)
                            continue;
                        var timeTo = timeEnd.Item2;

                        if (bookings.Any(booking => TimeCross(timeFrom, timeTo, booking.BeginDate.TimeOfDay, booking.EndDate.TimeOfDay)))
                            continue;
                        if (!ReservationResourceService.CheckDateRangeIsWork(
                            date.AddSeconds(timeFrom.TotalSeconds),
                            date.AddSeconds(timeTo.TotalSeconds),
                            reservationResourceAdditionalTimeDictionary,
                            reservationResourceTimesOfBookingDayOfWeekDictionary,
                            affiliateAdditionalTimeDictionary,
                            affiliateTimesOfBookingDayOfWeekDictionary))
                            continue;
                        timesTemp.Add(new Tuple<TimeSpan, TimeSpan>(timeFrom, timeTo));
                    }
                    times = timesTemp;
                }
                else if (times.Count > 0)
                {
                    //times = times.Where(x =>
                    //    ReservationResourceService.ExistDateRangeInTimeOfBooking(
                    //        date.Date + x.Item1,
                    //        date.Date + x.Item2, 
                    //        resourceAdditionalTimes, resourceTimesOfBooking,
                    //        affiliateAdditionalTimes, affiliateTimesOfBooking)).ToList();
                    times = times.Where(x =>
                        ReservationResourceService.CheckDateRangeIsWork(
                            date.Date + x.Item1,
                            date.Date + x.Item2,
                            reservationResourceAdditionalTimeDictionary,
                            reservationResourceTimesOfBookingDayOfWeekDictionary,
                            affiliateAdditionalTimeDictionary,
                            affiliateTimesOfBookingDayOfWeekDictionary)).ToList();
                }

                times = times.Count > 0
                    ? times.Where(time => (!disablePastTime || time.Item1 > DateTime.Now.TimeOfDay) &&
                    !bookings.Any(booking => TimeCross(time.Item1, time.Item2, booking.BeginDate.TimeOfDay, booking.EndDate.TimeOfDay))).ToList()
                    : times;
            }
            return times;
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, TimeSpan start, TimeSpan end)
        {
            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return start < to && from < end;
            //return (start <= from && from < end) || (start < to && to <= end);
        }
    }
}
