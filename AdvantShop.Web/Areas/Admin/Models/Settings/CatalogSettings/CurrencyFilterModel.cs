using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Settings.CatalogSettings
{
    public class CurrencyFilterModel : BaseFilterModel<int>
    {                
        public string Name { get; set; }
        public string Iso3 { get; set; }
    }
}
