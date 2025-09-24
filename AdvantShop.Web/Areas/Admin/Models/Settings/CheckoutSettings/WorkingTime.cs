using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Settings.CheckoutSettings
{
    public class WorkingTimeSettings
    {
        public string TimeZoneOffset { get; set; }
        public string NotAllowCheckoutText { get; set; }
        public Dictionary<int, List<WorkingTimeInterval>> WorkingTimes { get; set; }
        public List<string> AdditionalDate { get; set; }
    }
}