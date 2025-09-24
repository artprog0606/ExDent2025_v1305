using System.Globalization;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.ViewModel.Home;
using AdvantShop.Design;
using System.Linq;
using System;
using System.Web;
using AdvantShop.Core.Services.Shop;

namespace AdvantShop.Handlers.Common
{
    public sealed class TopPanelHandler
    {
        public TopPanelViewModel Get()
        {
            var isStoreClosed = SettingsMain.IsStoreClosed;
            var isTechDomain = SettingsMain.IsTechDomain(new Uri(HttpContext.Current.Request.Url.AbsoluteUri));
            var currentCustomer = CustomerContext.CurrentCustomer;

            var model = new TopPanelViewModel(currentCustomer)
            {
                IsShowNotificationStoreClosed = isStoreClosed && currentCustomer.IsAdmin,
                IsShowNotificationTechDomain = !isStoreClosed && isTechDomain && currentCustomer.IsAdmin
            };

            if (model.IsShowCurrency)
            {
                var currentCurrency = CurrencyService.CurrentCurrency;
                model.CurrentCurrency = currentCurrency;
                foreach (var currency in CurrencyService.GetAllCurrencies(true))
                {
                    model.Currencies.Add(new SelectListItem()
                    {
                        Text = currency.Name,
                        Value = currency.Iso3,
                        Selected = currency.Iso3 == currentCurrency.Iso3
                    });
                }
            }

            if (model.IsShowWishList)
            {
                var wishCount = ShoppingCartService.CurrentWishlist.Count;
                model.WishCount =
                    string.Format("{0} {1}",
                        wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
                        Strings.Numerals(wishCount,
                            LocalizationService.GetResource("Common.TopPanel.WishList0"),
                            LocalizationService.GetResource("Common.TopPanel.WishList1"),
                            LocalizationService.GetResource("Common.TopPanel.WishList2"),
                            LocalizationService.GetResource("Common.TopPanel.WishList5")));
            }

            if (model.IsTemplatePreview)
            {
                model.TemplatePreviewName = SettingsDesign.PreviewTemplate;
                var previewTemplate = TemplateService.GetTemplates().Items.First(tpl => tpl.StringId == SettingsDesign.PreviewTemplate);
                model.HasTemplate = previewTemplate.Active;
                model.BuyTemplateLink = previewTemplate.DetailsLink;
            }

            if (!isStoreClosed && !ShopService.AllowCheckoutNow())
                model.NotAllowCheckoutText = SettingsCheckout.NotAllowCheckoutText;

            return model;
        }
    }
}