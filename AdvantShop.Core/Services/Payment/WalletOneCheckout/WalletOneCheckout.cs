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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Taxes;
using AdvantShop.Helpers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;

namespace AdvantShop.Payment
{
    //
    // Документация: https://www.walletone.com/ru/merchant/documentation/#step5
    //
    [PaymentKey("WalletOneCheckout")]
    public class WalletOneCheckout : PaymentMethod
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
        public string SecretKey { get; set; }
        public string ExpiredDate { get; set; }
        public string PayWaysEnabled { get; set; }
        public string PayWaysDisabled { get; set; }
        public bool SendReceiptData { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {WalletOneCheckoutTemplate.MerchantId, MerchantId},
                    {WalletOneCheckoutTemplate.SecretKey, SecretKey},
                    {WalletOneCheckoutTemplate.PayWaysEnabled, PayWaysEnabled},
                    {WalletOneCheckoutTemplate.PayWaysDisabled, PayWaysDisabled},
                    {WalletOneCheckoutTemplate.SendReceiptData, SendReceiptData.ToString()},
                };
            }
            set
            {
                MerchantId = value.ElementOrDefault(WalletOneCheckoutTemplate.MerchantId);
                SecretKey = value.ElementOrDefault(WalletOneCheckoutTemplate.SecretKey);
                PayWaysEnabled = value.ElementOrDefault(WalletOneCheckoutTemplate.PayWaysEnabled, string.Empty);
                PayWaysDisabled = value.ElementOrDefault(WalletOneCheckoutTemplate.PayWaysDisabled, string.Empty);
                SendReceiptData = value.ElementOrDefault(WalletOneCheckoutTemplate.SendReceiptData).TryParseBool();
            }
        }


        public override PaymentForm GetPaymentForm(Order order)
        {
            var merchantParams = GetFormParams(order);
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://wl.walletone.com/checkout/checkout/Index",
                InputValues = merchantParams
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            if (ValidateRequest(req))
            {
                if (int.TryParse(req["WMI_PAYMENT_NO"], out var orderId))
                {
                    if (String.Equals(req["WMI_ORDER_STATE"], "ACCEPTED", StringComparison.OrdinalIgnoreCase))
                    {
                        Order order = OrderService.GetOrder(orderId);
                        if (order != null)
                        {
                            OrderService.PayOrder(orderId, true);
                            return "WMI_RESULT=OK&WMI_DESCRIPTION=Order successfully processed";
                        }
                    }
                }
                return "WMI_RESULT=RETRY&WMI_DESCRIPTION=Invalid Order ID";
            }
            return "WMI_RESULT=RETRY&WMI_DESCRIPTION=Invalid WMI_SIGNATURE";
        }

        private NameValueCollection GetFormParams(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            string sum = Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2).ToInvariantString();

            var merchantParams = new NameValueCollection
            {
                {"WMI_CULTURE_ID", "ru-RU"},
                {"WMI_CURRENCY_ID", paymentCurrency.NumIso3.ToString()},
                {"WMI_DESCRIPTION", "BASE64:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(GetOrderDescription(order.Number)))}, //StringHelper.Translit(GetOrderDescription(order.Number))},
                { "WMI_FAIL_URL", FailUrl},
                {"WMI_MERCHANT_ID", MerchantId},
                {"WMI_PAYMENT_AMOUNT", sum},
                {"WMI_PAYMENT_NO", order.OrderID.ToString()},
                {"WMI_RECIPIENT_LOGIN", order.OrderCustomer.Email},
                {"WMI_SUCCESS_URL", SuccessUrl},
                {"WMI_CUSTOMER_FIRSTNAME",order.OrderCustomer.FirstName},
                {"WMI_CUSTOMER_LASTNAME",order.OrderCustomer.LastName},
                {"WMI_CUSTOMER_EMAIL", order.OrderCustomer.Email},
            };

            var payWay = 
                PayWaysEnabled
                    .Replace(",", "")
                    .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .OrderBy(x => x)// для правильного формирования подписи запроса
                    .ToList();
            if (payWay.Count > 0)
            {
                for (var i = 0; i < payWay.Count; i++)
                {
                    merchantParams.Add("WMI_PTENABLED", payWay[i].Trim());
                }
            }

            var payDisWay = 
                PayWaysDisabled
                    .Replace(",", "")
                    .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .OrderBy(x => x)// для правильного формирования подписи запроса
                    .ToList();
            if (payDisWay.Count > 0)
            {
                for (var i = 0; i < payDisWay.Count; i++)
                {
                    merchantParams.Add("WMI_PTDISABLED", payDisWay[i].Trim());
                }
            }

            if (SendReceiptData)
            {
                if (order.OrderCustomer != null)
                {
                    if (string.IsNullOrEmpty(order.OrderCustomer.Email) &&
                        order.OrderCustomer.StandardPhone != null && order.OrderCustomer.StandardPhone != 0)
                    {
                        merchantParams.Add("WMI_CUSTOMER_PHONE", "+" + order.OrderCustomer.StandardPhone.ToString());
                    }
                }

                var items =
                     order
                         .GetOrderItemsForFiscal(paymentCurrency)
                         .Select(x => new WalletOneCheckoutProductItem()
                         {
                             Title = HttpUtility.HtmlEncode(x.Name).RemoveHtmlEncodedCharacters().Reduce(128),
                             UnitPrice = (float)Math.Round(x.Price, 2),
                             Quantity = (float)Math.Round(x.Amount, 2),
                             SubTotal = (float)Math.Round((x.Price * x.Amount), 2),
                             TaxType = GetTaxType(tax?.TaxType ?? x.TaxType, x.PaymentMethodType),
                             Tax = (float)Math.Round(GetTax(tax?.TaxType ?? x.TaxType, x.Price * x.Amount), 2),
                         })
                         .ToList();

                if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
                {
                    var certTax = TaxService.GetCertificateTax();
                    items.AddRange(order.OrderCertificates
                        .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                        .Select(x =>
                        new WalletOneCheckoutProductItem
                        {
                            Title = HttpUtility.HtmlEncode($"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}".RemoveHtmlEncodedCharacters().Reduce(128)),
                            UnitPrice = (float)Math.Round(x.Sum, 2),
                            SubTotal = (float)Math.Round(x.Sum, 2),
                            Quantity = 1,
                            TaxType = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                            Tax = (float)Math.Round(GetTax(tax?.TaxType ?? certTax?.TaxType, x.Sum), 2),
                        }));
                }

                var orderShippingCostWithDiscount = 
                    order.ShippingCostWithDiscount.
                        ConvertCurrency(order.OrderCurrency, paymentCurrency);
                if (orderShippingCostWithDiscount > 0)
                {
                    items.Add(new WalletOneCheckoutProductItem()
                    {
                        Title = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                        UnitPrice = (float)Math.Round(orderShippingCostWithDiscount, 2),
                        Quantity = 1,
                        SubTotal = (float)Math.Round(orderShippingCostWithDiscount, 2),
                        TaxType = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                        Tax = (float)Math.Round(GetTax(tax?.TaxType ?? order.ShippingTaxType, orderShippingCostWithDiscount), 2),
                    });
                }

                merchantParams.Add("WMI_ORDER_ITEMS", Newtonsoft.Json.JsonConvert.SerializeObject(items));
            }

            var signatureData = new StringBuilder();
            foreach (var key in merchantParams.AllKeys.OrderBy(x => x))
            foreach (var value in merchantParams.GetValues(key) ?? new []{string.Empty})
            {
                signatureData.Append(value);
            }

            // Формирование значения параметра WMI_SIGNATURE
            Byte[] bytes = Encoding.GetEncoding(1251).GetBytes(signatureData + SecretKey);
            Byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string signature = Convert.ToBase64String(hash);
            merchantParams.Add("WMI_SIGNATURE", signature);
            return merchantParams;
        }

        private bool ValidateRequest(HttpRequest req)
        {

            var merchantParams = new Dictionary<string, string>();

            foreach (string key in req.Form.AllKeys)
            {
                if (key.ToLower() != "WMI_SIGNATURE".ToLower() && key.ToLower() != "PaymentMethodID".ToLower())
                {
                    merchantParams.Add(key, req.Form[key]);
                }
            }

            var signatureData = new StringBuilder();
            foreach (string key in merchantParams.Keys.OrderBy(s => s))
            {
                //foreach (var item in merchantParams[key].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                //{
                //    signatureData.Append(item);
                //}
                signatureData.Append(merchantParams[key]);
            }

            // Формирование значения параметра WMI_SIGNATURE
            var sign = signatureData + SecretKey;
            Byte[] bytes = Encoding.GetEncoding(1251).GetBytes(sign);
            Byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string signature = Convert.ToBase64String(hash);

            return signature == req["WMI_SIGNATURE"];
        }

        /*
         tax_ru_1 — без НДС;
         tax_ru_2 — НДС по ставке 0%;
         tax_ru_3 — НДС чека по ставке 10%;
         tax_ru_4 — НДС чека по ставке 18%;
         tax_ru_5 — НДС чека по расчетной ставке 10/110;
         tax_ru_6 — НДС чека по расчетной ставке 18/118.
         tax_ru_7 — НДС чека по ставке 20%;
         tax_ru_8 — НДС чека по расчетной ставке 20/120;
        */
        private string GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return "tax_ru_1";

            if (taxType.Value == TaxType.Vat0)
                return "tax_ru_2";

            if (taxType.Value == TaxType.Vat10)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "tax_ru_5";
                else
                    return "tax_ru_3";
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "tax_ru_6";
                else
                    return "tax_ru_4";
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "tax_ru_8";
                else
                    return "tax_ru_7";
            }

            return "tax_ru_1";
        }

        private float GetTax(TaxType? taxType, float price)
        {
            var vat = 0;

            if (taxType == null || taxType.Value == TaxType.VatWithout)
                vat = 0;
            else
            {
                if (taxType.Value == TaxType.Vat0)
                    vat = 0;

                if (taxType.Value == TaxType.Vat10)
                    vat = 10;

                if (taxType.Value == TaxType.Vat18)
                    vat = 18;

                if (taxType.Value == TaxType.Vat20)
                    vat = 20;

                if (taxType.Value == TaxType.Vat5)
                    vat = 5;

                if (taxType.Value == TaxType.Vat7)
                    vat = 7;
            }

            var tax = price / (100 + vat) * vat;

            return tax;
        }
    }
}