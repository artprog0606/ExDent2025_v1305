using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Payment.QiwiKassa;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    /// <summary>
    /// https://developer.qiwi.com/ru/bill-payments/
    /// </summary>
    [PaymentKey("QiwiKassa")]
    public class QiwiKassa : PaymentMethod
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
        public string SecrectKey { get; set; }
        public string NewNotifyKey { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {QiwiKassaTemplate.SecrectKey, SecrectKey},
                    {QiwiKassaTemplate.NewNotifyKey, NewNotifyKey},
                };
            }
            set
            {
                SecrectKey = value.ElementOrDefault(QiwiKassaTemplate.SecrectKey);
                NewNotifyKey = value.ElementOrDefault(QiwiKassaTemplate.NewNotifyKey);
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new QiwiKassaApiService(SecrectKey);
            var response = service.CreateBill(
                order,
                GetOrderDescription(order.Number),
                PaymentCurrency,
                tax,
                new Dictionary<string, string>()
                {
                    {"apiClient", "AdvantShop"},
                    {"apiClientVersion", SettingsGeneral.SiteVersionDev}
                });

            if (response != null)
            {
                var uriBuilder = new UriBuilder(response.PayUrl);
                var parameters = HttpUtility.ParseQueryString(uriBuilder.Query);
                parameters["successUrl"] = SuccessUrl;
                //parameters["paySource"] = qw,card,mobile,sovest;
                uriBuilder.Query = parameters.ToString();
                return uriBuilder.Uri.ToString();
            }

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            bool isSuccess = false;
            string bodyPost = null;

            context.Request.InputStream.Seek(0, SeekOrigin.Begin);
            bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

            if (!string.IsNullOrEmpty(bodyPost))
            {
                var service = new QiwiKassaApiService(SecrectKey);
                IBillNotification notification;
                var validSignature = service.CheckNotificationSignature(
                    context.Request.Headers["X-Api-Signature-SHA256"] ?? context.Request.Headers["Signature"],
                    NewNotifyKey,
                    bodyPost,
                    context.Request.Headers["X-Api-Signature-SHA256"].IsNullOrEmpty(),
                    out notification);
                if (validSignature)
                {
                    var billStatus = service.GetBillInfo(notification.GetBillId());
                    if (billStatus != null && billStatus.Status.Value == EnumStatus.Paid)
                    {
                        var order = OrderService.GetOrder(notification.GetBillId().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().TryParseInt());

                        if (order != null)
                        {
                            var orderSum = order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency);
                            if (Math.Round((decimal)orderSum, 2, MidpointRounding.ToEven) == billStatus.Amount.Value)
                            {
                                OrderService.PayOrder(order.OrderID, true);
                                isSuccess = true;
                            }
                        }
                    }

                }
            }

            var result = JsonConvert.SerializeObject(new { error = isSuccess ? 0 : -1});

            // context.Response.Clear();
            // context.Response.ContentType = "application/json";
            // context.Response.Write(result);
            // context.Response.End();
            return result;

        }
    }
}
