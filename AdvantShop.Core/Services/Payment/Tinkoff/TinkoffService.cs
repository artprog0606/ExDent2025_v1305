//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Payment.Tinkoff
{
    // Документация: https://oplata.tinkoff.ru/landing/develop/documentation/termins_and_operations

    public class TinkoffService
    {
        private const string BaseUrl = "https://securepay.tinkoff.ru/v2/";

        private readonly string _terminalKey;
        private readonly string _secretKey;
        private readonly bool _sendReceiptData;
        private readonly bool _useFfd12;

        public TinkoffService(string terminalKey, string secretKey, bool sendReceiptData, bool useFfd12)
        {
            _terminalKey = terminalKey;
            _secretKey = secretKey;
            _sendReceiptData = sendReceiptData;
            _useFfd12 = useFfd12;
        }

        /// <summary>
        /// Запрос создание заказа
        /// </summary>
        public TinkoffInitResponse Init(Order order, string description, string taxation, Currency paymentCurrency, TaxElement tax)
        {
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;
            float quantity;
            var receipt = _sendReceiptData
                ? new Receipt()
                {
                    Email = !string.IsNullOrEmpty(order.OrderCustomer.Email) ? order.OrderCustomer.Email : null,
                    Phone = order.OrderCustomer.StandardPhone.HasValue && order.OrderCustomer.StandardPhone.ToString().Length <= 15 
                        ? ("+" + order.OrderCustomer.StandardPhone.ToString()) 
                        : null,
                    Taxation = taxation,
                    Items =
                        order
                            .GetOrderItemsForFiscal(paymentCurrency)
                            .Select(item => new Item()
                            {
                                Name = (item.Name + (item.Color.IsNotEmpty() || item.Size.IsNotEmpty()
                                    ? string.Format(" ({0}{1}{2})",
                                        item.Color.IsNotEmpty()
                                            ? $"{SettingsCatalog.ColorsHeader}: {item.Color}"
                                            : null,
                                        item.Color.IsNotEmpty() && item.Size.IsNotEmpty()
                                            ? ", "
                                            : null,
                                        item.Size.IsNotEmpty()
                                            ? $"{SettingsCatalog.SizesHeader}: {item.Size}"
                                            : null)
                                    : string.Empty)).Reduce(128),
                                ShopCode = item.ArtNo,
                                Quantity = quantity = (float)Math.Round(item.Amount, 3),
                                Amount = (int)Math.Round(item.Price * quantity * 100, 0),
                                Price = (int)Math.Round(item.Price * 100, 0),
                                Tax = GetTaxType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType),
                                PaymentMethod = GetPaymentMethodType(item.PaymentMethodType),
                                PaymentObject = item.PaymentSubjectType.ToString(),
                                MeasurementUnit = _useFfd12
                                    ? GetMeasure(item.MeasureType)
                                    : null
                            })
                            .ToList(),
                    FfdVersion = _useFfd12 ? "1.2" : "1.05"
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        Name = LocalizationService.GetResource("Core.Payment.Receipt.GiftCertificateName"),
                        ShopCode = x.CertificateCode,
                        Amount = (int)(Math.Round(x.Sum * 100, 0)),
                        Price = (int)(Math.Round(x.Sum * 100, 0)),
                        Quantity = 1,
                        Tax = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentMethod = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentObject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString(),
                        MeasurementUnit = _useFfd12
                            ? GetMeasure(MeasureType.Piece)
                            : null
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.Items.Add(new Item()
                {
                    Name = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    ShopCode = LocalizationService.GetResource("Core.Payment.Receipt.ShippingName"),
                    Amount = (int)(Math.Round(orderShippingCostWithDiscount * 100, 0)),
                    Price = (int)(Math.Round(orderShippingCostWithDiscount * 100, 0)),
                    Quantity = 1,
                    Tax = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    PaymentMethod = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    PaymentObject = order.ShippingPaymentSubjectType.ToString(),
                    MeasurementUnit = _useFfd12
                        ? GetMeasure(MeasureType.Piece)
                        : null
                });
            }
            
            long sum = 0;

            if (receipt != null && receipt.Items != null)
            {
                foreach (var item in receipt.Items)
                    sum += item.Amount;
            }
            else
            {
                sum = (long)Math.Round(Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2) * 100);
            }

            TinkoffInitResponse response;
            
            var orderStrId = string.Format("{0}_{1}", order.OrderID, DateTime.Now.ToUnixTime());

            var data = new TinkoffInitData
            {
                TerminalKey = _terminalKey,
                Amount = sum, // Сумма платежа в копейках
                OrderId = orderStrId,
                Description = description,
                Receipt = receipt
            };

            response = MakeRequest<TinkoffInitResponse, TinkoffInitData>("Init", data);

            if (response == null)
                return null;

            if (!response.Success)
            {
                Debug.Log.Warn(string.Format("TinkoffService Init. code: {0} error: {1}, details: {2}, response: {3}",
                    response.ErrorCode, response.Message, response.Details, JsonConvert.SerializeObject(response)));
            }
            
            return response;
        }

        // http://www.consultant.ru/document/cons_doc_LAW_53447/0833d33120c2d09bb836ec79d9c93260aa6a0ceb/
        // http://www.consultant.ru/document/cons_doc_LAW_53447/
        private string GetMeasure(MeasureType? itemMeasureType)
        {
            if (itemMeasureType is null)
                return null;
            
            switch (itemMeasureType.Value)
            {
                case MeasureType.Piece:
                    return "шт";
                case MeasureType.Gram:
                    return "г";
                case MeasureType.Kilogram:
                    return "кг";
                case MeasureType.Ton:
                    return "т";
                case MeasureType.Centimetre:
                    return "см";
                case MeasureType.Decimeter:
                    return "дм";
                case MeasureType.Metre:
                    return "м";
                case MeasureType.SquareCentimeter:
                    return "см2";
                case MeasureType.SquareDecimeter:
                    return "дм2";
                case MeasureType.SquareMeter:
                    return "м2";
                case MeasureType.Milliliter:
                    return "мл";
                case MeasureType.Liter:
                    return "л";
                case MeasureType.CubicMeter:
                    return "м3";
                case MeasureType.KilowattHour:
                    return "кВт*ч";
                case MeasureType.Gigacaloria:
                    return "Гкал";
                case MeasureType.Day:
                    return "сут";
                case MeasureType.Hour:
                    return "ч";
                case MeasureType.Minute:
                    return "мин";
                case MeasureType.Second:
                    return "с";
                case MeasureType.Kilobyte:
                    return "Кбайт";
                case MeasureType.Megabyte:
                    return "Мбайт";
                case MeasureType.Gigabyte:
                    return "Гбайт";
                case MeasureType.Terabyte:
                    return "Тбайт";
                case MeasureType.Other:
                    return null;
            }

            return null;
        }

        #region Private methods

        private string GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return "none";

            if (taxType.Value == TaxType.Vat0)
                return "vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat110";
                else
                    return "vat10";
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat118";
                else
                    return "vat18";
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat120";
                else
                    return "vat20";
            }

            if (taxType.Value == TaxType.Vat5)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat105";
                else
                    return "vat5";
            }

            if (taxType.Value == TaxType.Vat7)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat107";
                else
                    return "vat7";
            }


            return "none";
        }

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
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in Tinkoff");
            }
        }


        private T MakeRequest<T, TData>(string url, TData data = null) 
            where T : class
            where TData : TinkoffBaseData
        {
            try
            {
                var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.TerminalKey))
                        data.TerminalKey = _terminalKey;

                    data.Token = GenerateToken(AsDictionary(data));

                    string dataPost = JsonConvert.SerializeObject(data);

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

        #region Help

        public string GenerateToken(Dictionary<string, string> data)
        {
            if (data.ContainsKey("Token"))
                data.Remove("Token");

            if (data.ContainsKey("Password"))
                data.Remove("Password");
            data.Add("Password", _secretKey);

            var stringBuilder = new StringBuilder();
            foreach (var key in data.Keys.OrderBy(x => x))
            {
                var value = data[key];

                if (string.IsNullOrEmpty(value))
                    continue;

                stringBuilder.Append(value);
            }
            var token = stringBuilder.ToString().Sha256(upperCase: false, encoding: Encoding.UTF8);

            data.Remove("Password");

            return token;
        }

        public Dictionary<string, string> AsDictionary(object source, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var property in source.GetType().GetProperties(bindingAttr))
            {
                if (property.PropertyType.IsValueType
                    || property.PropertyType == typeof(string)
                    || property.PropertyType.IsEnum)
                {
                    var val = property.GetValue(source, BindingFlags.GetProperty, null, null, CultureInfo.InvariantCulture);
                    dictionary.Add(property.Name, Convert.ToString(val?.ToString(), CultureInfo.InvariantCulture));
                }
            }
            return dictionary;
        }

        public TinkoffNotifyData ReadNotifyData(string postPayload)
        {
            return JsonConvert.DeserializeObject<TinkoffNotifyData>(postPayload);
        }

        #endregion
    }
}
