//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Core.Services.Payment.Tinkoff;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Tinkoff интернет-эквайринг. 
    /// Документация: https://oplata.tinkoff.ru/landing/develop/documentation/termins_and_operations
    /// </summary>
    [PaymentKey("Tinkoff")]
    public class Tinkoff : PaymentMethod
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
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }


        public string TerminalKey { get; set; }
        public string SecretKey { get; set; }
        public bool SendReceiptData { get; set; }
        public string Taxation { get; set; }
        public EnTypeFfd TypeFfd { get; set; }
        public bool AuthorizedIsPaid { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {TinkoffTemplate.TerminalKey, TerminalKey},
                    {TinkoffTemplate.SecretKey, SecretKey},
                    {TinkoffTemplate.SendReceiptData, SendReceiptData.ToString()},
                    {TinkoffTemplate.Taxation, Taxation},
                    {TinkoffTemplate.TypeFfd, ((byte?)TypeFfd).ToString()},
                    {TinkoffTemplate.AuthorizedIsPaid, AuthorizedIsPaid.ToString()},
                };
            }
            set
            {
                TerminalKey = value.ElementOrDefault(TinkoffTemplate.TerminalKey);
                SecretKey = value.ElementOrDefault(TinkoffTemplate.SecretKey);
                SendReceiptData = value.ElementOrDefault(TinkoffTemplate.SendReceiptData).TryParseBool();
                Taxation = value.ElementOrDefault(TinkoffTemplate.Taxation);
                TypeFfd = (EnTypeFfd)value.ElementOrDefault(TinkoffTemplate.TypeFfd).TryParseInt((int)EnTypeFfd.Less1_2);
                AuthorizedIsPaid = value.ElementOrDefault(TinkoffTemplate.AuthorizedIsPaid).TryParseBool();
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new TinkoffService(TerminalKey, SecretKey, SendReceiptData, useFfd12: TypeFfd == EnTypeFfd.From1_2);
            var response = service.Init(order, GetOrderDescription(order.Number), Taxation, PaymentCurrency, tax);

            if (response != null)
                return response.PaymentURL;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            string bodyPost = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

            if (!string.IsNullOrEmpty(bodyPost))
            {

                var service = new TinkoffService(TerminalKey, SecretKey, SendReceiptData, useFfd12: TypeFfd == EnTypeFfd.From1_2);

                var notify = service.ReadNotifyData(bodyPost);

                if (notify == null || notify.OrderId.IsNullOrEmpty() || notify.TerminalKey != TerminalKey)
                {
                    return NotificationMessahges.InvalidRequestData;
                }

                if (notify.Token.IsNullOrEmpty() || notify.Token != service.GenerateToken(service.AsDictionary(notify)))
                {
                    return NotificationMessahges.InvalidRequestData;
                }

                if (notify.Success.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    var isPaid = notify.Status == "CONFIRMED";
                    if (!isPaid && AuthorizedIsPaid)
                        isPaid = notify.Status == "AUTHORIZED";

                    if (isPaid)
                    {
                        var order = notify.OrderId.IsNotEmpty()
                            ? OrderService.GetOrder(notify.OrderId.Split(new[] {'_'})[0].TryParseInt())
                            : null;

                        if (order != null)
                        {
                            if (Math.Abs((notify.Amount.TryParseFloat() / 100f) - order.Sum.ConvertCurrency(order.OrderCurrency,
                                    PaymentCurrency ?? order.OrderCurrency)) < 1)
                                OrderService.PayOrder(order.OrderID, true,
                                    changedBy: new OrderChangedBy(
                                        notify.Status == "AUTHORIZED"
                                            ? "Подтверждение холдирования оплаты платежной системой"
                                            : "Подтверждение оплаты платежной системой"));
                            else
                                return "AMOUNTS DO NOT MATCH";
                        }
                    }
                }
            }

            return "OK";
        }
        
        public enum EnTypeFfd
        {
            Less1_2 = 0,
            From1_2 = 1
        }

    }
}
