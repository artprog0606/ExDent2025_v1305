using System;
using AdvantShop.Core.Services.Shipping.DDelivery;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.DDelivery
{
    public class DDeliveryIsExistOrder
    {
        private readonly int _orderId;

        public DDeliveryIsExistOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod == null || shippingMethod.ShippingType != "DDelivery")
                return new CommandResult() { Error = "Order shipping method is not 'DDelivery' type" };

            var ddeliveryService = new DDeliveryService();

            var ddeliveryOrderNumber = ddeliveryService.GetDDeliveryOrderNumber(order.OrderID);
            
            return new CommandResult()
            {
                Result = !string.IsNullOrEmpty(ddeliveryOrderNumber),
                Message = "Заказ не найден",
                Error = "Заказ не найден"
            };
        }
    }
}
