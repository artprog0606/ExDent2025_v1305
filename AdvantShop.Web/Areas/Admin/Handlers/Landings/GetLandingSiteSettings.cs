using System;
using System.IO;
using System.Web;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Landings;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class GetLandingSiteSettings
    {
        private readonly int _id;
        private readonly LpSiteService _lpSiteService;

        public GetLandingSiteSettings(int id)
        {
            _id = id;
            _lpSiteService = new LpSiteService();
        }

        public LandingAdminSiteSettings Execute()
        {
            var site = _lpSiteService.Get(_id);
            if (site == null)
                return null;

            LpService.CurrentSiteId = site.Id;

            var model = new LandingAdminSiteSettings()
            {
                Id = site.Id,
                SiteName = site.Name,
                //SiteEnabled = site.Enabled,
                SiteUrl = site.Url,
                BlockInHead = LSiteSettings.BlockInHead,
                BlockInBodyBottom = LSiteSettings.BlockInBodyBottom,
                SiteCss = LSiteSettings.SiteCss,
                Favicon = LSiteSettings.Favicon,
                YandexCounterId = LSiteSettings.YandexCounterId,
                YandexCounterHtml = LSiteSettings.YandexCounterHtml,
                GoogleCounterId = LSiteSettings.GoogleCounterId,
                GoogleTagManagerId = LSiteSettings.GoogleTagManagerId,
                
                UseHttpsForSitemap = LSiteSettings.UseHttpsForSitemap,
                RobotsTxt = GetRobotsTxt(site.Id),

                HideAdvantshopCopyright = LSiteSettings.HideAdvantshopCopyright,
                UseDomainsManager = SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled,

                RequireAuth = LSiteSettings.RequireAuth,
                AuthRegUrl = LSiteSettings.AuthRegUrl,
                AuthFilterRule = LSiteSettings.AuthFilterRule,
                AuthLeadSalesFunnelId = LSiteSettings.AuthLeadSalesFunnelId ?? 0,
                AuthLeadDealStatusId = LSiteSettings.AuthLeadDealStatusId,

                OrderSourceId = OrderSourceService.GetOrderSource(Core.Services.Orders.OrderType.LandingPage, site.Id, site.Name).Id,

                MobileAppActive = LSiteSettings.MobileAppActive,
                MobileAppName = LSiteSettings.MobileAppName,
                MobileAppShortName = LSiteSettings.MobileAppShortName,
                MobileAppImgSrc = LSiteSettings.GetMobileAppIconImagePath(),
                GooglePlayMarketLink = LSiteSettings.MobileAppGooglePlayMarket,
                AppleAppStoreLink = LSiteSettings.MobileAppAppleAppStoreLink
            };

            return model;
        }

        private string GetRobotsTxt(int siteId)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath + "/robots.txt", siteId));

                if (File.Exists(filePath))
                {
                    using (var streamReader = new StreamReader(filePath))
                        return streamReader.ReadToEnd();
                }

                using (File.Create(filePath)) { } //nothing here, just  create file
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}
