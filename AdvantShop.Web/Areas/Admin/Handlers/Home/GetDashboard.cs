using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.DownloadableContent;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Admin.ViewModels.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetDashboard
    {
        public DashboardViewModel Execute()
        {
            var model = new DashboardViewModel()
            {
                ActionText = ShopActionsService.GetLast()
            };

            var mainSiteUrl = SettingsMain.SiteUrl.ToLower();

            var channel = SalesChannelService.GetByType(ESalesChannelType.Store);
            if (channel != null && channel.Enabled)
            {
                var sitePath = UrlService.GetUrl().ToLower().TrimEnd('/');
                var url = sitePath;
                var domain = SettingsMain.SiteUrl.TrimEnd('/');
                if (SettingsMain.IsTechDomainsReady)
                {
                    var mainUrlDomain = domain.Replace("http://", "").Replace("https://", "");
                    var isDomainBusyByFunnel = new LpSiteService()
                        .GetList()
                        .Any(lpSite => lpSite.DomainUrl == mainUrlDomain);
                    if (isDomainBusyByFunnel)
                    {
                        domain = SettingsMain.TechDomainStore.TrimEnd('/');
                        url = UrlService.GetUrlForAuthFromAdmin(SettingsMain.TechDomainStore);
                    }
                    else
                    {
                        url = UrlService.GetUrlForAuthFromAdmin(mainSiteUrl);
                    }
                }
                model.Sites.Add(new DashboardSiteItem
                {
                    Id = -1,
                    Name = SettingsMain.ShopName,
                    Type = DashboardSiteItemType.Store,
                    Domain = domain, // как определить урл магазина?
                    PreviewIframeUrl = domain,

                    EditUrl = SettingsDesign.IsMobileTemplate ? "design/index?showCommon=true" : "design",
                    ViewUrl = url,
                    ScreenShot = SettingsMain.StoreScreenShot,

                    Published = true,
                    IsMainSite = domain == mainSiteUrl,

                    ChangeDomainUrl = "service/domainsmanage"
                });
            }

            channel = SalesChannelService.GetByType(ESalesChannelType.Funnel);
            if (channel != null && channel.Enabled)
            {
                var funnels = new LpSiteService().GetList();
                var screenShotService = new ScreenshotService();

                foreach (var funnel in funnels)
                {
                    var url = !string.IsNullOrEmpty(funnel.DomainUrl)
                        ? "http://" + funnel.DomainUrl
                        : LpService.GetTechUrl(funnel.Url, "", true);

                    model.Sites.Add(new DashboardSiteItem()
                    {
                        Id = funnel.Id,
                        Name = funnel.Name,
                        Type = DashboardSiteItemType.Funnel,
                        Domain = !string.IsNullOrEmpty(funnel.DomainUrl) ? "http://" + funnel.DomainUrl : null,
                        PreviewIframeUrl = url + "?previewInAdmin=true",

                        EditUrl = "funnels/site/" + funnel.Id,
                        ViewUrl = url,
                        ScreenShot = funnel.ScreenShot,

                        Published = funnel.Enabled,
                        IsMainSite = url == mainSiteUrl,

                        ChangeDomainUrl = "funnels/site/" + funnel.Id + "?landingAdminTab=settings&landingSettingsTab=domains"
                    });
                }

                // update funnel screenshot
                Task.Run(() =>
                {
                    foreach (var funnel in funnels)
                    {
                        if (funnel.ScreenShotDate == null || funnel.ModifiedDate == null ||
                            funnel.ScreenShotDate < funnel.ModifiedDate)
                        {
                            screenShotService.UpdateFunnelScreenShotInBackground(funnel);
                        }
                        Thread.Sleep(300);
                    }
                });
            }

            model.Domains =
                model.Sites.Where(x => !string.IsNullOrEmpty(x.Domain))
                    .Select(
                        x =>
                            new DashboardSiteDomain()
                            {
                                Name = x.Name,
                                Url = x.Domain.ToLower(),
                                Type = x.Type,
                                TypeStr = x.TypeStr,
                                IsMainSite = x.IsMainSite
                            })
                    .ToList();

            model.SelectedDomain = model.Domains.FirstOrDefault(x => x.IsMainSite);

            return model;
        }
    }
}
