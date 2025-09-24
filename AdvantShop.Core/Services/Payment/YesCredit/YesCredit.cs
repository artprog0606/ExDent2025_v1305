using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Text;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    [PaymentKey("YesCredit")]
    public class YesCredit : PaymentMethod
    {
        /*
         * Примечание:
         * Заказы на сумму менее 15т. руб. не обарабатываются.
         * Похоже действует только в Москве и области, Санкт-Петербурге и области.
         * 
         */
        public string MerchantId { get; set; }
        public float MinimumPrice { get; set; }
        public float FirstPayment { get; set; }
        
        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl | NotificationType.Handler; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {YesCreditTemplate.MerchantId, MerchantId},
                    {YesCreditTemplate.MinimumPrice, MinimumPrice.ToString()},
                    {YesCreditTemplate.FirstPayment, FirstPayment.ToString()}
                };
            }
            set
            {
                MerchantId = value.ElementOrDefault(YesCreditTemplate.MerchantId);
                MinimumPrice = value.ElementOrDefault(YesCreditTemplate.MinimumPrice).TryParseFloat(0);
                FirstPayment =  value.ElementOrDefault(YesCreditTemplate.FirstPayment).TryParseFloat(10);
            }
        }


        public override string ProcessJavascript(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script src=\"http://yes-credit.su/crem/js/jquery-ui-1.8.23.custom.min.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"http://yes-credit.su/crem/js/crem.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<link href=\"http://yes-credit.su/crem/css/blizter.css\" rel=\"stylesheet\" type=\"text/css\" />");


            sb.AppendLine("<script type=\"text/javascript\"> ");
            sb.AppendFormat("var ycProducts = {0};\r\n", GetOrderJson(order));
            sb.AppendFormat("var ycMerchantId = {0};\r\n", MerchantId);
            sb.AppendFormat("var ycOrderId = \"{0}\";\r\n", order.OrderID);
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "yescreditmodul(ycProducts, ycMerchantId, ycOrderId);";
        }

        private string GetOrderJson(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;

            var shopCartItems = order
                .GetOrderItemsForFiscal(paymentCurrency)
                .Select(item => new
                {
                    model = item.Name,
                    count = item.Amount,
                    price = (float) Math.Round(item.Price * item.Amount)
                })
                .ToList();


            var shippingCostWithDiscount = order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, paymentCurrency);

            if (shippingCostWithDiscount > 0)
            {
                shopCartItems.Add(new
                {
                    model = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    count = 1F,
                    price = (float)Math.Round(shippingCostWithDiscount)
                });
            }

            return JsonConvert.SerializeObject(shopCartItems);
        }
    }
}