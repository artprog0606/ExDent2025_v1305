using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Text;
using System.Security.Cryptography;
using AdvantShop.Diagnostics;
using System.Xml.Linq;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Payment
{
    [PaymentKey("LiqPay")]
    public class LiqPay : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string MerchantSig { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler | NotificationType.ReturnUrl; }
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
                    {LiqPayTemplate.MerchantId, MerchantId},
                    {LiqPayTemplate.MerchantSig, MerchantSig},
                };
            }
            set
            {
                MerchantId = value.ElementOrDefault(LiqPayTemplate.MerchantId);
                MerchantSig = value.ElementOrDefault(LiqPayTemplate.MerchantSig);
            }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new [] {"RUB", "RUR", "UAH", "EUR", "USD"};

        private string GetOperationXml(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var sumStr =
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .ToString("F0", CultureInfo.InvariantCulture);
            var currencyIso3 =
                string.Equals(paymentCurrency.Iso3, "RUB", StringComparison.OrdinalIgnoreCase)
                    ? "RUR"
                    : paymentCurrency.Iso3;
            
            return $@"<request>
                    <version>1.2</version>
                    <merchant_id>{MerchantId}</merchant_id>
                    <result_url>{SuccessUrl}</result_url>
                    <server_url>{NotificationUrl}</server_url>
                    <order_id>ORDER_{order.OrderID}</order_id>
                    <amount>{sumStr}</amount>
                    <currency>{currencyIso3}</currency>
                    <description>Order {order.OrderID}</description>
                    <default_phone>{order.OrderCustomer.StandardPhone}</default_phone>
                    <pay_way>card,liqpay</pay_way>
                    <goods_id></goods_id>
                </request>";
        }

        private string GetSignature(string xml)
        {
            var sign = MerchantSig + xml + MerchantSig;

            return
                Convert.ToBase64String(
                    new SHA1CryptoServiceProvider().ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(sign))); // 1251
        }

        private string GetOperation(string xml)
        {
            return Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(xml));
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var xml = GetOperationXml(order);

            return new PaymentForm
            {
                Url = "https://www.liqpay.ua/?do=clickNbuy",
                InputValues = new NameValueCollection
                {
                    {"operation_xml", GetOperation(xml)},
                    {"signature", GetSignature(xml)}
                }
            };
        }

        /*<response>      
             <version>1.2</version>
             <merchant_id></merchant_id>
             <order_id> ORDER_123456</order_id>
             <amount>1.01</amount>
             <currency>UAH</currency>
             <description>Comment</description>
             <status>success</status>
             <code></code>
             <transaction_id>31</transaction_id>
             <pay_way>card</pay_way>
             <sender_phone>+3801234567890</sender_phone>
             <goods_id>1234</goods_id>
             <pays_count>5</pays_count>
         </response>*/
        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            try
            {
                Debug.Log.Info("in ProcessResponse signature= " + req["signature"] + " operation_xml=" + req["operation_xml"]);
                if (!string.IsNullOrEmpty(req["operation_xml"]) && !string.IsNullOrEmpty(req["signature"]))
                {
                    var xml = Encoding.UTF8.GetString(Convert.FromBase64String(req["operation_xml"]));

                    if (xml.IsNotEmpty())
                    {
                        var xDoc = XDocument.Parse(xml);

                        var elOrderId = xDoc.Root?.Element("order_id")?.Value;
                        var elStatus = xDoc.Root?.Element("status")?.Value;

                        if (int.TryParse(elOrderId?.Replace("ORDER_", ""), out var orderId) && elStatus == "success")
                        {
                            OrderService.PayOrder(orderId, true);
                            PaymentService.SaveOrderpaymentInfo(orderId, this.PaymentMethodId, "Signature", req["signature"]);
                            return NotificationMessahges.SuccessfullPayment(elOrderId);
                        }
                        else
                        {
                            Debug.Log.Error($"LiqPay: status {xDoc.Root?.Element("status")?.Value}, code {xDoc.Root?.Element("code")?.Value}");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(req["signature"]))
                {
                    var paymentInfo = PaymentService.GetOrderIdByPaymentIdAndCode(this.PaymentMethodId, req["signature"]);
                    if (paymentInfo != null)
                    {
                        if (OrderService.IsPaidOrder(paymentInfo.OrderId))
                        {
                            return NotificationMessahges.SuccessfullPayment(paymentInfo.OrderId.ToString());
                        }
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return NotificationMessahges.InvalidRequestData;
        }

    }
}