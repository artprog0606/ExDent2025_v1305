using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Web.Admin.Models.Settings.CatalogSettings;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;

namespace AdvantShop.Web.Admin.Handlers.Settings.Catalog
{
    public class GetCatalogSettings
    {
        public CatalogSettingsModel Execute()
        {
            var model = new CatalogSettingsModel()
            {
                SearchExample = SettingsCatalog.SearchExample,
                SearchDeep = SettingsCatalog.SearchDeep,
                SearchDeepInAdminPart = SettingsCatalog.SearchDeepInAdminPart,
                SearchMaxItems = SettingsCatalog.SearchMaxItems,
                SearchByCategories = SettingsCatalog.SearchByCategories,

                AllowToChangeCurrency = SettingsCatalog.AllowToChangeCurrency,
                AutoUpdateCurrencies = SettingsMain.EnableAutoUpdateCurrencies,
                DefaultCurrencyIso3 = SettingsCatalog.DefaultCurrency != null ? SettingsCatalog.DefaultCurrency.Iso3 : "",
                DefaultCurrencyName = SettingsCatalog.DefaultCurrency != null ? SettingsCatalog.DefaultCurrency.Name : "",

                DisplayModeOfPrices = SettingsCatalog.DisplayModeOfPrices,
                TextInsteadOfPrice = SettingsCatalog.TextInsteadOfPrice,

                MinimizeSearchResults = SettingsCatalog.MinimizeSearchResults,

                ImportBrandsModel = new ImportBrandsModel(),

                ShowAmountsTableInProduct = SettingsPriceRules.ShowAmountsTableInProduct,
                ShowAmountsTableInCatalog = SettingsPriceRules.ShowAmountsTableInCatalog,
                ShowPriceAmountNextDiscountsInCart = SettingsPriceRules.ShowPriceAmountNextDiscountsInCart,
                
                EnableOfferBarCode = SettingsCatalog.EnableOfferBarCode,
                EnableOfferWeightAndDimensions = SettingsCatalog.EnableOfferWeightAndDimensions,
                IsPropertyGroupsDropDownList = SettingsCatalog.IsPropertyGroupsDropDownList
            };


            var modules = AttachedModules.GetModules<IModuleProductSearchProvaider>();
            model.IsEnabledSearchModule = modules != null && modules.Count != 0;

            return model;
        }
    }
}
