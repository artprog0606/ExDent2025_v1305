using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Sberlogistic
{
    public class CancelSberlogisticOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public CancelSberlogisticOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var sberUuid = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData);
            var sberOrderIsCanceled = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderIsCanceledInOrderAdditionalData);

            if (string.IsNullOrEmpty(sberUuid))
            {
                Errors.Add("Заказ еще не был передан");
                return false;
            }

            if (!string.IsNullOrEmpty(sberOrderIsCanceled))
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
                typeof(Shipping.Sberlogistic.Sberlogistic).GetCustomAttributes(typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var sberMethod = new Shipping.Sberlogistic.Sberlogistic(order.ShippingMethod, null);

            var result = sberMethod.SberlogisticApiService.DeleteOrderDraft(sberUuid);
            if (result)
            {
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderIsCanceledInOrderAdditionalData, "true");
                OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                    Shipping.Sberlogistic.Sberlogistic.KeyNameSberlogisticOrderUuidInOrderAdditionalData, string.Empty);
                return true;
            }
            else
                Errors.Add("Не удалось удалить черновик заказа");

            return false;
        }
    }
}