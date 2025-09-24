using System.Linq;
using System.Collections.Generic;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetProductSalesChannelsHandler : AbstractCommandHandler<ProductEnableInChannelModel>
    {
        private int _productId;

        public GetProductSalesChannelsHandler(int productId)
        {
            _productId = productId;
        }

        protected override ProductEnableInChannelModel Handle()
        {
            var model = new ProductEnableInChannelModel()
            {
                ProductId = _productId
            };

            model.SalesChannelList = new List<SalesChannelModel>();

            var productChannels = SalesChannelService.GetExcludedProductSalesChannelList(_productId);

            foreach (var channel in SalesChannelService.GetList())
            {
                model.SalesChannelList.Add(new SalesChannelModel
                {                    
                    SalesChannelKey = channel.Type.ToString(),
                    SalesChannelName = channel.Name,
                    Enable = !productChannels.Any(item => item == channel.Type.ToString())
                });
            }
            return model;
        }
    }
}
