using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment.Rbkmoney2;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Документация: https://rbkmoney.github.io/docs/
    /// </summary>
    [PaymentKey("Rbkmoney2")]
    public class Rbkmoney2 : PaymentMethod
    {
        public string ShopId { get; set; }
        public string ApiKey { get; set; }

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
            get { return UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {Rbkmoney2Template.ShopId, ShopId},
                    {Rbkmoney2Template.ApiKey, ApiKey},
                };
            }
            set
            {
                ShopId = value.ElementOrDefault(Rbkmoney2Template.ShopId);
                ApiKey = value.ElementOrDefault(Rbkmoney2Template.ApiKey);
            }
        }

        public override string ProcessJavascript(Order order)
        {
            var service = new Rbkmoney2Service(ApiKey, ShopId);
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var result = service.CreateInvoice(order, GetOrderDescription(order.Number), PaymentCurrency, tax);
            if (result.Invoice == null || result.InvoiceAccessToken == null)
            {
                Debug.Log.ErrorFormat("Rbkmoney2 invoice not created: {0}", result.Message);
                return string.Empty;
            }

            var data = new Dictionary<string, string>
            {
                { "data-invoice-id", result.Invoice.Id },
                { "data-invoice-access-token", result.InvoiceAccessToken.Payload },
                { "data-name", GetOrderDescription(order.Number) },
                { "data-email", order.OrderCustomer.Email },
                { "data-logo", "https://checkout.rbk.money/images/logo.png" },
                { "data-label", "Оплатить с карты" },
                //{ "data-description", "" },
                { "data-pay-button-label", "Оплатить" },
            };

            return
                "<style type=\"text/css\">#rbkmoney-button { display: none; }</style>" + // hide rbk pay button
                "<script type=\"text/javascript\">" +
                    "this.rbkpay = function() { document.getElementById(\"rbkmoney-button\").click(); }" + 
                "</script>" +
                "<form action=\"" + SuccessUrl + "\" method=\"GET\" class=\"js-disable-autosubmit\">" +
                    "<script src=\"https://checkout.rbk.money/checkout.js\" class=\"rbkmoney-checkout\" " +
                        data.Select(kvp => string.Format("{0}=\"{1}\"", kvp.Key, kvp.Value)).AggregateString(" ") +
                    "></script>" +
                "</form>";
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "javascript:rbkpay();";
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            //var signHeader = req.Headers["Content-Signature"];
            //var sign = signHeader.IsNotEmpty() ? signHeader.Split("digest=").LastOrDefault() : null;
            //if (sign.IsNullOrEmpty())
            //    return string.Empty;
            var content = (new StreamReader(req.InputStream)).ReadToEnd();
            if (content.IsNotEmpty())
            {
                try
                {
                    var model = JsonConvert.DeserializeObject<Rbkmoney2WebhookModel>(content);
                    if (model == null || model.Invoice == null)
                        return string.Empty;
                    var service = new Rbkmoney2Service(ApiKey, ShopId);
                    // get invoice from rbk to check status. ignore status from webhook
                    var invoice = service.GetInvoice(model.Invoice.Id);
                    if (invoice != null && invoice.Metadata != null && (invoice.Status == EInvoiceStatus.Paid || invoice.Status == EInvoiceStatus.Fulfilled))
                    {
                        OrderService.PayOrder(invoice.Metadata.OrderId, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log.Info("error at deserialize webhook content", ex);
                }
            }

            return string.Empty;
        }
    }
}