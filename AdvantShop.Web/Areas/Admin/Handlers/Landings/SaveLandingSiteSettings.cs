using System;
using System.IO;
using System.Web;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Landings.MobileApp;
using AdvantShop.Web.Admin.Models.Landings;

namespace AdvantShop.Web.Admin.Handlers.Landings
{
    public class SaveLandingSiteSettings
    {
        private readonly LandingAdminSiteSettings _model;
        private readonly LpSiteService _lpSiteService;

        public SaveLandingSiteSettings(LandingAdminSiteSettings model)
        {
            _model = model;
            _lpSiteService = new LpSiteService();
        }

        public bool Execute()
        {
            var site = _lpSiteService.Get(_model.Id);
            if (site == null)
                return false;

            site.Name = StringHelper.RemoveHTML(_model.SiteName.DefaultOrEmpty()).Reduce(100);

            if (!string.IsNullOrWhiteSpace(_model.SiteUrl) && site.Url != _model.SiteUrl)
                site.Url = _lpSiteService.GetAvailableUrl(_model.SiteUrl.Reduce(100));

            _lpSiteService.Update(site);

            LpService.CurrentSiteId = site.Id;

            LSiteSettings.BlockInHead = _model.BlockInHead;
            LSiteSettings.BlockInBodyBottom = _model.BlockInBodyBottom;
            LSiteSettings.YandexCounterId = _model.YandexCounterId;
            LSiteSettings.YandexCounterHtml = _model.YandexCounterHtml;
            LSiteSettings.GoogleCounterId = _model.GoogleCounterId.DefaultOrEmpty().ToLower().Replace("ua-", "");
            LSiteSettings.GoogleTagManagerId = _model.GoogleTagManagerId;
            LSiteSettings.UseHttpsForSitemap = _model.UseHttpsForSitemap;
            LSiteSettings.SiteCss = _model.SiteCss;
            LSiteSettings.HideAdvantshopCopyright = _model.HideAdvantshopCopyright;
            LSiteSettings.RequireAuth = _model.RequireAuth;
            LSiteSettings.AuthRegUrl = _model.AuthRegUrl;
            LSiteSettings.AuthFilterRule = _model.AuthFilterRule;
            if (_model.AuthFilterRule != ELpAuthFilterRule.WithOrderAndProduct)
                LSiteSettings.AuthOrderProductIds = null;
            LSiteSettings.AuthLeadSalesFunnelId = _model.AuthFilterRule == ELpAuthFilterRule.WithLead ? _model.AuthLeadSalesFunnelId : null;
            LSiteSettings.AuthLeadDealStatusId = _model.AuthFilterRule == ELpAuthFilterRule.WithLead ? _model.AuthLeadDealStatusId : null;

            #region MobileApp

            var mobileSettingsChanged = LSiteSettings.MobileAppActive != _model.MobileAppActive
                                        || LSiteSettings.MobileAppName != _model.MobileAppName
                                        || LSiteSettings.MobileAppShortName != _model.MobileAppShortName
                                        || LSiteSettings.MobileAppAppleAppStoreLink != _model.AppleAppStoreLink
                || LSiteSettings.MobileAppGooglePlayMarket != _model.GooglePlayMarketLink;

            LSiteSettings.MobileAppActive = _model.MobileAppActive;
            LSiteSettings.MobileAppName = _model.MobileAppName;
            LSiteSettings.MobileAppShortName = _model.MobileAppShortName;
            LSiteSettings.MobileAppAppleAppStoreLink = _model.AppleAppStoreLink;
            LSiteSettings.MobileAppGooglePlayMarket = _model.GooglePlayMarketLink;

            if(mobileSettingsChanged)
                new WebManifestHandler(site.Id).Execute();

            #endregion

            try
            {
                var filePath = HttpContext.Current.Server.MapPath(string.Format(LpFiles.LpSitePath + "/robots.txt", site.Id));

                using (var streamWriter = new StreamWriter(filePath, false))
                    streamWriter.Write(_model.RobotsTxt ?? "");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Funnels_EditFunnelSettings);

            return true;
        }


    }
}
