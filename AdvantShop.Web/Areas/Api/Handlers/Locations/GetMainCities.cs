using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Locations;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Locations
{
    public sealed class GetMainCities : AbstractCommandHandler<GetMainCitiesResponse>
    {
        protected override GetMainCitiesResponse Handle()
        {
            var model = new GetMainCitiesResponse()
            {
                Countries = CountryService.GetCountriesByDisplayInPopup().Select(x => new LocationCountry(x)).ToList()
            };

            foreach (var country in model.Countries)
            {
                country.Cities = CityService.GetCitiesByCountryInPopup(country.CountryId)
                    .Select(x => new LocationCity(x))
                    .ToList();

                var cityIds = country.Cities.Select(x => x.CityId).ToList();
                
                var warehouseCities = WarehouseCityService.GetWarehouseCities(cityIds);

                foreach (var city in country.Cities)
                {
                    var region = RegionService.GetRegion(city.RegionId);
                    if (region != null)
                        city.RegionName = region.Name;

                    city.WarehouseIds =
                        warehouseCities != null && warehouseCities.Count > 0
                            ? warehouseCities
                                .Where(x => x.CityId == city.CityId)
                                .OrderBy(x => x.SortOrder)
                                .Select(x => x.WarehouseId)
                                .ToList()
                            : new List<int>();
                }
            }

            return model;
        }
    }
}