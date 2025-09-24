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
using System.Text;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Payment.Alfabank
{
    // https://pay.alfabank.ru/ecommerce/instructions/merchantManual/pages/fz_index.html
    public class AlfabankService
    {
        // для ReturnUrl
        public const string ReturnUrlParamNameMerchantOrder = "merchantorderid";

        private readonly string _gatewayUrl = "https://pay.alfabank.ru/payment/rest/";

        private readonly string _userName;
        private readonly string _password;
        private readonly string _merchantLogin;
        private readonly bool _useFfd12;

        public AlfabankService(string gatewayUrl, string userName, string password, string merchantLogin, bool useFfd12)
        {
            if (gatewayUrl.IsNotEmpty())
                _gatewayUrl = gatewayUrl;
            _userName = userName;
            _password = password;
            _merchantLogin = merchantLogin;
            _useFfd12 = useFfd12;
        }

        /// <summary>
        /// Регистрация заказа
        /// </summary>
        public AlfabankRegisterResponse Register(Order order, string description, bool sendReceiptData,
                                                 string taxation, Currency paymentCurrency, string returnUrl, 
                                                 string failUrl, TaxElement tax)
        {
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;
            int index = 1;
            Product tempProduct;
            Receipt receipt = sendReceiptData
                ? new Receipt()
                {
                    CustomerDetails = ValidationHelper.IsValidEmail(order.OrderCustomer.Email)
                        ? new CustomerDetails { Email = order.OrderCustomer.Email }
                        : null,
                    CartItems = new CartItems
                    {
                        Items =
                            order
                                .GetOrderItemsForFiscal()
                                .Select(item => new Item()
                                {
                                    PositionId = index++,
                                    Name = item.Name.Length > 100 ? item.Name.Substring(0, 100) : item.Name,
                                    ItemCode = item.ArtNo,
                                    ItemAmount = (int) Math.Round(item.Price.ConvertCurrency(order.OrderCurrency, paymentCurrency) * item.Amount * 100, 0),
                                    ItemPrice = (int) Math.Round(item.Price.ConvertCurrency(order.OrderCurrency, paymentCurrency) * 100, 0),
                                    Quantity = _useFfd12
                                        ? (BaseQuantity)new Quantity<byte?>
                                        {
                                            Value = item.Amount,
                                            Measure = GetMeasure(item.MeasureType)
                                        }
                                        : (BaseQuantity)new Quantity<string>()
                                        {
                                            Value = item.Amount,
                                            Measure = item.Unit.IsNotEmpty()
                                                ? item.Unit
                                                : "штук"
                                        },
                                    Tax = new Tax() {TaxType = GetTaxType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType)},
                                    ItemAttributes = new ItemAttributes
                                    {
                                        Attributes = new ItemAttribute[] {
                                            new ItemAttribute()
                                            {
                                                Name = "paymentMethod",
                                                Value = ((int)item.PaymentMethodType).ToString()
                                            },
                                            new ItemAttribute()
                                            {
                                                Name = "paymentObject",
                                                Value = ((int)item.PaymentSubjectType).ToString()
                                            }
                                        }
                                    }
                                }).ToList()
                    }
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.CartItems.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        Name = "Подарочный сертификат",
                        ItemCode = x.CertificateCode,
                        PositionId = index++,
                        ItemAmount = (int)(Math.Round(x.Sum, 2) * 100),
                        ItemPrice = (int)(Math.Round(x.Sum, 2) * 100),
                        Quantity = _useFfd12
                            ? (BaseQuantity)new Quantity<byte?> {Value = 1, Measure = GetMeasure(MeasureType.Piece)}
                            : (BaseQuantity)new Quantity<string>() {Value = 1, Measure = "штук"},
                        Tax = new Tax() { TaxType = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType) },
                        ItemAttributes = new ItemAttributes
                        {
                            Attributes = new ItemAttribute[] {
                                new ItemAttribute()
                                {
                                    Name = "paymentMethod",
                                    Value =((int)AdvantShop.Configuration.SettingsCertificates.PaymentMethodType).ToString()
                                },
                                new ItemAttribute()
                                {
                                    Name = "paymentObject",
                                    Value =((int)AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType).ToString()
                                }
                            }
                        }
                        //ItemAttributes = new ItemAttribute() { PaymentMethod = (int)ePaymentMethodType.advance, PaymentObject = (int)ePaymentSubjectType.payment }
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount.ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.CartItems.Items.Add(new Item()
                {
                    Name = "Доставка",
                    ItemCode = "Доставка",
                    PositionId = index++,
                    ItemAmount = (int)(Math.Round(orderShippingCostWithDiscount, 2) * 100),
                    ItemPrice = (int)(Math.Round(orderShippingCostWithDiscount, 2) * 100),
                    Quantity = _useFfd12
                        ? (BaseQuantity)new Quantity<byte?> {Value = 1, Measure = GetMeasure(MeasureType.Piece)}
                        : (BaseQuantity)new Quantity<string>() {Value = 1, Measure = "штук"},
                    Tax = new Tax() { TaxType = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType) },
                    ItemAttributes = new ItemAttributes
                    {
                        Attributes = new ItemAttribute[] {
                            new ItemAttribute()
                            {
                                Name = "paymentMethod",
                                Value =((int)order.ShippingPaymentMethodType).ToString()
                            },
                            new ItemAttribute()
                            {
                                Name = "paymentObject",
                                Value =((int)order.ShippingPaymentSubjectType).ToString()
                            }
                        }
                    }
                });
            }

            long sum = 0;

            if (receipt != null && receipt.CartItems != null && receipt.CartItems.Items != null)
                foreach (var item in receipt.CartItems.Items)
                    sum += item.ItemAmount;
            else
                sum = (long)Math.Round(Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2) * 100);

            int retriesNum = 0;
            string orderStrId;
            bool success = false;
            AlfabankRegisterResponse response;

            do
            {
                // если заказ уже есть в альфабанке, но был изменен на стороне магазина, подменяем id на id_номерпопытки
                orderStrId = retriesNum > 0
                    ? $"{order.Number}_{DateTime.Now.ToUnixTime()}"
                    : order.Number.ToString();

                var data = new Dictionary<string, string>()
                {
                    {"userName", _userName},
                    {"password", _password},
                    {"orderNumber", orderStrId},
                    {"amount", sum.ToString()},    // Сумма платежа в копейках (или центах)
                    //{"currency", ""}, // ISO 4217
                    {"returnUrl", $"{returnUrl}{(returnUrl.Contains("?") ? "&" : "?")}{ReturnUrlParamNameMerchantOrder}={HttpUtility.UrlEncode(order.Number)}"
                    },
                    {"failUrl", failUrl},
                    //{"pageView", "DESKTOP"}, // "MOBILE"
                    {"clientId", order.OrderCustomer.CustomerID.ToString()},
                    //{"bindingId", "" }                // Идентификатор связки, созданной ранее. Может использоваться, только если у магазина есть разрешение на работу со связками. Если этот параметр передаётся в данном запросе, то это означает: 1. Данный заказ может быть оплачен только с помощью связки; 2. Плательщик будет перенаправлен на платёжную страницу, где требуется только ввод CVC.
                };

                if (!string.IsNullOrEmpty(_merchantLogin))
                    data.Add("merchantLogin", _merchantLogin);  // Чтобы зарегистрировать заказ от имени дочернего мерчанта, укажите его логин в этом параметре.

                // ФЗ 54
                if (receipt != null)
                {
                    data.Add("taxSystem", taxation);
                    data.Add("orderBundle",
                        JsonConvert.SerializeObject(receipt,
                            new JsonSerializerSettings()
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                }


                response = MakeRequest<AlfabankRegisterResponse>("register.do", data);

                if (response == null)
                    return null;

                success = response.ErrorCode == 0;

                if (!success)
                {
                    Debug.Log.Info(string.Format("AlfabankService Register. code: {0} error: {1}, obj: {2}",
                                                    response.ErrorCode, response.ErrorMessage, JsonConvert.SerializeObject(response)));
                }
                retriesNum++;
            } while (response.ErrorCode == 1 && retriesNum < 3);

            return success ? response : null;
        }

        /// <summary>
        /// Запрос состояния заказа
        /// </summary>
        public AlfabankOrderStatusResponse GetOrderStatus(string alfaOrderId, string merchantOrderid)
        {
            var data = new Dictionary<string, string>()
            {
                {"userName", _userName},
                {"password", _password},
            };

            if (!string.IsNullOrEmpty(alfaOrderId))
                data.Add("orderId", alfaOrderId);

            if (!string.IsNullOrEmpty(merchantOrderid))
                data.Add("merchantOrderNumber", merchantOrderid);

            var response = MakeRequest<AlfabankOrderStatusResponse>("getOrderStatusExtended.do", data);

            if (response == null)
                return null;

            var success = response.ErrorCode == 0;

            if (!success)
            {
                Debug.Log.Info(string.Format("AlfabankService GetOrderStatus. code: {0} error: {1}, obj: {2}",
                                                response.ErrorCode, response.ErrorMessage, JsonConvert.SerializeObject(response)));
            }

            return response;
        }

        #region Private methods

        /*
         0 – без НДС;
         1 – НДС по ставке 0%;
         2 – НДС чека по ставке 10%;
         3 – НДС чека по ставке 18%;
         4 – НДС чека по расчётной ставке 10/110;
         5 – НДС чека по расчётной ставке 18/118. 
         */
        private byte GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return 0;

            if (taxType.Value == TaxType.Vat0)
                return 1;

            if (taxType.Value == TaxType.Vat10)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 4;
                else
                    return 2;
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 5;
                else
                    return 3;
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
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

        private T MakeRequest<T>(string url, Dictionary<string, string> data = null) where T : class
        {
            try
            {
                var request = WebRequest.Create((_gatewayUrl) + url) as HttpWebRequest;
                request.Timeout = 10000;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                if (data != null)
                {
                    string dataPost = "";
                    foreach (var key in data.Keys)
                    {
                        var value = data[key];

                        if (string.IsNullOrEmpty(value))
                            continue;

                        if (dataPost != "")
                            dataPost += "&";

                        dataPost += key + "=" + HttpUtility.UrlEncode(value);
                    }

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
