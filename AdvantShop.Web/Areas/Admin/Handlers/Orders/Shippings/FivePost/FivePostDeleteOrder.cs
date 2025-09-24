using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;
using System;
using System.Web.WebPages;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.FivePost
{
    public class FivePostDeleteOrder
    {
        private readonly int _orderId;

        public FivePostDeleteOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var trackNumber = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.FivePost.FivePost.TrackNumberOrderAdditionalDataName);
            if (trackNumber.IsNullOrEmpty())
                return new CommandResult() { Error = "Заказ не передавался в систему FivePost" };

            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "FivePost")
                return new CommandResult() { Error = "Order shipping method is not 'FivePost' type" };

            try
            {
                var result = new Shipping.FivePost.FivePost(shippingMethod, null).FivePostApiService.DeleteOrder(trackNumber);

                if (result != null)
                {
                    if (result.ErrorCode.IsNotEmpty() && result.ErrorCode != "false")
                        return new CommandResult() { Error = result.FullError };

                    OrderService.DeleteOrderAdditionalData(order.OrderID,
                        Shipping.FivePost.FivePost.TrackNumberOrderAdditionalDataName);
                    return new CommandResult() { Result = true, Message = "Заказ успешно удален из системы FivePost" };
                }

                return new CommandResult() { Error = "Не удалось удалить заказ" };
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return new CommandResult() { Error = "Ошибка " + ex.Message };
            }
        }
    }
}
