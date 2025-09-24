using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Shipping.PointDelivery;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Deliveries
{
    public class GetPointDeliveries : AbstractCommandHandler<PointDeliveryResponse>
    {
        private readonly PointDeliveryFilter _filter;

        public GetPointDeliveries(PointDeliveryFilter filter)
        {
            _filter = filter;
        }

        protected override PointDeliveryResponse Handle()
        {
            var shippingCalculationParameters = GetShippingCalculationParameters();
            
            var deliveryOptions =
                GetDeliveries(shippingCalculationParameters)
                    .Select(x => new PointDelivery(x, shippingCalculationParameters))
                    .Select(x => x.CalculateOptions(CalculationVariants.All))
                    .Where(x => x != null)
                    .SelectMany(x => x)
                    .Where(x => x != null)
                    .ToList();

            var deliveries = new List<PointDeliveryShipping>();
            
            foreach (var option in deliveryOptions)
            {
                var points = new List<DeliveryPointShipping>();
                
                switch (option)
                {
                    case PointDeliveryOption pointDeliveryOption:
                        points = pointDeliveryOption.ShippingPoints;
                        break;
                    case PointDeliveryMapOption pointDeliveryMapOption:
                        points = pointDeliveryMapOption.MapPoints;
                        break;
                }

                var inHouse =
                    SettingsApiAuth.ShowInHouseInDeliveryWidgetOnMain &&
                    SettingsApiAuth.InHouseShippingMethodIds != null &&
                    SettingsApiAuth.InHouseShippingMethodIds.Contains(option.MethodId);

                if (_filter.InHouse != null)
                {
                    if (_filter.InHouse.Value && !inHouse)
                        continue;
                    if (!_filter.InHouse.Value && inHouse)
                        continue;
                }
                
                deliveries.AddRange(points.Select(x => new PointDeliveryShipping(option, x, inHouse)));
            }
            
            return new PointDeliveryResponse(deliveries);
        }

        private ShippingCalculationParameters GetShippingCalculationParameters()
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

        private List<ShippingMethod> GetDeliveries(ShippingCalculationParameters shippingCalculationParameters)
        {
            var items = new List<ShippingMethod>();

            var methods = new List<string>() {"PointDelivery"};
            
            var shippings = 
                ShippingMethodService.GetAllShippingMethods(true)
                    .Where(x => methods.Contains(x.ShippingType, StringComparer.OrdinalIgnoreCase));

            if (_filter.TypeId != null)
                shippings = shippings.Where(x => x.ShippingMethodId == _filter.TypeId);

            foreach (var shipping in shippings)
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
    }
}