//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    [PaymentKey("PayAnyWay")]
    public class PayAnyWay : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string Signature { get; set; }
        public bool TestMode { get; set; }
        public string UnitId { get; set; }
        public string LimitIds { get; set; }
        public bool UseKassa { get; set; }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.NotificationUrl | UrlStatus.ReturnUrl | UrlStatus.FailUrl; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl | NotificationType.Handler; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayAnyWayTemplate.MerchantId, MerchantId},
                               {PayAnyWayTemplate.Signature, Signature},
                               {PayAnyWayTemplate.TestMode, TestMode.ToString()},
                               {PayAnyWayTemplate.UnitId, UnitId},
                               {PayAnyWayTemplate.LimitIds, LimitIds},
                               {PayAnyWayTemplate.UseKassa, UseKassa.ToString()},
                           };
            }
            set
            {
                if (value.ContainsKey(PayAnyWayTemplate.MerchantId))
                    MerchantId = value[PayAnyWayTemplate.MerchantId];
                Signature = value.ElementOrDefault(PayAnyWayTemplate.Signature);
                bool boolVal;
                TestMode = !bool.TryParse(value.ElementOrDefault(PayAnyWayTemplate.TestMode), out boolVal) || boolVal;
                UnitId = value.ElementOrDefault(PayAnyWayTemplate.UnitId);
                LimitIds = value.ElementOrDefault(PayAnyWayTemplate.LimitIds);
                UseKassa = value.ElementOrDefault(PayAnyWayTemplate.UseKassa).TryParseBool();
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr =
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .ToString("F2", CultureInfo.InvariantCulture);
            var currencyLabel = paymentCurrency.Iso3;

            var orderStrId = $"{order.OrderID}_{DateTime.Now.ToUnixTime()}";
            var payment = new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://www.moneta.ru/assistant.htm",
                InputValues = new NameValueCollection
                {
                    {"MNT_ID", MerchantId},
                    {"MNT_TRANSACTION_ID", orderStrId},
                    {"MNT_CURRENCY_CODE", currencyLabel},
                    {"MNT_AMOUNT", orderSumStr},
                    {"MNT_TEST_MODE", TestMode ? "1" : "0"},
                    {"MNT_DESCRIPTION", GetOrderDescription(order.Number)},
                    {
                        "MNT_SIGNATURE",
                        string.Format("{0}{1}{2}{3}{4}{5}", MerchantId, orderStrId, orderSumStr, currencyLabel,
                            TestMode ? "1" : "0", Signature).Md5()
                    },
                    {"MNT_SUCCESS_URL", SuccessUrl},
                    {"MNT_FAIL_URL", FailUrl},
                }
            };

            if (!string.IsNullOrEmpty(UnitId))
            {
                payment.InputValues.Add("paymentSystem.unitId", UnitId);
            }

            if (!string.IsNullOrEmpty(LimitIds))
            {
                payment.InputValues.Add("paymentSystem.limitIds", LimitIds);
            }

            //if (UseKassa)
            //{
            //    var productsJson = GetProductsJson(order);

            //    // суммарный объем​ ​CUSTOM​ ​полей​ ​не​ ​должен​ ​превышать​ ​1,5​ ​Килобайта,​ ​
            //    // суммарный​ ​объем​ ​всего​ ​запроса​ ​- не​ ​должен​ ​превышать​ ​2​ ​Килобайта. 
            //    if (Encoding.UTF8.GetBytes(productsJson).Length < 1500)
            //    {
            //        payment.InputValues.Add("MNT_CUSTOM1", "1");
            //        payment.InputValues.Add("MNT_CUSTOM2", productsJson);
            //    }
            //}

            return payment;
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            bool useQuery = req.Form["MNT_TRANSACTION_ID"].IsNullOrEmpty();

            var orderStrId = GetRequestParam(req, "MNT_TRANSACTION_ID", useQuery);
            if (orderStrId.Contains("_"))
                orderStrId = orderStrId.Split('_')[0];
            if (int.TryParse(orderStrId, out var orderId))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
                    var orderSumStr =
                        order.Sum
                            .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                            .ToString("F2", CultureInfo.InvariantCulture);
                    var currencyLabel = paymentCurrency.Iso3;

                    if (CheckData(req, order, orderSumStr, useQuery, currencyLabel) &&
                        GetRequestParam(req, "MNT_AMOUNT", useQuery) == orderSumStr &&
                        GetRequestParam(req, "MNT_CURRENCY_CODE", useQuery) == currencyLabel &&
                        GetRequestParam(req, "MNT_TEST_MODE", useQuery) == (TestMode ? "1" : "0"))
                    {
                        if (!order.Payed)
                            OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));

                        if (UseKassa)
                            SendXmlResponse(context, order);

                        return "SUCCESS";
                    }
                    //Добавил костыль, потому что с сервиса приходит в url только id заказа, подтверждение оплаты заказа приходит не по прямому url, а фоновым запросом.
                    else if (order.Payed)
                    {
                        //if (UseKassa)
                        //    SendXmlResponse(context, order);

                        return "SUCCESS";
                    }
                    Debug.Log.Info("PayAnyWay check data fail " + context.Request.Url.ToString());
                }
            }
            return "FAIL";
        }

        private string GetRequestParam(HttpRequest req, string name, bool useQuery)
        {
            return useQuery ? req[name] : req.Form[name];
        }

        public bool CheckData(HttpRequest req, Order order, string sum, bool useQuery, string currencyLabel)
        {
            var fields = new string[]
            {
                "MNT_ID",
                "MNT_TRANSACTION_ID",
                "MNT_AMOUNT",
                "MNT_CURRENCY_CODE",
                "MNT_TEST_MODE",
                "MNT_SIGNATURE"
            };

            return !fields.Any(val => string.IsNullOrEmpty(GetRequestParam(req, val, useQuery)))
                   && string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                       MerchantId,
                       GetRequestParam(req, "MNT_TRANSACTION_ID", useQuery) ?? "",
                       GetRequestParam(req, "MNT_OPERATION_ID", useQuery) ?? "",
                       sum,
                       currencyLabel,
                       GetRequestParam(req, "MNT_SUBSCRIBER_ID", useQuery) ?? "",
                       TestMode ? "1" : "0",
                       Signature).Md5(false) == GetRequestParam(req, "MNT_SIGNATURE", useQuery);

            //fields.Aggregate<string, StringBuilder, string>(new StringBuilder(), (str, field) => str.Append(field == "MNT_ID" ? MerchantId : field == "MNT_CURRENCY_CODE" ? CurrencyLabel : field == "MNT_SUBSCRIBER_ID" ? req["MNT_SUBSCRIBER_ID"] : field == "MNT_SIGNATURE" ? Signature : req[field]), Strings.ToString).Md5(true) != req["MNT_SIGNATURE"]);
        }

        private string GetProductsJson(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var items =
                    order
                        .GetOrderItemsForFiscal(paymentCurrency)
                        .Select(x => new PayAnyWayItem()
                        {
                            n = HttpUtility.HtmlEncode(x.Name).Reduce(100),
                            p = x.Price.ToString("F2", CultureInfo.InvariantCulture),
                            q = x.Amount.ToString("F2", CultureInfo.InvariantCulture),
                            t = GetTaxType(tax?.TaxType ?? x.TaxType, x.PaymentMethodType)
                        }).ToList();

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new PayAnyWayItem
                    {
                        n = LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"),
                        p = x.Sum.ToString("F2", CultureInfo.InvariantCulture),
                        q = 1.ToString("F2", CultureInfo.InvariantCulture),
                        t = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType)
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                items.Add(new PayAnyWayItem()
                {
                    n = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    p = orderShippingCostWithDiscount.ToString("F2", CultureInfo.InvariantCulture),
                    q = 1.ToString("F2", CultureInfo.InvariantCulture),
                    t = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType)
                });
            }

            var productsJson = JsonConvert.SerializeObject(new { customer = order.OrderCustomer.Email, items = items });

            return productsJson;
        }

        private string GetProductsInXml(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var items =
                    order
                        .GetOrderItemsForFiscal(paymentCurrency)
                        .Select(x => new PayAnyWayItemInXml()
                        {
                            name = x.Name.Reduce(100).Replace("\"", "").XmlEncode(),
                            price = x.Price.ToString("F2", CultureInfo.InvariantCulture),
                            quantity = x.Amount.ToString("F2", CultureInfo.InvariantCulture),
                            vatTag = GetTaxType(tax?.TaxType ?? x.TaxType, x.PaymentMethodType),
                            paymentMethod = GetPaymentMethodType(x.PaymentMethodType),
                            paymentObject = x.PaymentSubjectType.ToString()
                        }).ToList();

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new PayAnyWayItemInXml
                    {
                        name = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        price = x.Sum.ToString("F2", CultureInfo.InvariantCulture),
                        quantity = 1.ToString("F2", CultureInfo.InvariantCulture),
                        vatTag = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        paymentMethod = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        paymentObject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                items.Add(new PayAnyWayItemInXml()
                {
                    name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    price = orderShippingCostWithDiscount.ToString("F2", CultureInfo.InvariantCulture),
                    quantity = 1.ToString("F2", CultureInfo.InvariantCulture),
                    vatTag = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    paymentMethod = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    paymentObject = order.ShippingPaymentSubjectType.ToString()
                });
            }

            var productsJson = JsonConvert.SerializeObject(items);

            return productsJson;
        }

        // https://www.payanyway.ru/info/p/ru/public/merchants/Assistant54FZ.pdf
        private void SendXmlResponse(HttpContext context, Order order)
        {
            var req = context.Request;

            // md5(MNT_RESULT_CODE + MNT_ID + MNT_TRANSACTION_ID + Код проверки целостности данных)
            var signature = ("200" + req["MNT_ID"] + req["MNT_TRANSACTION_ID"] + Signature).Md5();
            var productsJson = GetProductsInXml(order);

            var responseText =
                $@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<MNT_RESPONSE>
<MNT_ID>{req["MNT_ID"]}</MNT_ID>
<MNT_TRANSACTION_ID>{req["MNT_TRANSACTION_ID"]}</MNT_TRANSACTION_ID>
<MNT_RESULT_CODE>200</MNT_RESULT_CODE>
<MNT_SIGNATURE>{signature}</MNT_SIGNATURE>
<MNT_ATTRIBUTES>
<ATTRIBUTE>
<KEY>INVENTORY</KEY>
<VALUE>{productsJson}</VALUE>
</ATTRIBUTE>
<ATTRIBUTE>
<KEY>CUSTOMER</KEY>
<VALUE>{order.OrderCustomer.Email}</VALUE>
</ATTRIBUTE>
<ATTRIBUTE>
<KEY>DELIVERY</KEY>
<VALUE>0</VALUE>
</ATTRIBUTE>
</MNT_ATTRIBUTES>
</MNT_RESPONSE>
";

            context.Response.Clear();
            context.Response.ContentType = "application/xml";
            context.Response.Write(responseText);
            context.Response.End();
        }

        /*
         * 1104 НДС 0%
           1103 НДС 10%
           1102 НДС 18%
           1105 НДС не облагается
           1107 НДС с рассч. ставкой 10%
           1106 НДС с рассч. ставкой 18%
         */
        private string GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null)
                return "1104";

            if (taxType.Value == TaxType.VatWithout)
                return "1105";

            if (taxType.Value == TaxType.Vat0)
                return "1104";

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "1107";
                else
                    return "1103";
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "1106";
                else
                    return "1102";
            }

            return "1104";
        }

        private string GetPaymentMethodType(ePaymentMethodType paymentMethodType)
        {
            switch (paymentMethodType)
            {
                case ePaymentMethodType.full_prepayment:
                    return "full_prepayment";
                case ePaymentMethodType.partial_prepayment:
                    return "prepayment"; // из-за этого значения нельзя enum.Tostring();
                case ePaymentMethodType.advance:
                    return "advance";
                case ePaymentMethodType.full_payment:
                    return "full_payment";
                case ePaymentMethodType.partial_payment:
                    return "partial_payment";
                case ePaymentMethodType.credit:
                    return "credit";
                case ePaymentMethodType.credit_payment:
                    return "credit_payment";
                default:
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in PayAnyWay");
            }
        }
    }
}