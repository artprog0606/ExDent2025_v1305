using AdvantShop.Core.Services.SalesChannels;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog.Products
{
    public class ProductEnableInChannelModel
    {
        public int ProductId { get; set; }
        public List<int> ProductIds { get; set; }

        public List<SalesChannelModel> SalesChannelList { get; set; }
    }

    public class ProductListEnableInChannelModel
    {        
        public CatalogFilterModel FilterModel { get; set; }
        public List<SalesChannelModel> SalesChannelList { get; set; }
    }


    public class SalesChannelModel
    {
        public string SalesChannelKey { get; set; }

        public string SalesChannelName { get; set; }

        public bool Enable { get; set; }

        public bool GrayEnable { get; set; }
    }
}
