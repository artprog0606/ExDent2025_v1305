using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Affiliate.Settings;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate.Settings
{
    public class GetSettingsTimesOfBookingHandler
    {
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly int _intervalMinutes;

        public GetSettingsTimesOfBookingHandler(Core.Services.Booking.Affiliate affiliate, int intervalMinutes)
        {
            _affiliate = affiliate;
            _intervalMinutes = intervalMinutes;
        }

        public SettingsTimesOfBooking Execute()
        {
            var listTimes = BookingService.GetListTimes(_intervalMinutes);

            var affiliateTimes = AffiliateTimeOfBookingService.GetByAffiliate(_affiliate.Id);

            return new SettingsTimesOfBooking
            {
                Times = listTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                MondayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Monday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                TuesdayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Tuesday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                WednesdayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Wednesday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                ThursdayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Thursday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                FridayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Friday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                SaturdayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Saturday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),

                SundayTimes = GetCross(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Sunday).ToList())
                    .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
            };
        }

        private List<Tuple<TimeSpan, TimeSpan>> GetCross(List<Tuple<TimeSpan, TimeSpan>> list,
            List<AffiliateTimeOfBooking> affiliateTimeOfBookings)
        {
            return list
                .Where(listTime =>
                    //affiliateTimeOfBookings.Any(affiliateTime => affiliateTime.StartTime == listTime.Item1 && affiliateTime.EndTime == listTime.Item2) ||
                    affiliateTimeOfBookings.Any(affiliateTime => TimeCross(listTime.Item1, listTime.Item2, affiliateTime.StartTime, affiliateTime.EndTime)))
                .ToList();
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, TimeSpan start, TimeSpan end)
        {
            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return start < to && from < end;
            //return (start <= from && from < end) || (start < to && to <= end);
        }
    }
}
