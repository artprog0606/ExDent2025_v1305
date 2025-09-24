using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost
{
    public class RussianPostDeleteOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public RussianPostDeleteOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.RussianPost.RussianPost.KeyNameOrderRussianPostIdInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof (Shipping.RussianPost.RussianPost).GetCustomAttributes(
                                typeof (ShippingKeyAttribute), false).First()).Value)
                    {
                        var russianPostMethod = new Shipping.RussianPost.RussianPost(shippingMethod, null);
                        var result = russianPostMethod.RussianPostApiService.DeleteOrder(orderAdditionalData.TryParseLong());

                        if (result.OrderIds != null && result.OrderIds.Count > 0)
                        {
                            OrderService.DeleteOrderAdditionalData(order.OrderID,
                                Shipping.RussianPost.RussianPost.KeyNameOrderRussianPostIdInOrderAdditionalData);

                            //order.TrackNumber = string.Empty;
                            //OrderService.UpdateOrderMain(order);

                            return true;
                        }
                        else if (result.Errors != null)
                            Errors.AddRange(result.Errors.Where(x => x.ErrorCodes != null).SelectMany(e => e.ErrorCodes.Select(ec => ec.Description)));
                        else if (russianPostMethod.RussianPostApiService.LastActionErrors != null)
                            Errors.AddRange(russianPostMethod.RussianPostApiService.LastActionErrors);
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
