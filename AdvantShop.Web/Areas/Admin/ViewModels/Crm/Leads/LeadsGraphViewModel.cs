using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Models.Shared.Common;
using System;

namespace AdvantShop.Web.Admin.ViewModels.Crm.Leads
{
    public class LeadsGraphViewModel
    {
        public LeadsChartDataModel Chart { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class LeadsChartDataModel : ChartDataJsonModel
    {
        public LeadsChartDataModel()
        {
            Series = new System.Collections.Generic.List<string> {
                LocalizationService.GetResource("Admin.Leads.LeadsGraph.LeadsListCount")
                //,LocalizationService.GetResource("Admin.Leads.LeadsGraph.Orders")
            };

            Colors = new System.Collections.Generic.List<string> {
                "#71c73e"
                //,"#77b7c4"
            };
        }
    }
}
