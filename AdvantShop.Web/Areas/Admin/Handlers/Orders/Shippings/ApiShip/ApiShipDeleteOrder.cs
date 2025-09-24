using System;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.ApiShip;
using AdvantShop.Shipping.ApiShip.Api;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.ApiShip
{
    public class ApiShipDeleteOrder
    {
        private readonly int _orderId;

        public ApiShipDeleteOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            //if (shippingMethod.ShippingType != "ApiShip")
            //    return new CommandResult() { Error = "Order shipping method is not 'ApiShip' type" };

            var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();
            var apiShipMethod = new Shipping.ApiShip.ApiShip(shippingMethod, calculationParameters);
            ApiShipDeleteOrderModel res = null;
            try
            {
                var apiShipOrderStatus = apiShipMethod.ApiShipService.GetApiShipOrderStatus(_orderId.ToString());

                if (apiShipOrderStatus != null && apiShipOrderStatus.OrderInfo != null)
                {
                    res = apiShipMethod.ApiShipService.ApiShipDeleteOrder(apiShipOrderStatus.OrderInfo.OrderId, "orders");
                }
                if (res != null && !res.Deleted.IsNullOrEmpty())
                {
                    OrderService.DeleteOrderAdditionalData(_orderId, Shipping.ApiShip.ApiShip.KeyTextSendOrder);
                    OrderService.DeleteOrderAdditionalData(_orderId, Shipping.ApiShip.ApiShip.KeyTextOrderIdApiShip);                    
                    return new CommandResult { Message = "Заказ удален в ApiShip", Result = true };
                }
                else
                { 
                    return new CommandResult() { Error = "Не удалось удалить заказ" };
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось удалить заказ: " + ex.Message };
            }
        }
    }
}
