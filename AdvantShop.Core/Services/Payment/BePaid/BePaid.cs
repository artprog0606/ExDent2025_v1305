using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Payment.BePaid;
using AdvantShop.Localization;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("BePaid")]
    public class BePaid : PaymentMethod
    {
        public string ShopId { get; set; }
        private string SecretKey { get; set; }
        public bool DemoMode { get; set; }
        public List<string> ActivePayments { get; set; }
        private int? EripServiceNo { get; set; }

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
            get { return UrlStatus.None; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {BePaidTemplate.ShopId, ShopId},
                    {BePaidTemplate.SecretKey, SecretKey},
                    {BePaidTemplate.DemoMode, DemoMode.ToString()},
                    {BePaidTemplate.ActivePayments, ActivePayments != null ? string.Join(",", ActivePayments) : string.Empty},
                    {BePaidTemplate.EripServiceNo, EripServiceNo.ToString()},
                };
            }
            set
            {
                ShopId = value.ElementOrDefault(BePaidTemplate.ShopId);
                SecretKey = value.ElementOrDefault(BePaidTemplate.SecretKey);
                DemoMode = value.ElementOrDefault(BePaidTemplate.DemoMode).TryParseBool();
                ActivePayments = (value.ElementOrDefault(BePaidTemplate.ActivePayments) ?? string.Empty).Split(",").ToList();
                EripServiceNo = value.ElementOrDefault(BePaidTemplate.EripServiceNo).TryParseInt(true);
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var service = new BePaidService(ShopId, SecretKey);
            var checkout = new CheckoutContainer
                {
                    Checkout =
                        new Checkout
                        {
                            Test = DemoMode,
                            TransactionType = TransactionType.Payment,

                            Order = new CheckoutOrder()
                            {
                                Amount = GetAmountOrder(order),
                                Currency = (PaymentCurrency ?? order.OrderCurrency).Iso3,
                                Description = GetOrderDescription(order.Number),
                                TrackingId = order.Number
                            },

                            Customer = new CheckoutCustomer()
                            {
                                Email = order.OrderCustomer.Email.IsNotEmpty() ? order.OrderCustomer.Email : null,
                                Phone = order.OrderCustomer.StandardPhone.ToString()
                            },

                            Settings = new CheckoutSettings()
                            {
                                SuccessUrl = SuccessUrl,
                                DeclineUrl = FailUrl,
                                FailUrl = FailUrl,
                                CancelUrl = CancelUrl,
                                NotificationUrl = NotificationUrl,
                                Language =
                                    Culture.Language == Culture.SupportLanguage.Russian
                                        ? "ru"
                                        : Culture.Language == Culture.SupportLanguage.English
                                            ? "en"
                                            : null
                            }
                        }
                };

            if (ActivePayments.Count > 0)
            {
                checkout.Checkout.PaymentMethod = new CheckoutPaymentMethod {Types = new HashSet<string>()};
                ActivePayments.ForEach(p => checkout.Checkout.PaymentMethod.Types.Add(p));
                
                if (ActivePayments.Contains("erip"))
                {
                    checkout.Checkout.PaymentMethod.Erip = new CheckoutEripPayment
                    {
                        AccountNumber = order.Number.Reduce(30),
                        ServiceNo = EripServiceNo,
                        ServiceInfo = new List<string>() { GetOrderDescription(order.Number) }
                    };
                }
            }

            var checkoutResult = service.CreateCheckout(checkout);

            if (checkoutResult != null)
                return checkoutResult.Checkout.RedirectUrl;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"];

            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    var credentials = authHeaderVal.Parameter;
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    credentials = encoding.GetString(Convert.FromBase64String(credentials));

                    int separator = credentials.IndexOf(':');
                    string shopId = credentials.Substring(0, separator);
                    string secretKey = credentials.Substring(separator + 1);

                    if (ShopId == shopId && SecretKey == secretKey)
                    {
                        string bodyPost = null;

                        context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                        bodyPost = (new StreamReader(context.Request.InputStream)).ReadToEnd();

                        if (!string.IsNullOrEmpty(bodyPost))
                        {
                            var service = new BePaidService(ShopId, SecretKey);

                            var notify = service.ReadNotifyData(bodyPost);

                            if (notify != null && notify.Transaction != null)
                            {
                                if (notify.Transaction.Type == "payment" &&
                                    notify.Transaction.Status == "successful")
                                {
                                    if (notify.Transaction.TrackingId.IsNotEmpty() &&
                                       notify.Transaction.PaymentMethodType.IsNotEmpty())
                                    {
                                        if (notify.Transaction.PaymentMethodType.Equals("credit_card", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var payment = service.GetPaymentInfoFromGateway(notify.Transaction.Uid);

                                            if (payment != null && payment.Transaction != null &&
                                                payment.Transaction.Status == "successful" && notify.Transaction.TrackingId == payment.Transaction.TrackingId)
                                            {
                                                var order = OrderService.GetOrderByNumber(payment.Transaction.TrackingId);

                                                if (
                                                    order != null &&
                                                    payment.Transaction.Amount == GetAmountOrder(order))
                                                {
                                                    OrderService.PayOrder(order.OrderID, true);
                                                    return NotificationMessahges.SuccessfullPayment(order.Number);
                                                }
                                            }
                                        }

                                        if (notify.Transaction.PaymentMethodType.Equals("erip", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var payment = service.GetPaymentInfoFromApi(notify.Transaction.Uid);

                                            if (payment != null && payment.Transaction != null &&
                                                payment.Transaction.Status == "successful" && notify.Transaction.TrackingId == payment.Transaction.TrackingId)
                                            {
                                                var order = OrderService.GetOrderByNumber(payment.Transaction.TrackingId);

                                                if (
                                                    order != null &&
                                                    payment.Transaction.Amount == GetAmountOrder(order))
                                                {
                                                    OrderService.PayOrder(order.OrderID, true);
                                                    return NotificationMessahges.SuccessfullPayment(order.Number);
                                                }
                                            }
                                        }

                                        context.Response.Clear();
                                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                        context.Response.End();
                                        return NotificationMessahges.InvalidRequestData;
                                    }
                                }
                                else
                                {
                                    return NotificationMessahges.InvalidRequestData;
                                }
                            }
                        }

                        context.Response.Clear();
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.End();
                        return NotificationMessahges.InvalidRequestData;
                    }
                }
            }

            // Ваш веб-сервер должен вернуть HTTP статус 200, если уведомление было обработано успешно. Иначе, через некоторое время, bePaid повторно вышлет уведомление.
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.End();
            return NotificationMessahges.InvalidRequestData;
        }

        private long GetAmountOrder(Order order)
        {
            return (long) (order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency) * 100);
        }
    }
}
