using System;

namespace AdvantShop.Web.Admin.Models.Settings.CheckoutSettings
{
    public class WorkingTimeInterval
    {
        public WorkingTimeInterval() { }

        public WorkingTimeInterval(TimeSpan timeFrom, TimeSpan timeTo)
        {
            TimeFrom = timeFrom.ToString(@"hh\:mm");
            TimeTo = timeTo.ToString(@"hh\:mm");
        }

        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
    }
}