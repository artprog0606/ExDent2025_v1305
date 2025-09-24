using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveDistributionOfOrderItemHandler : ICommandHandler
    {
        private readonly int _orderItemId;
        private readonly bool _updateAmount;
        private readonly List<DistributionOfOrderItemModel> _distributionItems;

        public SaveDistributionOfOrderItemHandler(
            int orderItemId, 
            bool updateAmount, 
            List<DistributionOfOrderItemModel> distributionItems)
        {
            _orderItemId = orderItemId;
            _updateAmount = updateAmount;
            _distributionItems = distributionItems;
        }

        public void Execute()
        {
            var orderItem = OrderService.GetOrderItem(_orderItemId);
            if (orderItem == null)
                throw new BlException("OrderItem not found");

            var newAmount = _distributionItems.Sum(item => item.Amount);
            if (_updateAmount is false
                && newAmount != orderItem.Amount)
                throw new BlException("The quantity does not match the current value.");

            Order order = null;
            var currentDistributions = DistributionOfOrderItemService.GetDistributionOfOrderItem(_orderItemId);
            foreach (var distributionItem in _distributionItems)
            {
                var distribution =
                    currentDistributions
                       .FirstOrDefault(current =>
                            current.WarehouseId == distributionItem.WarehouseId)
                    ?? new DistributionOfOrderItem()
                    {
                        OrderItemId = _orderItemId,
                        WarehouseId = distributionItem.WarehouseId
                    };

                distribution.Amount = distributionItem.Amount;
                DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(distribution);
                if (distribution.Amount != distribution.DecrementedAmount)
                {
                    if (!_updateAmount
                        || newAmount == orderItem.Amount)
                    {
                        // если не будет вызвано обновление кол-ва позиции
                        if (SettingsCheckout.DecrementProductsCount 
                            && (order = order ?? OrderService.GetOrder(orderItem.OrderID)).Decremented)
                        {
                            // необходимо принудительно вызывать списание/возврат остатков 
                            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);
                            var trackChanges = !order.IsDraft;
                            OrderService.UpdateDecrementedOfferAmount(orderItem, increment: false, trackChanges, changedBy);
                        }
                    }
                }
            }
            
            // отсутствующие обнуляем
            currentDistributions
               .Where(current => _distributionItems.All(distributionItem => current.WarehouseId != distributionItem.WarehouseId))
               .ForEach(current =>
                {
                    current.Amount = 0;
                    DistributionOfOrderItemService.AddOrUpdateDistributionOfOrderItem(current);
                });

            if (_updateAmount
                && newAmount != orderItem.Amount)
            {
                order = order ?? OrderService.GetOrder(orderItem.OrderID);
                
                orderItem.Amount = newAmount;

                // заменяем в списке позицию на обновленную
                var orderItemIndex = order.OrderItems.FindIndex(item => item.OrderItemID == orderItem.OrderItemID);
                order.OrderItems[orderItemIndex] = orderItem;

                new SetPriceRuleForOrderItem(order, orderItem).Execute();

                if (new UpdateOrderItems(order, resetOrderCargoParams: true).Execute() is false)
                    throw new BlException("Failed to update order item.");
            }
        }
    }
}