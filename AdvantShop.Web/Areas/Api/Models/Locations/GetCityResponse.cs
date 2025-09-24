using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Repository;

namespace AdvantShop.Areas.Api.Models.Locations
{
    public sealed class GetCityResponse : IApiResponse
    {
        public int Id { get; }
        public string Name { get; }
        public int RegionId { get; }
        public string District { get; }
        public string Zip { get; }
        public string Phone { get; }
        public string MobilePhone { get; }
        
        public string ShippingZones { get; }
        public string ShippingZonesIframe { get; }
        public string CityAddressPoints { get; }
        public string CityAddressPointsIframe { get; }
        
        public List<int> WarehouseIds { get; set; }
        
        public GetCityResponse(City city, List<int> warehouseIds)
        {
            Id = city.CityId;
            RegionId = city.RegionId;
            Name = city.Name;
            District = city.District;
            Zip = city.Zip;
            Phone = city.PhoneNumber;
            MobilePhone = city.MobilePhoneNumber;
            ShippingZones = city.AdditionalSettings.ShippingZones;
            ShippingZonesIframe = city.AdditionalSettings.ShippingZonesIframe;
            CityAddressPoints = city.AdditionalSettings.CityAddressPoints;
            CityAddressPointsIframe = city.AdditionalSettings.CityAddressPointsIframe;
            WarehouseIds = warehouseIds;
        }
    }
}