//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using AdvantShop.Diagnostics;
using System.Linq;
using AdvantShop.Taxes;
using System.IO;
using Newtonsoft.Json;
using AdvantShop.Core.Services.Payment.SberBankAcquiring;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Сбербанк интернет-эквайринг. 
    /// Документация: https://developer.sberbank.ru/acquiring-api-rest-about
    /// </summary>
    [PaymentKey("SberBankAcquiring")]
    public class SberBankAcquiring : PaymentMethod
    {
        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }


        public string UserName { get; set; }
        public string Password { get; set; }
        public bool TestMode { get; set; }
        public string MerchantLogin { get; set; }
        public enSno Sno { get; set; }
        public string Inn { get; set; }
        public string Email { get; set; }


        public bool SendReceiptData { get; set; }
        public EnTypeFfd TypeFfd { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {SberBankAcquiringTemplate.UserName, UserName},
                    {SberBankAcquiringTemplate.Password, Password},
                    {SberBankAcquiringTemplate.TestMode, TestMode.ToString()},
                    {SberBankAcquiringTemplate.MerchantLogin, MerchantLogin},
                    {SberBankAcquiringTemplate.SendReceiptData, SendReceiptData.ToString()},
                    {SberBankAcquiringTemplate.TypeFfd, ((byte?)TypeFfd).ToString()},
                    {SberBankAcquiringTemplate.Sno, ((byte?)Sno).ToString()},
                    {SberBankAcquiringTemplate.Inn, Inn},
                    {SberBankAcquiringTemplate.Email, Email},
                };
            }
            set
            {
                UserName = value.ElementOrDefault(SberBankAcquiringTemplate.UserName);
                Password = value.ElementOrDefault(SberBankAcquiringTemplate.Password);
                TestMode = value.ElementOrDefault(SberBankAcquiringTemplate.TestMode).TryParseBool();
                MerchantLogin = value.ElementOrDefault(SberBankAcquiringTemplate.MerchantLogin);
                SendReceiptData = value.ElementOrDefault(SberBankAcquiringTemplate.SendReceiptData).TryParseBool();
                TypeFfd = (EnTypeFfd)value.ElementOrDefault(SberBankAcquiringTemplate.TypeFfd).TryParseInt((int)EnTypeFfd.Less1_2);
                Sno = (enSno)value.ElementOrDefault(SberBankAcquiringTemplate.Sno).TryParseInt((int)enSno.osn);
                Inn = value.ElementOrDefault(SberBankAcquiringTemplate.Inn);
                Email = value.ElementOrDefault(SberBankAcquiringTemplate.Email);
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new SberBankAcquiringService(UserName, Password, MerchantLogin, TestMode, SendReceiptData, useFfd12: TypeFfd == EnTypeFfd.From1_2, Email, Inn, Sno.ToString());
            var response = service.Register(order, GetOrderDescription(order.Number), PaymentCurrency, SuccessUrl, FailUrl, tax);

            if (response != null)
                return response.FormUrl;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            string bodyPost = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

            NotifyData notifyData = null;

            if (!string.IsNullOrEmpty(bodyPost))
            {
                try
                {
                    notifyData = JsonConvert.DeserializeObject<NotifyData>(bodyPost);
                }
                catch (Exception ex) { 
                    Debug.Log.Error(bodyPost, ex);
                }
            }

            if (notifyData != null)
            {
                var orderId = notifyData.orderNumber.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                var order = orderId.IsNotEmpty() ? OrderService.GetOrder(orderId.TryParseInt()) : null;

                if (order == null)
                    return NotificationMessahges.InvalidRequestData;


                var service = new SberBankAcquiringService(UserName, Password, MerchantLogin, TestMode, false, false, Email, Inn, Sno.ToString());

                var response = service.GetOrderStatus(notifyData.mdOrder);

                if (response == null)
                {
                    // Если ответ будет с HTTP-кодом 200 OK, сбер воспримет это как успешное уведомление
                    // Нужно вернуть отличное от этого, тогда он повторит уведомления
                    // service.GetOrderStatus может не уложиться в 5сек и вернуть timeout (есть прецеденты)

                    throw new Exception("SberBankAcquiringService: Failed to get a status of order.");
                }
                else if (response.OrderStatus == 2)//Проведена полная авторизация суммы заказа
                {
                    OrderService.PayOrder(order.OrderID, true);
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }

                return "The notification was accepted. The order is not yet paid.";
            }
            else
            {
                throw new Exception("SberBankAcquiringService: Failed to get a status of order.");
            }
        }
        
        public enum EnTypeFfd
        {
            Less1_2 = 0,
            From1_2 = 1
        }


        public enum enSno
        {
            osn = 0,
            usn_income = 1,
            usn_income_outcome = 2,
            esn = 3,
            patent = 4
        }


    }
}