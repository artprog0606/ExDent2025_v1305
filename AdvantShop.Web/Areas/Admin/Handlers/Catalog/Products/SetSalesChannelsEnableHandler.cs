using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class SetSalesChannelsEnableHandler : AbstractCommandHandler
    {
        private ProductListEnableInChannelModel _model;

        public SetSalesChannelsEnableHandler(ProductListEnableInChannelModel model)
        {
            _model = model;
        }

        protected override void Handle()
        {
            var ids = new List<int>();
            if (_model.FilterModel.SelectMode == Infrastructure.Admin.SelectModeCommand.All)
            {
                
                foreach (var id in new GetCatalog(_model.FilterModel).GetItemsIds<int>("[Product].[ProductID]"))
                {
                    if (_model.FilterModel.Ids == null || _model.FilterModel.Ids.Count == 0 || !_model.FilterModel.Ids.Any(item => item == id))
                        ids.Add(id);
                }                
            }
            else
            {
                ids = _model.FilterModel.Ids;
                if (ids == null || ids.Count == 0)
                {
                    ids = new GetCatalog(_model.FilterModel).GetItemsIds<int>("[Product].[ProductID]");
                }
            }

            if (_model != null && _model.SalesChannelList.Count > 0 && ids != null && ids.Count > 0)
            {
                foreach (var productId in ids)
                {
                    SalesChannelService.DeleteExcludedProductSalesChannels(productId, _model.SalesChannelList.Where(item => !item.GrayEnable).Select(item => item.SalesChannelKey).ToList());
                    foreach (var item in _model.SalesChannelList.Where(item => !item.Enable && !item.GrayEnable))
                    {
                        SalesChannelService.SetExcludedProductSalesChannel(item.SalesChannelKey, productId);
                    }
                }
            }
            return;
        }
    }
}
