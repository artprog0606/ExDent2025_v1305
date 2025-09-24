using System.Collections.Generic;

namespace AdvantShop.App.Landing.Models
{
    public class GetBookingMonthDaysResult
    {
        public List<int> CurrentMonth { get; set; }
        public List<int> PrevMonth { get; set; }
        public List<int> NextMonth { get; set; }
    }
}
