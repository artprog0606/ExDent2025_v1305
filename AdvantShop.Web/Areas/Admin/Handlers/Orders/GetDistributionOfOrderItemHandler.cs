using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetDistributionOfOrderItemHandler : ICommandHandler<List<DistributionOfOrderItemModel>>
    {
        private readonly int _orderItemId;

        public GetDistributionOfOrderItemHandler(int orderItemId)
        {
            _orderItemId = orderItemId;
        }

        public List<DistributionOfOrderItemModel> Execute()
        {
            var orderItem = OrderService.GetOrderItem(_orderItemId);
            if (orderItem == null)
                throw new BlException("OrderItem not found");

            var distributionOfOrderItem = DistributionOfOrderItemService.GetDistributionOfOrderItem(_orderItemId);

            return distributionOfOrderItem
                  .Select(item => new DistributionOfOrderItemModel
                   {
                       WarehouseId = item.WarehouseId,
                       Amount = item.Amount,
                       DecrementedAmount = item.DecrementedAmount
                   })
                  .ToList();
        }
    }
}