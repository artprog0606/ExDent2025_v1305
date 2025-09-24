using AdvantShop.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using System.Collections.Generic;

namespace AdvantShop.Areas.Mobile.Models.Catalog
{
    public class CatalogMenuModel
    {
        public List<CatalogMenuItem> Items { get; set; }
        public bool ItemsLimited { get; set; }
        public SettingsMobile.eViewCategoriesOnMain ViewMode { get; set; }
        public bool ShowMenuLinkAll { get; set; }
        public SettingsMobile.eCatalogMenuViewMode CatalogMenuViewMode { get; set; }
        public string RootCategoryName { get; set; }
        public string RootUrl { get; set; }
        public bool IsRootItems { get; set; }

        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }

        public bool ShowProductsCount { get; set; }
    }

    public class CatalogMenuItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public CategoryPhoto Icon { get; set; }
        public CategoryPhoto SmallPicture { get; set; }
        public List<CatalogMenuItem> SubItems { get; set; }
        public bool SubItemsLimited { get; set; }
        public int Id { get; set; }
        public bool HasChild { get; set; }
        public int ProductsCount { get; set; }
    }
}