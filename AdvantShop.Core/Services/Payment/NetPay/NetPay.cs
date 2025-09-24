//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.NetPay;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    [PaymentKey("NetPay")]
    public class NetPay : PaymentMethod
    {
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

        public string ApiKey { get; set; }
        public string AuthSign { get; set; }
        public bool TestMode { get; set; }
        public bool SendReceiptData { get; set; }
        public string INN { get; set; }
        public string PaymentAddress { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {NetPayTemplate.ApiKey, ApiKey},
                               {NetPayTemplate.AuthSign, AuthSign},
                               {NetPayTemplate.TestMode, TestMode.ToString()},
                               {NetPayTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {NetPayTemplate.INN, INN},
                               {NetPayTemplate.PaymentAddress, PaymentAddress},
                           };
            }
            set
            {
                ApiKey = value.ElementOrDefault(NetPayTemplate.ApiKey);
                AuthSign = value.ElementOrDefault(NetPayTemplate.AuthSign);
                TestMode = value.ElementOrDefault(NetPayTemplate.TestMode).TryParseBool();
                SendReceiptData = value.ElementOrDefault(NetPayTemplate.SendReceiptData).TryParseBool();
                INN = value.ElementOrDefault(NetPayTemplate.INN);
                PaymentAddress = value.ElementOrDefault(NetPayTemplate.PaymentAddress);
            }
        }
        
        public override PaymentForm GetPaymentForm(Order order)
        {
            //1.	data (шифрованные данные с информацией об оплате)
            //2.	expire (дата в формате YYYY-MM-DD’V’HH:mm:ss)
            //3.	auth (auth код, полученый при регистрации)

            var expire = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd'V'HH:mm:ss");

            return new PaymentForm
            {
                Url = TestMode
                    ? "https://demo.net2pay.ru/billingService/paypage"
                    : "https://my.net2pay.ru/billingService/paypage",
                InputValues = new NameValueCollection
                {
                    {"data", EncodeData(order, expire)},
                    {"expire", HttpUtility.UrlEncode(expire)},
                    {"auth", AuthSign}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            if (req["orderID"].IsNotEmpty())
            {
                if (CheckData(req))
                {
                    var orderId = req["orderID"].TryParseInt();
                    var order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true);
                        return
                            String.Format(
                                "<notification><orderId>{0}</orderId><transactionType>{1}</transactionType><status>1</status></notification>",
                                req["orderID"], req["transactionType"]);
                    }
                }
                return
                    String.Format(
                        "<notification><orderId>{0}</orderId><transactionType>{1}</transactionType><status>0</status><error>{2}</error></notification>",
                        req["orderID"], req["transactionType"], NotificationMessahges.InvalidRequestData);
            }
            return string.Empty;
        }

        public bool CheckData(HttpRequest req)
        {
            return req["orderID"].IsNotEmpty() && req["auth"].IsNotEmpty() && req["auth"] == AuthSign &&
                    req["status"] == "APPROVED" &&
                    req["token"] == GetToken(req);
        }

        public string GetToken(HttpRequest req)
        {
            var token = string.Empty;
            var concat = false;
            foreach (var param in req.QueryString.AllKeys)
            {
                if (param.Equals("orderID", StringComparison.OrdinalIgnoreCase))
                    concat = true;
                if (!concat)
                    continue;
                if (!param.Equals("token", StringComparison.OrdinalIgnoreCase))
                    token += string.Format("{0};", req.QueryString[param]);
            }

            var encoding = Encoding.UTF8;
            token += string.Format("{0};", Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(ApiKey))));

            return token.Md5(false, encoding);
        }

        private string EncodeData(Order order, string expire)
        {
            //- amount - сумма к оплате (для разделения десятичной части числа должна использоваться десятичная точка);
            //- currency - валюта оплаты (в настоящий момент доступны USD, RUB, EUR)
            //- orderID - уникальный идентификатор платежа;
            //- successUrl - ссылка запроса GET, переход по которой будет осуществлен при успешной
            //  оплате (при отсутствии ссылки, переход осуществляется на страницу результата оплаты системы Net Pay);
            //- failUrl - ссылка запроса GET, переход по которой будет осуществлен при неуспешной
            //  оплате (при отсутствии ссылки, переход осуществляется на страницу результата оплаты системы NetPay);

            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSum = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            
            var encoding = Encoding.UTF8;
            var sb = new StringBuilder();

            var apiKeyHashed = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(ApiKey)));
            var cryptoWord = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(apiKeyHashed + expire)));
            
            var cryptoKey = encoding.GetBytes(cryptoWord.Substring(0, 16));

            sb.Append(EncryptStringUseAes(string.Format("amount={0}", orderSum.ToInvariantString()), cryptoKey, cryptoKey));

            var currencyCode = string.Equals(paymentCurrency.Iso3, "RUR", StringComparison.OrdinalIgnoreCase) ? "RUB" : paymentCurrency.Iso3;
            sb.Append("&" + EncryptStringUseAes(string.Format("currency={0}", currencyCode), cryptoKey, cryptoKey));

            sb.Append("&" + EncryptStringUseAes(string.Format("orderID={0}", order.OrderID.ToString()), cryptoKey, cryptoKey));

            sb.Append("&" + EncryptStringUseAes(string.Format("orderNumber={0}", order.Number), cryptoKey, cryptoKey));

            sb.AppendFormat("&" + EncryptStringUseAes(string.Format("successUrl={0}", SuccessUrl), cryptoKey, cryptoKey));

            sb.AppendFormat("&" + EncryptStringUseAes(string.Format("failUrl={0}", FailUrl), cryptoKey, cryptoKey));

            if (SendReceiptData)
            {
                var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
                var cashbox = new Cashbox()
                {
                    timestamp = string.Empty,
                    service = new Service()
                    {
                        callback_url = string.Empty,
                        inn = INN,
                        payment_address = TestMode ? PaymentAddress : SettingsMain.SiteUrlPlain
                    },
                    receipt = new Receipt()
                    {
                        attributes = new CustomerDetails
                        {
                            email = ValidationHelper.IsValidEmail(order.OrderCustomer.Email) ? order.OrderCustomer.Email : string.Empty,
                            phone = string.Empty
                        },


                        items =
                            order
                                .GetOrderItemsForFiscal(paymentCurrency)
                                .Select(item => new Item()
                                {
                                    name = item.Name.Reduce(64),
                                    price = (Math.Round(item.Price, 2)).ToString(CultureInfo.InvariantCulture),
                                    quantity = item.Amount.ToString(CultureInfo.InvariantCulture),
                                    sum = (Math.Round(item.Price * item.Amount, 2)).ToString(CultureInfo.InvariantCulture),
                                    tax = GetVatType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType)
                                }).ToList(),

                        payments = new List<Payments>()
                        {
                            new Payments()
                            {
                                type = 1,
                                sum = orderSum.ToInvariantString()
                            }
                        },

                        total = orderSum.ToInvariantString()
                    }
                };

                if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
                {
                    var certTax = TaxService.GetCertificateTax();
                    cashbox.receipt.items.AddRange(order.OrderCertificates
                        .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                        .Select(x =>
                        new Item
                        {
                            name = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                            price = Math.Round(x.Sum, 2).ToInvariantString(),
                            sum = Math.Round(x.Sum, 2).ToInvariantString(),
                            quantity = "1",
                            tax = GetVatType(tax?.TaxType ?? certTax?.TaxType, SettingsCertificates.PaymentMethodType)
                        }));
                }

                var orderShippingCostWithDiscount = 
                    order.ShippingCostWithDiscount
                        .ConvertCurrency(order.OrderCurrency, paymentCurrency);
                if (orderShippingCostWithDiscount > 0)
                {
                    cashbox.receipt.items.Add(new Item()
                    {
                        name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                        price = Math.Round(orderShippingCostWithDiscount, 2).ToString(CultureInfo.InvariantCulture),
                        sum = Math.Round(orderShippingCostWithDiscount, 2).ToString(CultureInfo.InvariantCulture),
                        quantity = "1",
                        tax = GetVatType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType)
                    });
                }

                sb.AppendFormat("&" + EncryptStringUseAes(string.Format("cashbox_data={0}", JsonConvert.SerializeObject(cashbox)), cryptoKey, cryptoKey));
            }

            if (TestMode)
            {
                sb.Append("&" + EncryptStringUseAes("param1=callback", cryptoKey, cryptoKey));
                sb.Append("&" + EncryptStringUseAes($"value1={NotificationUrl}", cryptoKey, cryptoKey));
            }

            return HttpUtility.UrlEncode(sb.ToString());
        }
        
        private string GetVatType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout)
                return "none";

            if (taxType.Value == TaxType.Vat0)
                return "vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat110";
                else
                    return "vat10";
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat118";
                else
                    return "vat18";
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat105";
                else
                    return "vat5";
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat107";
                else
                    return "vat7";
            }

            return "none";
        }


        private string EncryptStringUseAes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                return string.Empty;
            if (Key == null || Key.Length <= 0)
                return string.Empty;
            if (IV == null || IV.Length <= 0)
                return string.Empty;
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.ECB;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);

        }
    }
}