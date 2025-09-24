using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog.Warehouses;

namespace AdvantShop.Areas.Mobile.Handlers.Catalog
{
    public class GetCatalogMenu
    {
        private readonly UrlHelper _urlHelper;
        private readonly bool _showRoot;
        private readonly bool _isRootItems;
        private readonly int _categoryId;

        public GetCatalogMenu(int categoryId, bool showRoot = false, bool isRootItems = false)
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _showRoot = showRoot;
            _isRootItems = isRootItems;
            _categoryId = categoryId == 0
                ? CategoryService.GetAdaptiveRootCategoryId()
                : categoryId;
        }

        public CatalogMenuModel Execute()
        {
            var warehouseIds = WarehouseContext.GetAvailableWarehouseIds();

            var cacheName = CacheNames.MenuCatalog + "_mobile_menu_" + _categoryId + "_" +
                            (warehouseIds != null && warehouseIds.Count > 0 ? string.Join(",", warehouseIds) : null) + "_" +
                            SettingsCatalog.ShowOnlyAvalible;
            
            var items = 
                CacheManager.Get(cacheName, 
                    () => GetCategoryItems(_categoryId, _showRoot, !_isRootItems, warehouseIds: warehouseIds));
            
            var model = new CatalogMenuModel
            {
                Items = items.Take(SettingsCatalog.MaximumItemsInMenu).ToList(),
                ItemsLimited = items.Count > SettingsCatalog.MaximumItemsInMenu,
                RootUrl = _urlHelper.RouteUrl("CatalogRoot"),
                ShowMenuLinkAll = SettingsMobile.ShowMenuLinkAll,
                CatalogMenuViewMode = SettingsMobile.CatalogMenuViewMode.TryParseEnum(SettingsMobile.eCatalogMenuViewMode.RootCategories),
                RootCategoryName = CategoryService.GetCategory(0).Name,
                IsRootItems = _isRootItems,
                PhotoWidth = SettingsPictureSize.IconCategoryImageWidth,
                PhotoHeight = SettingsPictureSize.IconCategoryImageHeight,
                ShowProductsCount = SettingsCatalog.ShowProductsCount
            };

            return model;
        }

        private List<CatalogMenuItem> GetCategoryItems(int parentCategoryId, bool showRoot = false, bool isNeedParent = false, bool isStopRecursion = false, List<int> warehouseIds = null)
        {
            var list = new List<CatalogMenuItem>();
            var maximumSubItems = parentCategoryId == 0
                ? SettingsCatalog.MaximumItemsInMenu
                : SettingsCatalog.MaximumSubItemsInMenu;

            var rootCategoriesList = CategoryService.GetChildCategoriesByCategoryIdForMenu(parentCategoryId, maximumSubItems + 1, warehouseIds)
                .Where(cat => !cat.Hidden)
                .Select(x => new CatalogMenuItem()
                {
                    Name = x.Name,
                    Url = _urlHelper.RouteUrl("Category", new { url = x.UrlPath }),
                    SubItems = x.HasChild && isStopRecursion == false
                        ? GetCategoryItems(x.CategoryId, false, false, true, warehouseIds)
                        : new List<CatalogMenuItem>(),
                    Id = x.CategoryId,
                    Icon = x.Icon,
                    SmallPicture = x.MiniPicture,
                    HasChild = x.HasChild,
                    ProductsCount = SettingsCatalog.ShowOnlyAvalible
                        ? x.Available_Products_Count
                        : x.ProductsCount
                }).ToList();


            if (showRoot)
            {
                var root = CategoryService.GetCategory(0);
                list.Add(new CatalogMenuItem()
                {
                    Name = root.Name,
                    Url = _urlHelper.RouteUrl("CatalogRoot"),
                    SubItems = rootCategoriesList.Take(maximumSubItems).ToList(),
                    SubItemsLimited = rootCategoriesList.Count > maximumSubItems,
                    Id = 0,
                    Icon = new CategoryPhoto(),
                    SmallPicture = new CategoryPhoto(),
                    HasChild = rootCategoriesList.Any(),
                    ProductsCount = SettingsCatalog.ShowOnlyAvalible
                        ? root.Available_Products_Count
                        : root.ProductsCount
                });
            }
            else if (isNeedParent)
            {
                var parent = CategoryService.GetCategory(_categoryId);
                list.Add(new CatalogMenuItem()
                {
                    Name = parent.Name,
                    Url = _urlHelper.RouteUrl("Category", new { url = parent.UrlPath }),
                    SubItems = rootCategoriesList.Take(maximumSubItems).ToList(),
                    SubItemsLimited = rootCategoriesList.Count > maximumSubItems,
                    Id = parent.CategoryId,
                    Icon = parent.Icon,
                    SmallPicture = parent.MiniPicture,
                    HasChild = rootCategoriesList.Any(),
                    ProductsCount = SettingsCatalog.ShowOnlyAvalible
                        ? parent.Available_Products_Count
                        : parent.ProductsCount
                });
            }
            else
            {
                list = rootCategoriesList;
            }

            return list;
        }
    }
}
