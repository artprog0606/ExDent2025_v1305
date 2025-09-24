using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Deliveries;
using AdvantShop.Core.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Deliveries
{
    public sealed class GetShippingTypes : AbstractCommandHandler<GetShippingTypesResponse>
    {
        private readonly ShippingFilter _model;

        public GetShippingTypes(ShippingFilter model)
        {
            _model = model;
        }

        protected override GetShippingTypesResponse Handle()
        {
            var shippingCalculationParameters = GetShippingCalculationParameters();

            var shippings =
                GetShippings(shippingCalculationParameters)
                    .Select(x => new ShippingTypeResponseItem(x, _model.LoadWarehouses))
                    .ToList();

            return new GetShippingTypesResponse(shippings);
        }

        private ShippingCalculationParameters GetShippingCalculationParameters()
        {
            City city = null;
            Region region = null;
            Country country = null;

            if (_model != null)
            {
                region = !string.IsNullOrWhiteSpace(_model.Region)
                    ? RegionService.GetRegionByName(_model.Region)
                    : null;

                country = !string.IsNullOrWhiteSpace(_model.Country)
                    ? CountryService.GetCountryByName(_model.Country)
                    : null;

                if (!string.IsNullOrWhiteSpace(_model.ZipCode))
                    city = CityService.GetCityByZip(_model.ZipCode);

                if (city == null && !string.IsNullOrWhiteSpace(_model.City) && region != null)
                    city = CityService.GetCityByName(_model.City, region.RegionId);

                if (city == null && !string.IsNullOrWhiteSpace(_model.City))
                    city = CityService.GetCityByName(_model.City);

                if (country == null && region != null)
                    country = CountryService.GetCountry(region.CountryId);
            }

            return ShippingCalculationConfigurator
                  .Configure()
                  .WithCountry(country?.Name ?? "")
                  .WithRegion(region?.Name ?? "")
                  .WithCity(city?.Name ?? "")
                  .WithDistrict(_model?.District ?? "")
                  .WithPreOrderItems(new List<PreOrderItem>())
                  .WithCurrency(CurrencyService.CurrentCurrency)
                  .Build();
            return new ShippingCalculationParameters()
            {
                Country = country?.Name ?? "",
                Region = region?.Name ?? "",
                City = city?.Name ?? "",
                District = _model?.District ?? "",
                PreOrderItems = new List<PreOrderItem>(),
                Currency = CurrencyService.CurrentCurrency
            };
        }

        private List<ShippingMethod> GetShippings(ShippingCalculationParameters shippingCalculationParameters)
        {
            var items = new List<ShippingMethod>();
            
            var shippings = ShippingMethodService.GetAllShippingMethods(true);
                
            if (!string.IsNullOrWhiteSpace(_model.Type))
                shippings = shippings.Where(x => x.ShippingType == _model.Type).ToList();
            
            if (_model.Types != null && _model.Types.Count > 0)
                shippings = shippings.Where(x => _model.Types.Contains(x.ShippingType, StringComparer.OrdinalIgnoreCase)).ToList();

            var checkGeo =
                shippingCalculationParameters.Country.IsNotEmpty() ||
                shippingCalculationParameters.Region.IsNotEmpty() ||
                shippingCalculationParameters.City.IsNotEmpty() ||
                shippingCalculationParameters.District.IsNotEmpty();

            foreach (var shipping in shippings)
            {
                var validGeo = false;
                
                if (checkGeo && ShippingPaymentGeoMaping.IsExistGeoShipping(shipping.ShippingMethodId))
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