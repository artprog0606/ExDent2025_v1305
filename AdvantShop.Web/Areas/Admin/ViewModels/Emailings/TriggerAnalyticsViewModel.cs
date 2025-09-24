using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Triggers;

namespace AdvantShop.Web.Admin.ViewModels.Emailings
{
    public class TriggerAnalyticsViewModel
    {
        public TriggerAnalyticsViewModel()
        {
            TriggerActions = new List<TriggerAction>();
        }

        public int TriggerId { get; set; }
        public string Name { get; set; }

        public List<TriggerAction> TriggerActions { get; set; }
    }
}
