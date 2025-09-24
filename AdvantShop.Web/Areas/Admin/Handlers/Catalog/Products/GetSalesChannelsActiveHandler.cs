using System.Linq;
using System.Collections.Generic;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Web.Admin.Models.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetSalesChannelsEnableHandler : AbstractCommandHandler<ProductListEnableInChannelModel>
    {
        //private List<int> _ids;
        private CatalogFilterModel _filterModel;

        public GetSalesChannelsEnableHandler(CatalogFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        protected override ProductListEnableInChannelModel Handle()
        {
            var ids = new List<int>();
            if (_filterModel.SelectMode == Infrastructure.Admin.SelectModeCommand.All)
            {

                foreach (var id in new GetCatalog(_filterModel).GetItemsIds<int>("[Product].[ProductID]"))
                {
                    if (_filterModel.Ids == null || _filterModel.Ids.Count == 0 || !_filterModel.Ids.Any(item => item == id))
                        ids.Add(id);
                }
            }
            else
            {
                ids = _filterModel.Ids;
                if (ids == null || ids.Count == 0)
                {
                    ids = new GetCatalog(_filterModel).GetItemsIds<int>("[Product].[ProductID]");
                }
            }

            var channels = new List<SalesChannelModel>();

            var salesChannelsEnableCount = SalesChannelService.GetExcludedSalesChannelsByProducts(ids);

            foreach (var channel in SalesChannelService.GetList())
            {
                channels.Add(new SalesChannelModel
                {
                    SalesChannelKey = channel.Type.ToString(),
                    SalesChannelName = channel.Name,
                    Enable = !salesChannelsEnableCount.ContainsKey(channel.Type.ToString()),
                    GrayEnable = salesChannelsEnableCount.ContainsKey(channel.Type.ToString()) && (salesChannelsEnableCount[channel.Type.ToString()] < ids.Count && salesChannelsEnableCount[channel.Type.ToString()] > 0)
                });
            }

            return new ProductListEnableInChannelModel
            {
                FilterModel = _filterModel,
                SalesChannelList = channels
            };
        }
    }
}
