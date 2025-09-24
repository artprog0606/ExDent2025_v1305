using AdvantShop.Configuration;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Settings.CatalogSettings
{
    public class CurrencyPagingModel : Currency
    {
        public bool CanDelete { get { return Iso3 != SettingsCatalog.DefaultCurrencyIso3; } }
    }
}
