using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Core.Services.Payment.SberBankAcquiring;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using HttpUtility = System.Web.HttpUtility;
using System.Linq;
using System.Threading;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Primitives;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Taxes;
using Tax = AdvantShop.Core.Services.Payment.SberBankAcquiring.Tax;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;
using DocumentFormat.OpenXml.ExtendedProperties;
using AdvantShop.Core.Services.Mails.Analytics;
using Company = AdvantShop.Core.Services.Payment.SberBankAcquiring.Company;
using AdvantShop.Core.Properties;
using AdvantShop.Configuration;

namespace AdvantShop.Payment
{
    // Документация: https://ecomtest.sberbank.ru/doc

    public class SberBankAcquiringService
    {
        private const string BaseUrl = "https://proxy.advstatic.ru/ecommerce.sberbank.ru/ecomm/gw/partner/api/v1/";
        private const string BaseTestUrl = "https://proxy.advstatic.ru/ecomtest.sberbank.ru/ecomm/gw/partner/api/v1/";

        private readonly string _userName;
        private readonly string _password;
        private readonly string _merchantLogin;
        private readonly string _email;
        private readonly string _inn;
        private readonly bool _testMode;
        private readonly bool _sendReceiptData;
        private readonly bool _useFfd12;
        private readonly string _sno;

        public SberBankAcquiringService(string userName, string password, string merchantLogin, bool testMode,bool sendReceiptData, bool useFfd12, string email, string inn, string sno)
        {
            _userName = userName;
            _password = password;
            _merchantLogin = merchantLogin;
            _testMode = testMode;
            _sendReceiptData = sendReceiptData;
            _useFfd12 = useFfd12;
            _email = email;
            _inn = inn;
            _sno = sno;
        }

        /// <summary>
        /// Запрос  регистрации заказа
        /// </summary>
        public SberbankAcquiringRegisterResponse Register(Order order, string description, Currency paymentCurrency, string returnUrl, string failUrl, TaxElement tax)
        {
            int index = 1;
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;

            Receipt receipt = _sendReceiptData
                ? new Receipt()
                {
                    ffdVersion = _useFfd12 ? "1.2" : "1.05",
                    cartItems = new CartItems
                    {
                        items =
                            order
                                .GetOrderItemsForFiscal(paymentCurrency)
                                .Select(item => new Item()
                                {
                                    positionId = (index++).ToString(),
                                    name = item.Name.Reduce(100),
                                    itemCode = item.ArtNo.Reduce(100),
                                    itemAmount = (int)Math.Round((int)Math.Round(item.Price * 100, 0) * (float)Math.Round(item.Amount, 3), MidpointRounding.AwayFromZero),
                                    itemPrice = (int)Math.Round(item.Price * 100, 0),
                                    quantity = _useFfd12
                                        ? new Quantity
                                        {
                                            value = (float) Math.Round(item.Amount, 3),
                                            measure = GetMeasure(item.MeasureType).ToString()
                                        }
                                        : new BaseQuantity
                                        {
                                            value = (float) Math.Round(item.Amount, 3),
                                        },
                                    tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType) },
                                    paymentMethod = GetPaymentMethodType(item.PaymentMethodType),
                                    paymentObject = item.PaymentSubjectType.ToString()
                                }).ToList()
                    },
                    company = new Company() { email = _email, sno = _sno, inn = _inn, paymentAddress = SettingsMain.SiteUrl }
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.cartItems.items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        name = LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"),
                        itemCode = x.CertificateCode,
                        positionId = (index++).ToString(),
                        itemAmount = (int)(Math.Round(x.Sum * 100)),
                        itemPrice = (int)(Math.Round(x.Sum * 100)),
                        quantity = _useFfd12
                            ? new Quantity {value = 1, measure = GetMeasure(MeasureType.Piece).ToString() }
                            : new BaseQuantity{ value = 1 },
                        tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType) },

                        paymentMethod = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        paymentObject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.cartItems.items.Add(new Item()
                {
                    name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    itemCode = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    positionId = (index++).ToString(),
                    itemAmount = (int)(Math.Round(orderShippingCostWithDiscount * 100)),
                    itemPrice = (int)(Math.Round(orderShippingCostWithDiscount * 100)),
                    quantity = _useFfd12
                        ? new Quantity {value = 1, measure = GetMeasure(MeasureType.Piece).ToString()
                        }
                        : new BaseQuantity { value = 1 },
                    tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType) },

                    paymentMethod = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    paymentObject = order.ShippingPaymentSubjectType.ToString()
               });
            }

            long sum = 0;

            if (receipt != null && receipt.cartItems != null && receipt.cartItems.items != null)
            {
                foreach (var item in receipt.cartItems.items)
                    sum += item.itemAmount;
            }
            else
            {
                sum = (long)Math.Round(Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2) * 100);
            }

            if (receipt != null)
                receipt.payments = new List<Core.Services.Payment.SberBankAcquiring.Payment>() { new Core.Services.Payment.SberBankAcquiring.Payment() { sum = sum } };


            int retriesNum = 0;
            string orderStrId;
            bool success = false;
            SberbankAcquiringRegisterResponse response;

            do
            {
                // если заказ уже есть в сбербанке, но был изменен на стороне магазина, подменяем id на id_номерпопытки
                orderStrId = string.Format("{0}_{1}", order.OrderID, DateTime.Now.ToUnixTime());

                var data = new Dictionary<string, object>()
                {
                    {"userName", _userName},
                    {"password", _password},
                    {"orderNumber", orderStrId},
                    {"amount", sum},    // Сумма платежа в копейках
                    {"returnUrl", returnUrl},
                    {"failUrl", failUrl},
                    {"clientId", order.OrderCustomer.CustomerID},
                };
                
                if (order.OrderCustomer.Email.IsNotEmpty())
                    data.Add("email", order.OrderCustomer.Email);

                if (_merchantLogin.IsNotEmpty())
                {
                    data.Add("merchantLogin", _merchantLogin);
                }

                if (receipt != null)
                {
                    if (order.OrderCustomer.StandardPhone.HasValue)
                        data.Add("phone", $"+{order.OrderCustomer.StandardPhone}");
                    data.Add("orderBundle", receipt);
                }

                response = MakeRequest<SberbankAcquiringRegisterResponse>("register.do", data);

                if (response == null)
                    response = new SberbankAcquiringRegisterResponse { ErrorCode = "1" };
                //return null;

                string error;
                success = !HasRegisterError(response, out error);

                if (!success)
                {
                    Debug.Log.Info(string.Format("SberBankAcquiringService Register. code: {0} error: {1}, obj: {2}",
                                                    error, response.ErrorMessage, JsonConvert.SerializeObject(response)));

                }
                retriesNum++;
            } while (response.ErrorCode == "1" && retriesNum < 3);

            return success ? response : null;
        }

        /// <summary>
        /// Запрос состояния заказа
        /// </summary>
        public SberbankAcquiringOrderStatusResponse GetOrderStatus(string orderId)
        {
            var data = new Dictionary<string, object>()
            {
                {"userName", _userName},
                {"password", _password},
                {"orderId", orderId},
            };

            var result = MakeRequestWithResult<SberbankAcquiringOrderStatusResponse>("getOrderStatusExtended.do", data);

            if (!result.IsSuccess &&
                !string.IsNullOrEmpty(result.Error.Message) &&
                result.Error.Message.Contains("Could not create SSL/TLS secure channel", StringComparison.OrdinalIgnoreCase))
            {
                Thread.Sleep(500);
                result = MakeRequestWithResult<SberbankAcquiringOrderStatusResponse>("getOrderStatusExtended.do", data);
            }

            if (!result.IsSuccess)
                return null;

            var status = result.Value;

            Debug.Log.Info(string.Format("SberBankAcquiringService GetOrderStatus. code: {0} error: {1}, errorcode: {2}, obj: {3}, send: {4}",
                                            GetOrderStatusName(status.OrderStatus), status.ErrorMessage, status.ErrorCode, JsonConvert.SerializeObject(status), JsonConvert.SerializeObject(data)));

            return status;
        }



        #region Private methods


        private string GetPaymentMethodType(ePaymentMethodType paymentMethodType)
        {
            switch (paymentMethodType)
            {
                case ePaymentMethodType.full_prepayment:
                    return "full_prepayment";
                case ePaymentMethodType.partial_prepayment:
                    return "prepayment"; // из-за этого значения нельзя enum.Tostring();
                case ePaymentMethodType.advance:
                    return "advance";
                case ePaymentMethodType.full_payment:
                    return "full_payment";
                case ePaymentMethodType.partial_payment:
                    return "partial_payment";
                case ePaymentMethodType.credit:
                    return "credit";
                case ePaymentMethodType.credit_payment:
                    return "credit_payment";
                default:
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in Sber");
            }
        }


        /*
         0 – без НДС;
         1 – НДС по ставке 0%;
         2 – НДС чека по ставке 10%;
         3 – НДС чека по ставке 18%;
         4 – НДС чека по расчётной ставке 10/110;
         5 – НДС чека по расчётной ставке 18/118.
         6 - НДС чека по ставке 20%;
         7 - НДС чека по расчётной ставке 20/120.
         */
        private int GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return 0;

            if (taxType.Value == TaxType.Vat0)
                return 1;

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 4;
                else
                    return 2;
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                    paymentMethodType == ePaymentMethodType.partial_prepayment ||
                    paymentMethodType == ePaymentMethodType.advance))
                    return 5;
                else
                    return 3;
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                    paymentMethodType == ePaymentMethodType.partial_prepayment ||
                    paymentMethodType == ePaymentMethodType.advance))
                    return 7;
                else
                    return 6;
            }

            return 0;
        }

        private byte? GetMeasure(MeasureType? itemMeasureType)
        {
            if (itemMeasureType is null)
                return null;
            
            switch (itemMeasureType.Value)
            {
                case MeasureType.Piece:
                    return 0;
                case MeasureType.Gram:
                    return 10;
                case MeasureType.Kilogram:
                    return 11;
                case MeasureType.Ton:
                    return 12;
                case MeasureType.Centimetre:
                    return 20;
                case MeasureType.Decimeter:
                    return 21;
                case MeasureType.Metre:
                    return 22;
                case MeasureType.SquareCentimeter:
                    return 30;
                case MeasureType.SquareDecimeter:
                    return 31;
                case MeasureType.SquareMeter:
                    return 32;
                case MeasureType.Milliliter:
                    return 40;
                case MeasureType.Liter:
                    return 41;
                case MeasureType.CubicMeter:
                    return 42;
                case MeasureType.KilowattHour:
                    return 50;
                case MeasureType.Gigacaloria:
                    return 51;
                case MeasureType.Day:
                    return 70;
                case MeasureType.Hour:
                    return 71;
                case MeasureType.Minute:
                    return 72;
                case MeasureType.Second:
                    return 73;
                case MeasureType.Kilobyte:
                    return 80;
                case MeasureType.Megabyte:
                    return 81;
                case MeasureType.Gigabyte:
                    return 82;
                case MeasureType.Terabyte:
                    return 83;
                case MeasureType.Other:
                    return 255;
            }

            return null;
        }
        
        private bool HasRegisterError(SberbankAcquiringRegisterResponse response, out string error)
        {
            error = null;
            var code = response.ErrorCode ?? "";
            switch (code)
            {
                case "0":
                    error = "Обработка запроса прошла без системных ошибок";
                    break;

                case "1":
                    error = "Заказ с таким номером уже зарегистрирован в системе";
                    break;

                case "2":
                    error = "Обработка запроса прошла без системных ошибок";
                    break;

                case "3":
                    error = "Неизвестная (запрещенная) валюта";
                    break;

                case "4":
                    error = "Отсутствует обязательный параметр запроса";
                    break;

                case "5":
                    error = "Ошибка значение параметра запроса";
                    break;

                case "7":
                    error = "Системная ошибка";
                    break;
            }

            return code != "0" && code.IsNotEmpty();
        }

        private string GetOrderStatusName(int code)
        {
            switch (code)
            {
                case 0:
                    return "Заказ зарегистрирован, но не оплачен";

                case 1:
                    return "Предавторизованная сумма захолдирована (для двухстадийных платежей)";

                case 2:
                    return "Проведена полная авторизация суммы заказа";

                case 3:
                    return "Авторизация отменена";

                case 4:
                    return "По транзакции была проведена операция возврата";

                case 5:
                    return "Инициирована авторизация через ACS банка-эмитента";

                case 6:
                    return "Авторизация отклонена";
            }

            return string.Empty;
        }

        private T MakeRequest<T>(string url, Dictionary<string, object> data = null) where T : class
        {
            try
            {
                var request = WebRequest.Create((_testMode ? BaseTestUrl : BaseUrl) + url) as HttpWebRequest;
                request.Timeout = 50000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
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

                var dataAnswer = JsonConvert.DeserializeObject<T>(responseContent);

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
                                    Debug.Log.Error(error);
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
        
        private Result<T> MakeRequestWithResult<T>(string url, Dictionary<string, object> data = null) where T : class
        {
            var errorResult = "";
            
            try
            {
                var request = WebRequest.Create((_testMode ? BaseTestUrl : BaseUrl) + url) as HttpWebRequest;
                request.Timeout = 50000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
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

                var dataAnswer = JsonConvert.DeserializeObject<T>(responseContent);

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
                                    var error = errorResult = reader.ReadToEnd();
                                    Debug.Log.Error(error);
                                }
                    }
                    else
                    {
                        errorResult = ex.Message;
                        Debug.Log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                errorResult = ex.Message;
                Debug.Log.Error(ex);
            }

            return Result.Failure<T>(new Error(errorResult));
        }

        #endregion
    }
}