using System;
using AdvantShop.Core.Common;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class TimeOfWork
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public FlagDayOfWeek DayOfWeeks { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
    }
}