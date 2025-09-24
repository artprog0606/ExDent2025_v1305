using System;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Measoft
{
    public class MeasoftDeleteOrder
    {
        private readonly int _orderId;

        public MeasoftDeleteOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Measoft")
                return new CommandResult() { Error = "Order shipping method is not 'Measoft' type" };

            try
            {
                string trackNumber = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Measoft.Measoft.TrackNumberOrderAdditionalDataName);
                if (string.IsNullOrEmpty(trackNumber))
                {
                    return new CommandResult() { Result = false, Error = "Не найден заказ в системе Measoft, нет номера отслеживания" };
                }

                var result = new Shipping.Measoft.Measoft(shippingMethod, null).DeleteOrder(trackNumber);

                if (result != null && !string.IsNullOrEmpty(result.Error) && result.Error != "0")
                {
                    return new CommandResult() { Result = false, Error = result.Error };
                }
                else if (result != null && !string.IsNullOrEmpty(result.Result) && result.Result == "OK")
                {
                    OrderService.DeleteOrderAdditionalData(order.OrderID,
                                Shipping.Measoft.Measoft.TrackNumberOrderAdditionalDataName);
                    return new CommandResult() { Result = true, Message = "Заказ успешно удален из системы Measoft" };
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Ошибка " + ex.Message };
            }

            return new CommandResult() { Result = false, Error = "Не удалось удалить заказ из системы Measoft" };
        }
    }
}
