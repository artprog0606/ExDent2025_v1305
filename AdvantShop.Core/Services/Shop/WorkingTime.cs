using System;

namespace AdvantShop.Core.Services.Shop
{
    public class WorkingTime
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get;set; }
    }

    public class AdditionalWorkingTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsWork { get; set; }
    }
}
