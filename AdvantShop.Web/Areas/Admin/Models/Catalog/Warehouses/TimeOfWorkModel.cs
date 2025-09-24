using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class TimeOfWorkModel
    {
        public int Id { get; set; }
        public List<DayOfWeek> DayOfWeeks { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public string BreakStartTime { get; set; }
        public string BreakEndTime { get; set; }
    }
}