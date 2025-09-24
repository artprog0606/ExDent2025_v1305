using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.OzonRocket
{
    public class OzonRocketCancelOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public OzonRocketCancelOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var ozonRocketOrderId = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIdInOrderAdditionalData);
            var ozonRocketOrderOrderIsCanceled = OrderService.GetOrderAdditionalData(_orderId, 
                Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIsCanceledInOrderAdditionalData);

            if (string.IsNullOrEmpty(ozonRocketOrderId))
            {
                Errors.Add("Заказ еще не был передан");
                return false;
            }

            if (!string.IsNullOrEmpty(ozonRocketOrderOrderIsCanceled))
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

            var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
            if (shippingMethod == null || shippingMethod.ShippingType != ((ShippingKeyAttribute)
                typeof(Shipping.OzonRocket.OzonRocket).GetCustomAttributes(
                    typeof(ShippingKeyAttribute), false).First()).Value)
            {
                Errors.Add("Неверный метод доставки");
                return false;
            }

            var ozonRocketMethod = new Shipping.OzonRocket.OzonRocket(shippingMethod, null);

            var result = ozonRocketMethod.OzonRocketClient.Orders.Cancel(
                ozonRocketOrderId.TryParseInt());

            if (result != null)
            {
                if (result.Count > 0 && result[0].Success)
                {
                    // OrderService.DeleteOrderAdditionalData(order.OrderID,
                    //     Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderIdInOrderAdditionalData);
                    // OrderService.DeleteOrderAdditionalData(order.OrderID,
                    //     Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderPostingNumberInOrderAdditionalData);
                    // OrderService.DeleteOrderAdditionalData(order.OrderID,
                    //     Shipping.OzonRocket.OzonRocket.KeyNameOzonRocketOrderPostingIdInOrderAdditionalData);
                    OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                        Shipping.OzonRocket.OzonRocket
                            .KeyNameOzonRocketOrderIsCanceledInOrderAdditionalData,
                        "true");

                    order.TrackNumber = string.Empty;
                    OrderService.UpdateOrderMain(order);

                    return true;
                }

                if (result.Count > 0 && result[0].Errors != null)
                    Errors.AddRange(result[0].Errors.Select(x => x.Message));
            }

            if (ozonRocketMethod.OzonRocketClient.LastActionErrors != null)
                Errors.AddRange(ozonRocketMethod.OzonRocketClient.LastActionErrors);

            return false;
        }
    }
}