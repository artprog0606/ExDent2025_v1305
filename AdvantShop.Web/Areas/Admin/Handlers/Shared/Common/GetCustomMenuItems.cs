using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.CMS;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class GetCustomMenuItems
    {
        public List<MenuItemModel> Execute()
        {
            var items = MenuService.GetMenuItems(EMenuType.Admin, EMenuItemShowMode.All);

            var menu = items.DeepCloneJson(); // don't modify object in cache

            SetSelected(menu);

            return menu;
        }

        private void SetSelected(List<MenuItemModel> items)
        {
            var path = GetPath(HttpContext.Current.Request.RawUrl);
            if (path.IsNullOrEmpty())
                return;

            foreach (var item in items)
            {
                foreach (var subItem in item.SubItems)
                {
                    var subMenuPath = GetPath(subItem.UrlPath);
                    if (subMenuPath.IsNotEmpty() && subMenuPath.Contains(path))
                    {
                        subItem.Selected = true;
                        break;
                    }
                }
                var menuPath = GetPath(item.UrlPath);
                if (item.SubItems.Any(x => x.Selected) || (menuPath.IsNotEmpty() && menuPath.Contains(path)))
                {
                    item.Selected = true;
                }
            }
        }

        private string GetPath(string url)
        {
            if (url.IsNullOrEmpty())
                return url;
            return url.ToLower().Split(new string[] { "adminv2/", "adminv3/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
    }
}
