using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;
using AdvantShop.Shipping.Yandex.Api;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Yandex
{
    public class CancelYandexOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public CancelYandexOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var yandexRequestId = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Yandex.YandexDelivery.KeyNameYandexRequestIdInOrderAdditionalData);
            var yandexOrderIsCanceled = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Yandex.YandexDelivery.KeyNameYandexOrderIsCanceledInOrderAdditionalData);

            if (string.IsNullOrEmpty(yandexRequestId))
            {
                Errors.Add("Заказ еще не был передан");
                return false;
            }

            if (!string.IsNullOrEmpty(yandexOrderIsCanceled))
            {
                Errors.Add("Заказ уже отменен");
                return false;
            }
            
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
            {
                Errors.Add("Заказ не найден");
                return false;
            }

            if (order.ShippingMethod == null || order.ShippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.Yandex.YandexDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var yandexMethod = new Shipping.Yandex.YandexDelivery(order.ShippingMethod, null);

            var result = yandexMethod.YandexDeliveryApiService.CancelOrder(yandexRequestId);
            if(result != null)
            {
                if (result?.Status != CancelOrderStatusType.Error)
                {
                    OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                        Shipping.Yandex.YandexDelivery.KeyNameYandexOrderIsCanceledInOrderAdditionalData, "true");

                    return true;
                }
                Errors.Add(result.Description);
                return false;
            }
           
            if (yandexMethod.YandexDeliveryApiService.LastActionErrors != null)
                Errors.AddRange(yandexMethod.YandexDeliveryApiService.LastActionErrors);

            return false;
        }
    }
}