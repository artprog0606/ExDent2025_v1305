using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Shipping;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PointDelivery
{
    [ShippingKey("PointDelivery")]
    public class PointDelivery : BaseShipping, IShippingLazyData, IShippingNoUseExtraDeliveryTime, IShippingUseDeliveryInterval
    {        
        private readonly List<DeliveryPointShipping> _points;
        private readonly EnTypePoints _typePoints;
        private readonly float _rate;
        private readonly string _deliveryTime;
        private readonly string _yaMapsApiKey;
        private readonly string _pointListTitle;

        public override bool CurrencyAllAvailable => true;
        public override EnTypeOfDelivery? TypeOfDelivery => EnTypeOfDelivery.SelfDelivery;

        public PointDelivery(ShippingMethod method, ShippingCalculationParameters calculationParameters) : base(method, calculationParameters)
        {
            _typePoints = (EnTypePoints)method.Params.ElementOrDefault(PointDeliveryTemplate.TypePoints).TryParseInt();
            _rate = _method.Params.ElementOrDefault(PointDeliveryTemplate.ShippingPrice).TryParseFloat();
            _deliveryTime = _method.Params.ElementOrDefault(PointDeliveryTemplate.DeliveryTime);
            _yaMapsApiKey = _method.Params.ElementOrDefault(PointDeliveryTemplate.YaMapsApiKey);
            _pointListTitle = _method.Params.ElementOrDefault(PointDeliveryTemplate.PointListTitle, "Пункт самовывоза:");

            var oldPoints = method.Params.ElementOrDefault(PointDeliveryTemplate.Points);
            if (!string.IsNullOrEmpty(oldPoints))
                _points = oldPoints.Split(';').Select((x, index) => new DeliveryPointShipping
                {
                    Id = index.ToString(),
                    Address = x
                }).ToList();
            else
            {
                var newPoints = method.Params.ElementOrDefault(PointDeliveryTemplate.NewPoints)
                                     ?.Replace("\"PointX\":", "\"Latitude\":")
                                      .Replace("\"PointY\":", "\"Longitude\":");
                _points = !string.IsNullOrEmpty(newPoints)
                    ? JsonConvert.DeserializeObject<List<DeliveryPointShipping>>(newPoints)
                    : new List<DeliveryPointShipping>();
            }

            _points.Where(x => x.Id == "0" || x.Id.IsNullOrEmpty()).ForEach(x => x.Id = $"{_method.ShippingMethodId}-{x.Address.GetHashCode()}");
            if (_points.Any(x => x.WarehouseId.HasValue))
            {
                var warehouses = WarehouseService.GetList();
                var toRemove = new List<DeliveryPointShipping>();
                foreach (var pointShipping in _points
                            .Where(x => x.WarehouseId.HasValue))
                {
                    var warehouse = warehouses.FirstOrDefault(x => x.Id == pointShipping.WarehouseId.Value);
                    if (warehouse is null)
                    {
                        toRemove.Add(pointShipping);
                        continue;
                    }

                    pointShipping.Address = warehouse.Address.IsNotEmpty()
                        ? warehouse.Address
                        : warehouse.Name;
                    
                    pointShipping.AddressComment =
                        warehouse.AddressComment.IsNotEmpty() ? warehouse.AddressComment : null;
                    
                    pointShipping.Description = warehouse.Description;
                    pointShipping.Latitude = warehouse.Latitude;
                    pointShipping.Longitude = warehouse.Longitude;
                    
                    var phones = new List<string>(){warehouse.Phone, warehouse.Phone2}.Where(x => x.IsNotEmpty()).ToArray();
                    if (phones.Length > 0)
                        pointShipping.Phones = phones;
                
                    var timeOfWorkList = TimeOfWorkService.GetWarehouseTimeOfWork(warehouse.Id)
                        .Select(TimeOfWorkService.FormatTimeOfWork)
                        .ToArray();
                    if (timeOfWorkList.Length > 0)
                        pointShipping.TimeWorkStr = string.Join("<br/> ", timeOfWorkList);
                }
                toRemove.ForEach(r => _points.Remove(r));
            }
        }

        protected override IEnumerable<BaseShippingOption> CalcOptions(CalculationVariants calculationVariants)
        {
            if (!calculationVariants.HasFlag(CalculationVariants.PickPoint))
                return null;

            if (_points is null || _points.Count == 0)
                return null;
            
            var options = new List<BaseShippingOption>();

            string selectedPointId = null;
            if (_calculationParameters.ShippingOption != null
                && _calculationParameters.ShippingOption.ShippingType ==
                ((ShippingKeyAttribute) typeof(PointDelivery).GetCustomAttributes(typeof(ShippingKeyAttribute), false)
                                                             .First()).Value)
            {
                selectedPointId = _calculationParameters.ShippingOption.GetOrderPickPoint()?.PickPointId;
            }


            if (_typePoints == EnTypePoints.List || _calculationParameters.IsFromAdminArea || _yaMapsApiKey.IsNullOrEmpty())
                options.Add(new PointDeliveryOption(_method, _totalPrice)
                {
                    ShippingPoints = _points,
                    SelectedPoint = selectedPointId.IsNotEmpty()
                        ? _points.FirstOrDefault(x => x.Id == selectedPointId)
                        : null,
                    Rate = _rate,
                    DeliveryTime = _deliveryTime,
                    PointListTitle = _pointListTitle
                });
            else
            {
                var option = new PointDeliveryMapOption(_method, _totalPrice)
                {
                    MapPoints = _points,
                    SelectedPoint = selectedPointId.IsNotEmpty()
                        ? _points.FirstOrDefault(x => x.Id == selectedPointId)
                        : null,
                    PickpointId = selectedPointId,
                    Rate = _rate,
                    DeliveryTime = _deliveryTime,
                    MapParams = new MapParams(),
                    PointParams = new PointParams()
                };
                SetMapData(option);

                options.Add(option);
            }

            return options;
        }

        protected override IEnumerable<BaseShippingOption> CalcOptionsToPoint(string pointId)
        {
            var selectedPoint = _points.FirstOrDefault(point => point.Id == pointId);
            if (selectedPoint is null)
                return null;

            return new[]
            {
                new PointDeliveryOption(_method, _totalPrice)
                {
                    ShippingPoints = _points,
                    Rate = _rate,
                    DeliveryTime = _deliveryTime,
                    SelectedPoint = selectedPoint
                }
            };
        }

        private void SetMapData(PointDeliveryMapOption option)
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
            option.MapParams.Destination = string.Join(", ", new[] { _calculationParameters.Country, _calculationParameters.Region, _calculationParameters.District, _calculationParameters.City}.Where(x => x.IsNotEmpty()));

            option.PointParams.IsLazyPoints = option.MapPoints.Count > 30;
            option.PointParams.PointsByDestination = false;

            if (option.PointParams.IsLazyPoints)
            {
                option.PointParams.LazyPointsParams = null;
            }
            else
            {
                option.PointParams.Points = GetYaPoints(option.MapPoints);
            }
        }

        public FeatureCollection GetYaPoints(List<DeliveryPointShipping> points)
        {
            return new FeatureCollection
            {
                Features = points.Select(p =>
                {
                    var intId = p.Id.TryParseInt(p.Id.GetHashCode());
                    return new Feature
                    {
                        Id = intId,
                        Geometry = new PointGeometry {PointX = p.Latitude ?? 0f, PointY = p.Longitude ?? 0f},
                        Options = new PointOptions {Preset = "islands#dotIcon"},
                        Properties = new PointProperties
                        {
                            BalloonContentHeader = p.Address,
                            HintContent = p.Address,
                            BalloonContentBody =
                                string.Format(
                                    "{0}{1}<a class=\"btn btn-xsmall btn-submit\" href=\"javascript:void(0)\" onclick=\"window.PointDeliveryMap({2}, '{3}')\">Выбрать</a>",
                                    p.Description,
                                    p.Description.IsNotEmpty() ? "<br>" : "",
                                    intId,
                                    p.Id)
                        }
                    };
                }).ToList()
            };
        }

        public object GetLazyData(Dictionary<string, object> data)
        {
            return GetYaPoints(_points);
        }

        public override IEnumerable<BaseShippingPoint> CalcShippingPoints(float topLeftLatitude, float topLeftLongitude, float bottomRightLatitude,
            float bottomRightLongitude)
        {
            return _points
                  .Where(point => topLeftLatitude > point.Latitude)
                  .Where(point => topLeftLongitude < point.Longitude)
                  .Where(point => bottomRightLatitude < point.Latitude)
                  .Where(point => bottomRightLongitude > point.Longitude)
                  .Select(CastPoint);
        }

        public override BaseShippingPoint LoadShippingPointInfo(string pointId)
        {
            return _points
                  .Where(point => string.Equals(point.Id, pointId, StringComparison.OrdinalIgnoreCase))
                  .Select(CastPoint)
                  .FirstOrDefault();
        }

        public BaseShippingPoint CastPoint(DeliveryPointShipping point)
        {
            return new BaseShippingPoint
            {
                Id = point.Id,
                Name = point.Address,
                Address = point.Address,
                AddressComment = point.AddressComment,
                Description = point.Description,
                Phones = point.Phones,
                TimeWorkStr = point.TimeWorkStr,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                WarehouseId = point.WarehouseId
            };
        }
    }


    public enum EnTypePoints
    {
        /// <summary>
        /// Список
        /// </summary>
        [Localize("Список")]
        List = 0,

        /// <summary>
        /// Карта
        /// </summary>
        [Localize("Карта")]
        Map = 1,
    }

}