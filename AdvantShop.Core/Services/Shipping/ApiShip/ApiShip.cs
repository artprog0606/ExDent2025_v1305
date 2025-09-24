using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Helpers;
using System.Web;
using AdvantShop.Shipping.ApiShip.Api;
using AdvantShop.Repository;
using AdvantShop.Core.Services.Shipping.ApiShip.Api;
using AdvantShop.Shipping.PointDelivery;

namespace AdvantShop.Shipping.ApiShip
{
    [ShippingKey("ApiShip")]
    public partial class ApiShip : BaseShippingWithCargo, IShippingLazyData
    {
        /* Боевая среда #
Адрес отправления запросов: https://api.apiship.ru/v1/
            Интерактивная документация доступна по адресу: https://api.apiship.ru/doc/
            PHP SDK доступен по адресу: https://github.com/apiship/apiship-sdk-php
            Тестовая среда #
Адрес отправления запросов: http://api.dev.apiship.ru/v1/
            Интерактивная документация доступна по адресу: http://api.dev.apiship.ru/doc/
            ЛК для тестовой среды: http://a.dev.apiship.ru/
            Логин и пароль для тестов: test */
        #region Ctor

        private readonly string _apiKey;
        private readonly string _cityFrom;
        private readonly bool _showPointsAsList;
        private readonly bool _showAddressComment;
        private readonly string _yaMapsApiKey;
        private readonly string _senderRegion;
        private readonly string _senderAddress;
        private readonly int _sendedCountry;
        private readonly ApiShipShippingService _apiShipService;
        public const string KeyTextSendOrder = "SendOrderApiShip";
        public const string KeyTextOrderIdApiShip = "OrderIdApiShip";

        public override string[] CurrencyIso3Available { get { return new[] { "RUB" }; } }

        public ApiShip(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _apiKey = _method.Params.ElementOrDefault(ApiShipTemplate.ApiKey);
            _cityFrom = _method.Params.ElementOrDefault(ApiShipTemplate.CityFrom);
            _showPointsAsList = (method.Params.ElementOrDefault(ApiShipTemplate.ShowPointsAsList)?? "True").TryParseBool();
            _showAddressComment = method.Params.ElementOrDefault(ApiShipTemplate.ShowAddressComment).TryParseBool();
            _yaMapsApiKey = _method.Params.ElementOrDefault(ApiShipTemplate.YaMapsApiKey);
            _senderRegion = _method.Params.ElementOrDefault(ApiShipTemplate.SenderRegion);
            _senderAddress = _method.Params.ElementOrDefault(ApiShipTemplate.SenderAddress);
            _sendedCountry = int.Parse(method.Params.ElementOrDefault(ApiShipTemplate.SendedCountry) ?? "0");

            if (!string.IsNullOrEmpty(_apiKey))
                _apiShipService = new ApiShipShippingService(_apiKey);            
        }

        #endregion

        public string CityFrom => _cityFrom;
        public string YaMapsApiKey => _yaMapsApiKey;
        public bool ShowAddressComment => _showAddressComment;
        public bool ShowPointsAsList => _showPointsAsList;
        public string SenderRegion => _senderRegion;
        public string SenderAddress => _senderAddress;
        public ApiShipShippingService ApiShipService => _apiShipService;

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            var options = new List<BaseShippingOption>();
            try
            {
                var dimensionsInCentimeters = GetDimensions(rate: 10);
                var weightInKilogramms = GetTotalWeight();
                string cityActual = _calculationParameters.City;
                string region = _calculationParameters.Region;
                List<ApiShipTariff> tariffs = new List<ApiShipTariff>();
                List<ApiShipProvider> providers = new List<ApiShipProvider>();
                ApiShipCalculatorResponseModel apiCalc = new ApiShipCalculatorResponseModel();

                tariffs = ApiShipService.GetApiShipTariffs();
                providers = ApiShipService.GetApiShipProviders();   
                List<string> providerKeys = tariffs != null ? tariffs.Select(x => x.ProviderKey).Distinct().ToList() : new List<string>();
                
                apiCalc = GetCalculator(cityActual, region, _totalPrice, dimensionsInCentimeters, weightInKilogramms, tariffs, calculationVariants);
                if (apiCalc == null)
                    return options;

                if (apiCalc.DeliveryToPoint != null && calculationVariants.HasFlag(CalculationVariants.PickPoint))
                {
                    var pointsDelivery = ApiShipService.GetApiShipPoints(cityActual, providerKeys);
                    if (pointsDelivery.Count != 0)
                    {
                        foreach (var deliveryToPoint in apiCalc.DeliveryToPoint)
                        {
                            foreach (var tariff in deliveryToPoint.Tariffs)
                            {
                                if (!tariff.DeliveryTypes.Contains(2))
                                    continue;
                                tariff.ProviderKey = deliveryToPoint.ProviderKey;
                                var provider = tariff != null ? providers.FirstOrDefault(x => x.key == tariff.ProviderKey) : null;
                                var option = GetOptionToPoint(tariff, pointsDelivery, provider, cityActual, weightInKilogramms);
                                if (option == null)
                                    continue;
                                options.Add(option);
                            }
                        }
                    }
                }

                if (apiCalc.DeliveryToDoor != null && calculationVariants.HasFlag(CalculationVariants.Courier))
                {
                    foreach (var deliveryToPoint in apiCalc.DeliveryToDoor)
                    {
                        foreach (var tariff in deliveryToPoint.Tariffs)
                        {
                            if (!tariff.DeliveryTypes.Contains(1))
                                continue;
                            tariff.ProviderKey = deliveryToPoint.ProviderKey;
                            var provider = tariff != null ? providers.FirstOrDefault(x => x.key == tariff.ProviderKey) : null;
                            var option = GetOptionCourier(tariff, provider);
                            if (option == null)
                                continue;
                            options.Add(option);
                        }
                    }
                }

                return options;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message);
            }
            return options;
        }

        private BaseShippingOption GetOptionToPoint(ApiShipTariffToPoint tariffPoint, List<ApiShipPoint> pointsDelivery, ApiShipProvider provider, string cityActual, float weightInKilogramms)
        {
            BaseShippingOption option = null;
            List<ApiShipPoint> points = new List<ApiShipPoint>();
            ApiShipShippingPoint apiShipPointSelected = null;
            points = pointsDelivery.Where(x => tariffPoint.PointIds.Contains(x.Id))
                            .Where(x => (x.AvailableOperation == (int)ApiShipTypeOpertionOnPoint.extradition || x.AvailableOperation == (int)ApiShipTypeOpertionOnPoint.receptionAndDelivery)
                                        && (!x.Limits.MaxWeight.HasValue || (x.Limits.MaxWeight.HasValue && weightInKilogramms <= x.Limits.MaxWeight)))
                            .OrderBy(x => x.Street).ThenBy(x => x.House).ToList();

            if (points.Count == 0)
                return null;

            var firstPoint = points.FirstOrDefault();

            if (_calculationParameters.ShippingOption != null &&
                _calculationParameters.ShippingOption.SelectedPoint is ApiShipShippingPoint apiShipPoint &&
               points.Any(x => x.Id.ToString() == apiShipPoint.Id))
            {
                apiShipPointSelected = apiShipPoint;
            }


            if (!ShowPointsAsList && !YaMapsApiKey.IsNullOrEmpty())
            {
                option = new ApiShipPointDeliveryMapOption(_method, _totalPrice);
                if (option is ApiShipPointDeliveryMapOption deliveryMapOption)
                {
                    deliveryMapOption.CityTo = cityActual;
                    deliveryMapOption.ProviderCode = provider.key ?? string.Empty;
                    deliveryMapOption.TariffId = tariffPoint.TariffId.ToString();
                    deliveryMapOption.CurrentPoints = points.Select(x => new ApiShipShippingPoint(x)).ToList();
                    deliveryMapOption.CityTo = cityActual;
                    deliveryMapOption.SelectedPoint = apiShipPointSelected ?? new ApiShipShippingPoint(firstPoint);
                    SetMapData(deliveryMapOption, weightInKilogramms);
                    if (deliveryMapOption.CurrentPoints.Count == 0 || tariffPoint.DeliveryCost == 0)
                        return null;
                }
            }
            else
            {
                option = new ApiShipPointOption(_method, _totalPrice);
                if (option is ApiShipPointOption pointOption)
                {
                    pointOption.CityTo = cityActual;
                    pointOption.ProviderCode = provider.key ?? string.Empty;
                    pointOption.TariffId = tariffPoint.TariffId.ToString();
                    pointOption.CurrentPoints = points.Select(x => new ApiShipShippingPoint(x)).ToList();
                    pointOption.SelectedPoint = apiShipPointSelected ?? new ApiShipShippingPoint(firstPoint);
                    if (pointOption.CurrentPoints.Count == 0 || tariffPoint.DeliveryCost == 0)
                        return null;
                }
            }
            option.HideAddressBlock = true;
            option.IsAvailablePaymentCashOnDelivery = option.SelectedPoint.AvailableCashOnDelivery.HasValue
                ? option.SelectedPoint.AvailableCashOnDelivery.Value
            : true;
            option.Name = (provider != null ? provider.name : (tariffPoint.ProviderKey ?? option.Name)) + (!tariffPoint.TariffName.IsNullOrEmpty() ? " (" + tariffPoint.TariffName + ")" : "");
            option.DeliveryId = $"{tariffPoint.TariffId ?? 0}_{string.Join(",", tariffPoint.PickupTypes.OrderBy(x => x))}_{string.Join(",", tariffPoint.DeliveryTypes.OrderBy(x => x))}_PickPoint".GetHashCode(); // OrderBy потому что приходит в разном порядке
            option.Rate = (float)tariffPoint.DeliveryCost;
            option.DeliveryTime = tariffPoint.DaysMin == tariffPoint.DaysMax ? tariffPoint.DaysMin.ToString() + " дн." : tariffPoint.DaysMin + "-" + tariffPoint.DaysMax + " дн.";
            option.IconName = ShippingIcons.GetShippingIcon(option.ShippingType, _method.IconFileName?.PhotoName, option.Name);

            return option;
        }

        private BaseShippingOption GetOptionCourier(ApiShipTariffToDoor tariffToDoor, ApiShipProvider provider)
        {
            var option = new ApiShipCourierOption(_method, _totalPrice)
            {
                IsAvailablePaymentCashOnDelivery = true,
                ProviderKey = tariffToDoor.ProviderKey,
                TariffId = tariffToDoor.TariffId.ToString(),
                DeliveryId = $"{tariffToDoor.TariffId ?? 0}_{string.Join(",", tariffToDoor.PickupTypes.OrderBy(x => x))}_{string.Join(",", tariffToDoor.DeliveryTypes.OrderBy(x => x))}_Courier".GetHashCode() // OrderBy потому что приходит в разном порядке
            };
            option.Name = (provider != null ? provider.name : (tariffToDoor.ProviderKey ?? option.Name)) + (!tariffToDoor.TariffName.IsNullOrEmpty() ? " (" + tariffToDoor.TariffName + ")" : "");
            option.Rate = (float)tariffToDoor.DeliveryCost;
            option.DeliveryTime = tariffToDoor.DaysMin == tariffToDoor.DaysMax ? tariffToDoor.DaysMin.ToString() + " дн." : tariffToDoor.DaysMin + "-" + tariffToDoor.DaysMax + " дн.";
            option.IconName = ShippingIcons.GetShippingIcon(option.ShippingType, _method.IconFileName?.PhotoName, option.Name);
            return option;
        }

        private ApiShipCalculatorResponseModel GetCalculator(string city, string region, float sumCost, float[] dimensionsSizes, float weight, List<ApiShipTariff> tariffs, CalculationVariants calculationVariants)
        {
            var model = new ApiShipCalculatorRequestModel();
            string defaultCountryIso2 = "RU";
            var modelFrom = new ApiShipCalculatorObject();
            Country countryTo = new Country();
            Country countryFrom = new Country();
            if (!string.IsNullOrEmpty(_calculationParameters.Country))
            {
                countryTo = CountryService.GetCountryByName(_calculationParameters.Country);
            }
            if (!string.IsNullOrEmpty(region) && (countryTo == null || countryTo.CountryId == 0))
            {
                var regionSite = RegionService.GetRegionByName(region);
                if (regionSite != null && regionSite.RegionId > 0)
                {
                    countryTo = CountryService.GetCountry(regionSite.CountryId);
                }
            }

            if (_sendedCountry > 0)
            {
                countryFrom = CountryService.GetCountry(_sendedCountry);
            }

            modelFrom.CountryCode = countryFrom != null && countryFrom.CountryId > 0 ? countryFrom.Iso2 : defaultCountryIso2;
            modelFrom.AddressString = !SenderAddress.IsNullOrEmpty() ? SenderAddress : string.Empty;
            modelFrom.Region = !SenderRegion.IsNullOrEmpty() ? SenderRegion : string.Empty;
            modelFrom.City = !CityFrom.IsNullOrEmpty() ? CityFrom : string.Empty;
            model.From = modelFrom;
            var modelTo = new ApiShipCalculatorObject();

            modelTo.CountryCode = countryTo != null && countryTo.CountryId > 0 ? countryTo.Iso2 : defaultCountryIso2;
            modelTo.Region = region ?? string.Empty;
            modelTo.City = city ?? string.Empty;
            model.To = modelTo;

            model.Places = new List<ApiShipCalculatorPlaces>();
            var modelPlaces = new ApiShipCalculatorPlaces();
            modelPlaces.Height = (int)Math.Ceiling(dimensionsSizes[0] / 10);
            modelPlaces.Length = (int)Math.Ceiling(dimensionsSizes[1] / 10);
            modelPlaces.Width = (int)Math.Ceiling(dimensionsSizes[2] / 10);
            modelPlaces.Weight = (int)Math.Ceiling(weight * 1000);
            model.Places.Add(modelPlaces);
            model.PickupDate = DateTime.Today.ToString("yyyy-MM-dd");
            model.PickupTypes = new List<int>();
            model.DeliveryTypes = new List<int>();
            if (calculationVariants.HasFlag(CalculationVariants.Courier))
                model.DeliveryTypes.Add(1);
            if (calculationVariants.HasFlag(CalculationVariants.PickPoint))
                model.DeliveryTypes.Add(2);
            model.AssessedCost = (int)sumCost;
            model.CodCost = (int)sumCost;
            model.IncludeFees = false;
            model.Timeout = 10000;
            model.SkipTariffRules = false;
            List<string> providerKeys = tariffs != null ? tariffs.Select(x => x.ProviderKey).Distinct().ToList() : null;
            model.ProviderKeys = providerKeys;
            model.PromoCode = "";
            model.CustomCode = "";
            model.TariffIds = tariffs != null ? tariffs.Select(x => Convert.ToInt32(x.Id)).ToList() : new List<int>();
            var calculatorResult = ApiShipService.GetCalculator(model);
            return calculatorResult;
        }

        private void SetMapData(ApiShipPointDeliveryMapOption option, float goodsWeight)
        {
            string lang = "en_US";
            switch (Localization.Culture.Language)
            {
                case Localization.Culture.SupportLanguage.Russian:
                    lang = "ru_RU";
                    break;
                case Localization.Culture.SupportLanguage.English:
                    lang = "en_US";
                    break;
                case Localization.Culture.SupportLanguage.Ukrainian:
                    lang = "uk_UA";
                    break;
            }

            var city = _calculationParameters.City;
            var region = _calculationParameters.Region;
            var country = _calculationParameters.Country;

            option.MapParams = new MapParams();
            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = YaMapsApiKey;
            option.MapParams.Destination = string.Join(", ", new[] { country, region, city }.Where(x => x.IsNotEmpty()));
            option.PointParams = new PointParams();
            option.PointParams.IsLazyPoints = (option.CurrentPoints != null ? option.CurrentPoints.Count : 0) > 30;
            option.PointParams.PointsByDestination = true;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = new Dictionary<string, object>
                {
                    { "city", option.CityTo },
                    { "weight", goodsWeight.ToInvariantString() },
                    { "providerCode", option.ProviderCode.ToString() },
                };
            }
            else
            {
                option.PointParams.Points = GetFeatureCollection(option.CurrentPoints);
            }
        }

        public FeatureCollection GetFeatureCollection(List<ApiShipShippingPoint> points)
        {
            try
            {
                return new FeatureCollection
                {
                    Features = points.Select(p =>
                        new Feature
                        {
                            Id = Convert.ToInt32(p.Id),
                            Geometry = new PointGeometry { PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f },
                            Options = new PointOptions { Preset = "islands#dotIcon" },
                            Properties = new PointProperties
                            {
                                BalloonContentHeader = p.Address,
                                HintContent = p.Address,
                                BalloonContentBody =
                                    string.Format("<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({0}, '{1}')\">Выбрать</a>",
                                        p.Id,
                                        p.Code),
                                BalloonContentFooter = ShowAddressComment
                                    ? p.Description
                                    : null
                            }
                        }).ToList()
                };
            }
            catch
            {
                return new FeatureCollection();
            }
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            if (data == null || !data.ContainsKey("city") || data["city"] == null)
                return null;

            var city = data["city"].ToString();
            var goodsWeight = data["weight"].ToString().TryParseFloat();
            var providerCode = data["providerCode"].ToString();
            var points = ApiShipService.GetApiShipPoints(city,new List<string> { providerCode });
            points = points.Where(x => (x.AvailableOperation == (int)ApiShipTypeOpertionOnPoint.extradition || x.AvailableOperation == (int)ApiShipTypeOpertionOnPoint.receptionAndDelivery)
                                    && (!x.Limits.MaxWeight.HasValue || (x.Limits.MaxWeight.HasValue && goodsWeight <= x.Limits.MaxWeight))).ToList();
            var pointShipping = points.Select(x => new ApiShipShippingPoint(x)).ToList();
            return GetFeatureCollection(pointShipping ?? new List<ApiShipShippingPoint>());
        }
    }
}