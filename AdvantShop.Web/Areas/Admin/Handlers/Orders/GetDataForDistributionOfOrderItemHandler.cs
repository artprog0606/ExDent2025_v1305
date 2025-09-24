using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetDataForDistributionOfOrderItemHandler : ICommandHandler<DataForDistributionOfOrderItemModel>
    {
        private readonly int _orderItemId;

        public GetDataForDistributionOfOrderItemHandler(int orderItemId)
        {
            _orderItemId = orderItemId;
        }

        public DataForDistributionOfOrderItemModel Execute()
        {
            var orderItem = OrderService.GetOrderItem(_orderItemId);
            if (orderItem == null)
                throw new BlException("OrderItem not found");

            var model = new DataForDistributionOfOrderItemModel
            {
                Warehouses = WarehouseService.GetList()
                                             .Select(warehouse =>
                                                  new WarehouseDistributionInfoModel
                                                  {
                                                      Id = warehouse.Id,
                                                      Name = warehouse.Name
                                                  })
                                             .ToList()
            };
            
            var offer = OfferService.GetOffer(orderItem.ArtNo);
            if (orderItem.ProductID.HasValue
                && offer.ProductId != orderItem.ProductID)
                offer = null; // модификация другого товара
            
            var offerStocks =
                offer != null
                    ? WarehouseStocksService.GetOfferStocks(offer.OfferId)
                    : null;
            
            foreach (var warehouseModel in model.Warehouses)
            {
                warehouseModel.AvailableAmount =
                    offerStocks
                      ?.FirstOrDefault(stock =>
                            stock.WarehouseId == warehouseModel.Id)
                      ?.Quantity ?? 0f;
            }

            return model;
        }
    }
}