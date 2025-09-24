using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PecEasyway
{
    public class PecEasywayGetOrderLabel
    {
        private readonly int _orderId;

        public PecEasywayGetOrderLabel(int orderId)
        {
            _orderId = orderId;
        }

        public string Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.PecEasyway.PecEasyway.KeyNameOrderIdInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof(Shipping.PecEasyway.PecEasyway).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var pecMethod = new Shipping.PecEasyway.PecEasyway(shippingMethod, null);

                        return pecMethod.PecEasywayApiService.GetOrderLabel(orderAdditionalData);
                    }
                    return "Неверный метод доставки";
                }
                return "Заказ не найден";
            }
            
            return "Заказ еще не был передан";
        }
    }
}
