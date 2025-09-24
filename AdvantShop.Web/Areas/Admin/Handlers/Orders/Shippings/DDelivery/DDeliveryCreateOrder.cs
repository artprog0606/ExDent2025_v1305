using System;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Shipping.DDelivery;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.DDelivery
{
    public class DDeliveryCreateOrder
    {
        private readonly int _orderId;

        public DDeliveryCreateOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "DDelivery")
                return new CommandResult() { Error = "Order shipping method is not 'DDelivery' type" };


            var dDeliveryService = new DDeliveryService();

            var ddeliveryOrderNumber = dDeliveryService.GetDDeliveryOrderNumber(order.OrderID);
            if (!string.IsNullOrEmpty(ddeliveryOrderNumber))
            {
                return new CommandResult()
                {
                    Result = false,
                    Message = "Заказ уже существует в системе DDelivery, номер " + ddeliveryOrderNumber,
                    Error = "Заказ уже существует в системе DDelivery, номер " + ddeliveryOrderNumber
                };
            }

            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();

                var dDeliveryMethod = new Shipping.DDelivery.DDelivery(shippingMethod, calculationParameters);
                var result = dDeliveryMethod.CreateOrder(order);

                if (result.Success && result.Data != null)
                {
                    dDeliveryService.AddDDeliveryOrder(order.OrderID, result.Data.OrderId);
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);
                }
                
                return new CommandResult() { Result = result.Success, Message = result.Message, Obj = result.Data, Error = !result.Success ? result.Message : "" };

            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }
        }
    }
}
