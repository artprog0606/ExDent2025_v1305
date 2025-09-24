//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.DeliveryByZones
{
    [ShippingKey("DeliveryByZones")]
    public class DeliveryByZones : BaseShipping, IShippingLazyData, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval
    {
        public override bool CurrencyAllAvailable => true;

        private readonly List<DeliveryZone> _zones;
        private readonly string _yaMapsApiKey;
        private readonly string _withoutZoneMessage;

        public List<DeliveryZone> Zones => _zones;

        public override EnTypeOfDelivery? TypeOfDelivery => EnTypeOfDelivery.Courier;

        public DeliveryByZones(ShippingMethod method, ShippingCalculationParameters calculationParameters) 
            : base(method, calculationParameters)
        {
            _yaMapsApiKey = _method.Params.ElementOrDefault(DeliveryByZonesTemplate.YaMapsApiKey);

            var zones = method.Params.ElementOrDefault(DeliveryByZonesTemplate.Zones);
            _zones = !string.IsNullOrEmpty(zones)
                ? JsonConvert.DeserializeObject<List<DeliveryZone>>(zones)
                : new List<DeliveryZone>();
            
            _zones.Where(x => x.Id == 0).ForEach(x => x.Id = $"{_method.ShippingMethodId}-{x.Name}-{x.Rate}".GetHashCode());

            _withoutZoneMessage = _method.Params.ElementOrDefault(DeliveryByZonesTemplate.WithoutZoneMessage);
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (!calculationVariants.HasFlag(CalculationVariants.Courier))
                return null;

            if (_yaMapsApiKey.IsNullOrEmpty())
                return null;

            if (_zones.Count == 0)
                return null;

            // чистка несуществующих складов
            var warehousesId = WarehouseService.GetListIds();
            _zones
               .Where(x => x.CheckWarehouses?.Count > 0)
               .ForEach(x =>
                    x.CheckWarehouses =
                        x.CheckWarehouses
                         .Where(wId => warehousesId.Contains(wId))
                         .ToList());

            DeliveryZone zone = null;
            
            var geocoderMetaData = ParseAddress(out var error);

            if (error.IsNullOrEmpty())
            {
                zone = FindZone(geocoderMetaData.Point);
            
                if (zone is null) 
                    error = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.AddressIsOutsideZones");
            }
            
            var options = new List<BaseShippingOption>();

            if (_withoutZoneMessage.IsNullOrEmpty())
                zone = zone ?? _zones.OrderBy(x => x.Rate).First();

            if (error.IsNullOrEmpty() 
                && _totalPriceWithoutBonuses < zone?.MinimalOrderPrice)
            {
                error =
                    LocalizationService.GetResourceFormat("Admin.ShippingMethods.DeliveryByZones.MinimalOrderPrice",
                        zone.MinimalOrderPrice.Value.FormatPrice(),
                        (zone.MinimalOrderPrice.Value - _totalPriceWithoutBonuses).FormatPrice());
            }
            
            var rate = zone?.CostFree > 0f
                           && (zone?.CostFree ?? 0f) <= _totalPriceWithoutBonuses
                ? 0f
                : zone?.Rate ?? 0f;
            
            var option = new DeliveryByZonesOption(_method, _totalPriceWithoutBonuses)
            {
                Rate = rate,
                DeliveryTime = zone?.DeliveryTime,
                MessageAtAddress = error,
                Point = geocoderMetaData != null ? new[] {geocoderMetaData.Point.Latitude, geocoderMetaData.Point.Longitude} : null,
                BoundedBy = geocoderMetaData?.BoundedBy != null
                    ? new[]
                    {
                        new[] {geocoderMetaData.BoundedBy.LowerCorner.Latitude, geocoderMetaData.BoundedBy.LowerCorner.Longitude},
                        new[] {geocoderMetaData.BoundedBy.UpperCorner.Latitude, geocoderMetaData.BoundedBy.UpperCorner.Longitude}
                    }
                    : null,
                MapParams = new PointDelivery.MapParams(),
                ZoneDescription = zone?.Description,
                NotAvailablePayments = zone?.NotAvailablePayments,
                ZoneId = zone?.Id,
                MinimalOrderPrice = zone?.MinimalOrderPrice,
                CheckWarehouses = zone?.CheckWarehouses,
            };
            if (zone?.CheckWarehouses?.Count > 0)
                option.Warehouses = zone.CheckWarehouses;
            if ((zone?.ZeroPriceMessage).IsNotEmpty())
                option.ZeroPriceMessage = zone.ZeroPriceMessage;

            SetMapData(option);

            if (zone is null
                && _withoutZoneMessage.IsNotEmpty())
            {
                option.ZeroPriceMessage = _withoutZoneMessage;
                option.Rate = 0f;
            }
 
            options.Add(option);

            return options;
        }

        private void SetMapData(DeliveryByZonesOption option)
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

            option.MapParams.Lang = lang;
            option.MapParams.YandexMapsApikey = _yaMapsApiKey;
            option.MapParams.Destination = StringHelper.AggregateStrings(", ",
                    _calculationParameters.Country, 
                    _calculationParameters.Region, 
                    _calculationParameters.District,
                    _calculationParameters.City);
        }

        private GeocoderMetaData ParseAddress(out string error)
        {
            if (_calculationParameters.Street.IsNullOrEmpty())
            {
                error = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.EnterFullAddress");
                return default;
            }

            if (_calculationParameters.Longitude.HasValue
                && _calculationParameters.Latitude.HasValue)
            {
                error = default;
                return new GeocoderMetaData
                {
                    Point = new Point(
                        (decimal) _calculationParameters.Longitude.Value,
                        (decimal) _calculationParameters.Latitude.Value)
                };}

            var fullAddress = StringHelper.AggregateStrings(", ",
                _calculationParameters.Country,
                _calculationParameters.Region,
                _calculationParameters.District,
                _calculationParameters.City,
                _calculationParameters.Street,
                _calculationParameters.House,
                _calculationParameters.Structure);

            var isAddressHouse = !string.IsNullOrWhiteSpace(_calculationParameters.House)
                                 || !string.IsNullOrWhiteSpace(_calculationParameters.Structure);

            GeocoderMetaData Geocode()
            {
                return CacheManager.Get(
                    $"YandexGeocoder_{fullAddress}",
                    () => ModulesExecuter.Geocode(
                        fullAddress,
                        metaData =>
                        {
                            // достаточно до улицы, дальше ValidateAddress подскажет что нужно указать дом
                            if (metaData.Precision > Precision.Street
                                || metaData.Precision == Precision.None
                                || (isAddressHouse ? metaData.Kind >= Kind.Street : metaData.Kind > Kind.Street)
                                || metaData.Kind == Kind.None)
                                return false;
                            return true;
                        },
                        _yaMapsApiKey));
            }

            var geocoderMetaData = Geocode();

            if (geocoderMetaData is null
                && _calculationParameters.District.IsNotEmpty())
            {
                // пробуем без района
                fullAddress = StringHelper.AggregateStrings(", ",
                    _calculationParameters.Country,
                    _calculationParameters.Region,
                    // _calculationParameters.District,
                    _calculationParameters.City,
                    _calculationParameters.Street,
                    _calculationParameters.House,
                    _calculationParameters.Structure);
                
                geocoderMetaData = Geocode();
            }
            
            if (geocoderMetaData is null)
            {
                error = LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.IncorrectAddress");
                return default;
            }
            
            error = ValidateAddress(geocoderMetaData.Precision, geocoderMetaData.Kind);
            if (error.IsNotEmpty())
                return default;

            return geocoderMetaData;
        }

        private DeliveryZone FindZone(Point point)
        {
            return _zones.FirstOrDefault(zone => IsIncludeZone(zone, point));
        }

        private const float Coeff = 100000f;
        private bool IsIncludeZone(DeliveryZone zone, Point point)
        {
            var pointF = new System.Drawing.PointF((float) point.Latitude * Coeff, (float) point.Longitude * Coeff);

            // вхождение в основную область полигона
            if (!IsPointInsidePolygon(zone.Coordinates[0], pointF))
                return false;
            
            //вхождение в "дыры" полигона
            for (var index = 1; index < zone.Coordinates.Length; index++)
                if (IsPointInsidePolygon(zone.Coordinates[index], pointF))
                    return false;
            
            return true;
        }

        private bool IsPointInsidePolygon(decimal[,] zoneCoordinate, System.Drawing.PointF pointF)
        {
            var points = new System.Drawing.PointF[zoneCoordinate.GetLength(0)];
            for (var index = 0; index < zoneCoordinate.GetLength(0); index++)
                points[index] =
                    new System.Drawing.PointF(
                        (float) zoneCoordinate[index, 0] * Coeff,
                        (float) zoneCoordinate[index, 1] * Coeff);

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddPolygon(points);
                return path.IsVisible(pointF);
            }
        }

        private string ValidateAddress(Precision precision, Kind kind)
        {
            // if (precision == Precision.Number)
            //     return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.NotFoundCorpusOfAddress");

            // if (precision == Precision.Near)
            //     return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.HouseNotFound");
            
            // есть координаты с точностью до улицы
            if (precision == Precision.Street)
            {
                // дом не указан
                if (kind > Kind.House
                    || kind == Kind.None)
                    return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.NoHouse");
                
                return default;
                //     return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.AddressNotFound");
            }


            if (precision == Precision.Locality
                || precision == Precision.None)
                return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.StreetNotFound");

            // if ((precision != Precision.Exact && precision != Precision.Range)
            if (precision > Precision.Range
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                || precision == Precision.None
                || kind > Kind.House
                || kind == Kind.None)
                return LocalizationService.GetResource("Admin.ShippingMethods.DeliveryByZones.IsNotAddressOfHouse");

            return default;
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            return GetYaPolygons(_zones);
        }

        private FeatureCollection GetYaPolygons(List<DeliveryZone> zones)
        {
            return new FeatureCollection
            {
                MetaData = new MetaData()
                {
                    Creator = "advantshop",
                    Name = "DeliveryZones"
                },
                Features = zones.Select(zone =>
                    new Feature {
                        Id = zone.Id,
                        Geometry = new PointGeometry { Coordinates = zone.Coordinates},
                        Properties = new PolygonProperties
                        {
                            Description = zone.Description,
                            FillColor = zone.FillColor,
                            FillOpacity = zone.FillOpacity,
                            StrokeColor = zone.StrokeColor,
                            StrokeOpacity = zone.StrokeOpacity,
                            StrokeWidth = zone.StrokeWidth,
                        }
                    }).ToList()
            };
        }
    }
}