//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Services.FullSearch.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;

namespace AdvantShop.Configuration
{
    public enum ProductViewMode
    {
        [Localize("Core.Settings.SettingsCatalog.ProductViewMode.Tile")]
        Tile = 0,

        [Localize("Core.Settings.SettingsCatalog.ProductViewMode.List")]
        List = 1,

        [Localize("Core.Settings.SettingsCatalog.ProductViewMode.Table")]
        Table = 2,

        [Localize("Core.Settings.SettingsCatalog.ProductViewMode.Single")]
        Single = 3
    }

    public enum ProductViewPage
    {
        Catalog = 0,
        Search = 1
    }

    public enum EDisplayModeOfPrices
    {
        [Localize("Core.Settings.SettingsCatalog.DisplayModeOfPrices.AllCustomers")]
        AllCustomers = 0,

        [Localize("Core.Settings.SettingsCatalog.DisplayModeOfPrices.OnlyRegistered")]
        OnlyRegistered = 1,

        [Localize("Core.Settings.SettingsCatalog.DisplayModeOfPrices.OnlyChosenCustomers")]
        OnlyChosenCustomerGroups = 2
    }

    public enum ColorsViewMode
    {
        [Localize("Core.Settings.SettingsCatalog.ColorsViewMode.Icon")]
        Icon = 0,

        [Localize("Core.Settings.SettingsCatalog.ColorsViewMode.Text")]
        Text = 1,

        [Localize("Core.Settings.SettingsCatalog.ColorsViewMode.IconAndText")]
        IconAndText = 2
    }

    public enum ReviewsSortingOnMainPage
    {
        [Localize("Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByLikes")]
        ByLikes = 0,
        
        [Localize("Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByReviewRatingThenDate")]
        ByReviewRatingThenDate = 1,
        
        [Localize("Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByDate")]
        ByDate = 2,
    }

    public class SettingsCatalog
    {
        public static int ProductsPerPage
        {
            get => int.Parse(SettingProvider.Items["ProductsPerPage"]);
            set => SettingProvider.Items["ProductsPerPage"] = value.ToString();
        }

        public static bool ShowSpinboxInCatalog
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowSpinboxInCatalog"]);
            set => SettingProvider.Items["ShowSpinboxInCatalog"] = value.ToString();
        }

        public static string DefaultCurrencyIso3
        {
            get => SettingProvider.Items["DefaultCurrencyISO3"];
            set => SettingProvider.Items["DefaultCurrencyISO3"] = value;
        }

        public static bool AllowToChangeCurrency
        {
            get => Convert.ToBoolean(SettingProvider.Items["AllowToChangeCurrency"]);
            set => SettingProvider.Items["AllowToChangeCurrency"] = value.ToString();
        }

        public static Currency DefaultCurrency =>
            CurrencyService.Currency(DefaultCurrencyIso3) ??
            CurrencyService.GetAllCurrencies().FirstOrDefault();


        public static ProductViewMode DefaultCatalogView
        {
            get => (ProductViewMode)int.Parse(SettingProvider.Items["DefaultCatalogView"]);
            set => SettingProvider.Items["DefaultCatalogView"] = ((int)value).ToString();
        }

        public static ProductViewMode DefaultSearchView
        {
            get => (ProductViewMode)int.Parse(SettingProvider.Items["DefaultSearchView"]);
            set => SettingProvider.Items["DefaultSearchView"] = ((int)value).ToString();
        }

        public static ProductViewMode GetViewMode(bool canChange, string cookieName, ProductViewMode defaultView,
            bool isMobile)
        {
            if (!canChange)
                return defaultView;

            var cookieMode = CommonHelper.GetCookieString(cookieName);
            var mode = cookieMode.Parse<ProductViewMode>(defaultView);

            if ((!isMobile && mode == ProductViewMode.Single) ||
                (isMobile && mode == ProductViewMode.Table))
            {
                CommonHelper.SetCookie(cookieName, defaultView.ToString().ToLower());
                return defaultView;
            }

            return mode;
        }

        public static bool EnableProductRating
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableProductRating"]);
            set => SettingProvider.Items["EnableProductRating"] = value.ToString();
        }

        public static bool EnablePhotoPreviews
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnablePhotoPreviews"]);
            set => SettingProvider.Items["EnablePhotoPreviews"] = value.ToString();
        }

        public static bool ShowCountPhoto
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowCountPhoto"]);
            set => SettingProvider.Items["ShowCountPhoto"] = value.ToString();
        }


        public static bool ShowProductsCount
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowProductsCount"]);
            set => SettingProvider.Items["ShowProductsCount"] = value.ToString();
        }

        public static bool EnableCompareProducts
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCompareProducts"]);
            set => SettingProvider.Items["EnableCompareProducts"] = value.ToString();
        }

        public static bool EnabledCatalogViewChange
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCatalogViewChange"]);
            set => SettingProvider.Items["EnableCatalogViewChange"] = value.ToString();
        }

        public static bool EnabledSearchViewChange
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableSearchViewChange"]);
            set => SettingProvider.Items["EnableSearchViewChange"] = value.ToString();
        }

        //public static bool CompressBigImage
        //{
        //    get { return Convert.ToBoolean(SettingProvider.Items["CompressBigImage"]); }
        //    set { SettingProvider.Items["CompressBigImage"] = value.ToString(); }
        //}

        public static string RelatedProductName
        {
            get => SettingProvider.Items["RelatedProductName"];
            set => SettingProvider.Items["RelatedProductName"] = value;
        }

        public static string AlternativeProductName
        {
            get => SettingProvider.Items["AlternativeProductName"];
            set => SettingProvider.Items["AlternativeProductName"] = value;
        }

        public static bool AllowReviews
        {
            get => Convert.ToBoolean(SettingProvider.Items["AllowReviews"]);
            set => SettingProvider.Items["AllowReviews"] = value.ToString();
        }

        public static bool DisplayReviewsImage
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayReviewsImage"]);
            set => SettingProvider.Items["DisplayReviewsImage"] = value.ToString();
        }

        public static bool AllowReviewsImageUploading
        {
            get => Convert.ToBoolean(SettingProvider.Items["AllowReviewsImageUploading"]);
            set => SettingProvider.Items["AllowReviewsImageUploading"] = value.ToString();
        }

        public static bool ModerateReviews
        {
            get => Convert.ToBoolean(SettingProvider.Items["ModerateReviewed"]);
            set => SettingProvider.Items["ModerateReviewed"] = value.ToString();
        }

        public static bool ReviewsVoiteOnlyRegisteredUsers
        {
            get => Convert.ToBoolean(SettingProvider.Items["ReviewsVoiteOnlyRegisteredUsers"]);
            set => SettingProvider.Items["ReviewsVoiteOnlyRegisteredUsers"] = value.ToString();
        }

        public static ReviewsSortingOnMainPage ReviewsSortingOnMainPage
        {
            get => (ReviewsSortingOnMainPage)Convert.ToInt32(SettingProvider.Items["ReviewsSortingOnMainPage"]);
            set => SettingProvider.Items["ReviewsSortingOnMainPage"] = ((int)value).ToString();
        }

        public static bool ComplexFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ComplexFilter"]);
            set => SettingProvider.Items["ComplexFilter"] = value.ToString();
        }

        public static string SizesHeader
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["SizesHeader"]);
            set => SettingProvider.Items["SizesHeader"] = value;
        }


        public static string ColorsHeader
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["ColorsHeader"]);
            set => SettingProvider.Items["ColorsHeader"] = value;
        }

        public static ColorsViewMode ColorsViewMode
        {
            get => (ColorsViewMode)int.Parse(SettingProvider.Items["ColorsViewMode"]);
            set => SettingProvider.Items["ColorsViewMode"] = ((int)value).ToString();
        }

        public static bool ShowQuickView
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowQuickView"]);
            set => SettingProvider.Items["ShowQuickView"] = value.ToString();
        }

        public static bool ShowProductArtNo
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowProductArtNo"]);
            set => SettingProvider.Items["ShowProductArtNo"] = value.ToString();
        }


        public static bool ExcludingFilters
        {
            get => Convert.ToBoolean(SettingProvider.Items["ExluderingFilters"]);
            set => SettingProvider.Items["ExluderingFilters"] = value.ToString();
        }

        public static string GetRelatedProductName(int relatedType)
        {
            if (relatedType == 0)
                return RelatedProductName;
            else if (relatedType == 1)
                return AlternativeProductName;

            return string.Empty;
        }


        public static string BuyButtonText
        {
            get => SettingProvider.Items["BuyButtonText"];
            set => SettingProvider.Items["BuyButtonText"] = value;
        }

        public static string PreOrderButtonText
        {
            get => SettingProvider.Items["PreOrderButtonText"];
            set => SettingProvider.Items["PreOrderButtonText"] = value;
        }

        public static bool DisplayBuyButton
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayBuyButton"]);
            set => SettingProvider.Items["DisplayBuyButton"] = value.ToString();
        }

        public static bool DisplayPreOrderButton
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayPreOrderButton"]);
            set => SettingProvider.Items["DisplayPreOrderButton"] = value.ToString();
        }

        public static bool DisplayCategoriesInBottomMenu
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayCategoriesInBottomMenu"]);
            set => SettingProvider.Items["DisplayCategoriesInBottomMenu"] = value.ToString();
        }

        public static bool ShowStockAvailability
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowStockAvailability"]);
            set => SettingProvider.Items["ShowStockAvailability"] = value.ToString();
        }
        
        public static bool ShowProductArtNoOnProductCard
        {
            get
            {
                var value = SettingProvider.Items["ShowProductArtNoOnProductCard"];
                return string.IsNullOrEmpty(value) 
                    ? TemplateSettingsProvider.Items["SkuProductCardVisibility"] != null 
                        ? TemplateSettingsProvider.Items["SkuProductCardVisibility"].TryParseBool() 
                        : true
                    : Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["ShowProductArtNoOnProductCard"] = value.ToString();
        }

        public static bool ShowColorFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowColorFilter"]);
            set => SettingProvider.Items["ShowColorFilter"] = value.ToString();
        }

        public static bool ShowSizeFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowSizeFilter"]);
            set => SettingProvider.Items["ShowSizeFilter"] = value.ToString();
        }

        public static bool ShowWarehouseFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowWarehouseFilter"]);
            set => SettingProvider.Items["ShowWarehouseFilter"] = value.ToString();
        }

        public static string SearchExample
        {
            get => SettingProvider.Items["SearchExample"];
            set => SettingProvider.Items["SearchExample"] = value;
        }

        public static int SearchMaxItems
        {
            get => SettingProvider.Items["SearchMaxItems"].TryParseInt();
            set => SettingProvider.Items["SearchMaxItems"] = value.ToString();
        }

        public static ESearchDeep SearchDeep
        {
            get
            {
                var item = SettingProvider.Items["SearchDeep"];
                return string.IsNullOrWhiteSpace(item)
                    ? ESearchDeep.WordsStartFrom
                    : item.TryParseEnum<ESearchDeep>();
            }
            set => SettingProvider.Items["SearchDeep"] = value.ToString();
        }

        public static ESearchDeep SearchDeepInAdminPart
        {
            get
            {
                var item = SettingProvider.Items["SearchDeepInAdminPart"];
                return string.IsNullOrWhiteSpace(item)
                    ? ESearchDeep.WordsBetween
                    : item.TryParseEnum<ESearchDeep>();
            }
            set => SettingProvider.Items["SearchDeepInAdminPart"] = value.ToString();
        }

        public static bool DisplayWeight
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayWeight"]);
            set => SettingProvider.Items["DisplayWeight"] = value.ToString();
        }

        public static bool DisplayDimensions
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayDimensions"]);
            set => SettingProvider.Items["DisplayDimensions"] = value.ToString();
        }

        public static bool ShowProducerFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowProducerFilter"]);
            set => SettingProvider.Items["ShowProducerFilter"] = value.ToString();
        }

        public static bool ShowPriceFilter
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowPriceFilter"]);
            set => SettingProvider.Items["ShowPriceFilter"] = value.ToString();
        }

        public static bool ShowPropertiesFilterInProductList
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowPropertiesFilterInProductList"]);
            set => SettingProvider.Items["ShowPropertiesFilterInProductList"] = value.ToString();
        }

        public static bool ShowPropertiesFilterInParentCategories
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowPropertiesFilterInParentCategories"]);
            set => SettingProvider.Items["ShowPropertiesFilterInParentCategories"] = value.ToString();
        }

        public static bool ShowProductsInBrand
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowProductsInBrand"]);
            set => SettingProvider.Items["ShowProductsInBrand"] = value.ToString();
        }

        public static bool ShowCategoryTreeInBrand
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowCategoryTree"]);
            set => SettingProvider.Items["ShowCategoryTree"] = value.ToString();
        }

        public static ESortOrder DefaultSortOrderProductInBrand
        {
            get
            {
                var value = SettingProvider.Items["DefaultSortOrderProductInBrand"];

                if (!string.IsNullOrEmpty(value))
                    return (ESortOrder)int.Parse(value);
                return ESortOrder.NoSorting;
            }
            set => SettingProvider.Items["DefaultSortOrderProductInBrand"] = ((int)value).ToString();
        }


        public static bool ShowOnlyAvalible
        {
            get => SettingProvider.Items["ShowOnlyAvalible"] == "True";
            set => SettingProvider.Items["ShowOnlyAvalible"] = value.ToString();
        }
        public static bool ShowUnitsInCatalog
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowUnitsInCatalog"]);
            set => SettingProvider.Items["ShowUnitsInCatalog"] = value.ToString();
        }

        public static bool MoveNotAvaliableToEnd
        {
            get => Convert.ToBoolean(SettingProvider.Items["MoveNotAvaliableToEnd"]);
            set => SettingProvider.Items["MoveNotAvaliableToEnd"] = value.ToString();
        }

        public static bool ShowNotAvaliableLable
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowNotAvaliableLable"]);
            set => SettingProvider.Items["ShowNotAvaliableLable"] = value.ToString();
        }

        public static bool ShowNotAvaliableLableInProduct
        {
            get
            {
                var value = SettingProvider.Items["ShowNotAvaliableLableInProduct"];
                return value == null ? true : Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["ShowNotAvaliableLableInProduct"] = value.ToString();
        }

        public static bool ShowAvaliableLableInProduct
        {
            get
            {
                var value = SettingProvider.Items["ShowInAvaliableLableInProduct"];
                return value == null ? true : Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["ShowInAvaliableLableInProduct"] = value.ToString();
        }

        public static bool ShowAvailableInWarehouseInProduct
        {
            get => SettingProvider.Items["ShowAvailableInWarehouseInProduct"].TryParseBool();
            set => SettingProvider.Items["ShowAvailableInWarehouseInProduct"] = value.ToString();
        }

        public static bool ShowOnlyAvailableWarehousesInProduct
        {
            get => SettingProvider.Items["ShowOnlyAvailableWarehousesInProduct"].TryParseBool();
            set => SettingProvider.Items["ShowOnlyAvailableWarehousesInProduct"] = value.ToString();
        }

        public static string YaMapsApiKeyFoMapWarehouse
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["YaMapsApiKeyFoMapWarehouse"]);
            set => SettingProvider.Items["YaMapsApiKeyFoMapWarehouse"] = value;
        }

        public static int BrandsPerPage
        {
            get => int.Parse(SettingProvider.Items["BrandsPerPage"]);
            set => SettingProvider.Items["BrandsPerPage"] = value.ToString();
        }

        public static int RelatedProductsMaxCount
        {
            get => Convert.ToInt32(SettingProvider.Items["RelatedProductsMaxCount"]);
            set => SettingProvider.Items["RelatedProductsMaxCount"] = value.ToString();
        }

        public static bool AvaliableFilterEnabled
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["AvaliableFilterEnabled"]);
            set => SettingProvider.Items["AvaliableFilterEnabled"] = value.ToString();
        }
        
        #region BestMainPageProducts

        public static string BestDescription
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["BestDescription"]);
            set => SettingProvider.Items["BestDescription"] = value.ToString();
        }

        public static bool BestEnabled
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["BestEnabled"]);
            set => SettingProvider.Items["BestEnabled"] = value.ToString();
        }

        public static bool ShuffleBestOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShuffleBestOnMainPage"]);
            set => SettingProvider.Items["ShuffleBestOnMainPage"] = value.ToString();
        }

        public static bool ShowBestOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShowBestOnMainPage"]);
            set => SettingProvider.Items["ShowBestOnMainPage"] = value.ToString();
        }

        public static int BestSorting
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["BestSorting"]);
            set => SettingProvider.Items["BestSorting"] = value.ToString();
        }
        
        #endregion

        #region NewMainPageProducts
        
        public static string NewDescription
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["NewDescription"]);
            set => SettingProvider.Items["NewDescription"] = value.ToString();
        }

        public static bool NewEnabled
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["NewEnabled"]);
            set => SettingProvider.Items["NewEnabled"] = value.ToString();
        }

        public static bool ShuffleNewOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShuffleNewOnMainPage"]);
            set => SettingProvider.Items["ShuffleNewOnMainPage"] = value.ToString();
        }

        public static bool ShowNewOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShowNewOnMainPage"]);
            set => SettingProvider.Items["ShowNewOnMainPage"] = value.ToString();
        }
        
        public static int NewSorting
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["NewSorting"]);
            set => SettingProvider.Items["NewSorting"] = value.ToString();
        }
        
        #endregion

        #region SalesMainPageProducts

        public static string DiscountDescription
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["DiscountDescription"]);
            set => SettingProvider.Items["DiscountDescription"] = value.ToString();
        }

        public static bool SalesEnabled
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["SalesEnabled"]);
            set => SettingProvider.Items["SalesEnabled"] = value.ToString();
        }

        public static bool ShuffleSalesOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShuffleSalesOnMainPage"]);
            set => SettingProvider.Items["ShuffleSalesOnMainPage"] = value.ToString();
        }

        public static bool ShowSalesOnMainPage
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShowSalesOnMainPage"]);
            set => SettingProvider.Items["ShowSalesOnMainPage"] = value.ToString();
        }
        
        public static int SalesSorting
        {
            get => SQLDataHelper.GetInt(SettingProvider.Items["SalesSorting"]);
            set => SettingProvider.Items["SalesSorting"] = value.ToString();
        }

        #endregion

        public static int DefaultTaxId
        {
            get => Convert.ToInt32(SettingProvider.Items["DefaultTaxId"]);
            set => SettingProvider.Items["DefaultTaxId"] = value.ToString();
        }

        public static EDisplayModeOfPrices DisplayModeOfPrices
        {
            get => SettingProvider.Items["DisplayModeOfPrices"].TryParseEnum<EDisplayModeOfPrices>();
            set => SettingProvider.Items["DisplayModeOfPrices"] = ((int)value).ToString();
        }

        public static string TextInsteadOfPrice
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["TextInsteadOfPrice"]);
            set => SettingProvider.Items["TextInsteadOfPrice"] = value;
        }

        public static string AvalableCustomerGroups
        {
            get => SQLDataHelper.GetString(SettingProvider.Items["AvalableCustomerGroups"]);
            set => SettingProvider.Items["AvalableCustomerGroups"] = value;
        }

        public static bool MinimizeSearchResults
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["MinimizeSearchResults"]);
            set => SettingProvider.Items["MinimizeSearchResults"] = value.ToString();
        }

        public static bool EnableOfferBarCode
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["Features.EnableOfferBarCode"]);
            set => SettingProvider.Items["Features.EnableOfferBarCode"] = value.ToString();
        }

        public static bool EnableOfferWeightAndDimensions
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["Features.EnableOfferWeightAndDimensions"]);
            set => SettingProvider.Items["Features.EnableOfferWeightAndDimensions"] = value.ToString();
        }

        public static bool IsPropertyGroupsDropDownList
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["Features.IsPropertyGroupsDropDownList"]);
            set => SettingProvider.Items["Features.IsPropertyGroupsDropDownList"] = value.ToString();
        }

        public static bool HidePrice
        {
            get
            {
                if (!Saas.SaasDataService.IsEnabledFeature(Saas.ESaasProperty.HavePriceVisibility))
                    return false;

                switch (DisplayModeOfPrices)
                {
                    case EDisplayModeOfPrices.OnlyRegistered:
                        return !CustomerContext.CurrentCustomer.RegistredUser;

                    case EDisplayModeOfPrices.OnlyChosenCustomerGroups:
                        var avalableCustomerGroups = SettingsCatalog.AvalableCustomerGroups
                            .Split(new[] { ";" }, StringSplitOptions.None).OfType<string>().ToList();
                        return !CustomerContext.CurrentCustomer.RegistredUser || !avalableCustomerGroups.Any(item =>
                            item == CustomerContext.CurrentCustomer.CustomerGroupId.ToString());
                }

                return false;
            }
        }

        public static bool ShowImageSearchEnabled
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowImageSearchEnabled"]);
            set
            {
                SettingProvider.Items["ShowImageSearchEnabled"] = value.ToString();
                if (!value)
                    ImageSearchEnabled = false;
            }
        }

        public static bool ImageSearchEnabled
        {
            get => Convert.ToBoolean(SettingProvider.Items["ImageSearchEnabled"]);
            set => SettingProvider.Items["ImageSearchEnabled"] = value.ToString();
        }

        public static bool DisplayLatestProductsInNewOnMainPage
        {
            get
            {
                var value = SettingProvider.Items["DisplayLatestProductsInNewOnMainPage"];
                return value == null || SQLDataHelper.GetBoolean(value);
            }
            set
            {
                SettingProvider.Items["DisplayLatestProductsInNewOnMainPage"] = value.ToString();
                ProductOnMain.ClearCache();
            }
        }


        public static bool IsLimitedPhotoNameLength
        {
            get => SettingProvider.Items["IsLimitedPhotoNameLength"].TryParseBool();
            set => SettingProvider.Items["IsLimitedPhotoNameLength"] = value.ToString();
        }

        public static bool SearchByCategories
        {
            get
            {
                var value = SettingProvider.Items["SearchByCategories"];
                return value == null ? true : SQLDataHelper.GetBoolean(value);
            }
            set => SettingProvider.Items["SearchByCategories"] = value.ToString();
        }

        public static int CountLinesProductName => SettingProvider.Items["CountLinesProductName"].TryParseInt(3);

        public static int MaximumItemsInMenu
        {
            get => SettingProvider.Items["MaximumItemsInMenu"].TryParseInt(defaultValue: 20);
            set => SettingProvider.Items["MaximumItemsInMenu"] = value.ToString();
        }

        public static int MaximumSubItemsInMenu
        {
            get => SettingProvider.Items["MaximumSubItemsInMenu"].TryParseInt(defaultValue: 20);
            set => SettingProvider.Items["MaximumSubItemsInMenu"] = value.ToString();
        }

        public static bool UseAdaptiveRootCategory
        {
            get => SettingProvider.Items["UseAdaptiveRootCategory"].TryParseBool(isNullable: true) ?? true;
            set => SettingProvider.Items["UseAdaptiveRootCategory"] = value.ToString();
        }    
        
        public static bool LimitedCategoryMenu
        {
            get => SettingProvider.Items["LimitedCategoryMenu"].TryParseBool();
            set => SettingProvider.Items["LimitedCategoryMenu"] = value.ToString();
        }
        
        public static int DefaultWarehouse
        {
            get => int.Parse(SettingProvider.Items["DefaultWarehouse"]);
            set => SettingProvider.Items["DefaultWarehouse"] = value.ToString();
        }
        public static bool CatalogVisibleBriefDescription
        {
            get => Convert.ToBoolean(SettingProvider.Items["CatalogVisibleBriefDescription"]);
            set => SettingProvider.Items["CatalogVisibleBriefDescription"] = value.ToString();
        }
    }
}