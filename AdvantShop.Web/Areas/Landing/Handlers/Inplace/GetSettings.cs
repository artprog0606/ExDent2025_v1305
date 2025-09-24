using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.App.Landing.Domain;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.App.Landing.Models.Inplace;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.App.Landing.Handlers.Inplace
{
    public class GetSettings
    {
        private readonly int _landingId;

        public GetSettings(int landingId)
        {
            _landingId = landingId;
        }

        public InplaceSettingsModel Execute()
        {
            if (LpService.CurrentLanding == null)
                return null;

            var model = new InplaceSettingsModel()
            {
                LpName = LpService.CurrentLanding.Name,
                PageTitle = LPageSettings.PageTitle,
                PageKeywords = LPageSettings.PageKeywords,
                PageDescription = LPageSettings.PageDescription,
                PageHeadHtml = LPageSettings.PageHeadHtml,
                PageCss = LPageSettings.PageCss,
                ColorSchemes = LSiteSettings.ColorSchemes,
                LineHeight = LSiteSettings.LineHeight,
                FontMain = LSiteSettings.FontMain,
                Fonts = LSiteSettings.GetDefaultFonts()
                    .Select(x => new SelectListItem() {Text = x.Name, Value = x.Name})
                    .ToList(),

                DisableBlocksOnAllPages = LPageSettings.DisableBlocksOnAllPages,
                LpUrl = LpService.CurrentLanding.Url,
                LpSiteUrl = UrlService.GetUrl("adminv3/funnels/site/" + LpService.CurrentLanding.LandingSiteId + "#?landingAdminTab=settings"),
                Lp = LpService.CurrentLanding,
                Favicon = LSiteSettings.GetFaviconPath(LpService.CurrentLanding.LandingSiteId),
                ShowShoppingCart = LPageSettings.ShowShoppingCart(LpService.CurrentLanding.Id),
                ShoppingCartHideShipping = LPageSettings.ShoppingCartHideShipping(LpService.CurrentLanding.Id),
                ShoppingCartType = LPageSettings.GetShoppingCartType(LpService.CurrentLanding.Id),
                IgnoreActionParams = LPageSettings.IgnoreActionParams(LpService.CurrentLanding.Id),

                SiteRequireAuth = LSiteSettings.RequireAuth,
                AllowAccess = LPageSettings.AllowAccess,

                RequireAuth = LPageSettings.RequireAuth,
                AuthRegUrl = LPageSettings.AuthRegUrl,
                AuthFilterRule = LPageSettings.AuthFilterRule,
                AuthOrderProductIds = LPageSettings.AuthOrderProductIds,
                AuthLeadSalesFunnelId = LPageSettings.AuthLeadSalesFunnelId,
                AuthLeadDealStatusId = LPageSettings.AuthLeadDealStatusId,

            };

            return model;
        }
    }
}
