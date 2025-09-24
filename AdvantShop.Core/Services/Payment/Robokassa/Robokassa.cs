//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.Robokassa;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for Robokassa
    /// </summary>
    [PaymentKey("Robokassa")]
    public class Robokassa : PaymentMethod, ICreditPaymentMethod
    {
        public const string ProtocolForm = "Form";
        public const string ProtocolIframe = "Iframe";
        #region Receipt

        private class Receipt
        {
            public List<Item> items { get; set; }
        }

        private class Item
        {
            public string name { get; set; }
            public float quantity { get; set; }
            public float sum { get; set; }
            public string tax { get; set; }
            public string payment_method { get; set; }
            public string payment_object { get; set; }
        }

        /*
        none – без НДС;
        vat0 – НДС по ставке 0%;
        vat10 – НДС чека по ставке 10%;
        vat18 – НДС чека по ставке 18%;
        vat110 – НДС чека по расчетной ставке 10/110;
        vat118 – НДС чека по расчетной ставке 18/118.
        */
        private string GetVatType(TaxType? taxType, float? taxRate, ePaymentMethodType paymentMethodType)
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

            if (taxType.Value == TaxType.Vat18)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat118";
                else
                    return "vat18";
            }

            if (taxType.Value == TaxType.Vat20)
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

            if (taxType.Value == TaxType.Other &&
                taxRate.HasValue)
                return "vat" + taxRate;

            return "none";
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
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in Robokassa");
            }
        }

        #endregion

        public override ProcessType ProcessType
        {
            get
            {
                switch (Protocol)
                {
                    case ProtocolForm:
                        return ProcessType.FormPost;
                    case ProtocolIframe:
                        return ProcessType.Javascript;
                    default:
                        return ProcessType.FormPost;
                }
            }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }
        public string MerchantLogin { get; set; }
        public string Password { get; set; }
        public string PasswordNotify { get; set; }
        public List<string> CurrencyLabels { get; set; }
        public bool SendReceiptData { get; set; }
        public bool IsTest { get; set; }
        public float Fee { get; set; }
        public string GatewayCountry { get; set; }
        public string Protocol { get; set; }

        #region CreditPayment

        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment
        {
            get
            {
                return CurrencyLabels?.Any(currencyLabel => currencyLabel.StartsWith("AlwaysYes")
                       || currencyLabel.StartsWith("OTP")
                       || currencyLabel.StartsWith("Podeli")) is true;
            }
        }
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;

        #endregion

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {RobokassaTemplate.MerchantLogin, MerchantLogin},
                               {RobokassaTemplate.CurrencyLabels, CurrencyLabels == null ? string.Empty : string.Join(",", CurrencyLabels)},
                               {RobokassaTemplate.Password, Password},
                               {RobokassaTemplate.PasswordNotify, PasswordNotify},
                               {RobokassaTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {RobokassaTemplate.IsTest, IsTest.ToString()},
                               {RobokassaTemplate.Fee, Fee.ToInvariantString()},
                               {RobokassaTemplate.GatewayCountry, GatewayCountry},
                               {RobokassaTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {RobokassaTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {RobokassaTemplate.FirstPayment, FirstPayment.ToInvariantString()},
                               {RobokassaTemplate.Protocol, Protocol},
                           };
            }
            set
            {
                if (value.ContainsKey(RobokassaTemplate.MerchantLogin))
                    MerchantLogin = value[RobokassaTemplate.MerchantLogin];
                Password = value.ElementOrDefault(RobokassaTemplate.Password);
                PasswordNotify = value.ElementOrDefault(RobokassaTemplate.PasswordNotify);
                CurrencyLabels = value.ContainsKey(RobokassaTemplate.CurrencyLabels)
                                    ? value[RobokassaTemplate.CurrencyLabels]?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                                    : null;
                SendReceiptData = value.ElementOrDefault(RobokassaTemplate.SendReceiptData).TryParseBool();
                IsTest = value.ElementOrDefault(RobokassaTemplate.IsTest).TryParseBool();
                Fee = value.ElementOrDefault(RobokassaTemplate.Fee).TryParseFloat();
                GatewayCountry = value.ElementOrDefault(RobokassaTemplate.GatewayCountry);
                MinimumPrice = value.ElementOrDefault(RobokassaTemplate.MinimumPrice).TryParseFloat();
                MaximumPrice = value.ElementOrDefault(RobokassaTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(RobokassaTemplate.FirstPayment).TryParseFloat();
                Protocol = value.ElementOrDefault(RobokassaTemplate.Protocol, ProtocolForm);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            
            var sum = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency).SubtractFee(Fee);

            bool isRobomarket = SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData?.Name?.ToLower().Contains("robomarket") == true;

            var receiptString = GetStringReceiptData(order, paymentCurrency);

            var handler = new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = GetGatewayUrl(GatewayCountry),
                InputValues = new NameValueCollection
                {
                    {"MrchLogin", MerchantLogin},
                    {"OutSum", sum.ToInvariantString()},
                    {"InvId", order.OrderID.ToString()},
                    {"Desc", GetOrderDescription(order.Number)},
                    {"IsTest", IsTest ? "1" : "0"},
                    {"Culture", Culture.Language == Culture.SupportLanguage.Russian ? "ru" : "en"},
                    {"ResultUrl2", NotificationUrl},
                    {"SuccessUrl2", SuccessUrl},
                    {"SuccessUrl2Method", "POST" },
                    {"FailUrl2", CancelUrl},
                    {"FailUrl2Method", "POST"},
                    {"shp_partner", "API_Advantshop"},


                    {
                        "SignatureValue",
                        (MerchantLogin + ":"
                                       + sum.ToInvariantString() + ":" 
                                       + order.OrderID + ":" 
                                       + (receiptString.IsNotEmpty() ? receiptString + ":" : string.Empty) 
                                       + NotificationUrl + ":" 
                                       + SuccessUrl + ":" 
                                       + "POST" + ":" 
                                       + CancelUrl + ":" 
                                       + "POST" + ":" 
                                       + Password + ":" 
                                       + "shp_partner=API_Advantshop"
                                       + (isRobomarket ? ":shp_robomarket=true" : "")).Md5()
                    },
                    {"receipt", receiptString }
                }
            };

            if (isRobomarket)
            {
                handler.InputValues.Add("shp_robomarket", "true");
            }


            if (order.OrderCustomer?.Email != null)
                handler.InputValues.Add("Email", order.OrderCustomer.Email);

            CurrencyLabels?.ForEach(x => handler.InputValues.Add("PaymentMethods", x));

            return handler;
        }

        #region Widget

        public override string ProcessJavascript(Order order)
        {
            return "<script type=\"text/javascript\" src=\"https://auth.robokassa.ru/Merchant/bundle/robokassa_iframe.js\"></script>";
        }

        public override string ProcessJavascriptButton(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var sum = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency).SubtractFee(Fee);
            bool isRobomarket = SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData?.Name?.ToLower().Contains("robomarket") == true;
            var receiptString = GetStringReceiptData(order, paymentCurrency);
            var signature = string.Format("{0}:{1}:{2}:{3}{4}:shp_partner=API_Advantshop{5}",
                MerchantLogin,
                sum.ToInvariantString(),
                order.OrderID,
                receiptString.IsNotEmpty() 
                    ? receiptString + ":" : 
                    string.Empty,
                Password,
                isRobomarket ? ":shp_robomarket=true" : string.Empty).Md5();

            return string.Format(@"Robokassa.{8}({{
                MerchantLogin: '{0}',
                OutSum: '{1}',
                InvId: {2},
                shp_partner: 'API_Advantshop',
                {3}
                Culture: '{4}',
                Encoding: 'utf-8',
                {5}
                {6}
                SignatureValue: '{7}'}})",
                MerchantLogin,
                sum,
                order.OrderID,
                isRobomarket ? "shp_robomarket:'true'," : "",
                Culture.Language == Culture.SupportLanguage.Russian ? "ru" : "en",
                CurrencyLabels?.Count > 0 ? $"Settings: JSON.stringify({{PaymentMethods:['{string.Join("','", CurrencyLabels)}'], Mode:'modal'}})," : string.Empty,
                receiptString.IsNotEmpty() ? $"Receipt: '{receiptString}'," : string.Empty,
                signature,
                CurrencyLabels?.Count > 0 ? "Render" : "StartPayment"
                );
        }

        #endregion

        private string GetStringReceiptData(Order order, Repository.Currencies.Currency paymentCurrency)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            if (!SendReceiptData)
                return null;

            var receipt = new Receipt
            {
                items =
                    order
                        .GetOrderItemsForFiscal(paymentCurrency)
                        .Select(item => new Item()
                        {
                            name = item.Name.Reduce(64),
                            sum = (float)Math.Round(item.Price * item.Amount, 2),
                            quantity = item.Amount,
                            tax = GetVatType(tax?.TaxType ?? item.TaxType, tax?.Rate ?? item.TaxRate, item.PaymentMethodType),
                            payment_method = GetPaymentMethodType(item.PaymentMethodType),
                            payment_object = item.PaymentSubjectType.ToString()
                        })
                        .ToList()
            };

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        name = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        sum = x.Sum,
                        quantity = 1,
                        tax = GetVatType(tax?.TaxType ?? certTax?.TaxType, tax?.Rate ?? certTax?.Rate ?? 0f, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        payment_method = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        payment_object = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount =
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                receipt?.items.Add(new Item
                {
                    name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    sum = orderShippingCostWithDiscount,
                    quantity = 1,
                    tax = GetVatType(tax?.TaxType ?? order.ShippingTaxType, tax?.Rate, order.ShippingPaymentMethodType),
                    payment_method = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    payment_object = order.ShippingPaymentSubjectType.ToString()
                });
            }

            return receipt != null ? HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(receipt)) : null;
        }

        private string GetGatewayUrl(string gatewayCountry)
        {
            switch (gatewayCountry)
            {
                case "ru":
                    return "https://auth.robokassa.ru/Merchant/Index.aspx";
                
                case "kz":
                    return "https://auth.robokassa.kz/Merchant/Index.aspx";
            }
            return "https://auth.robokassa.ru/Merchant/Index.aspx";
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (context.Request.Url.AbsolutePath.Contains("paymentnotification"))
                return ProcessResponseNotify(context);
            return ProcessResponseReturn(context);
        }

        private string ProcessResponseReturn(HttpContext context)
        {
            var req = context.Request;
            int orderId = 0;

            if (int.TryParse(req["InvId"], out orderId))
            {
                if (CheckFields(req))
                {

                    Order order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                        return NotificationMessahges.SuccessfullPayment(orderId.ToString());
                    }
                }
                return NotificationMessahges.InvalidRequestData;
            }
            return string.Empty;
        }

        private bool CheckFields(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["OutSum"]) || string.IsNullOrEmpty(req["InvId"]) || string.IsNullOrEmpty(req["Culture"]) ||
                string.IsNullOrEmpty(req["SignatureValue"]))
                return false;
            if (req["SignatureValue"].ToLower() !=
                (req["OutSum"].Trim() + ":" + req["InvId"] + ":" + Password).Md5(false))
                return false;
            return true;
        }

        private string ProcessResponseNotify(HttpContext context)
        {
            var req = context.Request;
            int orderId = 0;
            if (CheckFieldsExt(req) && int.TryParse(req["InvId"], out orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    return string.Format("OK{0}", req["InvId"]);
                }
            }
            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFieldsExt(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["OutSum"]) || string.IsNullOrEmpty(req["InvId"]) || string.IsNullOrEmpty(req["SignatureValue"]))
                return false;
            if (req["SignatureValue"].ToLower() !=
                (req["OutSum"].Trim() + ":" + req["InvId"] + ":" + PasswordNotify + 
                (string.IsNullOrEmpty(req["shp_partner"]) ? "" : ":" + "shp_partner=API_Advantshop") +
                (string.IsNullOrEmpty(req["shp_robomarket"]) ? "" : ":" + "shp_robomarket=true")
                ).Md5(false))
                return false;
            return true;
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;
    }
}