using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Settings.CheckoutSettings
{
    public class AdditionalWorkingTimeSettings
    {
        public bool IsWork { get; set; }
        public List<WorkingTimeInterval> WorkingTimes { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}