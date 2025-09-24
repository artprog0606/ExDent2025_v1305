using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Pec
{
    public class PecCancelOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PecCancelOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof(Shipping.Pec.Pec).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var pecMethod = new Shipping.Pec.Pec(shippingMethod, null);

                        var result = pecMethod.PecApiService.CancellationCargos(orderAdditionalData);

                        if (result != null)
                        {
                            if (result.Success && result.Result != null)
                            {
                                if (result.Result.Count > 0 && result.Result[0].Success)
                                {
                                    OrderService.DeleteOrderAdditionalData(order.OrderID,
                                        Shipping.Pec.Pec.KeyNameCargoCodeInOrderAdditionalData);

                                    order.TrackNumber = string.Empty;
                                    OrderService.UpdateOrderMain(order);

                                    return true;
                                }
                                else if (pecMethod.PecApiService.LastActionErrors != null)
                                {
                                    if (result.Result.Count > 0 && result.Result[0].Description.IsNotEmpty())
                                        Errors.Add(result.Result[0].Description);

                                    Errors.AddRange(pecMethod.PecApiService.LastActionErrors);
                                }
                            }
                            else if (pecMethod.PecApiService.LastActionErrors != null)
                            {
                                Errors.AddRange(pecMethod.PecApiService.LastActionErrors);
                            }
                        }
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
