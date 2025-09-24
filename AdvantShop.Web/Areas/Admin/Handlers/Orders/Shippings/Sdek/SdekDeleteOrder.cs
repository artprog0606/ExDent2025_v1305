using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sdek
{
    public class SdekDeleteOrder
    {
        private readonly int _orderId;

        public SdekDeleteOrder(int orderId)
        {
            _orderId = orderId;
        }

        public CommandResult Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return new CommandResult() { Error = "Order is null" };

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod.ShippingType != "Sdek")
                return new CommandResult() { Error = "Order shipping method is not 'Sdek' type" };

            try
            {
                var sdekOrderUuid = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData);
                var sdekOrderNumber = OrderService.GetOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData);

                var sdek = new Shipping.Sdek.Sdek(shippingMethod, null);

                if (sdekOrderUuid.IsNullOrEmpty() && sdekOrderNumber.IsNotEmpty())
                {
                    var getOrderResult = sdek.SdekApiService20.GetOrder(null, sdekOrderNumber, null);
                    if (getOrderResult?.Entity != null)
                        sdekOrderUuid = getOrderResult.Entity.Uuid.ToString();
                }
                
                if (sdekOrderUuid.IsNullOrEmpty())
                    return new CommandResult() { Error = "Нет Uuid заказа для его удаления" };


                var deleteOrderResult = sdek.SdekApiService20.DeleteOrder(sdekOrderUuid.TryParseGuid());
                var requestDelete = deleteOrderResult?.Requests?.OrderByDescending(x => x.DateTime)?.FirstOrDefault(x => x.Type == "DELETE");

                if (requestDelete?.State != "INVALID")
                {
                    OrderService.DeleteOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameSdekOrderUuidInOrderAdditionalData);
                    OrderService.DeleteOrderAdditionalData(order.OrderID, Shipping.Sdek.Sdek.KeyNameDispatchNumberInOrderAdditionalData);
                    return new CommandResult() { Result = true, Message = $"Заказ {order.Number} удален из системы СДЭК" };
                }
                else if(requestDelete?.Errors != null)
                    return new CommandResult() { Error = string.Join(" ", requestDelete.Errors.Select(x => x.Message)) };
                else 
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
