using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Sidebar;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Web.Admin.Handlers.Cms.StaticPages;

namespace AdvantShop.Areas.Mobile.Handlers.Sidebar
{
    public class SidebarHandler
    {
        public SidebarMobileViewModel Get()
        {
            var model = new SidebarMobileViewModel()
            {
                Customer = CustomerContext.CurrentCustomer,
                StoreName = SettingsMain.ShopName,
                DisplayCity = SettingsMobile.DisplayCity,
                CurrentCity = SettingsMobile.DisplayCity ? IpZoneContext.CurrentZone.City : string.Empty,
                IsShowAdminLink = CustomerContext.CurrentCustomer.Enabled && (CustomerContext.CurrentCustomer.IsAdmin || CustomerContext.CurrentCustomer.IsModerator)
            };

            var isRegistered = CustomerContext.CurrentCustomer.RegistredUser;
            var cacheName = !isRegistered
                                ? CacheNames.GetMainMenuCacheObjectName() + "TopMenu_Mobile" 
                                : CacheNames.GetMainMenuAuthCacheObjectName() + "TopMenu_Mobile";

            var menuType = isRegistered
                                ? EMenuItemShowMode.Authorized
                                : EMenuItemShowMode.NotAuthorized;

            var menuItems = CacheManager.Get(cacheName, 5, () => MenuService.GetMenuItems(0, EMenuType.Mobile, menuType).ToList()).DeepCloneJson();
            foreach (var menuItem in menuItems.Where(menuItem => UrlService.IsCurrentUrl(menuItem.UrlPath)))
                menuItem.Selected = true;
            model.Menu = menuItems;

            model.IsShowCurrency = SettingsCatalog.AllowToChangeCurrency;

            if (model.IsShowCurrency)
            {
                var currentCurrency = CurrencyService.CurrentCurrency;
                model.CurrentCurrency = CurrencyService.CurrentCurrency;
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

            model.CatalogMenuViewMode = SettingsMobile.CatalogMenuViewMode.TryParseEnum(SettingsMobile.eCatalogMenuViewMode.RootCategories);

            model.MobileAppAppleAppStoreLink = SettingsApi.IosPublishedAppUrl;
            model.MobileAppGooglePlayMarket = SettingsApi.AndroidPublishedAppUrl;
            model.MobileAppRustoreLink = SettingsApi.RustorePublishedAppUrl;
            model.MobileAppGalleryLink = SettingsApi.AppGalleryPublishedAppUrl;

            model.IsModuleMobileAppActive = ModulesRepository.IsActiveModule("MobileApp");

            model.IsShowMobileAppLink = model.MobileAppAppleAppStoreLink.IsNotEmpty()
                                            || model.MobileAppGooglePlayMarket.IsNotEmpty()
                                            || model.MobileAppRustoreLink.IsNotEmpty()
                                            || model.MobileAppGalleryLink.IsNotEmpty();


            if (!model.IsShowMobileAppLink && SettingsMobile.MobileAppActive && SettingsMobile.MobileAppShowBadges
                && (SettingsMobile.MobileAppGooglePlayMarket.IsNotEmpty() || SettingsMobile.MobileAppAppleAppStoreLink.IsNotEmpty()))
            {
                model.MobileAppAppleAppStoreLink = SettingsMobile.MobileAppAppleAppStoreLink;
                model.MobileAppGooglePlayMarket = SettingsMobile.MobileAppGooglePlayMarket;
                model.IsShowMobileAppLink = true;
            }

            return model;
        }
    }
}