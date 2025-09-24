using System;
using System.Linq;
using AdvantShop.Core.Common;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.YandexDelivery
{
    public class CreateYandexDeliveryOrder
    {
        private readonly int _orderId;

        public CreateYandexDeliveryOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() {Error = "Order is null"};
            
            var shippingMethod = order.ShippingMethod;
            if (shippingMethod.ShippingType != "YandexDelivery")
                return new CommandResult() { Error = "Order shipping method is not 'YandexDelivery' type" };
            
            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();

                var yandexDelivery = new Shipping.ShippingYandexDelivery.YandexDelivery(shippingMethod, calculationParameters);
                var result = yandexDelivery.CreateOrder(order);
                
                if (result)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                return result
                    ? new CommandResult() {Result = true, Message = "Черновик заказа успешно создан"}
                    : new CommandResult() {Error = "Не удалось создать черновик заказа"};
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }
        }
    }
}
