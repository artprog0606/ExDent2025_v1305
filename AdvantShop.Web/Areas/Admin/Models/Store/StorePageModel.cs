using System.Collections.Generic;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.DownloadableContent;

namespace AdvantShop.Web.Admin.Models.Store
{
    public class StorePageModel
    {
        public List<SalesChannelSubMenuItem> MenuItems { get; set; }
        
        public string StoreName { get; set; }

        public string StoreDomain { get; set; }
        public bool UseDomainsManager { get; set; }
    }
}
