using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Booking.Journal
{
    public class MonthsFreeDaysModel
    {
        public List<int> PrevMonth { get; set; }
        public List<int> CurrentMonth { get; set; }
        public List<int> NextMonth { get; set; }
    }
}
