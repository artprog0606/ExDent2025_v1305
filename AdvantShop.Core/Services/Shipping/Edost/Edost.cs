//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AdvantShop.Core.Services.Shipping;

namespace AdvantShop.Shipping.Edost
{
    [ShippingKey("Edost")]
    public class Edost : BaseShippingWithCargoAndCache, IShippingSupportingPaymentCashOnDelivery, IShippingSupportingPaymentPickPoint
    {
        private const string Url = "http://www.edost.ru/edost_calc_kln.php";
        private readonly string _shopId;
        private readonly string _password;
        private readonly EDemensionsUnit _demensionsUnit;

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public Edost(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _shopId = _method.Params.ElementOrDefault(EdostTemplate.ShopId);
            _password = _method.Params.ElementOrDefault(EdostTemplate.Password);
            _demensionsUnit = _method.Params.ElementOrDefault(EdostTemplate.DemensionsUnit).TryParseEnum<EDemensionsUnit>();
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (!_calculationParameters.City.IsNotEmpty() || !_calculationParameters.Region.IsNotEmpty())
                return null;

            var result = _calc(_calculationParameters.City + " (" + _calculationParameters.Region + ")", calculationVariants);

            if (!result.Any())
            {
                result = _calc(_calculationParameters.City, calculationVariants);
            }

            if (!result.Any() && !string.IsNullOrWhiteSpace(_calculationParameters.Region) && !string.IsNullOrWhiteSpace(_calculationParameters.Country))
            {
                result = _calc(_calculationParameters.City + " (" + _calculationParameters.Region + ", " + _calculationParameters.Country + ")", calculationVariants);
            }

            if (!result.Any() && !string.IsNullOrWhiteSpace(_calculationParameters.Region))
            {
                result = _calc(_calculationParameters.Region, calculationVariants);
            }

            if (!result.Any() && !string.IsNullOrWhiteSpace(_calculationParameters.Country))
            {
                result = _calc(_calculationParameters.Country, calculationVariants);
            }
            return result;

        }

        private string GetParam(string destination)
        {
            if (destination.IsNullOrEmpty() || _demensionsUnit == EDemensionsUnit.None)
                return string.Empty;

            var dimensions = GetDimensions(rate: (int)_demensionsUnit);

            var length = dimensions[0];
            var width = dimensions[1];
            var height = dimensions[2];

            var totalPrice = _totalPrice;
            var weight = GetTotalWeight();

            var a = new StringBuilder();
            a.Append("to_city=" + destination.Replace("ё", "е"));
            a.Append("&zip=" + _calculationParameters.Zip);
            a.Append("&weight=" + weight.ToString("F3"));
            a.Append("&strah=" + (totalPrice).ToString("F2"));
            a.Append("&id=" + _shopId);
            a.Append("&p=" + _password);
            a.Append("&ln=" + length);
            a.Append("&wd=" + width);
            a.Append("&hg=" + height);
            return a.ToString();
        }

        private IEnumerable<EdostTarif> FillTarif(XDocument doc)
        {
            var tarifs = new List<EdostTarif>();
            foreach (var el in doc.Root.Elements("tarif"))
            {
                var idEl = el.Element("id");
                var priceEl = el.Element("price");
                var priceCashEl = el.Element("pricecash");
                var priceTransferEl = el.Element("transfer");
                var nameEl = el.Element("name");
                var pickpointMapEl = el.Element("pickpointmap");
                var companyEl = el.Element("company");
                var dayEl = el.Element("day");

                if (idEl == null || priceEl == null || nameEl == null || companyEl == null)
                    continue;

                var item = new EdostTarif
                {
                    Id = idEl.Value.TryParseInt(),
                    Price = priceEl.Value.TryParseFloat(),
                    PriceCash = priceCashEl != null ? priceCashEl.Value.TryParseFloat() : (float?)null,
                    PriceTransfer = priceTransferEl != null ? priceTransferEl.Value.TryParseFloat() : (float?)null,
                    Name = nameEl != null ? nameEl.Value : string.Empty,
                    Company = companyEl != null ? companyEl.Value : string.Empty,
                    PickpointMap = pickpointMapEl != null ? pickpointMapEl.Value : string.Empty,
                    Day = dayEl != null ? dayEl.Value : string.Empty
                };

                item.Day = PrepareDeliveryTime(item.Day);

                if (pickpointMapEl != null)
                    tarifs.Insert(0, item);
                else
                    tarifs.Add(item);
            }
            return tarifs;
        }

        private List<EdostOffice> FillOffice(XDocument doc)
        {
            var offices = new List<EdostOffice>();
            foreach (var el in doc.Root.Elements("office"))
            {
                foreach (var tarif in el.Element("to_tarif").Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var item = new EdostOffice
                    {
                        Id = el.Element("id").Value.TryParseInt(),
                        Code = el.Element("code").Value,
                        Name = el.Element("name").Value,
                        Address = el.Element("address").Value,
                        Tel = el.Element("tel").Value,
                        Scheldule = el.Element("schedule").Value,
                        TarifId = tarif.TryParseInt()
                    };
                    offices.Add(item);
                }
            }
            return offices;
        }


        //1 - успех
        //2 - доступ к расчету заблокирован
        //3 - неверные данные магазина (пароль или идентификатор)
        //4 - неверные входные параметры
        //5 - неверный город или страна
        //6 - внутренняя ошибка сервера расчетов
        //7 - не заданы компании доставки в настройках магазина
        //8 - сервер расчета не отвечает
        //9 - превышен лимит расчетов за день
        //11 - не указан вес
        //12 - не заданы данные магазина (пароль или идентификатор)
        private static void GetErrorEdost(string str, string postData)
        {
            var error = "";
            switch (str)
            {
                case "2":
                    error = "доступ к расчету заблокирован";
                    break;

                case "3":
                    error = "неверные данные магазина (пароль или идентификатор)";
                    break;

                case "4":
                    error = "неверные входные параметры. postData: " + postData;
                    break;

                case "5":
                    error = "неверный город или страна. postData: " + postData;
                    break;

                case "6":
                    error = "внутренняя ошибка сервера расчетов";
                    break;

                case "7":
                    error = "не заданы компании доставки в настройках магазина";
                    break;

                case "8":
                    error = "сервер расчета не отвечает";
                    break;

                case "9":
                    error = "превышен лимит расчетов за день";
                    break;

                case "11":
                    error = "не указан вес";
                    break;

                case "12":
                    error = "не заданы данные магазина (пароль или идентификатор)";
                    break;
            }

            Debug.Log.Warn("Edost: " + error);
        }

        private List<BaseShippingOption> _calc(string destination, CalculationVariants calculationVariants)
        {
            if (destination.IsNullOrEmpty())
                return new List<BaseShippingOption>();

            string postData = GetParam(destination);

            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(Url);
            request.Method = "POST";

            byte[] byteArray = Encoding.GetEncoding("windows-1251").GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            request.Timeout = 3000;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return new List<BaseShippingOption>();

                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        return ParseAnswer(responseFromServer, postData, calculationVariants).ToList();
                    }
                }
            }
        }

        private IEnumerable<BaseShippingOption> ParseAnswer(string responseFromServer, string postData, CalculationVariants calculationVariants)
        {
            if (responseFromServer.IsNullOrEmpty())
                return new List<BaseShippingOption>();

            var shippingOptions = new List<EdostOption>();

            var doc = XDocument.Parse(responseFromServer);

            if (doc.Root == null)
                return shippingOptions;

            var status = doc.Root.Element("stat");
            if (status != null && status.Value != "1")
            {
                GetErrorEdost(status.Value, postData);
                return shippingOptions;
            }

            var tarifs = FillTarif(doc);
            var offices = FillOffice(doc);

            foreach (var tarif in tarifs)
            {
                var shippingOption = CreateOption(tarif, offices, calculationVariants);
                if (shippingOption != null)
                    shippingOptions.Add(shippingOption);
            }

            return shippingOptions;
        }


        private EdostOption CreateOption(EdostTarif tarif, IEnumerable<EdostOffice> offices, CalculationVariants calculationVariants)
        {
            var isOffice = offices.Any(x => x.TarifId == tarif.Id);
            var isPoint = tarif.PickpointMap.IsNotEmpty();

            if ((isOffice || isPoint)
                && !calculationVariants.HasFlag(CalculationVariants.PickPoint))
                return null;
            
            if (!(isOffice || isPoint)
                && !calculationVariants.HasFlag(CalculationVariants.Courier))
                return null;

            if (tarif.PriceCash != null || tarif.PriceTransfer != null)
            {
                if (isOffice)
                {
                    return new EdostCashOnDeliveryBoxberryOption(_method, _totalPrice, tarif, offices)
                    {
                        DeliveryId = tarif.Id,
                        Name = tarif.Company + (string.IsNullOrWhiteSpace(tarif.Name)
                                   ? string.Empty
                                   : " (" + tarif.Name + ")")
                    };
                }

                if (isPoint)
                {
                    return new EdostCashOnDeliveryPickPointOption(_method, _totalPrice, tarif)
                    {
                        DeliveryId = tarif.Id,
                        Name = tarif.Company + (string.IsNullOrWhiteSpace(tarif.Name)
                                   ? string.Empty
                                   : " (" + tarif.Name + ")")
                    };
                }

                return new EdostCashOnDeliveryOption(_method, _totalPrice, tarif)
                {
                    DeliveryId = tarif.Id,
                    Name = tarif.Company + (string.IsNullOrWhiteSpace(tarif.Name)
                               ? string.Empty
                               : " (" + tarif.Name + ")")
                };
            }

            if (isOffice)
            {
                return new EdostBoxberryOption(_method, _totalPrice, tarif, offices)
                {
                    DeliveryId = tarif.Id,
                    Name = tarif.Company +
                           (string.IsNullOrWhiteSpace(tarif.Name) ? string.Empty : " (" + tarif.Name + ")")
                };
            }

            if (isPoint)
            {
                return new EdostPickPointOption(_method, _totalPrice, tarif)
                {
                    DeliveryId = tarif.Id,
                    Name = tarif.Company +
                           (string.IsNullOrWhiteSpace(tarif.Name) ? string.Empty : " (" + tarif.Name + ")")
                };
            }

            return new EdostOption(_method, _totalPrice, tarif)
            {
                DeliveryId = tarif.Id,
                Name = tarif.Company + (string.IsNullOrWhiteSpace(tarif.Name) ? string.Empty : " (" + tarif.Name + ")")
            };
        }

        protected override int GetHashForCache()
        {
            string postData = GetParam(_calculationParameters.City + _calculationParameters.Region + _calculationParameters.Country);
            var hash = _method.ShippingMethodId ^ postData.GetHashCode();
            return hash;
        }

        private string PrepareDeliveryTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time) || _method.ExtraDeliveryTime == 0)
                return time;

            var arr = time.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 2)
            {
                var days = arr[0].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                if (days.Length == 2 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0}-{1} {2}", days[0].TryParseInt() + _method.ExtraDeliveryTime, days[1].TryParseInt() + _method.ExtraDeliveryTime, arr[1]);
                }

                if (days.Length == 1 && days[0].TryParseInt() != 0)
                {
                    return string.Format("{0} {1}", days[0].TryParseInt() + _method.ExtraDeliveryTime, arr[1]);
                }
            }

            if (arr.Length == 1 && arr[0].TryParseInt() != 0)
            {
                return (arr[0].TryParseInt() + _method.ExtraDeliveryTime).ToString();
            }

            return time;
        }
    }
}