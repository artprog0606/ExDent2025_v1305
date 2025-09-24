using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.Modulbank;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Payment
{
    //https://modulbank.ru/support/formation_payment_request
    //https://modulbank.ru/support/sending_receipts
    [PaymentKey("Modulbank")]
    public class Modulbank : PaymentMethod
    {
        public string MerchantId { get; set; }
        private string SecretKey { get; set; }
        public bool DemoMode { get; set; }
        public bool SendReceiptData { get; set; }
        public string Taxation { get; set; }

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
            //get { return UrlStatus.FailUrl | UrlStatus.CancelUrl | UrlStatus.ReturnUrl; }
            get { return UrlStatus.None; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {ModulbankTemplate.MerchantId, MerchantId},
                    {ModulbankTemplate.SecretKey, SecretKey},
                    {ModulbankTemplate.DemoMode, DemoMode.ToString()},
                    {ModulbankTemplate.SendReceiptData, SendReceiptData.ToString()},
                    {ModulbankTemplate.Taxation, Taxation},
                };
            }
            set
            {
                MerchantId = value.ElementOrDefault(ModulbankTemplate.MerchantId);
                SecretKey = value.ElementOrDefault(ModulbankTemplate.SecretKey);
                DemoMode = value.ElementOrDefault(ModulbankTemplate.DemoMode).TryParseBool();
                SendReceiptData = value.ElementOrDefault(ModulbankTemplate.SendReceiptData).TryParseBool();
                Taxation = value.ElementOrDefault(ModulbankTemplate.Taxation);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var receipt = SendReceiptData
                ? order
                    .GetOrderItemsForFiscal(PaymentCurrency)
                    .Select(item => new ReceiptItem()
                    {
                        Name = item.Name,
                        Price = Math.Round(item.Price, 2),
                        Quantity = Math.Round(item.Amount, 3),
                        Taxation = Taxation,
                        Tax = GetVatType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType),
                        PaymentMethod = GetPaymentMethodType(item.PaymentMethodType),
                        PaymentObject = item.PaymentSubjectType.ToString()
                    }).ToList()
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                    .Select(x =>
                    new ReceiptItem
                    {
                        Name = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        Price = Math.Round(x.Sum, 2),
                        Quantity = 1,
                        Taxation = Taxation,
                        Tax = GetVatType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentMethod = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentObject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.Add(new ReceiptItem
                {
                    Name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    Price = Math.Round(orderShippingCostWithDiscount, 2),
                    Quantity = 1,
                    Taxation = Taxation,
                    Tax = GetVatType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    PaymentMethod = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    PaymentObject = order.ShippingPaymentSubjectType.ToString()
                });
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()},
                NullValueHandling = NullValueHandling.Ignore
            };

            var receiptString = receipt != null ? JsonConvert.SerializeObject(receipt, jsonSerializerSettings) : null;
            double sum = 0f;

            sum = Math.Round(
                receipt != null 
                    ? receipt.Sum(item => item.Price * item.Quantity) 
                    : order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency),
                2);

            var inputValues = new NameValueCollection
            {
                {"merchant", MerchantId},
                {"amount", sum.ToString("0.00", CultureInfo.InvariantCulture)},
                {"order_id", order.OrderID.ToString()},
                {"custom_order_id", order.Number},
                {"description", GetOrderDescription(order.Number)},
                {"success_url", SuccessUrl},
                {"fail_url", FailUrl},
                {"cancel_url", CancelUrl},
                {"callback_url", NotificationUrl},
                {"testing", DemoMode ? "1" : "0"},
                {"callback_on_failure", "0"},
                {
                    "client_phone",
                    order.OrderCustomer.StandardPhone.HasValue ? order.OrderCustomer.StandardPhone.ToString() : null
                },
                {"client_email", order.OrderCustomer.Email},
                {"receipt_contact", receiptString != null ? order.OrderCustomer.Email : null},
                {"receipt_items", receiptString},
                {"unix_timestamp", DateTime.UtcNow.ToUnixTime().ToString()},
                {"salt", Guid.NewGuid().ToString("N")},
            };

            inputValues.Add("signature", ModulbankApiService.GetSignature(SecretKey, inputValues));

            var handler = new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://pay.modulbank.ru/pay",
                InputValues = inputValues
            };

            return handler;

        }

        public override string ProcessResponse(HttpContext context)
        {
            return ProcessResponseNotify(context);
        }

        private string ProcessResponseNotify(HttpContext context)
        {
            var signature = ModulbankApiService.GetSignature(
                SecretKey,
                context.Request.Form.AllKeys.ToDictionary(key => key, key => context.Request.Form[key]));

            if (!signature.Equals(context.Request.Form["signature"], StringComparison.Ordinal))
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.End();
                return NotificationMessahges.InvalidRequestData;
            }

            if (context.Request.Form["state"].Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                var transactionId = context.Request.Form["transaction_id"];
                if (transactionId.IsNotEmpty())
                {
                    var service = new ModulbankApiService(MerchantId, SecretKey);
                    var payment = service.GetTransaction(transactionId);

                    // иногда не удается получить данные о платеже в следствии
                    // The request was aborted: Could not create SSL/TLS secure channel.
                    if (payment == null)
                        payment = service.GetTransaction(transactionId);

                    if (payment != null && payment.State.Equals("complete", StringComparison.OrdinalIgnoreCase) && context.Request.Form["order_id"] == payment.OrderId)
                    {
                        var order = OrderService.GetOrder(payment.OrderId.TryParseInt());

                        var orderSum = order?.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency) ?? 0f;
                        if (
                            order != null &&
                            Math.Abs(payment.Amount - (float)Math.Round(orderSum, 2)) < 1)
                        {
                            OrderService.PayOrder(order.OrderID, true);
                            return NotificationMessahges.SuccessfullPayment(order.Number);
                        }
                    }
                }
            }

            return NotificationMessahges.InvalidRequestData;
        }

        private string GetPaymentMethodType(ePaymentMethodType paymentMethodType)
        {
            if (paymentMethodType == ePaymentMethodType.partial_prepayment)
                return "prepayment";

            return paymentMethodType.ToString();
        }

        /*
        none - Без НДС
        vat0 - НДС по ставке 0%
        vat10 - НДС чека по ставке 10%
        vat20 - НДС чека по ставке 20%
        vat110 - НДС чека по расчетной ставке 10%
        vat120 - НДС чека по расчетной ставке 20%
        */
        private string GetVatType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout)
                return "none";

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

            return "none";
        }
    }
}
