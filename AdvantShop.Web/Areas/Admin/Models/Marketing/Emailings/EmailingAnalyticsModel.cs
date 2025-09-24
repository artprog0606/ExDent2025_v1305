using System;
using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Shared.Common;

namespace AdvantShop.Web.Admin.Models.Marketing.Emailings
{
    public class EmailingAnalyticsModel
    {
        public ChartDataJsonModel ChartData { get; set; }
        public List<EmailStatusAnalyticsModel> StatusesData { get; set; }
    }

    public class EmailStatusAnalyticsModel
    {
        public string StatusName { get; set; }
        public string Status { get; set; }
        public int Count { get; set; }
        public int Percent { get; set; }
    }
}
