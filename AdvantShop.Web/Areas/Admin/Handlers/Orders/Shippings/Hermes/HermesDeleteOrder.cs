using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Shipping.Hermes.Api;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.Hermes
{
    public class HermesDeleteOrder
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public HermesDeleteOrder(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var orderAdditionalData = OrderService.GetOrderAdditionalData(_orderId,
                Shipping.Hermes.Hermes.KeyNameBarcodeInOrderAdditionalData);

            if (!string.IsNullOrEmpty(orderAdditionalData))
            {
                var order = OrderService.GetOrder(_orderId);
                if (order != null)
                {
                    var shippingMethod = ShippingMethodService.GetShippingMethod(order.ShippingMethodId);
                    if (shippingMethod != null &&
                        shippingMethod.ShippingType ==
                        ((ShippingKeyAttribute)
                            typeof(Shipping.Hermes.Hermes).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var hermesMethod = new Shipping.Hermes.Hermes(shippingMethod, null);
                        var client = new RestApiClient(hermesMethod.SecuredToken, hermesMethod.PublicToken);
                        var result = client.DeleteParcel(orderAdditionalData);

                        if (result.IsSuccess == true)
                        {
                            OrderService.DeleteOrderAdditionalData(order.OrderID,
                                Shipping.Hermes.Hermes.KeyNameBarcodeInOrderAdditionalData);

                            //order.TrackNumber = string.Empty;
                            //OrderService.UpdateOrderMain(order);

                            return true;
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
