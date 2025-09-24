using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Models.Settings.CatalogSettings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Catalog
{
    public class SaveCatalogSettingsHandler
    {
        private readonly CatalogSettingsModel _model;
        public SaveCatalogSettingsHandler(CatalogSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            // search
            SettingsCatalog.SearchDeep = _model.SearchDeep;
            SettingsCatalog.SearchDeepInAdminPart = _model.SearchDeepInAdminPart;
            SettingsCatalog.SearchMaxItems = _model.SearchMaxItems;
            SettingsCatalog.SearchExample = _model.SearchExample.DefaultOrEmpty();
            SettingsCatalog.MinimizeSearchResults = _model.MinimizeSearchResults;
            SettingsCatalog.SearchByCategories = _model.SearchByCategories;

            // currency
            var tempIso3 = SettingsCatalog.DefaultCurrencyIso3;

            if (_model.DefaultCurrencyIso3 != null && CurrencyService.Currency(_model.DefaultCurrencyIso3) != null)
                SettingsCatalog.DefaultCurrencyIso3 = _model.DefaultCurrencyIso3;

            if (SettingsCatalog.DefaultCurrencyIso3 != tempIso3)
            {
                CurrencyService.CurrentCurrency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);
                CategoryService.ClearCategoryCache();
            }

            SettingsCatalog.AllowToChangeCurrency = _model.AllowToChangeCurrency;
            SettingsMain.EnableAutoUpdateCurrencies = _model.AutoUpdateCurrencies;

            // price show mode
            SettingsCatalog.DisplayModeOfPrices = _model.DisplayModeOfPrices;
            SettingsCatalog.TextInsteadOfPrice = _model.TextInsteadOfPrice;

            SettingsPriceRules.ShowAmountsTableInProduct = _model.ShowAmountsTableInProduct;
            SettingsPriceRules.ShowAmountsTableInCatalog = _model.ShowAmountsTableInCatalog;
            SettingsPriceRules.ShowPriceAmountNextDiscountsInCart = _model.ShowPriceAmountNextDiscountsInCart;
            
            SettingsCatalog.EnableOfferBarCode = _model.EnableOfferBarCode;
            SettingsCatalog.EnableOfferWeightAndDimensions = _model.EnableOfferWeightAndDimensions;
            SettingsCatalog.IsPropertyGroupsDropDownList = _model.IsPropertyGroupsDropDownList;
        }
    }
}