using System.Linq;
using AdvantShop.Core.Services.Domains;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.DomainOfCities
{
    public sealed class DomainGeoLocationFilterResult : FilterResult<DomainOfCityGridItem>
    {
        
    }

    public sealed class DomainOfCityGridItem
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public string Cities
        {
            get
            {
                var cities = DomainGeoLocationService.GetCities(Id);
                return string.Join(", ", cities.Select(x => x.CityName));
            }
        }
    } 
}