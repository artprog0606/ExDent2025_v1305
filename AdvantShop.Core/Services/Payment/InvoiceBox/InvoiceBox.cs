//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    //https://www.invoicebox.ru/ru/integration/webapi/paymentform/fields.html
    [PaymentKey("InvoiceBox")]
    public class InvoiceBox : PaymentMethod
    {
        public string ShopId { get; set; }
        public string ShopId2 { get; set; }
        public string SecretKey { get; set; }
        public string TypePayer { get; set; }
        public bool TestMode { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {InvoiceBoxTemplate.ShopId, ShopId},
                               {InvoiceBoxTemplate.ShopId2, ShopId2},
                               {InvoiceBoxTemplate.SecretKey, SecretKey},
                               {InvoiceBoxTemplate.TypePayer, TypePayer},
                               {InvoiceBoxTemplate.TestMode, TestMode.ToString()},
                           };
            }
            set
            {
                ShopId = value.ElementOrDefault(InvoiceBoxTemplate.ShopId);
                ShopId2 = value.ElementOrDefault(InvoiceBoxTemplate.ShopId2);
                SecretKey = value.ElementOrDefault(InvoiceBoxTemplate.SecretKey);
                TypePayer = value.ElementOrDefault(InvoiceBoxTemplate.TypePayer);
                TestMode = value.ElementOrDefault(InvoiceBoxTemplate.TestMode).TryParseBool();
            }
        }

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
            get { return UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var sum = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                .ToInvariantString();

            var currencyLabel = paymentCurrency.Iso3;
            var values = new NameValueCollection
            {
                { "itransfer_participant_id", ShopId},
                { "itransfer_participant_ident", ShopId2},
                { "itransfer_order_id", order.OrderID.ToString()},
                { "itransfer_order_amount", sum},
                { "itransfer_order_currency_ident", currencyLabel},
                { "itransfer_order_description", GetOrderDescription(order.Number)},
                { "itransfer_body_type", TypePayer},
                { "itransfer_person_email", order.OrderCustomer != null && ValidationHelper.IsValidEmail(order.OrderCustomer.Email) ? order.OrderCustomer.Email : string.Empty},
                { "itransfer_url_returnsuccess", SuccessUrl},
                { "itransfer_testmode", TestMode ? "1" : "0"},
                { "itransfer_participant_sign", $"{ShopId}{order.OrderID}{sum}{currencyLabel}{SecretKey}".Md5(false)},
                { "itransfer_url_notify", TestMode ? NotificationUrl : ""},
            };

            int index = 0;
            Product product;
            foreach (var item in order.GetOrderItemsForFiscal(PaymentCurrency))
            {
                index++;

                values.Add($"itransfer_item{index}_name", item.Name);
                values.Add($"itransfer_item{index}_quantity", item.Amount.ToInvariantString());
                values.Add($"itransfer_item{index}_measure", item.Unit.IsNotEmpty()
                    ? item.Unit
                    : "шт.");
                values.Add($"itransfer_item{index}_price", ((float)Math.Round(item.Price, 2)).ToInvariantString());
                values.Add($"itransfer_item{index}_vatrate", GetVatType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType));
                //values.Add(string.Format("itransfer_item{0}_vat", index), GetVatValue(item.TaxType, item.Price));
            }

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();

                foreach (var cert in order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency))
                {
                    index++;
                    values.Add($"itransfer_item{index}_name", LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"));
                    values.Add($"itransfer_item{index}_quantity", "1");
                    values.Add($"itransfer_item{index}_measure", "шт.");
                    values.Add($"itransfer_item{index}_price", cert.Sum.ToInvariantString());
                    values.Add($"itransfer_item{index}_vatrate", GetVatType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType));
                    //values.Add(string.Format("itransfer_item{0}_vat", index), GetVatValue(order.ShippingTaxType, order.ShippingCost));
                }
            }

            var shippingCostWithDiscount =
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (shippingCostWithDiscount > 0)
            {
                index++;

                values.Add($"itransfer_item{index}_name", LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"));
                values.Add($"itransfer_item{index}_quantity", "1");
                values.Add($"itransfer_item{index}_measure", "шт.");
                values.Add($"itransfer_item{index}_price", shippingCostWithDiscount.ToInvariantString());
                values.Add($"itransfer_item{index}_vatrate", GetVatType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType));
                //values.Add(string.Format("itransfer_item{0}_vat", index), GetVatValue(order.ShippingTaxType, order.ShippingCostWithDiscount));
            }

            return new PaymentForm
            {
                Url = "https://go.invoicebox.ru/module_inbox_auto.u",
                InputValues = values
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var request = context.Request;
            if (request["participantId"] != ShopId)
                return NotificationMessahges.InvalidRequestData;

            if (request["participantOrderId"].IsNullOrEmpty())
                return NotificationMessahges.InvalidRequestData;

            if (request["sign"] !=
                string.Concat( 
                        request["participantId"],
                        request["participantOrderId"], 
                        request["ucode"], 
                        request["timetype"], 
                        request["time"],
                        request["amount"], 
                        request["currency"], 
                        request["agentName"], 
                        request["agentPointName"],
                        request["testMode"],
                        SecretKey)
                    .Md5(false, Encoding.UTF8))
                return NotificationMessahges.InvalidRequestData;

            var orderId = context.Request["participantOrderId"].TryParseInt();

            var order = OrderService.GetOrder(orderId);

            if (order != null)
            {
                OrderService.PayOrder(order.OrderID, true);
                return "OK";
            }

            return NotificationMessahges.InvalidRequestData;
        }

        #region Help

        private string GetVatValue(TaxType? taxType, float price)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout || taxType.Value == TaxType.Vat0)
                return "0";

            float rate = 0;
            if (taxType.Value == TaxType.Vat10)
                rate = 10;

            if (taxType.Value == TaxType.Vat18)
                rate = 18;

            if (taxType.Value == TaxType.Vat20)
                rate = 20;

            if (rate > 0)
                // стоимость товара / (100 + ндс) * ндс
                return ((float)Math.Round(price / (100 + rate) * rate, 2)).ToInvariantString();

            return "0";
        }

        /*
        ставка НДС 20% (18%) - 1
        ставка НДС 10% - 2
        ставка НДС расч. 20/120 (18/118) - 3
        ставка НДС расч. 10/110 - 4
        НДС 0% - 5
        НДС не облагается - 6
        */
        private string GetVatType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout)
                return "6";

            if (taxType.Value == TaxType.Vat0)
                return "5";

            if (taxType.Value == TaxType.Vat10)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "4";
                else
                    return "2";
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "3";
                else
                    return "1";
            }

            return "6";
        }

        #endregion
    }
}
