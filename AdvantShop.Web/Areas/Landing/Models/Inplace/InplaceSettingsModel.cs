using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.App.Landing.Domain.Auth;
using AdvantShop.App.Landing.Domain.ColorSchemes;
using AdvantShop.App.Landing.Domain.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Models.Inplace
{
    public class InplaceSettingsModel
    {
        public InplaceSettingsModel()
        {
            AuthFilterRules =
                Enum.GetValues(typeof(ELpAuthFilterRule))
                    .Cast<ELpAuthFilterRule>()
                    .Select(x => new SelectListItem() { Text = x.Localize(), Value = ((int)x).ToString() })
                    .ToList();

            ShoppingCartTypes =
                Enum.GetValues(typeof(ELpShoppingCartType))
                    .Cast<ELpShoppingCartType>()
                    .Where(x => x != ELpShoppingCartType.Booking)
                    .Select(x => new SelectListItem() { Text = x.Localize(), Value = ((int)x).ToString() })
                    .ToList();

            SalesFunnels =
                SalesFunnelService.GetList()
                    .Select(x => new SelectListItem() {Text = x.Name, Value = x.Id.ToString()})
                    .ToList();
        }

        public string PageTitle { get; set; }
        public string PageKeywords { get; set; }
        public string PageDescription { get; set; }
        public string PageCss { get; set; }
        public string PageHeadHtml { get; set; }

        public string BlockInHead { get; set; }
        public string BlockInBodyBottom { get; set; }
        public string Favicon { get; set; }

        
        public string YandexCounterId { get; set; }
        public string YandexCounterHtml { get; set; }
        public string GoogleCounterId { get; set; }
        public string GoogleTagManagerId { get; set; }

        public string FontH { get; set; }
        public string FontMain { get; set; }

        public string LineHeight { get; set; }

        public LpColorScheme ColorScheme { get; set; }
        public List<LpColorScheme> ColorSchemes { get; set; }

        public List<SelectListItem> Fonts { get; set; }

        public string LpUrl { get; set; }
        public bool DisableBlocksOnAllPages { get; set; }
        public Lp Lp { get; set; }
        public string LpSiteUrl { get; set; }
        public string LpName { get; set; }
        public bool ShowShoppingCart { get; set; }
        public bool ShoppingCartHideShipping { get; set; }
        public ELpShoppingCartType ShoppingCartType { get; set; }
        public List<SelectListItem> ShoppingCartTypes { get; private set; }
        public bool IgnoreActionParams { get; set; }


        public bool RequireAuth { get; set; }
        public string AuthRegUrl { get; set; }
        public ELpAuthFilterRule AuthFilterRule { get; set; }
        public List<int> AuthOrderProductIds { get; set; }
        public int? AuthLeadSalesFunnelId { get; set; }
        public int? AuthLeadDealStatusId { get; set; }
        public List<SelectListItem> AuthFilterRules { get; private set; }
        public List<SelectListItem> SalesFunnels { get; private set; }
        public bool SiteRequireAuth { get; set; }
        public bool AllowAccess { get; set; }
    }
}
