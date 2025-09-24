using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.Location
{
    public class CitiesListFilterModel : BaseFilterModel
    {
        public string CityName { get; set; }
        public int? RegionId { get; set; }
        public int? CountryId { get; set; }
    }
}