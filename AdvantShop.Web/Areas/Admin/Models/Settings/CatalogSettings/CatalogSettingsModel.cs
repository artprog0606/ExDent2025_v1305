using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.FullSearch.Core;
using AdvantShop.Repository.Currencies;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Settings.CatalogSettings
{
    public class CatalogSettingsModel
    {
        // search
        public bool IsEnabledSearchModule { get; set; }
        public List<SelectListItem> SearchDeepList { get; set; }
        public ESearchDeep SearchDeep { get; set; }
        public ESearchDeep SearchDeepInAdminPart { get; set; }
        public int SearchMaxItems { get; set; }
        public string SearchExample { get; set; }
        public bool MinimizeSearchResults { get; set; }
        public bool SearchByCategories { get; set; }


        // currency
        public List<SelectListItem> Currencies { get; set; }
        public bool AllowToChangeCurrency { get; set; }
        public bool AutoUpdateCurrencies { get; set; }
        public string DefaultCurrencyIso3 { get; set; }
        public string DefaultCurrencyName { get; set; }

        // price show mode
        public EDisplayModeOfPrices DisplayModeOfPrices { get; set; }
        public List<SelectListItem> DisplayModeOfPricesList { get; set; }
        public List<SelectListItem> CustomerGroups { get; set; }
        public List<SelectListItem> AvalableCustomerGroups { get; set; }

        public string CustomerGroupsSerialized { get; set; }
        public string AvalableCustomerGroupsSerialized { get; set; }
        public string TextInsteadOfPrice { get; set; }


        // Properties
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        //Brands
        public ImportBrandsModel ImportBrandsModel { get; set; }

        // price rules
        public bool ShowAmountsTableInProduct { get; set; }
        public bool ShowAmountsTableInCatalog { get; set; }
        public bool ShowPriceAmountNextDiscountsInCart { get; set; }

        //additional
        public bool EnableOfferBarCode { get; set; }
        public bool EnableOfferWeightAndDimensions { get; set; }

        public bool IsPropertyGroupsDropDownList { get; set; }


        public CatalogSettingsModel()
        {
            Currencies =
                CurrencyService.GetAllCurrencies()
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Iso3 })
                    .ToList();

            SearchDeepList = new List<SelectListItem>();
            foreach (ESearchDeep item in Enum.GetValues(typeof(ESearchDeep)))
            {
                SearchDeepList.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            DisplayModeOfPricesList = new List<SelectListItem>();
            foreach (EDisplayModeOfPrices mode in Enum.GetValues(typeof(EDisplayModeOfPrices)))
            {
                DisplayModeOfPricesList.Add(new SelectListItem() { Text = mode.Localize(), Value = mode.ToString() });
            }

            CustomerGroups = new List<SelectListItem>();
            foreach (var cGroup in CustomerGroupService.GetCustomerGroupList())
            {
                CustomerGroups.Add(new SelectListItem()
                {
                    Text = cGroup.GroupName,
                    Value = cGroup.CustomerGroupId.ToString()
                });
            }
            CustomerGroupsSerialized = JsonConvert.SerializeObject(CustomerGroups);

            var avalableCustomerGroups =
                SettingsCatalog.AvalableCustomerGroups.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                    .OfType<string>()
                    .ToList();

            AvalableCustomerGroups = new List<SelectListItem>();
            foreach (var group in CustomerGroupService.GetCustomerGroupList())
            {
                if (avalableCustomerGroups.Any(item => item == group.CustomerGroupId.ToString()))
                    AvalableCustomerGroups.Add(new SelectListItem()
                    {
                        Text = group.GroupName,
                        Value = group.CustomerGroupId.ToString()
                    });
            }
            AvalableCustomerGroupsSerialized = JsonConvert.SerializeObject(AvalableCustomerGroups);
        }
    }
    
    /*
    public class OldCatalogSettingsModel : IValidatableObject
    {
        public OldCatalogSettingsModel()
        {
            Currencies = CurrencyService.GetAllCurrencies().Select(x => new SelectListItem() { Text = x.Name, Value = x.Iso3 }).ToList();

            SearchDeepList = new List<SelectListItem>();
            foreach (ESearchDeep item in Enum.GetValues(typeof(ESearchDeep)))
            {
                SearchDeepList.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            DefaultViewList = new List<SelectListItem>();
            foreach (ProductViewMode item in Enum.GetValues(typeof(ProductViewMode)))
            {
                DefaultViewList.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            ShowShippingsMethods = new List<SelectListItem>();
            foreach (SettingsDesign.eShowShippingsInDetails item in Enum.GetValues(typeof(SettingsDesign.eShowShippingsInDetails)))
            {
                ShowShippingsMethods.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            RelatedProductTypes = new List<SelectListItem>();
            foreach (SettingsDesign.eRelatedProductSourceType item in Enum.GetValues(typeof(SettingsDesign.eRelatedProductSourceType)))
            {
                RelatedProductTypes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            DisplayModeOfPricesList = new List<SelectListItem>();
            foreach (EDisplayModeOfPrices mode in Enum.GetValues(typeof(EDisplayModeOfPrices)))
            {
                DisplayModeOfPricesList.Add(new SelectListItem() { Text = mode.Localize(), Value = mode.ToString() });
            }

            CustomerGroups = new List<SelectListItem>();
            foreach (var cGroup in CustomerGroupService.GetCustomerGroupList())
            {
                CustomerGroups.Add(new SelectListItem() { Text = cGroup.GroupName, Value = cGroup.CustomerGroupId.ToString() });
            }


            var avalableCustomerGroups = SettingsCatalog.AvalableCustomerGroups.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).OfType<string>().ToList();

            AvalableCustomerGroups = new List<SelectListItem>();
            foreach (var cGroup in CustomerGroupService.GetCustomerGroupList())
            {
                if (avalableCustomerGroups.Any(item => item == cGroup.CustomerGroupId.ToString()))
                    AvalableCustomerGroups.Add(new SelectListItem() { Text = cGroup.GroupName, Value = cGroup.CustomerGroupId.ToString() });
            }
        }

        public bool ShowQuickView { get; set; }
        public int ProductsPerPage { get; set; }
        public bool ShowProductsCount { get; set; }
        public bool ShowProductArtNo { get; set; }
        public bool EnableProductRating { get; set; }
        public bool EnableCompareProducts { get; set; }
        public bool EnablePhotoPreviews { get; set; }
        public bool ShowCountPhoto { get; set; }
        public bool ExcludingFilters { get; set; }
        public bool FilterVisibility { get; set; }
        public bool ShowPriceFilter { get; set; }
        public bool ShowProducerFilter { get; set; }
        public bool ShowSizeFilter { get; set; }
        public bool ShowColorFilter { get; set; }
        public bool ComplexFilter { get; set; }
        public string SizesHeader { get; set; }
        public string ColorsHeader { get; set; }
        public int ColorIconWidthCatalog { get; set; }
        public int ColorIconHeightCatalog { get; set; }
        public int ColorIconWidthDetails { get; set; }
        public int ColorIconHeightDetails { get; set; }
        public string BuyButtonText { get; set; }
        public string PreOrderButtonText { get; set; }
        public bool DisplayBuyButton { get; set; }
        public bool DisplayPreOrderButton { get; set; }
        public bool DisplayCategoriesInBottomMenu { get; set; }
        public int BrandsPerPage { get; set; }
        public string SearchExample { get; set; }
        public bool ShowCategoryTreeInBrand { get; set; }
        public bool ShowProductsInBrand { get; set; }
        public bool ShowOnlyAvalible { get; set; }
        public bool MoveNotAvaliableToEnd { get; set; }

        public List<SelectListItem> Currencies { get; set; }
        public bool AllowToChangeCurrency { get; set; }
        public bool AutoUpdateCurrencies { get; set; }

        public List<SelectListItem> SearchDeepList { get; set; }
        public ESearchDeep SearchDeep { get; set; }
        public int SearchMaxItems { get; set; }

        public List<SelectListItem> DefaultViewList { get; set; }
        public ProductViewMode DefaultCatalogView { get; set; }
        public ProductViewMode DefaultSearchView { get; set; }

        public bool EnableCatalogViewChange { get; set; }
        public bool EnableSearchViewChange { get; set; }


        // product
        public bool DisplayWeight { get; set; }
        public bool DisplayDimensions { get; set; }
        public bool ShowStockAvailability { get; set; }

        public bool CompressBigImage { get; set; }

        public bool ModerateReviews { get; set; }
        public bool AllowReviews { get; set; }
        public bool DisplayReviewsImage { get; set; }
        public bool AllowReviewsImageUploading { get; set; }
        public int ReviewImageWidth { get; set; }
        public int ReviewImageHeight { get; set; }

        public bool EnableZoom { get; set; }

        public List<SelectListItem> ShowShippingsMethods { get; set; }
        public SettingsDesign.eShowShippingsInDetails ShowShippingsMethodsInDetails { get; set; }
        public int ShippingsMethodsInDetailsCount { get; set; }

        public string RelatedProductName { get; set; }
        public string AlternativeProductName { get; set; }
        public int RelatedProductsMaxCount { get; set; }

        public List<SelectListItem> RelatedProductTypes { get; set; }
        public SettingsDesign.eRelatedProductSourceType RelatedProductSourceType { get; set; }

        public List<SelectListItem> DisplayModeOfPricesList { get; set; }
        public List<SelectListItem> CustomerGroups { get; set; }
        public List<SelectListItem> AvalableCustomerGroups { get; set; }

        public string TextInsteadOfPrice { get; set; }
        public EDisplayModeOfPrices DisplayModeOfPrices { get; set; }
        public string DefaultCurrencyIso3 { get; set; }
        public string DefaultCurrencyName { get; set; }


        // Properties
        public int GroupId { get; set; }
        public string GroupName { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductsPerPage <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtProductsPerPage"), new[] { "ProductsPerPage" });
            }

            if (BrandsPerPage <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtBrandsPerPage"), new[] { "BrandsPerPage" });
            }

            if (SearchMaxItems <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtSearchMaxItems"), new[] { "SearchMaxItems" });
            }

            if (SizesHeader.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtSizesHeader"), new[] { "SizesHeader" });
            }

            if (ColorsHeader.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorsHeader"), new[] { "ColorsHeader" });
            }

            if (BuyButtonText.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtBuyButtonText"), new[] { "BuyButtonText" });
            }

            if (PreOrderButtonText.IsNullOrEmpty())
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtPreOrderButtonText"), new[] { "PreOrderButtonText" });
            }

            if (ColorIconWidthCatalog <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconWidthCatalog"), new[] { "ColorIconWidthCatalog" });
            }

            if (ColorIconHeightCatalog <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconHeightCatalog"), new[] { "ColorIconHeightCatalog" });
            }

            if (ColorIconWidthDetails <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconWidthDetails"), new[] { "ColorIconWidthDetails" });
            }

            if (ColorIconHeightDetails <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconHeightDetails"), new[] { "ColorIconHeightDetails" });
            }
        }
    }
    */
}
