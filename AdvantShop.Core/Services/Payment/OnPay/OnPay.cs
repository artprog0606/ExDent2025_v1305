//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("OnPay")]
    public class OnPay : PaymentMethod
    {
        public string FormPay { get; set; }
        public string SendMethod { get; set; }
        public bool CheckMd5 { get; set; }
        public string SecretKey { get; set; }

        public static Dictionary<string, string> GetCurrencies()
        {
            return Currencies;
        }

        public static readonly Dictionary<string, string> Currencies = new Dictionary<string, string>
        {
            {"EUR", "Банковский перевод EUR"},
            {"LIE", "Visa MasterCard EUR (LiqPay)"},
            {"LIQ", "Visa MasterCard RUR (LiqPay)"},
            {"LIU", "Visa MasterCard UAH (LiqPay)"},
            {"LIZ", "Visa MasterCard USD (LiqPay)"},
            {"LRU", "Liberty Reserve, LRUSD"},
            {"MCZ", "Вывод на карту MC Loyalbank в долл"},
            {"MMR", "Moneymail.ru"},
            {"PPL", "PayPal"},
            {"RUR", "Рублевый счет"},
            {"USD", "Банковский перевод USD"},
            {"VCZ", "Вывод на VISA долл"},
            {"WMB", "Webmoney WMB "},
            {"WME", "Webmoney WME"},
            {"WMR", "Webmoney WMR"},
            {"WMU", "Webmoney WMU"},
            {"WMZ", "Webmoney WMZ"},
            {"Y05", "Яндекс Карта 500 руб (1)"},
            {"YC1", "Яндекс Карта 1000 руб (0)"},
            {"YC3", "Яндекс Карта 3000 руб (0)"},
            {"YC5", "Яндекс Карта 5000 руб (0)"},
            {"YCX", "Яндекс Карта 10000 руб (0)"},
            {"YDM", "ЮMoney вывод"}
        };

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {OnPayTemplate.FormPay, FormPay},
                    {OnPayTemplate.SendMethod, SendMethod},
                    {OnPayTemplate.CheckMd5, CheckMd5.ToString()},
                    {OnPayTemplate.SecretKey, SecretKey},
                };

            }
            set
            {
                FormPay = value.ElementOrDefault(OnPayTemplate.FormPay);
                CheckMd5 = value.ElementOrDefault(OnPayTemplate.CheckMd5).TryParseBool();
                SendMethod = value.ElementOrDefault(OnPayTemplate.SendMethod);
                SecretKey = value.ElementOrDefault(OnPayTemplate.SecretKey);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            string sumStr = Math.Round(
                    order.Sum
                        .ConvertCurrency(order.OrderCurrency, paymentCurrency), 1)
                .ToString("F1", CultureInfo.InvariantCulture);

            var currencyLabel = 
                string.Equals(paymentCurrency.Iso3, "RUB", StringComparison.OrdinalIgnoreCase)
                    ? "RUR"
                    : paymentCurrency.Iso3;
            
            return CheckMd5
                ? new PaymentForm
                {
                    FormName = "_xclick",
                    Method = SendMethod == "POST" ? FormMethod.POST : FormMethod.GET,
                    Url = "https://secure.onpay.ru/pay/" + FormPay,
                    InputValues = new NameValueCollection
                    {
                        {"url_success", SuccessUrl},
                        {"pay_mode", "fix"},
                        {"price", sumStr},
                        {"ticker", currencyLabel},
                        {"pay_for", order.OrderID.ToString()},
                        {"price_final", "true"},
                        {
                            "md5",
                            ("fix;" + sumStr + ";" + currencyLabel + ";" + order.Number + ";yes;" + SecretKey).Md5(
                                false)
                        }
                    }
                }
                : new PaymentForm
                {
                    FormName = "_xclick",
                    Method = SendMethod == "POST" ? FormMethod.POST : FormMethod.GET,
                    Url = "https://secure.onpay.ru/pay/" + FormPay,
                    InputValues = new NameValueCollection
                    {
                        {"url_success", SuccessUrl},
                        {"pay_mode", "fix"},
                        {"price", sumStr},
                        {"ticker", currencyLabel},
                        {"pay_for", order.OrderID.ToString()}
                    }
                };
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            int orderId;

            // пришел запрос с проверкой, необходимо отдать xml-ответ
            if (req["type"].IsNotEmpty() && req["type"] == "check")
            {
                SendCheckResponseXml(context);
                return "";
            }

            // пришел запрос об оплате
            if (req["type"].IsNotEmpty() && req["type"] == "pay")
            {
                if (int.TryParse(req["pay_for"], out orderId))
                {
                    Order order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true);

                        SendPayResponseXml(context, 0, orderId, req["order_amount"], req["onpay_id"], "OK", req["order_currency"]);
                        return "";
                    }
                }

                SendPayResponseXml(context, 2, orderId, req["order_amount"], req["onpay_id"], "Error", req["order_currency"]);
                return "";
            }


            if (CheckFields(req) && int.TryParse(req["pay_for"], out orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    return NotificationMessahges.SuccessfullPayment(orderId.ToString());
                }
            }
            return "";
        }

        private void SendCheckResponseXml(HttpContext context)
        {
            var req = context.Request;

            var md5 = $"check;{req["pay_for"]};{req["order_amount"]};{req["order_currency"]};{0};{SecretKey}".Md5(true);
            var responseText =
                $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <result>
                        <code>0</code>
                        <pay_for>{req["pay_for"]}</pay_for>
                        <comment>OK</comment>
                        <md5>{md5}</md5>
                    </result>";
            context.Response.Clear();
            context.Response.ContentType = "application/xml";
            context.Response.Write(responseText);
            context.Response.End();
        }

        /// <summary>
        /// Возвращает xml при запросе об оплате
        /// </summary>
        /// <param name="iCode">0 - ok, 2 -error</param>
        /// <param name="payFor">orderId из запроса</param>
        /// <param name="orderAmount">кол-во</param>
        /// <param name="onpayId">onpay_id</param>
        /// <param name="comment">OK - все хорошо, иначе ошибка</param>
        /// <param name="orderCurrency">order_currency</param>
        /// <returns></returns>
        private void SendPayResponseXml(HttpContext context, int iCode, int payFor, string orderAmount, string onpayId, string comment, string orderCurrency)
        {
            var md5 =
                $"pay;{payFor};{onpayId};{payFor};{orderAmount};{orderCurrency};{iCode};{SecretKey}".Md5(true);
            
            var responseText =
                $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <result>
                        <code>{iCode}</code>
                        <comment>{comment}</comment>
                        <onpay_id>{onpayId}</onpay_id>
                        <pay_for>{payFor}</pay_for>
                        <order_id>{payFor}</order_id>
                        <md5>{md5}</md5>
                    </result>";
            context.Response.Clear();
            context.Response.ContentType = "application/xml";
            context.Response.Write(responseText);
            context.Response.End();
        }

        private bool CheckFields(HttpRequest req)
        {
            if (CheckMd5)
            {
                if (string.IsNullOrEmpty(req["price"]) || string.IsNullOrEmpty(req["pay_for"]) || string.IsNullOrEmpty(req["md5"]))
                    return false;
                if (req["md5"].ToLower() !=
                    (req["pay_mode"] + req["price"] + ";" + req["currency"] + ";" + req["pay_for"] + ";yes;" + SecretKey).Md5(true))
                    return false;
                return true;
            }
            else
            {
                return !(string.IsNullOrEmpty(req["pay_for"])); // string.IsNullOrEmpty(req["price"]) || 
            }
        }
    }
}