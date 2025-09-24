using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdvantShop.Core.Services.Payment.YandexKassa
{
    public class YandexKassaApiService
    {
        private const string ApiEndpoint = "https://api.yookassa.ru/v3";

        private string _shopId { get; set; }
        private string _secretKey { get; set; }

        public YandexKassaApiService(string shopId, string secretKey)
        {
            _shopId = shopId;
            _secretKey = secretKey;
        }

        public Payment CreatePayment(Order order, string yaPaymentType, string description, bool? savePaymentMethod, Currency paymentCurrency, CreatePaymentConfirmation confirmation)
        {
            return CreatePayment(order, yaPaymentType, description, savePaymentMethod, paymentCurrency, null, confirmation);
        }

        public Payment CreatePaymentWithReceipt(Order order, string yaPaymentType, string description, bool? savePaymentMethod, 
        Currency paymentCurrency, TaxElement tax, CreatePaymentConfirmation confirmation, byte? taxSystemCode, bool sendMeasure)
        {
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;
            PaymentReceipt receipt = new PaymentReceipt()
            {
                Customer = new ReceiptCustomer() {
                    Email = ValidationHelper.IsValidEmail(order.OrderCustomer.Email) ? order.OrderCustomer.Email : null,
                    Phone = order.OrderCustomer.StandardPhone.ToString().Length == 11 ? (order.OrderCustomer.StandardPhone.ToString()) : null },
                Items =
                        order
                            .GetOrderItemsForFiscal(paymentCurrency)
                            .Select(item => new ReceiptItem()
                            {
                                Description = item.Name.Reduce(128),
                                Quantity = item.Amount,
                                Measure = sendMeasure
                                    ? GetMeasure(item.MeasureType)
                                    : null,
                                Amount = new PaymentAmount { Value = item.Price, Currency = paymentCurrency.Iso3 },
                                VatCode = GetVatType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType),
                                PaymentMode = item.PaymentMethodType,
                                PaymentSubject = item.PaymentSubjectType
                            })
                            .ToList(),
                TaxSystemCode = taxSystemCode
            };

            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new ReceiptItem
                    {
                        Description = $"{LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName")} {x.CertificateCode}",
                        Quantity = 1f,
                        Measure = sendMeasure
                            ? GetMeasure(MeasureType.Piece)
                            : null,
                        Amount = new PaymentAmount { Value = x.Sum, Currency = paymentCurrency.Iso3 },
                        VatCode = GetVatType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentMode = AdvantShop.Configuration.SettingsCertificates.PaymentMethodType,
                        PaymentSubject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType
                    }));
            }

            var shippingCost = order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (shippingCost > 0)
            {
                receipt.Items.Add(new ReceiptItem()
                {
                    Description = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    Quantity = 1f,
                    Measure = sendMeasure
                        ? GetMeasure(MeasureType.Piece)
                        : null,
                    Amount = new PaymentAmount { Value = shippingCost, Currency = paymentCurrency.Iso3 },
                    VatCode = GetVatType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    PaymentMode = order.ShippingPaymentMethodType,
                    PaymentSubject = order.ShippingPaymentSubjectType
                });
            }

            return CreatePayment(order, yaPaymentType, description, savePaymentMethod, paymentCurrency, receipt, confirmation);
        }

        private Payment CreatePayment(Order order, string yaPaymentType, string description, bool? savePaymentMethod, Currency paymentCurrency, PaymentReceipt receipt, CreatePaymentConfirmation confirmation)
        {
            var orderSum = 
                receipt != null 
                    ? (float)Math.Round(receipt.Items.Sum(x => (float)Math.Round(x.Amount.Value * x.Quantity, 2, MidpointRounding.AwayFromZero)), 2, MidpointRounding.AwayFromZero)
                    : order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency);
            
            var data = new CreatePayment
            {
                Amount = new PaymentAmount { Value = orderSum, Currency = paymentCurrency.Iso3 },
                Description = description.Reduce(128),
                Receipt = receipt,
                PaymentMethodData = yaPaymentType.IsNotEmpty() ? new PaymentMethodData(yaPaymentType) : null,
                Confirmation = confirmation,
                //Confirmation = new CreatePaymentConfirmationRedirect() { ReturnUrl = SettingsMain.SiteUrl.ToLower() },
                Capture = true,
                ClientIp = order.OrderCustomer.CustomerIP,
                Metadata = new PaymentMetadata { OrderNumber = order.Number},
                SavePaymentMethod = savePaymentMethod
            };

            if (yaPaymentType == "mobile_balance")
            {
                var phone = order.PaymentDetails != null
                    ? System.Text.RegularExpressions.Regex.Replace(order.PaymentDetails.Phone, @"[^\d]", "")
                    : null;
                if (phone.IsNullOrEmpty())
                    phone = order.OrderCustomer.StandardPhone.ToString();

                data.PaymentMethodData = new PaymentMobileMethodData { Phone = phone };
                //data.Confirmation = new CreatePaymentConfirmationExternal();
            }

            if (yaPaymentType == "b2b_sberbank")
            {
                var orderTaxes = order.Taxes;
                var dataPaymentMethodData = new PaymentSberbankBusinessMethodData()
                {
                    PaymentPurpose = description.Reduce(210)
                };
                data.PaymentMethodData = dataPaymentMethodData;

                paymentCurrency = paymentCurrency ?? order.OrderCurrency;
                if (orderTaxes?.Count == 1 && orderTaxes[0].Rate != null)
                {
                    dataPaymentMethodData.VatData = new VatData()
                    {
                        Type = VatDataType.Calculated,
                        Rate = (int)Math.Ceiling(orderTaxes[0].Rate.Value),
                        Amount = new VatDataAmount()
                        {
                            Value = orderTaxes.Sum(tax => tax.Sum.Value).ConvertCurrency(order.OrderCurrency, paymentCurrency),
                            Currency = paymentCurrency.Iso3
                        }
                    };
                }
                else if (orderTaxes?.Count > 1)
                {
                    dataPaymentMethodData.VatData = new VatData()
                    {
                        Type = VatDataType.Mixed,
                        Amount = new VatDataAmount()
                        {
                            Value = orderTaxes.Sum(tax => tax.Sum ?? 0f).ConvertCurrency(order.OrderCurrency, paymentCurrency),
                            Currency = paymentCurrency.Iso3
                        }
                    };
                }
                else
                {
                    dataPaymentMethodData.VatData = new VatData()
                    {
                        Type = VatDataType.UnTaxed
                    };
                }
            }
            
            var orderRequestKey = data.GetHashCode().ToString();
            var orderKey = OrderService.GetOrderAdditionalData(order.OrderID, "YandexKassaByApi-OrderKey");
            if (orderRequestKey != orderKey)
            {
                // если не равны, значит данные изменились
                // запоминаем ключ и шлем запрос
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, "YandexKassaByApi-OrderKey", orderRequestKey);
            }
            else
            {
                // если равны, значит данные не изменились
                orderRequestKey = data.GetHashCode().ToString();

                // проверяем генерировали ли до этого другой идентификатор запроса
                var orderNewKey = OrderService.GetOrderAdditionalData(order.OrderID, "YandexKassaByApi-OrderNewKey");
                if (orderNewKey.IsNotEmpty())
                    orderRequestKey = orderNewKey;
            }

            var payment = MakeRequest<Payment>("/payments", data, orderRequestKey);

            // платеж ранее уже создавался и уже отменен или оплачен
            if (payment != null && payment.Status != "pending")
            {
                // тогда запрос помечаем другим уникальным ключом
                // чтобы получить новый платеж
                orderRequestKey = Guid.NewGuid().ToString();
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, "YandexKassaByApi-OrderNewKey", orderRequestKey);

                payment = MakeRequest<Payment>("/payments", data, orderRequestKey);
            }

            if (payment != null)
                OrderService.AddUpdateOrderAdditionalData(order.OrderID, "YandexKassaByApi-OrderPaymentId", payment.Id);

            return payment;
        }

        public Payment GetPayment(string paymentId)
        {
            return MakeRequest<Payment>("/payments/" + paymentId, method: "GET");
        }

        public Payment GetPaymentByOrder(int orderId)
        {
            var paymentId = OrderService.GetOrderAdditionalData(orderId, "YandexKassaByApi-OrderPaymentId");
            if (paymentId.IsNullOrEmpty())
                return null;

            return GetPaymentNotPending(paymentId);
        }

        private Payment GetPaymentNotPending(string paymentId, int tryCount = 0)
        {
            var payment = GetPayment(paymentId);
            if (payment != null && payment.Status == "pending" && tryCount < 4)
            {
                System.Threading.Thread.Sleep(1000);
                return GetPaymentNotPending(paymentId, tryCount++);
            }
            return payment;
        }


        #region Help methods

        private byte GetVatType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null)
                return 1;

            if (taxType.Value == TaxType.VatWithout)
                return 1;

            if (taxType.Value == TaxType.Vat0)
                return 2;

            if (taxType.Value == TaxType.Vat10)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 5;
                else
                    return 3;
            }

            if (taxType.Value == TaxType.Vat18 || taxType.Value == TaxType.Vat20)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 6;
                else
                    return 4;
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 9;
                else
                    return 7;
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 10;
                else
                    return 8;
            }

            return 1;
        }

        private string GetMeasure(MeasureType? itemMeasureType)
        {
            if (itemMeasureType is null)
                return null;
            
            switch (itemMeasureType.Value)
            {
                case MeasureType.Piece:
                    return "piece";
                case MeasureType.Gram:
                    return "gram";
                case MeasureType.Kilogram:
                    return "kilogram";
                case MeasureType.Ton:
                    return "ton";
                case MeasureType.Centimetre:
                    return "centimeter";
                case MeasureType.Decimeter:
                    return "decimeter";
                case MeasureType.Metre:
                    return "meter";
                case MeasureType.SquareCentimeter:
                    return "square_centimeter";
                case MeasureType.SquareDecimeter:
                    return "square_decimeter";
                case MeasureType.SquareMeter:
                    return "square_meter";
                case MeasureType.Milliliter:
                    return "milliliter";
                case MeasureType.Liter:
                    return "liter";
                case MeasureType.CubicMeter:
                    return "cubic_meter";
                case MeasureType.KilowattHour:
                    return "kilowatt_hour";
                case MeasureType.Gigacaloria:
                    return "gigacalorie";
                case MeasureType.Day:
                    return "day";
                case MeasureType.Hour:
                    return "hour";
                case MeasureType.Minute:
                    return "minute";
                case MeasureType.Second:
                    return "second";
                case MeasureType.Kilobyte:
                    return "kilobyte";
                case MeasureType.Megabyte:
                    return "megabyte";
                case MeasureType.Gigabyte:
                    return "gigabyte";
                case MeasureType.Terabyte:
                    return "terabyte";
                case MeasureType.Other:
                    return "other";
            }

            return null;
        }

        public NotifyEvent ReadNotifyData(string postPayload)
        {
            return JsonConvert.DeserializeObject<NotifyEvent>(postPayload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }

        #endregion

        #region Private methods

        private T MakeRequest<T>(string url, object data = null, string requestKey = null, string method = "POST")
            where T : class
        {
            try
            {
                var request = WebRequest.Create(ApiEndpoint + url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = method;
                request.ContentType = "application/json;charset=UTF-8";
                request.Credentials = new NetworkCredential(_shopId, _secretKey);
                if (requestKey.IsNotEmpty())
                    request.Headers.Add("Idempotence-Key", requestKey);

                if (data != null)
                {
                    //request.Headers.Add("Idempotence-Key", data.GetHashCode().ToString());

                    string dataPost = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        },
                        NullValueHandling = NullValueHandling.Ignore
                    });

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

                var dataAnswer = JsonConvert.DeserializeObject<T>(responseContent, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

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
                                    Debug.Log.Error(error, ex);
                                }
                            else
                                Debug.Log.Error(ex);
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

        #endregion
    }
}
