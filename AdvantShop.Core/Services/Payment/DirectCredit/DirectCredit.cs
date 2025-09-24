using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Text;
using Newtonsoft.Json;
using AdvantShop.Taxes;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("DirectCredit")]
    public class DirectCredit : PaymentMethod, ICreditPaymentMethod
    {
        /*
         * Примечание:
         * Заказы на сумму менее 3000 руб. не обарабатываются
         * 
         * Тестовые данные:
         * Id партнера: 1-178YO4Z
         * API key: 123qwe
         * API secret ($salt или "соль" подписи сообщения): 321ewq
         * СМС код подтверждения - 1010
         * 
         */

        private const int MinOrderPrice = 1500;
        private const int DefFirstPayment = 25;

        public string PartnerId { get; set; }
        public string CodeTT { get; set; }

        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment => true;
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {DirectCreditTemplate.PartnerId, PartnerId},
                               {DirectCreditTemplate.CodeTT, CodeTT},
                               {DirectCreditTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {DirectCreditTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {DirectCreditTemplate.FirstPayment, FirstPayment.ToInvariantString()}
                              
                           };
            }
            set
            {
                PartnerId = value.ElementOrDefault(DirectCreditTemplate.PartnerId);
                CodeTT = value.ElementOrDefault(DirectCreditTemplate.CodeTT);
                MinimumPrice = value.ElementOrDefault(DirectCreditTemplate.MinimumPrice) != null ? value.ElementOrDefault(DirectCreditTemplate.MinimumPrice).TryParseFloat() : MinOrderPrice;
                MaximumPrice = value.ElementOrDefault(DirectCreditTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(DirectCreditTemplate.FirstPayment) != null ? value.ElementOrDefault(DirectCreditTemplate.FirstPayment).TryParseFloat() : DefFirstPayment;
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;

        public override string ProcessJavascript(Order order)
        {
            
            string JSONObjs = GetOrderJson(order, PartnerId);
            
            var sb = new StringBuilder();

            sb.Append("<script type='text/javascript'>");
            sb.Append("(function () {"+
                                "var po = document.createElement('script'); po.type = 'text/javascript'; po.async = false; " + // po.charset='windows-1251';
                                "po.src = '//api.direct-credit.ru/JsHttpRequest.js';" +
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);" +
                                "})();" +

                                "(function () {" +
                                "var po = document.createElement('script'); po.type = 'text/javascript'; po.async = false; " + // po.charset='windows-1251';
                                "po.src = '//api.direct-credit.ru/dc.js';" +
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);" +
                                "})();" +
                                "(function () {" +
                                "var po = document.createElement('link');" +
                                "po.rel = 'stylesheet';" +
                                "po.type = 'text/css';" +
                                "po.async = false;" +
                                "po.href = '//api.direct-credit.ru/style.css';" +
                                "var s = document.getElementsByTagName('head')[0]; s.appendChild(po, s);" +
                                "})();"
            );

            var phone = order.OrderCustomer != null ? order.OrderCustomer.StandardPhone.ToString() : string.Empty;
            if (phone.Length > 10)
                phone = phone.Substring(phone.Length - 10);

            var orderJs = new
            {
                order = order.OrderID.ToString(),
                phone = phone,
                codeTT = CodeTT,
                firstName = order.OrderCustomer?.FirstName,
                lastName = order.OrderCustomer?.LastName,
                secondName = order.OrderCustomer?.Patronymic,
                email = order.OrderCustomer?.Email,
                errorText =
                    "При попытке оформить заявку на получение кредита произошла ошибка.Попробуйте обновить страницу."
            };

            sb.Append("function DirectCred(){"+
                                
                                "var debug = false;"+
                                "var JSONitems = "+ JSONObjs  +";"+
                                "var arrProducts = [];" +
                                "var results = JSONitems.items;" +
                                "for (var i = 0, len = results.length; i < len; i++) {"+
                                "var result = results[i];"+
                                "arrProducts.push({ id: result.id, name: result.name, price: result.price, type: result.type, count: result.count  });" +
                                "}"+


                                "DCLoans('" + PartnerId + "', 'delProduct', false, function (result) {" +
                                    "if (result.status == true) {"+
                                        "DCLoans('" + PartnerId + "', 'addProduct', { products: arrProducts }, function (result) {" +
                                            "if (result.status == true) {" +
                                                "DCLoans('" + PartnerId + "', 'saveOrder', "+ JsonConvert.SerializeObject(orderJs) +", function (result) {if (result.status == false) {alert(result.error);}}, debug);" +
                                            "} else { " +
                                                "alert(result.error);" +
                                            "}" +
                                        "}, debug);" +
                                    "} else { "+
                                        "alert(result.error);" +
                                    "}" +
                                "}, debug);" +
                          
                        "};");
            sb.Append("</script>");

            return sb.ToString();
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "DirectCred();";
        }

     
        /// <summary>
        /// Генерирует json с заказами
        /// </summary>
        private string GetOrderJson(Order order, string partnerId)
        {
            var orderItems = order.GetOrderItemsForFiscal(PaymentCurrency);

            var shopCartItems = orderItems
                .Where(item => item.ProductID.HasValue)
                .Select(item => new
                {
                    id = item.ArtNo,
                    name = item.Name,
                    price = (float) Math.Round(item.Price),
                    type = ProductService.GetCategoriesByProductId((int) item.ProductID).FirstOrDefault()?.Name,
                    count = item.Amount.ToString("#")

                })
                .ToList();

            var array = new
            {
                items = shopCartItems
            };

            return JsonConvert.SerializeObject(array);
        }
    }

}