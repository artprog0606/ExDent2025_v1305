using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PecEasyway
{
    public class PecEasywayCancelOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public PecEasywayCancelOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
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

                        var result = pecMethod.PecEasywayApiService.CancelOrder(orderAdditionalData);

                        if (result != null)
                        {
                            if (result.Success && result.Result.Data != null
                                && result.Result.Data.Count > 0)
                            {
                                if (result.Result.Data[0].Cancel)
                                {
                                    OrderService.AddUpdateOrderAdditionalData(order.OrderID,
                                        Shipping.PecEasyway.PecEasyway.KeyNameOrderIsCanceledInOrderAdditionalData,
                                        "true");

                                    //OrderService.DeleteOrderAdditionalData(order.OrderID,
                                    //    Shipping.PecEasyway.PecEasyway.KeyNameOrderIdInOrderAdditionalData);

                                    //order.TrackNumber = string.Empty;
                                    //OrderService.UpdateOrderMain(order);

                                    return true;
                                }
                                else
                                {
                                    if (result.Result.Data[0].Description.IsNotEmpty())
                                        Errors.Add(result.Result.Data[0].Description);
                                }
                            }
                            else if (result.Error != null)
                            {
                                Errors.AddRange(result.Error.Errors.Select(x => x.Description));
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
