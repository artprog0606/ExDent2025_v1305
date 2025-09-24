using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Catalog.DomainOfCities
{
    public class DomainGeoLocationModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string GeoName { get; set; }
        public List<DomainCityItem> Cities { get; set; }
    }

    public sealed class DomainCityItem
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
    } 
}