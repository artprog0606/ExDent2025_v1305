using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Areas.Mobile.Models.Sidebar
{
    public class SidebarMobileViewModel
    {


        public SidebarMobileViewModel()
        {
            Currencies = new List<SelectListItem>();
        }


        public string StoreName { get; set; }

        public Customer Customer { get; set; }

        public string CurrentCity { get; set; }
        public bool DisplayCity { get; set; }

        public List<MenuItemModel> Menu { get; set; } 
        
        public bool IsShowCurrency { get; set; }

        public Currency CurrentCurrency { get; set; }

        public List<SelectListItem> Currencies { get; set; }

        public bool IsShowAdminLink { get; set; }

        public SettingsMobile.eCatalogMenuViewMode CatalogMenuViewMode { get; set; }
        public bool IsShowMobileAppLink { get; set; }
        public string MobileAppGooglePlayMarket { get; set; }
        public string MobileAppAppleAppStoreLink { get; set; }
        public string MobileAppRustoreLink { get; set; }
        public string MobileAppGalleryLink { get; set; }
        public bool IsModuleMobileAppActive { get; set; }
    }
}