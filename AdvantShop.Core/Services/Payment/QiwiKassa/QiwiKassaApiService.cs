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
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.QiwiKassa
{
    public class QiwiKassaApiService
    {
        public const string BillsUrl = "https://api.qiwi.com/partner/bill/v1/bills/";
        private readonly string _secretKey;

        public QiwiKassaApiService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public BillResponse CreateBill(Order order, string description, Currency paymentCurrency, TaxElement tax, Dictionary<string, string> customFields = null)
        {
            var keyOrderIdQiwi = "QiwiKassaBillId";
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;

            int retriesNum = 0;
            BillResponse billResponse = null;
            var orderIdQiwi = OrderService.GetOrderAdditionalData(order.OrderID, keyOrderIdQiwi);
            if (orderIdQiwi.IsNullOrEmpty())
                // счет впервые выставляется
                orderIdQiwi = order.OrderID.ToString();

            do
            {
                if (billResponse != null)
                    // предыдущий выставленный счет
                    // просрочился или уже оплачен
                    orderIdQiwi = string.Format("{0}_{1}", order.OrderID, DateTime.Now.ToUnixTime());

                var bill = new CreateBillInfo
                {
                    Amount = new MoneyAmount
                    {
                        Value = (decimal)order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency),
                        Currency = paymentCurrency.Iso3
                    },
                    Comment = description.Reduce(255),
                    ExpirationDateTime = DateTime.Now.AddDays(45),
                    Customer = new Customer
                    {
                        Email = order.OrderCustomer.Email,
                        //Phone = order.OrderCustomer.Phone //в международном формате
                    },
                    CustomFields = customFields
                };

                billResponse = MakeRequest<BillResponse, CreateBillInfo>(orderIdQiwi, bill, "PUT");

                retriesNum++;
            } while ((billResponse == null || (billResponse.Status.Value != EnumStatus.Paid && billResponse.Status.Value != EnumStatus.Waiting)) && retriesNum < 3);

            if (billResponse != null && billResponse.Status.Value == EnumStatus.Waiting)
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, keyOrderIdQiwi, orderIdQiwi);

            return billResponse;
        }

        public BillResponse GetBillInfo(string billId)
        {
            return MakeRequest<BillResponse, CreateBillInfo>(billId, null, "GET");
        }

        public bool CheckNotificationSignature(string validSignature, string newNotifyKey, string bodyPost, bool isNewNotify, out IBillNotification notification)
        {
            notification = null;
            Dictionary<string, string> fields = null;

            if (isNewNotify)
            {
                var notificationPayment = DeserializeObject<NotificationPayment>(bodyPost);

                if (notificationPayment != null
                    && notificationPayment.Payment != null)
                {
                    notification = notificationPayment;

                    fields = new Dictionary<string, string>
                    {
                        {"payment.paymentId", notificationPayment.Payment.PaymentId ?? ""},
                        {"payment.createdDateTime", notificationPayment.Payment.CreatedDateTime ?? ""},
                        {
                            "payment.amount.value",
                            notificationPayment.Payment.Amount.Value.ToString(CultureInfo.InvariantCulture)
                        },
                    };
                }
            }
            else
            {
                var notificationBill = DeserializeObject<NotificationBill>(bodyPost);

                if (notificationBill != null
                    && notificationBill.Bill != null)
                {
                    notification = notificationBill;

                    fields = new Dictionary<string, string>
                    {
                        {"amount.currency", notificationBill.Bill.Amount.Currency ?? ""},
                        {"amount.value", notificationBill.Bill.Amount.Value.ToString(CultureInfo.InvariantCulture)},
                        {"billId", notificationBill.Bill.BillId ?? ""},
                        {"siteId", notificationBill.Bill.SiteId ?? ""},
                        {
                            "status",
                            typeof(EnumStatus).GetMember(notificationBill.Bill.Status.Value.ToString()).First()
                                              .GetCustomAttributes(false).OfType<EnumMemberAttribute>().First().Value
                            ?? ""
                        }
                    };
                }
            }

            if (fields != null)
            {
                var signatureValues = string.Join("|", fields.Values);
                var encoding = Encoding.UTF8;
                var keyByte = encoding.GetBytes(
                    isNewNotify 
                        ? newNotifyKey 
                        : _secretKey);
                var messageBytes = encoding.GetBytes(signatureValues);
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    var hash = hmacsha256.ComputeHash(messageBytes);
                    return validSignature == hash.Aggregate(new StringBuilder(), (curr, value) => curr.Append(value.ToString("x2"))).ToString();
                }
            }

            notification = null;
            return false;
        }

        private string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz",
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        private T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            DateFormatString = "yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"
                        });
        }

        private TR MakeRequest<TR, TD>(string url, TD data = null, string method = "POST") 
            where TR : class, new()
            where TD : class
        {
            try
            {
                var request = WebRequest.Create(BillsUrl + url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = method;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _secretKey);

                if (data != null)
                {
                    string dataPost = SerializeObject(data);

                    byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                var dataAnswer = DeserializeObject<TR>(responseContent);

                return dataAnswer;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    if (error.IsNotEmpty())
                                        Debug.Log.Error(error);
                                    else
                                        Debug.Log.Error(ex);
                                }
                    }
                    else
                        Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
    }
}
