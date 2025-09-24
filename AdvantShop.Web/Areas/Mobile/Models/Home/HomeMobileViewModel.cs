using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Mobile.Models.Home
{
    public class HomeMobileViewModel
    {
        public List<SelectListItem> CategoriesUrl { get; set; }

        public List<ProductViewModel> ProductLists { get; set; }

        public bool HideNewProductsLink { get; set; }

        public bool NewArrivals { get; set; }

        public HomeMobileViewModel()
        {
            ProductLists = new List<ProductViewModel>();
        }

        public SettingsMobile.eMainPageCatalogView MainPageCatalogView { get; set; }
    }
}