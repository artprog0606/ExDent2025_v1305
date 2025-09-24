using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.DomainOfCities
{
    public sealed class DomainGeoLocationFilterModel : BaseFilterModel
    {
        public string Url { get; set; }
        public int? CityId { get; set; }
    }
}