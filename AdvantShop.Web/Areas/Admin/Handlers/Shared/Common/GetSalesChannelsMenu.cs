using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class GetSalesChannelsMenu
    {
        public List<SalesChannel> Execute()
        {
            var salesChannels = SalesChannelService.GetList().Where(x => x.IsShowInMenu()).ToList();
            
            var items = salesChannels.DeepClone();

            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] as string;
            var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"] as string;
            var url = HttpContext.Current.Request.Url.AbsolutePath.ToLower();

            if (controller != null)
                controller = controller.ToLower();
            if (action != null)
                action = action.ToLower();

            foreach (var menuItem in items)
                if (menuItem.SetSelected(action, controller, url))
                    break;

            if (items.Any(x => !string.IsNullOrEmpty(x.ModuleStringId)))
            {
                var item = items.Where(x => !string.IsNullOrEmpty(x.ModuleStringId)).FirstOrDefault(x => url.EndsWith(x.Url));
                if (item != null)
                {
                    foreach (var menuItem in items)
                        menuItem.Selected = false;

                    item.Selected = true;
                }
            }

            return items;
        }
    }
}
