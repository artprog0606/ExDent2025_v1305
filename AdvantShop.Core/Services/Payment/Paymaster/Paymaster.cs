//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Payment
{

    [PaymentKey("Paymaster")]
    public class Paymaster : PaymentMethod
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
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public string MerchantId { get; set; }
        public string SecretWord { get; set; }
        public bool SendReceiptData { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PaymasterTemplate.MerchantId, MerchantId},
                               {PaymasterTemplate.SecretWord, SecretWord},
                               {PaymasterTemplate.SendReceiptData, SendReceiptData.ToString()},
                           };
            }
            set
            {
                MerchantId = value.ElementOrDefault(PaymasterTemplate.MerchantId);
                SecretWord = value.ElementOrDefault(PaymasterTemplate.SecretWord, string.Empty);
                SendReceiptData = value.ElementOrDefault(PaymasterTemplate.SendReceiptData).TryParseBool();
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var merchantParams = GetFormParams(order);
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = " https://paymaster.ru/Payment/Init",
                InputValues = merchantParams
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;

            if (req["LMI_PREREQUEST"] == "1")
            {
                return "YES";
            }

            if (ValidateRequest(req))
            {
                if (int.TryParse(req["LMI_PAYMENT_NO"], out var orderId))
                {
                    Order order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true);
                        return "OK"; // любой ответ со статусом 200
                    }

                }
                throw new Exception("Order #" + req["LMI_PAYMENT_NO"] + " not found"); // обязательно выкидывать Exception. Любой ответ со статусом 200, воспринимается как OK
            }
            throw new Exception("Invalid signature for order #" + req["LMI_PAYMENT_NO"]);
        }

        private NameValueCollection GetFormParams(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            string orderSumStr = Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2).ToInvariantString();

            var merchantParams = new NameValueCollection
            {
                {"LMI_MERCHANT_ID", MerchantId},
                {"LMI_PAYMENT_AMOUNT", orderSumStr},
                {"LMI_CURRENCY", paymentCurrency?.NumIso3.ToString()},
                {"LMI_PAYMENT_NO", order.OrderID.ToString()},
                {"LMI_PAYMENT_DESC", GetOrderDescription(order.Number)},
                {"LMI_INVOICE_CONFIRMATION_URL", NotificationUrl},
                {"LMI_PAYMENT_NOTIFICATION_URL", NotificationUrl},
                {"LMI_SUCCESS_URL", SuccessUrl},
                {"LMI_FAILURE_URL", FailUrl},
                {"LMI_PAYER_EMAIL", order.OrderCustomer.Email},
            };

            if (SendReceiptData)
            {
                var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
                var orderItems = order.GetOrderItemsForFiscal(paymentCurrency, toIntegerAmount: true).ToList();
                var index = 0;
                for (index = 0; index < orderItems.Count; index++)
                {
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS["+ index + "].NAME", orderItems[index].Name);
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].QTY", orderItems[index].Amount.ToString("F3", CultureInfo.InvariantCulture));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].PRICE", orderItems[index].Price.ToString("F2", CultureInfo.InvariantCulture));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].TAX", GetTaxType(tax?.TaxType ?? orderItems[index].TaxType, orderItems[index].PaymentMethodType));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].METHOD", ((int)orderItems[index].PaymentMethodType).ToString());
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].SUBJECT", ((int)orderItems[index].PaymentSubjectType).ToString());
                }

                if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
                {
                    var certTax = TaxService.GetCertificateTax();
                    foreach (var cert in order.OrderCertificates.ConvertCurrency(order.OrderCurrency, paymentCurrency))
                    {
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].NAME", $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {cert.CertificateCode}");
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].QTY", 1f.ToString("F3", CultureInfo.InvariantCulture));
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].PRICE", cert.Sum.ToString("F2", CultureInfo.InvariantCulture));
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].TAX", GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType));
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].METHOD", ((int)AdvantShop.Configuration.SettingsCertificates.PaymentMethodType).ToString());
                        merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].SUBJECT", ((int)AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType).ToString());

                        index++;
                    }
                }

                var orderShippingCostWithDiscount = 
                    order.ShippingCostWithDiscount
                        .ConvertCurrency(order.OrderCurrency, paymentCurrency);
                if (orderShippingCostWithDiscount > 0)
                {
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].NAME", LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].QTY", 1f.ToString("F3", CultureInfo.InvariantCulture));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].PRICE", orderShippingCostWithDiscount.ToString("F2", CultureInfo.InvariantCulture));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].TAX", GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType));
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].METHOD", ((int)order.ShippingPaymentMethodType).ToString());
                    merchantParams.Add("LMI_SHOPPINGCART.ITEMS[" + index + "].SUBJECT", ((int)order.ShippingPaymentSubjectType).ToString());
                }

            }

            return merchantParams;
        }

        private bool ValidateRequest(HttpRequest req)
        {
            var paramString = new[] { req["LMI_MERCHANT_ID"],
                                      req["LMI_PAYMENT_NO"],
                                      req["LMI_SYS_PAYMENT_ID"],
                                      req["LMI_SYS_PAYMENT_DATE"],
                                      req["LMI_PAYMENT_AMOUNT"],
                                      req["LMI_CURRENCY"],
                                      req["LMI_PAID_AMOUNT"],
                                      req["LMI_PAID_CURRENCY"],
                                      req["LMI_PAYMENT_SYSTEM"],
                                      req["LMI_SIM_MODE"],
                                        }.Select(str => str ?? "").AggregateString(";");

            var strWithPass = paramString + ";" + SecretWord;

            var hashBytes =  new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(strWithPass));

            string signature = Convert.ToBase64String(hashBytes);

            return signature == req["LMI_HASH"];
        }

        private string GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return "no_vat";

            if (taxType.Value == TaxType.Vat0)
                return "vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat110";
                else
                    return "vat10";
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat120";
                else
                    return "vat20";
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat105";
                else
                    return "vat5";
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat107";
                else
                    return "vat7";
            }

            return "no_vat";
        }

    }
}