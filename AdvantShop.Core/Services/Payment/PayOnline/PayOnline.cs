//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public enum PayOnlineType
    {
        Select,
        WebMoney,
        QIWI,
        YandexMoney,
        CreditCard_EN,
        CreditCard_RU

    }

    [PaymentKey("PayOnline")]
    public class PayOnline : PaymentMethod
    {
        private string Url
        {
            get
            {
                switch (PayType)
                {
                    case PayOnlineType.QIWI:
                        //Форма оплаты через QIWI:
                        return "https://secure.payonlinesystem.com/ru/payment/select/qiwi/";

                    case PayOnlineType.WebMoney:
                        //Форма оплаты через WebMoney:
                        return "https://secure.payonlinesystem.com/ru/payment/select/paymaster/";

                    case PayOnlineType.YandexMoney:
                        return "https://secure.payonlinesystem.com/ru/payment/select/yandexmoney/";

                    case PayOnlineType.CreditCard_EN:
                        //Форма оплаты с банковской карты – английский интерфейс
                        return "https://secure.payonlinesystem.com/en/payment/";

                    case PayOnlineType.CreditCard_RU:
                        //Форма оплаты с банковской карты  – русский интерфейс
                        return "https://secure.payonlinesystem.com/ru/payment/";
                    default:
                        //Форма выбора платежного инструмента:
                        return "https://secure.payonlinesystem.com/ru/payment/select/";
                }
            }
        }

        public string MerchantId { get; set; }
        public string SecretKey { get; set; }
        public PayOnlineType PayType { get; set; }

       
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
            get { return UrlStatus.NotificationUrl; }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[] {"USD", "RUB", "EUR"};

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayOnlineTemplate.MerchantId, MerchantId},
                               {PayOnlineTemplate.SecretKey, SecretKey},
                               {PayOnlineTemplate.PayType, ((int)PayType).ToString()}
                           };
            }
            set
            {
                MerchantId = value.ElementOrDefault(PayOnlineTemplate.MerchantId);
                SecretKey = value.ElementOrDefault(PayOnlineTemplate.SecretKey);
                int intval;
                PayType = int.TryParse(value.ElementOrDefault(PayOnlineTemplate.PayType), out intval) ? (PayOnlineType)intval : PayOnlineType.Select;
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                .ToString("F2", CultureInfo.InvariantCulture);

            var currency = paymentCurrency.Iso3;
            return new PaymentForm
            {
                Url = Url,
                InputValues = new NameValueCollection
                {
                    {"MerchantId", MerchantId},
                    {"OrderId", paymentNo},
                    {"Amount",orderSumStr},
                    {"Currency",currency}, // Можно использовать следующие валюты: RUB, USD и EUR
                    {"SecurityKey",GetMd5("MerchantId="+MerchantId+"&OrderId="+paymentNo+"&Amount="+orderSumStr+"&Currency="+currency+"&PrivateSecurityKey="+SecretKey) },
                    {"ReturnUrl", HttpUtility.UrlDecode(SuccessUrl)},
                    {"FailUrl", HttpUtility.UrlDecode(FailUrl)}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["OrderId"];
            if (int.TryParse(paymentNumber, out var orderId) && OrderService.GetOrder(orderId) != null)
            {
                OrderService.PayOrder(orderId, true);
                return NotificationMessahges.SuccessfullPayment(paymentNumber);
            }
            return NotificationMessahges.Fail;
        }

        private static string GetMd5(string str)
        {
            return str.Md5(false, Encoding.UTF8);
        }

        private bool CheckData(HttpRequest req)
        {
            return !new[]
                        {
                            "DateTime",
                            "TransactionID",
                            "OrderId",
                            "Amount",
                            "Currency",
                        }.Any(field => string.IsNullOrEmpty(req[field]))
                   &&
                   GetMd5("DateTime=" + req["DateTime"] +
                          "&TransactionID=" + req["TransactionID"] +
                          "&OrderId=" + req["OrderId"] +
                          "&Amount=" + req["Amount"] +
                          "&Currency=" + req["Currency"] +
                          (req["QrCodeId"].IsNotEmpty() 
                              ? "&QrCodeId=" + req["QrCodeId"] 
                              : string.Empty) +
                          "&PrivateSecurityKey=" + SecretKey
                   ) == req["SecurityKey"];
        }
    }
}