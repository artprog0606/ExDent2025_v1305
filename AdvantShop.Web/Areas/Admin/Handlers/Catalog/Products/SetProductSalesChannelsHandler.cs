using System.Linq;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class SetProductSalesChannelsHandler : AbstractCommandHandler
    {
        private ProductEnableInChannelModel _model;

        public SetProductSalesChannelsHandler(ProductEnableInChannelModel model)
        {
            _model = model;
        }

        protected override void Handle()
        {
            if (_model != null && _model.SalesChannelList.Count > 0)
            {
                SalesChannelService.DeleteExcludedProductSalesChannels(_model.ProductId);
                foreach (var item in _model.SalesChannelList.Where(item => !item.Enable))
                {
                    SalesChannelService.SetExcludedProductSalesChannel(item.SalesChannelKey, _model.ProductId);
                }
            }
            return;
        }
    }
}
