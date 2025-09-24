using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Shipping.PickPoint.Api;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PickPoint
{
    public class PickPointDeleteOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PickPointDeleteOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.PickPoint.PickPoint.KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof(Shipping.PickPoint.PickPoint).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var pickPointMethod = new Shipping.PickPoint.PickPoint(shippingMethod, null);

                        var result = pickPointMethod.PickPointApiService.CancelInvoice(new CancelInvoiceParams { Ikn = pickPointMethod.Ikn, InvoiceNumber = orderAdditionalData });

                        if (result != null && result.Result)
                        {
                            OrderService.DeleteOrderAdditionalData(order.OrderID,
                                Shipping.PickPoint.PickPoint.KeyNameOrderPickPointInvoiceNumberInOrderAdditionalData);

                            order.TrackNumber = string.Empty;
                            OrderService.UpdateOrderMain(order);

                            return true;
                        }

                        if (pickPointMethod.PickPointApiService.LastActionErrors != null)
                            Errors.AddRange(pickPointMethod.PickPointApiService.LastActionErrors);

                    }
                    else
                        Errors.Add("Неверный метод доставки");
                }
                else
                    Errors.Add("Заказ не найден");
            }
            else Errors.Add("Заказ еще не был передан");

            return false;
        }
    }
}
