using AdvantShop.Areas.Mobile.Models.Footer;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Mobile.Handlers.Footer
{
    public class FooterHandler
    {
        public FooterMobileViewModel Get()
        {
            FooterMobileViewModel model = new FooterMobileViewModel 
            { 
                MobileAppAppleAppStoreLink = SettingsApi.IosPublishedAppUrl,
                MobileAppGooglePlayMarket = SettingsApi.AndroidPublishedAppUrl,
                MobileAppRustoreLink = SettingsApi.RustorePublishedAppUrl,
                MobileAppGalleryLink = SettingsApi.AppGalleryPublishedAppUrl,
                MobileFooterTextStaticBlock = StaticBlockService.GetPagePartByKeyWithCache("mobile_footer_text"),
                IsModuleMobileAppActive = AttachedModules.GetModuleById("MobileApp", active: true)
                                          != null
            };

            model.IsShowMobileAppLink = model.MobileAppAppleAppStoreLink.IsNotEmpty()
                                        || model.MobileAppGooglePlayMarket.IsNotEmpty()
                                        || model.MobileAppRustoreLink.IsNotEmpty()
                                        || model.MobileAppGalleryLink.IsNotEmpty();

            if (!model.IsShowMobileAppLink && SettingsMobile.MobileAppActive && SettingsMobile.MobileAppShowBadges)
            {
                model.MobileAppAppleAppStoreLink = SettingsMobile.MobileAppAppleAppStoreLink;
                model.MobileAppGooglePlayMarket = SettingsMobile.MobileAppGooglePlayMarket;
                model.IsShowMobileAppLink = model.MobileAppAppleAppStoreLink.IsNotEmpty() || model.MobileAppGooglePlayMarket.IsNotEmpty();
            }
            return model;
        }
    }
}
