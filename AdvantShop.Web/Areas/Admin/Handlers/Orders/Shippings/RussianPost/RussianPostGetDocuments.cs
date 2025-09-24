using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders.Shippings.RussianPost
{
    public class RussianPostGetDocuments
    {
        private readonly int _orderId;
        public List<string> Errors { get; set; }

        public RussianPostGetDocuments(int orderId)
        {
            _orderId = orderId;
            Errors = new List<string>();
        }

        public string Execute()
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
                            typeof(Shipping.RussianPost.RussianPost).GetCustomAttributes(
                                typeof(ShippingKeyAttribute), false).First()).Value)
                    {
                        var russianPostMethod = new Shipping.RussianPost.RussianPost(shippingMethod, null);

                        var path = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                        FileHelpers.CreateDirectory(path);

                        var result = russianPostMethod.RussianPostApiService.GetDocuments(orderAdditionalData.TryParseLong(), path);

                        Errors = ErrorProcessing(russianPostMethod.RussianPostApiService.LastActionErrors);;

                        return result;
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

        private static List<string> ErrorProcessing(List<string> errors)
        {
            if (errors is null)
                return null;
            
            var result = new List<string>();
            foreach (var error in errors)
            {
                if (error.Contains("Forbidden mail type", StringComparison.OrdinalIgnoreCase))
                    result.Add("Не доступно для данного отправления");
            }

            return result;
        }
    }
}
