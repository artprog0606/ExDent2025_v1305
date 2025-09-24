//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Core.Services.Booking
{
    public class AffiliateAdditionalTime
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public DateTime StartTime { get; set; }

        private DateTime _endTime;
        public DateTime EndTime
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

        public bool IsWork { get; set; }
    }
}
