using AdvantShop.CMS;

namespace AdvantShop.Areas.Mobile.Models.Footer
{
    public class FooterMobileViewModel
    {
        public bool IsShowMobileAppLink { get; set; }
        public string MobileAppGooglePlayMarket { get; set; }
        public string MobileAppAppleAppStoreLink { get; set; }
        public string MobileAppRustoreLink { get; set; }
        public string MobileAppGalleryLink{ get; set; }
        public StaticBlock MobileFooterTextStaticBlock { get; set; }

        public bool IsModuleMobileAppActive { get; set; }
    }
}