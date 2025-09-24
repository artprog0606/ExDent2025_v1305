namespace AdvantShop.Core.Services.Domains
{
    public sealed class DomainGeoLocation
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string GeoName { get; set; }
    }

    public sealed class DomainCityLink
    {
        public int DomainGeoLocationId { get; set; }
        public int CityId { get; set; }

        public DomainCityLink(int domainGeoLocationId, int cityId)
        {
            DomainGeoLocationId = domainGeoLocationId;
            CityId = cityId;
        }
    }
}