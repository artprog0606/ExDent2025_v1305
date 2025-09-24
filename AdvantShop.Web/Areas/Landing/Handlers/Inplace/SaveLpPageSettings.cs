using System.Collections.Generic;
using System.Web;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class SaveLpPageSettings
    {
        #region Ctor

        private readonly int _landingId;
        private readonly InplaceSettingsModel _settings;
        private readonly bool _reGenerateCss;
        private readonly LpService _lpService;

        public SaveLpPageSettings(int landingId, InplaceSettingsModel settings, bool reGenerateCss)
        {
            _landingId = landingId;
            _settings = settings;
            _reGenerateCss = reGenerateCss;
            _lpService = new LpService();
        }

        #endregion

        public string Execute()
        {
            var lp = new LpService().Get(_landingId);
            if (lp == null)
                return null;

            if (!string.IsNullOrWhiteSpace(_settings.LpUrl) && lp.Url != _settings.LpUrl)
            {
                lp.Url = _lpService.GetAvailableUrl(lp.LandingSiteId, _settings.LpUrl);
            }

            if (!string.IsNullOrEmpty(_settings.LpName))
                lp.Name = _settings.LpName.DefaultOrEmpty();

            _lpService.Update(lp);

            LpService.CurrentLanding = lp;

            LPageSettings.PageTitle = _settings.PageTitle.DefaultOrEmpty();
            LPageSettings.PageKeywords = _settings.PageKeywords.DefaultOrEmpty();
            LPageSettings.PageDescription = _settings.PageDescription.DefaultOrEmpty();
            LPageSettings.PageCss = _settings.PageCss.DefaultOrEmpty();
            LPageSettings.PageHeadHtml = _settings.PageHeadHtml.DefaultOrEmpty();
            LPageSettings.DisableBlocksOnAllPages = _settings.DisableBlocksOnAllPages;
            LPageSettings.SetShowShoppingCart(LpService.CurrentLanding.Id, _settings.ShowShoppingCart);
            LPageSettings.SetShoppingCartHideShipping(LpService.CurrentLanding.Id, _settings.ShoppingCartHideShipping);

            LPageSettings.SetShoppingCartType(LpService.CurrentLanding.Id, _settings.ShoppingCartType);
            LPageSettings.SetIgnoreActionParams(LpService.CurrentLanding.Id, _settings.IgnoreActionParams);

            if (_reGenerateCss)
            {
                LSiteSettings.FontMain = _settings.FontMain.DefaultOrEmpty();
                LSiteSettings.ColorSchemes = _settings.ColorSchemes;
                LSiteSettings.LineHeight = _settings.LineHeight;

                _lpService.ReGenerateCss(lp.LandingSiteId);
            }

            LPageSettings.AllowAccess = _settings.AllowAccess;
            LPageSettings.RequireAuth = _settings.RequireAuth;
            LPageSettings.AuthRegUrl = _settings.AuthRegUrl;
            LPageSettings.AuthFilterRule = _settings.AuthFilterRule;
            LPageSettings.AuthOrderProductIds = _settings.AuthFilterRule == ELpAuthFilterRule.WithOrderAndProduct ? _settings.AuthOrderProductIds : null;
            LPageSettings.AuthLeadSalesFunnelId = _settings.AuthFilterRule == ELpAuthFilterRule.WithLead ? _settings.AuthLeadSalesFunnelId : null;
            LPageSettings.AuthLeadDealStatusId = _settings.AuthFilterRule == ELpAuthFilterRule.WithLead ? _settings.AuthLeadDealStatusId : null;

            LpSiteService.UpdateModifiedDate(lp.LandingSiteId);

            return _lpService.GetLpLink(HttpContext.Current.Request.Url.Host, lp);
        }
    }
}
