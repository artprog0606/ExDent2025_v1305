using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries.v2;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Deliveries.v2
{
    internal sealed class GetDeliveries : AbstractCommandHandler<DeliveryResponse>
    {
        private readonly DeliveryFilter _filter;

        public GetDeliveries(DeliveryFilter filter)
        {
            _filter = filter;
        }

        protected override DeliveryResponse Handle()
        {
            var methodsWithPoints = new List<string>() {"PointDelivery", "SelfDelivery"};

            var calculationParameters = GetCalculationParameters();
            var deliveries = new List<Delivery>();
            
            foreach (var method in GetShippingMethods(calculationParameters))
            {
                var delivery = new Delivery(method);

                if (methodsWithPoints.Contains(method.ShippingType, StringComparer.OrdinalIgnoreCase)) 
                    delivery.Points = GetPoints(method, calculationParameters);
                
                deliveries.Add(delivery);
            }

            var nearest = _filter.UserCoordinates != null ? GetNearestDelivery(deliveries) : null;

            return new DeliveryResponse(deliveries, nearest);
        }

        private ShippingCalculationParameters GetCalculationParameters()
        {
            City city = null;
            Region region = null;
            Country country = null;

            if (_filter != null)
            {
                region = !string.IsNullOrWhiteSpace(_filter.Region)
                    ? RegionService.GetRegionByName(_filter.Region)
                    : null;

                country = !string.IsNullOrWhiteSpace(_filter.Country)
                    ? CountryService.GetCountryByName(_filter.Country)
                    : null;

                if (!string.IsNullOrWhiteSpace(_filter.ZipCode))
                    city = CityService.GetCityByZip(_filter.ZipCode);

                if (city == null && !string.IsNullOrWhiteSpace(_filter.City) && region != null)
                    city = CityService.GetCityByName(_filter.City, region.RegionId);

                if (city == null && !string.IsNullOrWhiteSpace(_filter.City))
                    city = CityService.GetCityByName(_filter.City);

                if (country == null && region != null)
                    country = CountryService.GetCountry(region.CountryId);
            }

            return new ShippingCalculationParameters()
            {
                Country = country?.Name ?? "",
                Region = region?.Name ?? "",
                City = city?.Name ?? "",
                District = _filter?.District ?? "",
                PreOrderItems = new List<PreOrderItem>(),
                Currency = CurrencyService.CurrentCurrency
            };
        }

        private List<ShippingMethod> GetShippingMethods(ShippingCalculationParameters shippingCalculationParameters)
        {
            var items = new List<ShippingMethod>();
            
            var shippingMethods = ShippingMethodService.GetAllShippingMethods(true);
                
            if (!string.IsNullOrWhiteSpace(_filter.Type))
                shippingMethods = shippingMethods.Where(x => x.ShippingType == _filter.Type).ToList();
            
            if (_filter.Types != null && _filter.Types.Count > 0)
                shippingMethods = shippingMethods.Where(x => _filter.Types.Contains(x.ShippingType, StringComparer.OrdinalIgnoreCase)).ToList();

            foreach (var shipping in shippingMethods)
            {
                var validGeo = false;
                
                if (ShippingPaymentGeoMaping.IsExistGeoShipping(shipping.ShippingMethodId))
                {
                    if (ShippingPaymentGeoMaping.CheckShippingEnabledGeo(
                            shipping.ShippingMethodId, 
                            shippingCalculationParameters.Country, 
                            shippingCalculationParameters.Region, 
                            shippingCalculationParameters.City, 
                            shippingCalculationParameters.District))
                        validGeo = true;
                }
                else
                    validGeo = true;

                if (validGeo)
                    items.Add(shipping);
            }

            return items;
        }

        private List<DeliveryPoint> GetPoints(ShippingMethod method, ShippingCalculationParameters calculationParameters)
        {
            var inHouse =
                SettingsApiAuth.ShowInHouseInDeliveryWidgetOnMain &&
                SettingsApiAuth.InHouseShippingMethodIds != null &&
                SettingsApiAuth.InHouseShippingMethodIds.Contains(method.ShippingMethodId);

            if (_filter.InHouse != null)
            {
                if (_filter.InHouse.Value && !inHouse)
                    return null;
                if (!_filter.InHouse.Value && inHouse)
                    return null;
            }

            var type = ReflectionExt.GetTypeByAttributeValue<ShippingKeyAttribute>(typeof(BaseShipping), atr => atr.Value, method.ShippingType);

            if (type is null)
                return null;
            
            var shipping = (BaseShipping)Activator.CreateInstance(type, method, calculationParameters.DeepCloneJson(Newtonsoft.Json.TypeNameHandling.All));
                    
            // получение всех точек с координатами
            var pointDeliveryOption = shipping.CalcShippingPoints(
                topLeftLatitude: 90f,
                topLeftLongitude:-180f,
                bottomRightLatitude:-90f,
                bottomRightLongitude: 180f);

            return pointDeliveryOption
                 ?.Select(p =>
                   {
                       var deliveryPoint = DeliveryPoint.CreateBy(p);
                       deliveryPoint.SetInHouse(inHouse);
                       return deliveryPoint;
                   })
                  .ToList();
        }

        private NearestDelivery GetNearestDelivery(List<Delivery> deliveries)
        {
            Delivery nearestDelivery = null;
            DeliveryPoint pointOfNearestDelivery = null;
            double minimalDeviation = double.MaxValue;
            
            foreach (var delivery in deliveries)
            {
                if (delivery.Points is null
                    || delivery.Points.Count == 0)
                    continue;

                var nearestPoint = delivery.Points
                                           .Where(p => p.Coordinates != null)
                                           .OrderBy(p => CalcDeviation(_filter.UserCoordinates, p))
                                           .First();
                var pointDeviation = CalcDeviation(_filter.UserCoordinates, nearestPoint);

                if (minimalDeviation > pointDeviation)
                {
                    minimalDeviation = pointDeviation;
                    nearestDelivery = delivery;
                    pointOfNearestDelivery = nearestPoint;
                }
            }

            if (nearestDelivery is null
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                || pointOfNearestDelivery is null)
                return null;
            
            return new NearestDelivery()
            {
                Id = nearestDelivery.Id, 
                PointId = pointOfNearestDelivery.Id
            };
        }

        private double CalcDeviation(UserCoordinates coordinates, DeliveryPoint point)
            => Math.Sqrt(
                Math.Pow((coordinates.Latitude - point.Coordinates.Latitude) / 1.7F, 2)
                + Math.Pow(coordinates.Longitude - point.Coordinates.Longitude, 2));
    }
}