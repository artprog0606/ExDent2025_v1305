using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Saas;
using AdvantShop.Trial;
using AdvantShop.Web.Admin.Models.Store;

namespace AdvantShop.Web.Admin.Handlers.Store
{
    public class GetStorePage
    {
        public StorePageModel Execute()
        {
            var model = new StorePageModel()
            {
                StoreName = SettingsMain.ShopName,
                StoreDomain = SettingsMain.SiteUrl,
                UseDomainsManager = SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled
            };

            var store = SalesChannelService.GetByType(ESalesChannelType.Store);
            model.MenuItems = store != null ? store.MenuItems.Where(x => !x.IsHidden).ToList() : new List<SalesChannelSubMenuItem>();

            return model;
        }
    }
}
