//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;


namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for Assist
    /// </summary>
    [PaymentKey("Assist")]
    public class Assist : PaymentMethod
    {
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[]
        {
            "AUD", "BYR", "DKK", "USD", "EUR", "ISK", "KZT", "CAD", "CNY",
            "TRY", "NOK", "RUB", "XDR", "SGD", "UAH", "GBP", "SEK", "CHF",
            "JPY"
        };

        public int MerchantID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string UrlWorkingMode { get; set; }
        public string UrlTestMode { get; set; }

        //public string ReturnUrl { get; set; }
        //public string CancelUrl { get; set; }
        //public string FailUrl { get; set; }
        public bool Sandbox { get; set; }
        public bool Delay { get; set; }

        public bool CardPayment { get; set; }
        public bool WebMoneyPayment { get; set; }
        public bool PayCashPayment { get; set; }
        public bool QiwiBeelinePayment { get; set; }
        public bool AssistIdCcPayment { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {AssistTemplate.MerchantID, MerchantID.ToString()},
                               {AssistTemplate.Login, Login},
                               {AssistTemplate.Password, Password},
                               {AssistTemplate.UrlWorkingMode, UrlWorkingMode},
                               {AssistTemplate.UrlTestMode, UrlTestMode},
                               //{AssistTemplate.ReturnUrl, ReturnUrl},
                               //{AssistTemplate.CancelUrl, CancelUrl},
                               //{AssistTemplate.FailUrl, FailUrl},
                               {AssistTemplate.Sandbox, Sandbox.ToString()},
                               {AssistTemplate.Delay, Delay.ToString()},
                               //{AssistTemplate.CardPayment, CardPayment.ToString()},
                               //{AssistTemplate.WebMoneyPayment, WebMoneyPayment.ToString()},
                               //{AssistTemplate.PayCashPayment, PayCashPayment.ToString()},
                               //{AssistTemplate.QiwiBeelinePayment, QiwiBeelinePayment.ToString()},
                               //{AssistTemplate.AssistIdCcPayment, AssistIdCcPayment.ToString()}
                           };
            }
            set
            {
                MerchantID = value.ElementOrDefault(AssistTemplate.MerchantID).TryParseInt();
                Login = value.ElementOrDefault(AssistTemplate.Login);
                Password = value.ElementOrDefault(AssistTemplate.Password);
                UrlWorkingMode = value.ElementOrDefault(AssistTemplate.UrlWorkingMode);
                UrlTestMode = value.ElementOrDefault(AssistTemplate.UrlTestMode);
                //ReturnUrl = !value.ContainsKey(AssistTemplate.ReturnUrl) ? SettingsMain.SiteUrl : value[AssistTemplate.ReturnUrl];
                //CancelUrl = !value.ContainsKey(AssistTemplate.CancelUrl) ? SettingsMain.SiteUrl : value[AssistTemplate.CancelUrl];
                //FailUrl = !value.ContainsKey(AssistTemplate.FailUrl) ? SettingsMain.SiteUrl : value[AssistTemplate.FailUrl];
                
                Sandbox = value.ElementOrDefault(AssistTemplate.Sandbox).TryParseBool();
                Delay = value.ElementOrDefault(AssistTemplate.Delay).TryParseBool();


                //CardPayment = !bool.TryParse(value.ElementOrDefault(AssistTemplate.CardPayment), out boolVal) || boolVal;
                //WebMoneyPayment = !bool.TryParse(value.ElementOrDefault(AssistTemplate.WebMoneyPayment), out boolVal) || boolVal;
                //PayCashPayment = !bool.TryParse(value.ElementOrDefault(AssistTemplate.PayCashPayment), out boolVal) || boolVal;
                //QiwiBeelinePayment = !bool.TryParse(value.ElementOrDefault(AssistTemplate.QiwiBeelinePayment), out boolVal) || boolVal;
                //AssistIdCcPayment = !bool.TryParse(value.ElementOrDefault(AssistTemplate.AssistIdCcPayment), out boolVal) || boolVal;
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSum = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            return new PaymentForm
            {
                FormName = "pay",
                Method = FormMethod.POST,
                //Url = string.Format("https://{0}/pay/order.cfm", Sandbox ? "test.paysecure.ru" : PayUrl),//"secure.paysecure.ru"
                Url = Sandbox ? UrlTestMode : UrlWorkingMode, //"secure.paysecure.ru"
                InputValues = new NameValueCollection
                {
                    {"Merchant_ID", MerchantID.ToString()},
                    {"OrderNumber", order.Number},
                    {"Delay", Delay ? "1" : "0"},
                    {"OrderAmount", orderSum.ToInvariantString()},
                    {"OrderCurrency", paymentCurrency.Iso3},
                    {"URL_RETURN", CancelUrl},
                    {"URL_RETURN_OK", SuccessUrl},
                    {"URL_RETURN_NO", FailUrl},
                    //{"CardPayment", CardPayment ? "1" : "0"},
                    //{"WMPayment", WebMoneyPayment ? "1" : "0"},
                    //{"PayCashPayment", PayCashPayment ? "1" : "0"},
                    //{"QIWIBeelinePayment", QiwiBeelinePayment ? "1" : "0"},
                    //{"AssistIDCCPayment", AssistIdCcPayment ? "1" : "0"},
                    {"TestMode", Sandbox ? "1" : "0"}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (Sandbox)
                return NotificationMessahges.TestMode;
            if (string.IsNullOrEmpty(context.Request["ordernumber"]) && string.IsNullOrWhiteSpace(context.Request["billnumber"]))
                return NotificationMessahges.InvalidRequestData;
            string orderNumber = context.Request["ordernumber"];
            string billnumber = context.Request["billnumber"];

            try
            {
                if (Delay)
                {
                    //if 2 stage 
                    var client = new WebClient();
                    var data =
                        client.UploadValues(
                        //string.Format("https://{0}/charge/charge.cfm", Sandbox ? "test.paysecure.ru" : PayUrl),//"secure.paysecure.ru"
                            (Sandbox ? UrlTestMode : UrlWorkingMode).Replace("pay/order.cfm", "charge/charge.cfm"),//"secure.paysecure.ru"
                            new NameValueCollection
                            {
                                {"Billnumber",billnumber},
                                {"Merchant_ID", MerchantID.ToString()},
                                {"Login", Login},
                                {"Password", Password},
                                //XML
                                {"Format", "3"}
                            });


                    var xml = new XmlDocument();
                    var temp = Encoding.UTF8.GetString(data).ToLower();
                    var start = temp.IndexOf("<!doctype");
                    var end = temp.IndexOf("]>");
                    temp = temp.Remove(start, (end + 2) - start);
                    xml.LoadXml(temp);
                    if (xml.DocumentElement != null && xml.DocumentElement.Name != "result")
                        throw new Exception("Invalid XML response");
                    if (xml.DocumentElement != null)
                    {
                        var orders = xml.DocumentElement.SelectNodes(string.Format("descendant::order[ordernumber='{0}' and response_code='AS000' and orderstate='Approved' ]", orderNumber.ToLower()));
                        if (orders != null && orders.Count > 0)
                            OrderService.PayOrder(OrderService.GetOrderIdByNumber(orderNumber), true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                        else
                            return NotificationMessahges.InvalidRequestData;
                    }
                }
                else
                {
                    //if 1 stage
                    OrderService.PayOrder(OrderService.GetOrderIdByNumber(orderNumber), true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                }
                return NotificationMessahges.SuccessfullPayment(orderNumber);
            }
            catch (Exception ex)
            {
                return NotificationMessahges.LogError(ex);
            }
        }
    }
}