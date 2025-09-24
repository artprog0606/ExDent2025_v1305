using System;

namespace AdvantShop.Core.Services.Booking
{
    public class ReservationResourceTimeOfBooking
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public int ReservationResourceId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get
            {
                //for 23:00-00:00, 23:45-00:00
                if (_endTime < StartTime)
                    _endTime += new TimeSpan(1, 0, 0, 0);

                return _endTime;
            }
            set { _endTime = value; }
        }
    }
}
