using AdvantShop.Core.Common.Attributes;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.PickPoint
{
    public class GetPickPointMakeZebraLabel
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public GetPickPointMakeZebraLabel(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public Tuple<string, string> Execute()
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

                        var path = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                        FileHelpers.CreateDirectory(path);

                        var fullFilePath = pickPointMethod.PickPointApiService.GetMakeZebraLabel(orderAdditionalData, path);

                        Errors = pickPointMethod.PickPointApiService.LastActionErrors;

                        return new Tuple<string, string>(fullFilePath, System.IO.Path.GetFileName(fullFilePath));
                    }
                    else
                        Errors.Add("Неверный метод доставки");

                }
                else
                    Errors.Add("Заказ не найден");

            }
            else Errors.Add("Заказ еще не был передан");

            return null;
        }

    }
}
