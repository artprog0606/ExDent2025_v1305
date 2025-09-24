using System;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Diagnostics;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;

namespace AdvantShop.Payment
{

    [PaymentKey("CloudPayments")]
    public class CloudPayments : PaymentMethod
    {
        public string PublicId { get; set; }
        public string ApiSecret { get; set; }
        public string Site { get; set; }

        public bool SendReceiptData { get; set; }
        public int TaxationSystem { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
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
                               {CloudPaymentsTemplate.PublicId, PublicId},
                               {CloudPaymentsTemplate.APISecret, ApiSecret},
                    {CloudPaymentsTemplate.Site, Site},
                    {CloudPaymentsTemplate.SendReceiptData, SendReceiptData.ToString()},
                    {CloudPaymentsTemplate.TaxationSystem, TaxationSystem.ToString()}
                           };
            }
            set
            {
                PublicId = value.ElementOrDefault(CloudPaymentsTemplate.PublicId);
                ApiSecret = value.ElementOrDefault(CloudPaymentsTemplate.APISecret);
                Site = value.ElementOrDefault(CloudPaymentsTemplate.Site);
                SendReceiptData = value.ElementOrDefault(CloudPaymentsTemplate.SendReceiptData).TryParseBool();
                TaxationSystem = value.ElementOrDefault(CloudPaymentsTemplate.TaxationSystem).TryParseInt();
            }
        }

      

        public override string ProcessJavascript(Order order)
        {
            string format = @"<script src='https://widget.{8}/bundles/cloudpayments'></script>
                            <script>this.pay = function () {{
                            var widget = new cp.CloudPayments();
                            widget.charge({{ 
                            publicId: '{0}',
                            description: '{1}',
                            amount: {2},
                            currency: '{3}',
                            invoiceId: '{4}',
                            accountId: '{5}',
                            onSuccess: '{6}',
                            onFail: '{7}',
                            data: {9}
                            }},
                        function(options) {{ 
                            window.location = '{6}';
                            }},
                        function(reason, options) {{ 
                            window.location = '{7}'; 
                            }});
                        }};
                </script>";
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var receipt = new
            {
                cloudPayments = new
                {
                    customerReceipt = new CloudPaymentsCustomerReceipt
                    {
                        Items = order
                            .GetOrderItemsForFiscal(paymentCurrency)
                            .Select(item => new CloudPaymentsItem()
                            {
                                label = item.Name.Reduce(100),
                                price = item.Price,
                                quantity = item.Amount,
                                amount = item.Price * item.Amount,
                                vat = GetTaxType(tax != null ? tax.TaxType : item.TaxType, tax?.Rate ?? item.TaxRate, item.PaymentMethodType),                            
                                method = (int)item.PaymentMethodType,
                                @object = (int)item.PaymentSubjectType,
                                measurementUnit = item.Unit,
                                unitCode = GetMeasure(item.MeasureType)
                            }).ToList(),
                        taxationSystem = TaxationSystem,
                        email = order.OrderCustomer.Email,
                        phone = order.OrderCustomer.StandardPhone.ToString()
                    }
                }
            };

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.cloudPayments.customerReceipt.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new CloudPaymentsItem
                    {
                        label = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        price = x.Sum,
                        quantity = 1,
                        amount = x.Sum,
                        vat = GetTaxType(tax?.TaxType ?? certTax?.TaxType, tax?.Rate ?? certTax?.Rate ?? 0f, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        method = (int)AdvantShop.Configuration.SettingsCertificates.PaymentMethodType,
                        @object = (int)AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType,
                        measurementUnit = "шт",
                        unitCode = GetMeasure(MeasureType.Piece)
                    }));
            }

            var shippingCostWithDiscount = order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (shippingCostWithDiscount > 0)
            {
                receipt.cloudPayments.customerReceipt.Items.Add(new CloudPaymentsItem()
                {
                    label = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    price = shippingCostWithDiscount,
                    quantity = 1,
                    amount = shippingCostWithDiscount,
                    vat = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, tax?.Rate, order.ShippingPaymentMethodType),
                    method = (int)order.ShippingPaymentMethodType,
                    @object = (int)order.ShippingPaymentSubjectType,
                    measurementUnit = "шт",
                    unitCode = GetMeasure(MeasureType.Piece)
                });
            }

            var result = string.Format(format, PublicId, "Оплата заказа № " + order.Number,
                order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency).ToString("F2", CultureInfo.InvariantCulture),
                PaymentCurrency.Iso3, order.Number, order.OrderCustomer.Email, SuccessUrl, FailUrl, Site,
                SendReceiptData ? JsonConvert.SerializeObject(receipt) : "null");

            return result;
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "javascript:pay();";
        }

        public override string ButtonText => "Оплатить";

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;


            string orderNumber = req["InvoiceId"];
            var order = OrderService.GetOrderByNumber(orderNumber);
            if (order == null)
            {
                Debug.Log.Info($"Order {orderNumber} not found");
                return JsonConvert.SerializeObject(new { code = $"Order {orderNumber} not found"});
            }

            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSum = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (Math.Round(orderSum, 2) != Math.Round(req["Amount"].TryParseFloat(), 2))
            {
                Debug.Log.Info($"Order sum is {Math.Round(orderSum, 2)}, not {req["Amount"]}");
                return JsonConvert.SerializeObject(new { 
                    code = $"Order sum is {Math.Round(orderSum, 2)}, not {req["Amount"]}"
                });
            }

            if (!paymentCurrency.Iso3.Equals(req["Currency"], StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log.Info($"Order Currency is {paymentCurrency.Iso3}, not {req["Currency"]}");
                return JsonConvert.SerializeObject(new { 
                    code = $"Order Currency is {paymentCurrency.Iso3}, not {req["Currency"]}"
                });
            }

            var parameters = req.Url.Query.TrimStart('?');
            if (parameters.IsNullOrEmpty())
            {
                System.IO.MemoryStream memstream = new System.IO.MemoryStream();
                req.InputStream.CopyTo(memstream);
                memstream.Position = 0;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(memstream))
                {
                    parameters = reader.ReadToEnd().TrimStart('?');
                }
            }

            var myToken = CreateToken(parameters, ApiSecret);

            if (!myToken.Equals(req.Headers["Content-HMAC"], StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log.Info($"My hash is {myToken}, headers hash is {req.Headers["Content-HMAC"]} for request: {parameters}");
                return JsonConvert.SerializeObject(new { 
                    code = "Hash is invalid"
                });
            }
            
            OrderService.PayOrder(order.OrderID, true);

            return JsonConvert.SerializeObject(new { code = 0 });

        }

        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /*
        null или не указано — НДС не облагается
        20 — НДС 20%
        18 — НДС 18%
        10 — НДС 10%
        0 — НДС 0%
        110 — расчетный НДС 10/110
        118 — расчетный НДС 18.118             
        120 — расчетный НДС 20/120             
        */
        private int? GetTaxType(TaxType? taxType, float? taxRate, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return null;

            if (taxType.Value == TaxType.Vat0)
                return 0;

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 110;
                else
                    return 10;
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 118;
                else
                    return 18;
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 120;
                else
                    return 20;
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 105;
                else
                    return 5;
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 107;
                else
                    return 7;
            }

            if (taxType.Value == TaxType.Other &&
                taxRate.HasValue)
            {
                return (int)taxRate.Value;
            }

            return null;
        }
        
        private byte? GetMeasure(MeasureType? itemMeasureType)
        {
            if (itemMeasureType is null)
                return null;
            
            switch (itemMeasureType.Value)
            {
                case MeasureType.Piece:
                    return 0;
                case MeasureType.Gram:
                    return 10;
                case MeasureType.Kilogram:
                    return 11;
                case MeasureType.Ton:
                    return 12;
                case MeasureType.Centimetre:
                    return 20;
                case MeasureType.Decimeter:
                    return 21;
                case MeasureType.Metre:
                    return 22;
                case MeasureType.SquareCentimeter:
                    return 30;
                case MeasureType.SquareDecimeter:
                    return 31;
                case MeasureType.SquareMeter:
                    return 32;
                case MeasureType.Milliliter:
                    return 40;
                case MeasureType.Liter:
                    return 41;
                case MeasureType.CubicMeter:
                    return 42;
                case MeasureType.KilowattHour:
                    return 50;
                case MeasureType.Gigacaloria:
                    return 51;
                case MeasureType.Day:
                    return 70;
                case MeasureType.Hour:
                    return 71;
                case MeasureType.Minute:
                    return 72;
                case MeasureType.Second:
                    return 73;
                case MeasureType.Kilobyte:
                    return 80;
                case MeasureType.Megabyte:
                    return 81;
                case MeasureType.Gigabyte:
                    return 82;
                case MeasureType.Terabyte:
                    return 83;
                case MeasureType.Other:
                    return 255;
            }

            return null;
        }
    }

    public class CloudPaymentsCustomerReceipt
    {
        public List<CloudPaymentsItem> Items { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int taxationSystem { get; set; }

    }

    public class CloudPaymentsItem
    {
        public string label { get; set; }
        public float price { get; set; }
        public float quantity { get; set; }
        public float amount { get; set; }
        public int? vat { get; set; } // null, 0, 10, 18
        public int method { get; set; }
        public int @object {get; set;}
        public string measurementUnit { get; set; }
        public byte? unitCode { get; set; }
    }
}