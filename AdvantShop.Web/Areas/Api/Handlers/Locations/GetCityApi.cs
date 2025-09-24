using AdvantShop.Areas.Api.Models.Locations;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Locations
{
    public sealed class GetCityApi : AbstractCommandHandler<GetCityResponse>
    {
        private readonly GetCityModel _model;

        public GetCityApi(GetCityModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model == null ||
                (string.IsNullOrWhiteSpace(_model.ZipCode) &&
                 string.IsNullOrWhiteSpace(_model.City) &&
                 string.IsNullOrWhiteSpace(_model.Region)))
            {
                throw new BlException("Укажите почтовый индекс или город и область");
            }
        }

        protected override GetCityResponse Handle()
        {
            City city = null;

            if (!string.IsNullOrWhiteSpace(_model.ZipCode))
                city = CityService.GetCityByZip(_model.ZipCode);

            if (city == null && !string.IsNullOrWhiteSpace(_model.Region) && !string.IsNullOrWhiteSpace(_model.City))
            {
                var region = RegionService.GetRegionByName(_model.Region);
                if (region != null)
                    city = CityService.GetCityByName(_model.City, region.RegionId);
            }

            if (city == null && !string.IsNullOrWhiteSpace(_model.City))
            {
                city = CityService.GetCityByName(_model.City);
            }
            
            if (city == null)
                throw new BlException("Город не найден");

            var warehouseIds = WarehouseCityService.GetWarehouseIds(city.CityId);

            return new GetCityResponse(city, warehouseIds);
        }
    }
}