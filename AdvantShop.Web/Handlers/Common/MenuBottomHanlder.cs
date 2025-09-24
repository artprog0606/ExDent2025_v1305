using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.ViewModel.Common;
using AdvantShop.Web.Admin.Handlers.Cms.StaticPages;

namespace AdvantShop.Handlers.Common
{
    public class MenuBottomHanlder
    {
        public MenuBottomViewModel Get()
        {
            var isRegistered = CustomerContext.CurrentCustomer.RegistredUser;
            var cacheName = !isRegistered
                                ? CacheNames.GetBottomMenuCacheObjectName()
                                : CacheNames.GetBottomMenuAuthCacheObjectName();
            var menuType = isRegistered
                                ? EMenuItemShowMode.Authorized
                                : EMenuItemShowMode.NotAuthorized;

            if (CacheManager.Contains(cacheName))
            {
                var cachedModel = CacheManager.Get<MenuBottomViewModel>(cacheName);
                return cachedModel;
            }

            MenuBottomViewModel model;

            if (!CacheManager.TryGetValue(cacheName, out model))
            {
                model = new MenuBottomViewModel();

                if (SettingsCatalog.DisplayCategoriesInBottomMenu)
                    model.Categories =
                        CategoryService.GetChildCategoriesByCategoryId(0)
                            .Where(cat => cat.Enabled && cat.ParentsEnabled && !cat.Hidden)
                            .ToList();

                model.MenuItems = MenuService.GetMenuItems(0, EMenuType.Bottom, menuType);

                foreach (var item in model.MenuItems)
                {
                    if (item.HasChild)
                        item.SubItems = MenuService.GetMenuItems(item.ItemId, EMenuType.Bottom, menuType);
                }

                CacheManager.Insert(cacheName, model, 20);
            }

            return model;
        }
    }
}