namespace AdvantShop.Repository
{
    public class IpZone
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public int RegionId { get; set; }
        public string Region { get; set; }

        public int CityId { get; set; }
        public string City { get; set; }
        public string District { get; set; }

        public int? DialCode { get; set; }

        public string Zip { get; set; }

        public IpZone()
        {
            CountryName = string.Empty;
            Region = string.Empty;
            City = string.Empty;
            District = string.Empty;
        }

        public static IpZone Create(City city)
        {
            var region = RegionService.GetRegion(city.RegionId);
            var country = CountryService.GetCountry(region.CountryId);
            
            return new IpZone()
            {
                City = city.Name,
                CityId = city.CityId,
                District = city.District,
                Zip = city.Zip,
                RegionId = city.RegionId,
                Region = region?.Name,
                CountryId = country?.CountryId ?? 0,
                CountryName = country?.Name ?? "",
                DialCode = country?.DialCode
            };
        }
    }
}