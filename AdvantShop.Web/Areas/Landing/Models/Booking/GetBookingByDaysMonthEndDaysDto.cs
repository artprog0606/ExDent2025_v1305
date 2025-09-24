using System;
using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class GetBookingByDaysMonthEndDaysDto
    {
        public GetBookingByDaysMonthEndDaysDto()
        {
            SelectedServices = new List<int>();
        }

        public int AffiliateId { get; set; }
        public int ResourceId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool LoadPrevMonth { get; set; }
        public bool LoadCurrentMonth { get; set; }
        public bool LoadNextMonth { get; set; }
        public List<int> SelectedServices { get; set; }
        public DateTime StartDate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeEnd { get; set; }
        public bool TimeEndAtNextDay { get; set; }
    }
}
