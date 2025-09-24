using System;
using AdvantShop.Core.Common;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Boxberry
{
    public class BoxberryCreateOrder
    {
        private readonly int _orderId;

        public BoxberryCreateOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Boxberry")
                return new CommandResult() { Error = "Order shipping method is not 'Boxberry' type" };

            try
            {
                var calculationParameters =
                    ShippingCalculationConfigurator.Configure()
                                                   .ByOrder(order)
                                                   .FromAdminArea()
                                                   .Build();

                var boxberryMethod = new Shipping.Boxberry.Boxberry(shippingMethod, calculationParameters);
                var result = boxberryMethod.UnloadOrder(order);

                if (!result.Success)
                    return new CommandResult() { Result = false, Error = result.Message };

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Orders_OrderSentToDeliveryService, shippingMethod.ShippingType);

                return new CommandResult() { Result = true, Message = "Черновик заказа успешно создан.", Obj = result.Message };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Не удалось создать черновик заказа: " + ex.Message };
            }

            return new CommandResult() { Error = "Не удалось создать черновик заказа." };
        }
    }
}
