//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment.Alfabank;
using AdvantShop.Orders;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    [PaymentKey("Alfabank")]
    public class Alfabank : PaymentMethod
    {
        public const string AlfabankOrderId = "alfabankorderId";
        public const string GatewayUrlTestMode = "https://alfa.rbsuat.com/payment/rest/";
        public const string GatewayUrlTest2Mode = "https://tws.egopay.ru/api/ab/rest/";
        [Obsolete]
        public const string GatewayUrlKz = "https://pay.alfabank.kz/payment/rest/";
        public const string GatewayUrlRu = "https://pay.alfabank.ru/payment/rest/";
        public const string GatewayUrlRuNew = "https://payment.alfabank.ru/payment/rest/";
        public const string GatewayUrlRu2 = "https://ecom.alfabank.ru/api/rest/";
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MerchantLogin { get; set; }
        public bool SendReceiptData { get; set; }
        public string Taxation { get; set; }
        [Obsolete]
        public string UseTestMode { get; set; }
        [Obsolete]
        public string GatewayCountry { get; set; }

        private string _gatewayUrl;
        public string GatewayUrl
        {
            get
            {
                if (_gatewayUrl.IsNullOrEmpty())
                {
                    //поддержка старых настроек
                    if (UseTestMode.TryParseBool(true) ?? true)
                        _gatewayUrl = GatewayUrlTestMode;
                    else if (GatewayCountry == "kz")
                        _gatewayUrl = GatewayUrlKz;
                    else
                        _gatewayUrl = GatewayUrlRu;
                }
                else if (_gatewayUrl == "https://web.rbsuat.com/ab/rest/")
                    _gatewayUrl = GatewayUrlTestMode; // изменился адрес тестового шлюза
                return _gatewayUrl;
            }
            set => _gatewayUrl = value;
        }
        public EnTypeFfd TypeFfd { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl | NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {AlfabankTemplate.UserName, UserName},
                               {AlfabankTemplate.Password, Password},
                               {AlfabankTemplate.MerchantLogin, MerchantLogin},
                               {AlfabankTemplate.UseTestMode, UseTestMode},
                               {AlfabankTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {AlfabankTemplate.Taxation, Taxation},
                               {AlfabankTemplate.GatewayCountry, GatewayCountry},
                               {AlfabankTemplate.GatewayUrl, GatewayUrl},
                               {AlfabankTemplate.TypeFfd, ((byte?)TypeFfd).ToString()},
                           };
            }
            set
            {
                UserName = value.ElementOrDefault(AlfabankTemplate.UserName);
                Password = value.ElementOrDefault(AlfabankTemplate.Password);
                MerchantLogin = value.ElementOrDefault(AlfabankTemplate.MerchantLogin);
                UseTestMode = value.ElementOrDefault(AlfabankTemplate.UseTestMode);
                SendReceiptData = value.ElementOrDefault(AlfabankTemplate.SendReceiptData).TryParseBool();
                Taxation = value.ElementOrDefault(AlfabankTemplate.Taxation);
                GatewayCountry = value.ElementOrDefault(AlfabankTemplate.GatewayCountry);
                GatewayUrl = value.ElementOrDefault(AlfabankTemplate.GatewayUrl);
                TypeFfd = (EnTypeFfd)value.ElementOrDefault(AlfabankTemplate.TypeFfd).TryParseInt((int)EnTypeFfd.Less1_2);
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new AlfabankService(GatewayUrl, UserName, Password, MerchantLogin, useFfd12: TypeFfd == EnTypeFfd.From1_2);
            var response = service.Register(order, GetOrderDescription(order.Number), SendReceiptData, Taxation, PaymentCurrency, SuccessUrl, FailUrl, tax);

            if (response != null)
                return response.FormUrl;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            string alfaOrderId;

            /*
             * Warning
             * orderNumber = context.Request[AlfabankService.ReturnUrlParamNameMerchantOrder]
             * является узвимостью (можно оплатить заказ за 10руб, а потом его использовать для выставления других заказов как оплаченных
            */
            
            if (context.Request["orderId"].IsNotEmpty())
            {
                //returnUrl
                alfaOrderId = context.Request["orderId"];
            }
            else
            {
                // NotificationUrl
                alfaOrderId = context.Request["mdOrder"];
            }
            
            if (alfaOrderId.IsNullOrEmpty())
            {
                return NotificationMessahges.InvalidRequestData;
            }

            var service = new AlfabankService(GatewayUrl, UserName, Password, MerchantLogin, useFfd12: TypeFfd == EnTypeFfd.From1_2);
            var response = service.GetOrderStatus(alfaOrderId, merchantOrderid: null);

            if (response == null 
                || response.ErrorCode != 0 
                || response.OrderStatus != "2")
                return NotificationMessahges.InvalidRequestData;
            
            var orderNumber = response.OrderNumber;
            if (orderNumber.IsNotEmpty()
                && OrderService.GetOrderIdByNumber(orderNumber) == 0 // заказ отсутствует
                && orderNumber.Contains("_")) // номер с постфиксом
            {
                orderNumber = orderNumber.Substring(0, orderNumber.LastIndexOf("_"));
            }
            var order = orderNumber.IsNotEmpty() ? OrderService.GetOrderByNumber(orderNumber) : null;

            if (order == null)
                return NotificationMessahges.InvalidRequestData;

            OrderService.PayOrder(order.OrderID, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
            return NotificationMessahges.SuccessfullPayment(order.Number);
        }
        
        public enum EnTypeFfd
        {
            Less1_2 = 0,
            From1_2 = 1
        }

    }
}
