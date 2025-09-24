using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Repository
{
    public class LocationModel
    {
        public int CountryId { get; set; }
        public string Country { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string District { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Zip { get; set; }
        public bool ShowRegion { get; set; }
        public bool ShowDistrict { get; set; }

        public static explicit operator LocationModel(IpZone zone)
        {
            return new LocationModel
            {
                Name = zone.City,
                CityId = zone.CityId,
                District = zone.District,
                RegionId = zone.RegionId,
                Region = zone.Region,
                CountryId = zone.CountryId,
                Country = zone.CountryName,
                Zip = zone.Zip,
                ShowRegion = zone.Region.IsNotEmpty() && !string.Equals(zone.Region, zone.City, System.StringComparison.OrdinalIgnoreCase),
                ShowDistrict = zone.District.IsNotEmpty() && !string.Equals(zone.District, zone.City, System.StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}