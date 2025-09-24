using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Payment;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Core.Services.Payment.PSBank
{
    [PaymentKey("PSBank")]
    public class PSBank : PaymentMethod
    {
        private const string TestUrl = "https://test.3ds.payment.ru/cgi-bin/cgi_link";
        private const string GeneralUrl = "https://3ds.payment.ru/cgi-bin/cgi_link";

        public string FirstComponent { get; set; }
        public string SecondComponent { get; set; }
        public string Terminal { get; set; }
        public string Merchant { get; set; }
        public string MerchantName { get; set; }
        public bool UseTestMode { get; set; }

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

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                                {PSBankTemplate.FirstComponent, FirstComponent},
                                {PSBankTemplate.SecondComponent, SecondComponent},
                                {PSBankTemplate.Terminal, Terminal},
                                {PSBankTemplate.Merchant, Merchant},
                                {PSBankTemplate.MerchantName, MerchantName},
                                {PSBankTemplate.UseTestMode, UseTestMode.ToString()},
                           };
            }
            set
            {
                FirstComponent = value.ElementOrDefault(PSBankTemplate.FirstComponent);
                SecondComponent = value.ElementOrDefault(PSBankTemplate.SecondComponent);
                Terminal = value.ElementOrDefault(PSBankTemplate.Terminal);
                Merchant = value.ElementOrDefault(PSBankTemplate.Merchant);
                MerchantName = value.ElementOrDefault(PSBankTemplate.MerchantName);
                UseTestMode = value.ElementOrDefault(PSBankTemplate.UseTestMode).TryParseBool();
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr = 
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .ToString("#0.##", CultureInfo.InvariantCulture);
            
            // порядок из формата
            var values = new NameValueCollection
            {
                {"AMOUNT", orderSumStr},
                {"CURRENCY", paymentCurrency.Iso3},
                {"ORDER", GetOrderIdString(order.OrderID)},
                {"MERCH_NAME", MerchantName},
                {"MERCHANT", Merchant},
                {"TERMINAL", Terminal},
                {"EMAIL", order.OrderCustomer.Email},
                {"TRTYPE", "1"},
                {"TIMESTAMP", DateTime.UtcNow.ToString("yyyyMMddHHmmss")},
                {"NONCE", GenerateNonce()},
                {"BACKREF", SuccessUrl},
            };
            values.Add("P_SIGN", GetPSign(values));
            values.Add("DESC", GetOrderDescription(order.Number));
            values.Add("NOTIFY_URL", NotificationUrl);
            return new PaymentForm
            {
                FormName = "payment_form",
                Method = FormMethod.POST,
                Url = UseTestMode ? TestUrl : GeneralUrl,
                InputValues = values
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (CheckFields(req) && int.TryParse(req["ORDER"], out var orderId))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        #region private methods

        private string GetPSign(NameValueCollection values)
        {
            var sb = new StringBuilder();
            foreach (var key in values.AllKeys)
            foreach (var val in values.GetValues(key) ?? new []{string.Empty})
            {
                if (val.IsNotEmpty())
                    sb.AppendFormat("{0}{1}", val.Length, val);
                else
                    sb.Append("-");
            }

            var byte1 = Enumerable.Range(0, FirstComponent.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(FirstComponent.Substring(x, 2), 16))
                     .ToArray();
            
            var byte2 = Enumerable.Range(0, SecondComponent.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(SecondComponent.Substring(x, 2), 16))
                     .ToArray();
            
            var bitesArr = new byte[byte1.Length];
            for (var i = 0; i < byte1.Length; i++)
            {
                bitesArr[i] = (byte)(byte1[i] ^ byte2[i]);
            }
            
            var encoding = Encoding.UTF8;
            var messageBytes = encoding.GetBytes(sb.ToString());
            
            using (var hmacSha256 = new HMACSHA256(bitesArr))
            {
                var hash = hmacSha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hash).ToUpper();
            }
        }

        private static string GenerateNonce()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            
            var bytes = new byte[16];
            rand.NextBytes(bytes);

            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private bool CheckFields(HttpRequest req)
        {
            var fields = new []
            {
                "AMOUNT",
                "CURRENCY",
                "ORDER",
                "DESC",
                "TERMINAL",
                "TRTYPE",
                "MERCH_NAME",
                "MERCHANT",
                // "EMAIL",
                "TIMESTAMP",
                "NONCE",
                "BACKREF",
                "RESULT",
                "P_SIGN",
                // "NAME",
                // "CARD",
            };
            if (fields.Any(val => string.IsNullOrEmpty(req[val])))
                return false;

            //RESULT - Код ответа
            //0 – Операция успешно завершена
            //1 – Запрос идентифицирован как повторный
            //2 – Запрос отклонен Банком
            //3 – Запрос отклонен ПШ
            if (req["RESULT"] != "0")
                return false;

            // порядок из формата
            var values = new NameValueCollection
            {
                {"AMOUNT", req["AMOUNT"]},
                {"CURRENCY", req["CURRENCY"]},
                {"ORDER", req["ORDER"]},
                {"MERCH_NAME", req["MERCH_NAME"]},
                {"MERCHANT", req["MERCHANT"]},
                {"TERMINAL", req["TERMINAL"]},
                {"EMAIL", req["EMAIL"]},
                {"TRTYPE", req["TRTYPE"]},
                {"TIMESTAMP", req["TIMESTAMP"]},
                {"NONCE", req["NONCE"]},
                {"BACKREF", req["BACKREF"]},
                {"RESULT", req["RESULT"]},
                {"RC", req["RC"]},
                {"RCTEXT", req["RCTEXT"]},
                {"AUTHCODE", req["AUTHCODE"]},
                {"RRN", req["RRN"]},
                {"INT_REF", req["INT_REF"]},
            };
            return string.Equals(req["P_SIGN"].Contains('-') ? GetPSign(values) : GetPSign(values).Replace("-", ""), req["P_SIGN"], StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Номер заказа, длиной не меньше шести символов
        /// </summary>
        /// <param name="orderId">Номер заказа</param>
        /// <returns></returns>
        private static string GetOrderIdString(int orderId)
        {
            return orderId.ToString().Length < 6 ? orderId.ToString().PadLeft(6, '0') : orderId.ToString();
        }

        #endregion
    }
}
