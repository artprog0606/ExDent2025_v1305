//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.YandexKassa;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Saas;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    [PaymentKey("YandexKassa")]
    public class YandexKassa : PaymentMethod, ICreditPaymentMethod
    {
        public const string ProtocolApi = "Api";
        public const string ProtocolWidget = "Widget";
        public string ShopId { get; set; }
        public string ScId { get; set; }
        //public bool DemoMode { get; set; }
        public string YaPaymentType { get; set; }
        public string Password { get; set; }
        private string SecretKey { get; set; }
        public bool SendReceiptData { get; set; }
        //public int VatType { get; set; }
        public string Protocol { get; set; }
        public byte? TaxSystemCode { get; set; }
        public EnTypeFfd TypeFfd { get; set; }
        public float MinimumPrice { get; set; }
        public float? MaximumPrice { get; set; }
        public EnTypePresentationOfCreditInformation TypePresentationOfCreditInformation =>
            EnTypePresentationOfCreditInformation.FirstPayment;
        public float FirstPayment { get; set; }
        public bool ActiveCreditPayment
        {
            get { return YaPaymentType == "installments" || YaPaymentType == "CR"; }
        }
        public bool ShowCreditButtonInProductCard => true;
        public string CreditButtonTextInProductCard => null;


        public override ProcessType ProcessType
        {
            get
            {
                switch (Protocol)
                {
                    case ProtocolApi:
                        return ProcessType.ServerRequest;
                    case ProtocolWidget:
                        return ProcessType.Javascript;
                    default:
                        return ProcessType.FormPost;
                }
            }
        }

        public override NotificationType NotificationType
        {
            get
            {
                return Protocol == ProtocolWidget || Protocol == ProtocolApi
                    ? NotificationType.ReturnUrl | NotificationType.Handler
                    : NotificationType.Handler;
            }
        }

        public override UrlStatus ShowUrls
        {
            get
            {
                return Protocol == ProtocolApi || Protocol == ProtocolWidget
                    ? UrlStatus.NotificationUrl
                    : UrlStatus.FailUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl;
            }
        }

        public override string NotificationUrl
        {
            get
            {
                return Protocol == ProtocolApi || Protocol == ProtocolWidget || !SaasDataService.IsSaasEnabled
                    ? base.NotificationUrl.Replace("http://", "https://")
                    : "https://gate.advantshop.net/yandexkassa/" + StringHelper.EncodeTo64(base.NotificationUrl)
                                .Replace("/", "-slash-").Replace("+", "-plus-").Replace("=", "-equal-");
            }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[] {"RUB"};

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {YandexKassaTemplate.ShopID, ShopId},
                               {YandexKassaTemplate.ScID, ScId},
                               //{YandexKassaTemplate.DemoMode, DemoMode.ToString()},
                               {YandexKassaTemplate.YaPaymentType, YaPaymentType},
                               {YandexKassaTemplate.Password, Password},
                               {YandexKassaTemplate.SendReceiptData, SendReceiptData.ToString()},
                               //{YandexKassaTemplate.VatType, VatType.ToString()}
                               {YandexKassaTemplate.Protocol, Protocol},
                               {YandexKassaTemplate.MinimumPrice, MinimumPrice.ToInvariantString()},
                               {YandexKassaTemplate.MaximumPrice, MaximumPrice?.ToInvariantString()},
                               {YandexKassaTemplate.FirstPayment, FirstPayment.ToInvariantString()},
                               {YandexKassaTemplate.TaxSystemCode, TaxSystemCode?.ToString()},
                               {YandexKassaTemplate.TypeFfd, ((byte?)TypeFfd).ToString()},
                           };
            }
            set
            {
                ShopId = value.ElementOrDefault(YandexKassaTemplate.ShopID);
                ScId = value.ElementOrDefault(YandexKassaTemplate.ScID);
                YaPaymentType = value.ElementOrDefault(YandexKassaTemplate.YaPaymentType);
                Password = value.ElementOrDefault(YandexKassaTemplate.Password);
                SecretKey = value.ElementOrDefault(YandexKassaTemplate.Password);
                //DemoMode = value.ElementOrDefault(YandexKassaTemplate.DemoMode).TryParseBool();
                SendReceiptData = value.ElementOrDefault(YandexKassaTemplate.SendReceiptData).TryParseBool();
                //VatType = value.ElementOrDefault(YandexKassaTemplate.VatType).TryParseInt();
                var isNewMethod = !value.ContainsKey(YandexKassaTemplate.ScID) && !value.ContainsKey(YandexKassaTemplate.Protocol);
                Protocol = value.ElementOrDefault(YandexKassaTemplate.Protocol).IsNullOrEmpty() && isNewMethod
                    ? ProtocolApi
                    : value.ElementOrDefault(YandexKassaTemplate.Protocol);
                MinimumPrice = value.ElementOrDefault(YandexKassaTemplate.MinimumPrice).TryParseFloat();
                MaximumPrice = value.ElementOrDefault(YandexKassaTemplate.MaximumPrice).TryParseFloat(true);
                FirstPayment = value.ElementOrDefault(YandexKassaTemplate.FirstPayment).TryParseFloat();
                TaxSystemCode = (byte?)value.ElementOrDefault(YandexKassaTemplate.TaxSystemCode).TryParseInt(true);
                TypeFfd = (EnTypeFfd)value.ElementOrDefault(YandexKassaTemplate.TypeFfd).TryParseInt((int)EnTypeFfd.Less1_2);
            }
        }

        public float? GetFirstPayment(float finalPrice)
        {
            return finalPrice * FirstPayment / 100;
        }

        public (float AmountPyament, int NumberOfPayments) GetAmountAndNumberOfPayments(float finalPrice) => default;

        public override BasePaymentOption GetOption(BaseShippingOption shippingOption, float preCoast, CustomerType? customerType)
        {
            BasePaymentOption option = null;
            if (Protocol == ProtocolApi && YaPaymentType == "mobile_balance")
            {
                option = new YandexKassaWithPhonePaymentOption(this, preCoast);
            }

            if (option == null)
                option = base.GetOption(shippingOption, preCoast, customerType);

            if (ActiveCreditPayment)
            {
                try
                {
                    var creditPreSchedule = RequestHelper.MakeRequest<CreditPreSchedule>(
                        string.Format(
                            "https://yoomoney.ru/credit/order/ajax/credit-pre-schedule?shopId={0}&sum={1}",
                            HttpUtility.UrlEncode(ShopId), HttpUtility.UrlEncode(preCoast.ToInvariantString())),
                        method: ERequestMethod.GET);

                    if (creditPreSchedule.creditPercent > 0 || creditPreSchedule.term > 0 ||
                        creditPreSchedule.amount > 0 || creditPreSchedule.totalAmount > 0)
                        option.Desc = (option.Desc ?? string.Empty) + string.Format(
                                          " Кредит под {0}% в месяц на {1} месяцев с платежом {2} в месяц. Полная сумма {3}.",
                                          creditPreSchedule.creditPercent,
                                          creditPreSchedule.term,
                                          creditPreSchedule.amount,
                                          creditPreSchedule.totalAmount);
                }
                catch (Exception e)
                {
                    Debug.Log.Warn(e);
                }
            }

            return option;
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (Protocol == ProtocolWidget && !context.Request.Url.AbsolutePath.Contains("paymentnotification"))
                return ProcessResponseByOrder(context);
            
            if (Protocol == ProtocolApi && context.Request.Url.AbsolutePath.Contains("paymentreturnurl"))
                return ProcessResponseByApiOrder(context);
            
            return Protocol == ProtocolApi || Protocol == ProtocolWidget
                ? ProcessResponseByJson(context)
                : ProcessResponseByForm(context);
        }

        #region New Api

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new YandexKassaApiService(ShopId, SecretKey);
            var confirmation = new CreatePaymentConfirmationRedirect()
            {
                // URL, на который вернется пользователь после подтверждения или отмены платежа.
                // На SuccessUrl проверим оплачен заказ или нет ProcessResponseByApiOrder
                ReturnUrl = $"{SuccessUrl}?c={order.Code}"
            };
            var payment = 
                SendReceiptData
                    ? service.CreatePaymentWithReceipt(order, 
                                                        YaPaymentType, GetOrderDescription(order.Number), 
                                                        null, 
                                                        PaymentCurrency, 
                                                        tax, 
                                                        confirmation, 
                                                        TaxSystemCode, 
                                                        sendMeasure: TypeFfd == EnTypeFfd.From1_2)
                    : service.CreatePayment(order, 
                                            YaPaymentType, 
                                            GetOrderDescription(order.Number), 
                                            null,
                                            PaymentCurrency, confirmation);

            if (payment != null)
                return ((PaymentConfirmationRedirect)payment.Confirmation).ConfirmationUrl;

            return "";
        }

        private string ProcessResponseByJson(HttpContext context)
        {
            string bodyPost = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

            if (!string.IsNullOrEmpty(bodyPost))
            {

                var service = new YandexKassaApiService(ShopId, SecretKey);

                var notify = service.ReadNotifyData(bodyPost);

                if (notify == null || notify.Event != "payment.succeeded" || notify.Object == null || notify.Object.Metadata.OrderNumber.IsNullOrEmpty())
                {
                    return NotificationMessahges.InvalidRequestData;
                }

                var payment = service.GetPayment(notify.Object.Id);

                // иногда не удается получить данные о платеже в следствии
                // The request was aborted: Could not create SSL/TLS secure channel.
                if (payment == null)
                    payment = service.GetPayment(notify.Object.Id);

                if (payment != null && payment.Status == "succeeded")
                {
                    var order = OrderService.GetOrderByNumber(payment.Metadata.OrderNumber);

                    if (
                        order != null &&
                        Math.Abs(payment.Amount.Value - order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)) < 1)
                    {
                        OrderService.PayOrder(order.OrderID, true);
                    }
                }
            }

            return "";
        }

        private string ProcessResponseByApiOrder(HttpContext context)
        {
            Order order;
            if (context.Request["c"].IsNullOrEmpty() ||
                (order = OrderService.GetOrderByCode(context.Request["c"])) == null)
            {
                return null;
            }

            return order.Payed 
                ? NotificationMessahges.SuccessfullPayment(order.Number)
                : null;
        }

        #endregion

        #region Widget

        public override string ProcessJavascript(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new YandexKassaApiService(ShopId, SecretKey);
            var confirmation = new CreatePaymentConfirmationEmbedded();
            var payment = SendReceiptData
                ? service.CreatePaymentWithReceipt(order, YaPaymentType, GetOrderDescription(order.Number), false, PaymentCurrency, tax, confirmation, TaxSystemCode, sendMeasure: TypeFfd == EnTypeFfd.From1_2)
                : service.CreatePayment(order, YaPaymentType, GetOrderDescription(order.Number), false, PaymentCurrency, confirmation);

            if (payment == null)
                return "";

            var script = $@"<script type='text/javascript' src='https://yookassa.ru/checkout-widget/v1/checkout-widget.js'></script>
                <script type='text/javascript'>
                    var yooMoneyRenderTimer;
                    function yooMoneyRenderForm () {{
                        document.getElementById('yooMoney-loading').classList.remove('ng-hide');
                        if (yooMoneyRenderTimer != null) {{
                            clearTimeout(yooMoneyRenderTimer);
                        }}
                        if (typeof(window.YooMoneyCheckoutWidget) == 'undefined') {{
                            yooMoneyRenderTimer = setTimeout(function() {{
                                yooMoneyRenderForm();
                            }}, 100);
                        }} else {{
                            const yooMoneyCheckout = new window.YooMoneyCheckoutWidget({{
                                confirmation_token: '{((PaymentConfirmationEmbedded)payment.Confirmation).ConfirmationToken}',
                                return_url: '{SuccessUrl}?orderNum={order.Number}',
                                error_callback(error) {{
                                    console.log('yooMoney: ' + error);
                                }}
                            }});
                            yooMoneyCheckout.render('yooMoney-payment-form').then(function () {{
                                document.getElementById('yooMoney-loading').classList.add('ng-hide');
                            }});
                        }}
                    }};
                    yooMoneyRenderForm();
                </script>
                <style type=""text/css"">.btn--pay {{ display: none!important; }}</style>
                <div class=""flex center-xs text-center m-lg"" id=""yooMoney-loading"">
                    <span class=""icon-spinner-before icon-animate-spin-before h3"">  {LocalizationService.GetResource("Core.Payment.YandexKassa.Loading")}</span>
                </div>                                
                <div id=""yooMoney-payment-form""></div>";

            return script;
        }

        private string ProcessResponseByOrder(HttpContext context)
        {
            Order order;
            if (context.Request["orderNum"].IsNullOrEmpty() || (order = OrderService.GetOrderByNumber(context.Request["orderNum"])) == null)
                return NotificationMessahges.InvalidRequestData;

            var service = new YandexKassaApiService(ShopId, SecretKey);
            var payment = service.GetPaymentByOrder(order.OrderID);
            if (payment == null)
                return null;

            if (payment.Status == "succeeded")
            {
                if (Math.Abs(payment.Amount.Value - order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)) < 1)
                {
                    OrderService.PayOrder(order.OrderID, true);
                }
                return NotificationMessahges.SuccessfullPayment(order.Number);
            }

            var msg = payment != null && payment.CancellationDetails != null ? payment.CancellationDetails.GetErrorMessage() : null;
            
            return NotificationMessahges.Fail + (msg.IsNotEmpty() ? ": " + msg : null);
        }

        #endregion

        #region Old Api

        public override PaymentForm GetPaymentForm(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var receipt = SendReceiptData ? new Receipt()
            {
                customer =  new Customer() {
                    email = order.OrderCustomer.Email.IsNotEmpty() ? order.OrderCustomer.Email : null,
                    phone = order.OrderCustomer.StandardPhone.ToString().Length == 11 ? ("+" + order.OrderCustomer.StandardPhone.ToString()) : null },
                
                items = 
                    order
                        .GetOrderItemsForFiscal(paymentCurrency)
                        .Select(item => new Item()
                        {
                            text = item.Name.Length > 100 ? item.Name.Substring(0, 100) : item.Name,
                            price = new Price { amount = item.Price },
                            quantity = (float)Math.Round(item.Amount, 2),
                            tax = GetVatType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType),
                            paymentMethodType = item.PaymentMethodType,
                            paymentSubjectType = item.PaymentSubjectType
                        })
                        .ToList()
            }
            : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        price = new Price() {amount = x.Sum},
                        quantity = 1,
                        tax = GetVatType(
                            tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        text = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        paymentMethodType = AdvantShop.Configuration.SettingsCertificates.PaymentMethodType,
                        paymentSubjectType = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.items.Add(new Item()
                {
                    price = new Price() { amount = orderShippingCostWithDiscount },
                    quantity = 1,
                    tax = GetVatType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    text = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    paymentMethodType = order.ShippingPaymentMethodType,
                    paymentSubjectType = order.ShippingPaymentSubjectType
                });
            }

            var orderSum = 
                SendReceiptData 
                    ? receipt.items.Sum(x => x.price.amount * x.quantity) 
                    : order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency);

            var handler = new PaymentForm
            {
                Url = /*DemoMode ? "https://demomoney.yandex.ru/eshop.xml" :*/ "https://yoomoney.ru/eshop.xml",
                Method = FormMethod.POST,
                InputValues =
                {
                    {"shopId", ShopId},
                    {"scid", ScId},
                    {"sum", (orderSum).ToString("F2", CultureInfo.InvariantCulture)},
                    {"customerNumber", order.OrderCustomer.Email.IsNotEmpty() && order.OrderCustomer.Email.Length <= 128 ? order.OrderCustomer.Email.Normalize() : order.OrderCustomer.CustomerID.ToString().Normalize()},
                    {"orderNumber", order.OrderID.ToString(CultureInfo.InvariantCulture).Normalize()},
                    {"shopSuccessURL", HttpUtility.UrlEncode(SuccessUrl)},
                    {"shopFailURL", HttpUtility.UrlEncode(FailUrl)},
                    {"cps_email", order.OrderCustomer.Email.IsNotEmpty() ? order.OrderCustomer.Email : null},
                    {"paymentType", YaPaymentType},
                    {
                        "cps_phone",
                        order.OrderCustomer.StandardPhone.HasValue && order.OrderCustomer.StandardPhone.ToString().Length <= 15
                            ? order.OrderCustomer.StandardPhone.ToString()
                            : null
                    },
                    {"cms_name", "AdVantShop.NET"},
                    {"ym_merchant_receipt", receipt != null ?  JsonConvert.SerializeObject(receipt,  new JsonSerializerSettings() { NullValueHandling= NullValueHandling.Ignore }) : null }
                }
            };

            // https://tech.yandex.ru/money/doc/payment-solution/payment-form/payment-form-http-docpage/
            if (YaPaymentType == "KV")
            {
                if (SendReceiptData && receipt != null)
                {
                    for (var i = 0; i < receipt.items.Count; i++)
                    {
                        handler.InputValues.Add("goods_name_" + i, receipt.items[i].text.Reduce(255));
                        handler.InputValues.Add("goods_quantity_" + i, receipt.items[i].quantity.ToString("F2").Replace(",", "."));
                        handler.InputValues.Add("goods_cost_" + i, receipt.items[i].price.amount.ToString("F2").Replace(",", "."));
                        //handler.InputValues.Add("category_code_" + i, "11111");
                        handler.InputValues.Add("goods_description_" + i, receipt.items[i].text.Reduce(255));
                    }
                }
                else
                {
                    for (var i = 0; i < order.OrderItems.Count; i++)
                    {
                        handler.InputValues.Add("goods_name_" + i, order.OrderItems[i].Name.Reduce(255));
                        handler.InputValues.Add("goods_quantity_" + i, order.OrderItems[i].Amount.ToString("F2").Replace(",", "."));
                        handler.InputValues.Add("goods_cost_" + i, order.OrderItems[i].Price.ToString("F2").Replace(",", "."));
                        //handler.InputValues.Add("category_code_" + i, "11111");
                        handler.InputValues.Add("goods_description_" + i, order.OrderItems[i].Name.Reduce(255));
                    }
                }
                handler.InputValues.Add("seller_id", ShopId);
                handler.InputValues.Add("fixed_term", false.ToString());
            }

            return handler;
        }

        private string ProcessResponseByForm(HttpContext context)
        {
            var typeRequest = TypeRequestYandex.checkOrder;
            var processingResult = ProcessingResult.ErrorParsing;
            var invoiceId = string.Empty;
            string error = string.Empty;

            try
            {
                ProcessingMd5(context, out processingResult, out typeRequest, out invoiceId, out error);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                processingResult = ProcessingResult.Exception;
            }

            var result = RendAnswer(typeRequest, processingResult, invoiceId, error);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/xml";
            context.Response.Write(result);
            //context.Response.End();
            return string.Empty;
        }

        private string RendAnswer(TypeRequestYandex typeRequest, ProcessingResult processingResult, string invoiceId, string error)
        {
            return string.Format(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><{0}Response performedDatetime=\"{1}\" code=\"{2}\" invoiceId=\"{3}\" shopId=\"{4}\" {5}/>",
                typeRequest, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fzzz"), (int)processingResult,
                invoiceId, ShopId,
                error.IsNotEmpty() ? "ErrorAdvantShop=\"" + error + "\"" : string.Empty);
        }

        private bool IsCheckFields(Dictionary<string, string> parameters, TypeRequestYandex typeRequest)
        {
            if (parameters["shopid"].Equals(ShopId, StringComparison.InvariantCultureIgnoreCase) &&
                parameters["invoiceid"].IsNotEmpty() && parameters["invoiceid"].All(char.IsDigit) &&
                parameters["ordernumber"].IsNotEmpty() && parameters["ordernumber"].All(char.IsDigit) &&
                parameters["ordersumamount"].IsNotEmpty() &&
                decimal.TryParse(parameters["ordersumamount"], NumberStyles.Float, CultureInfo.InvariantCulture, out var orderSumAmount))
            {
                var ord = OrderService.GetOrder(parameters["ordernumber"].TryParseInt());

                if (ord != null &&
                    // Если это запрос "Уведомление о переводе", которые могут повторяться несколько раз (упомянуто в документации),
                    // тогда неважно заказ был уже отмечен оплаченным или уже отменен
                    (typeRequest == TypeRequestYandex.paymentAviso || (!ord.Payed && ord.OrderStatusId != OrderStatusService.CanceledOrderStatus)) &&
                    //ord.OrderCustomer.CustomerID.ToString().Normalize().Equals(parameters["customernumber"], StringComparison.InvariantCultureIgnoreCase) &&
                    //orderSumAmount >= Math.Round((decimal)(ord.Sum * GetCurrencyRate(ord.OrderCurrency)), 2)
                    Math.Abs(orderSumAmount - (decimal)Math.Round(ord.Sum.ConvertCurrency(ord.OrderCurrency, PaymentCurrency ?? ord.OrderCurrency), 2)) < 1
                    )
                {
                    return true;
                }
            }
            return false;
        }

        #region NVP/MD5

        private void ProcessingMd5(HttpContext context, out ProcessingResult processingResult,
            out TypeRequestYandex typeRequest, out string invoiceId, out string error)
        {
            var parameters = ReadParametersMd5(context, out typeRequest);

            invoiceId = parameters.ContainsKey("invoiceid") ? parameters["invoiceid"] : string.Empty;

            if (IsCheckMd5(parameters, out error))
            {
                if (IsCheckFields(parameters, typeRequest))
                {
                    if (typeRequest == TypeRequestYandex.paymentAviso)
                        OrderService.PayOrder(parameters["ordernumber"].TryParseInt(), true);

                    processingResult = ProcessingResult.Success;
                }
                else
                {
                    processingResult = typeRequest == TypeRequestYandex.checkOrder
                        ? ProcessingResult.TranslationFailure
                        : ProcessingResult.ErrorParsing;

                }
            }
            else
                processingResult = ProcessingResult.ErrorAuthorize;
        }

        private Dictionary<string, string> ReadParametersMd5(HttpContext context, out TypeRequestYandex typeRequest)
        {
            typeRequest = TypeRequestYandex.unknown;

            if (context.Request["action"].IsNotEmpty())
            {
                if (context.Request["action"].Equals("checkOrder", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.checkOrder;
                else if (context.Request["action"].Equals("paymentAviso", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.paymentAviso;
            }

            return context.Request.Params.AllKeys.ToDictionary(key => key.ToLower(), key => context.Request[key]);
        }

        private bool IsCheckMd5(Dictionary<string, string> parameters, out string error)
        {
            bool isValid = true;
            error = string.Empty;

            if (!parameters.ContainsKey("action"))
            {
                error += "Нет поля action. ";
                isValid = false;
            }

            if (!parameters.ContainsKey("ordersumamount"))
            {
                error += "Нет поля ordersumamount. ";
                isValid = false;
            }
            if (!parameters.ContainsKey("ordersumcurrencypaycash"))
            {
                error += "Нет поля ordersumcurrencypaycash. ";
                isValid = false;
            }
            if (!parameters.ContainsKey("ordersumbankpaycash"))
            {
                error += "Нет поля ordersumbankpaycash. ";
                isValid = false;
            }
            if (!parameters.ContainsKey("shopid"))
            {
                error += "Нет поля shopid. ";
                isValid = false;
            }
            if (!parameters.ContainsKey("invoiceid"))
            {
                error += "Нет поля invoiceid. ";
                isValid = false;
            }
            if (!parameters.ContainsKey("customernumber"))
            {
                error += "Нет поля customernumber. ";
                isValid = false;
            }

            if (!isValid)
                return false;


            string md5before = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                       parameters["action"],
                       parameters["ordersumamount"],
                       parameters["ordersumcurrencypaycash"],
                       parameters["ordersumbankpaycash"],
                       parameters["shopid"],
                       parameters["invoiceid"],
                       parameters["customernumber"],
                       Password);

            string md5 = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                       parameters["action"],
                       parameters["ordersumamount"],
                       parameters["ordersumcurrencypaycash"],
                       parameters["ordersumbankpaycash"],
                       parameters["shopid"],
                       parameters["invoiceid"],
                       parameters["customernumber"],
                       Password).Md5(false);

            if (parameters["md5"].ToLower() != md5before.Md5(false))
            {
                error = "Неверная цифровая подпись MD5. Возможно неверный пароль. md5before=" + md5before;
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        /*
         Без НДС - 1
         НДС по ставке 0% - 2
         НДС чека по ставке 10% - 3
         НДС чека по ставке 18% - 4
         НДС чека по расчетной ставке 10/110 - 5
         НДС чека по расчетной ставке 18/118 - 6
        */
        private int GetVatType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null)
                return 1;

            if (taxType.Value == TaxType.VatWithout)
                return 1;

            if (taxType.Value == TaxType.Vat0)
                return 2;

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 5;
                else
                    return 3;
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 6;
                else
                    return 4;
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 9;
                else
                    return 7;
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 10;
                else
                    return 8;
            }

            return 1;
        }

        private enum TypeRequestYandex
        {
            //Do not change the register
            checkOrder,
            paymentAviso,
            unknown
        }

        private enum ProcessingResult : int
        {
            Success = 0,
            ErrorAuthorize = 1,
            TranslationFailure = 100,
            ErrorParsing = 200,
            Exception = 1000
        }

        private class Receipt
        {
            //public string customerContact { get; set; }
            public Customer customer { get; set; }
            public List<Item> items { get; set; }
        }

        private class Customer
        {
            public string email { get; set; }
            public string phone { get; set; }
        }

        private class Item
        {
            public float quantity { get; set; }
            public Price price { get; set; }
            public int tax { get; set; }
            public string text { get; set; }
            public ePaymentSubjectType paymentSubjectType { get; set; }
            public ePaymentMethodType paymentMethodType { get; set; }
        }

        private class Price
        {
            public float amount { get; set; }
            public string currency { get { return "RUB"; } }
        }

        #endregion

        #region CreditPreSchedule

        public class CreditPreSchedule
        {
            public long term { get; set; }
            public float creditPercent { get; set; }
            public float amount { get; set; }
            public float totalAmount { get; set; }
        }

        #endregion
        
        public enum EnTypeFfd
        {
            Less1_2 = 0,
            From1_2 = 1
        }
    }
}