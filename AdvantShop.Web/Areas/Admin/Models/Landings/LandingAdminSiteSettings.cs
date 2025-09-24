using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SEO;

namespace AdvantShop.Web.Admin.Models.Landings
{
    public class LandingAdminSiteSettings
    {
        public LandingAdminSiteSettings()
        {
            AuthFilterRules = Enum.GetValues(typeof(ELpAuthFilterRule)).Cast<ELpAuthFilterRule>()
                .Select(x => new SelectListItem() { Text = x.Localize(), Value = ((int)x).ToString() }).ToList();

            SalesFunnels = SalesFunnelService.GetList()
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            SalesFunnels.Insert(0, new SelectListItem { Text = LocalizationService.GetResource("Admin.Cms.NotSelected"), Value = "0" });
        }

        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        //public bool SiteEnabled { get; set; }

        public string BlockInHead { get; set; }
        public string BlockInBodyBottom { get; set; }
        public string Favicon { get; set; }
        public string YandexCounterId { get; set; }
        public string YandexCounterHtml { get; set; }
        public string GoogleCounterId { get; set; }
        public string GoogleTagManagerId { get; set; }

        public string RobotsTxt { get; set; }
        public bool UseHttpsForSitemap { get; set; }
        public string SiteCss { get; set; }

        public bool HideAdvantshopCopyright { get; set; }
        public bool UseDomainsManager { get; set; }

        public bool RequireAuth { get; set; }
        public string AuthRegUrl { get; set; }
        public ELpAuthFilterRule AuthFilterRule { get; set; }
        public int? AuthLeadSalesFunnelId { get; set; }
        public int? AuthLeadDealStatusId { get; set; }
        public List<SelectListItem> AuthFilterRules { get; private set; }
        public List<SelectListItem> SalesFunnels { get; private set; }

        public int OrderSourceId { get; set; }

        public bool DisableCopyrightEdit => CopyrightService.IsCopyrightLocked;

        #region MobileApp

        public bool MobileAppActive { get; set; }
        public string MobileAppName { get; set; }
        public string MobileAppShortName { get; set; }
        public string MobileAppImgSrc { get; set; }
        public string AppleAppStoreLink { get; set; }
        public string GooglePlayMarketLink { get; set; }

        #endregion
    }
}
