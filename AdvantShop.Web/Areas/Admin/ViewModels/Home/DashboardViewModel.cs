using System.Collections.Generic;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Web.Admin.ViewModels.Home
{
    public enum DashboardSiteItemType
    {
        Store = 0,
        Funnel = 1
    }

    public class DashboardViewModel
    {
        public List<DashboardSiteItem> Sites { get; set; }

        public bool UseDomainsManager { get; protected set; }

        public string ActionText { get; set; }
        public List<DashboardSiteDomain> Domains { get; set; }
        public DashboardSiteDomain SelectedDomain { get; set; }


        public DashboardViewModel()
        {
            Sites = new List<DashboardSiteItem>();

            UseDomainsManager = SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled;
        }
    }

    public class DashboardSiteItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DashboardSiteItemType Type { get; set; }

        public string TypeStr
        {
            get
            {
                return 
                    Type == DashboardSiteItemType.Store 
                        ? LocalizationService.GetResource("Admin.Dashboard.Types.OnlineStore") 
                        : LocalizationService.GetResource("Admin.Dashboard.Types.SalesFunnel");
            }
        }

        public string Domain { get; set; }
        public string PreviewIframeUrl { get; set; }

        public string EditUrl { get; set; }
        public string ViewUrl { get; set; }
        public string ChangeDomainUrl { get; set; }

        public string ScreenShot { get; set; }
        public bool Published { get; set; }
        public bool IsMainSite { get; set; }
    }

    public class DashboardSiteDomain
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DashboardSiteItemType Type { get; set; }
        public string TypeStr { get; set; }
        public bool IsMainSite { get; set; }
    }
}
