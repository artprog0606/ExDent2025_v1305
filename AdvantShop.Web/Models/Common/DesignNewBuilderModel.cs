using AdvantShop.Configuration;
using AdvantShop.Design;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Customers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Models.Common
{
    public class DesignNewBuilderModel
    {
        #region Common

        public string CurrentTheme { get; set; }
        public List<Theme> Themes { get; set; }

        public string CurrentBackGround { get; set; }
        public List<Theme> Backgrounds { get; set; }

        public string CurrentColorScheme { get; set; }
        public List<Theme> Colors { get; set; }

        public string MainPageMode { get; set; }
        public string MainPageModeType { get; set; }
        public List<SelectListItem> MainPageModeOptions { get; set; }
        public List<ImageSelectListOption> MainPageModeImageOptions { get; set; }

        public string MenuStyle { get; set; }
        public List<SelectListItem> MenuStyleOptions { get; set; }

        public string FontStyle { get; set; }
        public List<SelectListItem> FontStyleOptions { get; set; }
        public string FontSize { get; set; }
        public List<SelectListItem> FontSizeOptions { get; set; }

        public string TitleStyle { get; set; }
        public List<SelectListItem> TitleStyleOptions { get; set; }
        public string TitleSize { get; set; }
        public List<SelectListItem> TitleSizeOptions { get; set; }
        public string TitleWeight { get; set; }
        public List<SelectListItem> TitleWeightOptions { get; set; }


        public string SearchBlockLocation { get; set; }
        public List<SelectListItem> SearchBlockLocationOptions { get; set; }

        public string TopPanel { get; set; }
        public List<ImageSelectListOption> TopPanelOptions { get; set; }

        public string Header { get; set; }
        public List<ImageSelectListOption> HeaderOptions { get; set; }

        public string TopMenu { get; set; }
        public List<ImageSelectListOption> TopMenuOptions { get; set; }

        public bool RecentlyViewVisibility { get; set; }
        // public bool? MobileAppBannerVisibility { get; set; }
        public bool WishListVisibility { get; set; }
        public bool IsStoreClosed { get; set; }
        public bool EnableInplace { get; set; }
        public bool DisplayToolBarBottom { get; set; }
        public bool AutodetectCity { get; set; }
        public bool HideCityInTopPanel { get; set; }
        public int? DefaultCityIdIfNotAutodetect { get; set; }
        public string DefaultCityIfNotAutodetect { get; set; }
        public string DefaultCityDescription { get; set; }
        public string AdditionalHeadMetaTag { get; set; }

        public bool ShowUserAgreementText { get; set; }
        public bool AgreementDefaultChecked { get; set; }
        public string UserAgreementText { get; set; }

        public SettingsDesign.EDisplayCityBubbleType DisplayCityBubbleType { get; set; }
        public List<SelectListItem> DisplayCityBubbleTypes { get; set; }
        public bool ShowCookiesPolicyMessage { get; set; }
        public string CookiesPolicyMessage { get; set; }

        public string SiteLanguage { get; set; }
        public List<SelectListItem> Languages { get; set; }
        public bool TopMenuVisibility { get; set; }

        public string CopyrightMode { get; set; }

        public List<SelectListItem> CopyrightModeVariants { get; set; }
        
        public  string CopyrightText { get; set; }
        public bool ShowUserAgreementForPromotionalNewsletter { get; set; }
        public string UserAgreementForPromotionalNewsletter { get; set; }
        public bool SetUserAgreementForPromotionalNewsletterChecked { get; set; }
        public bool UseAdaptiveRootCategory { get; set; }
        public bool LimitedCategoryMenu { get; set; }
        public bool ShowPriceInMiniCart { get; set; }
        public SettingsDesign.eCartAddTypeButton CartAddType { get; set; }
        #endregion

        #region Main page

        public bool CarouselVisibility { get; set; }
        public int CarouselAnimationSpeed { get; set; }
        public int CarouselAnimationDelay { get; set; }

        public bool MainPageProductsVisibility { get; set; }
        public int CountMainPageProductInSection { get; set; }
        public int CountMainPageProductInLine { get; set; }

        public bool NewsVisibility { get; set; }
        public bool NewsSubscriptionVisibility { get; set; }
        public bool CheckOrderVisibility { get; set; }
        public bool GiftSertificateVisibility { get; set; }
        public bool BrandCarouselVisibility { get; set; }

        public bool MainPageCategoriesVisibility { get; set; }
        public int CountMainPageCategoriesInSection { get; set; }
        public int CountMainPageCategoriesInLine { get; set; }

        public bool MainPageVisibleBriefDescription { get; set; }
        
        public bool MainPageProductReviewsVisibility { get; set; }
        public int CountMainPageProductReviewsInSection { get; set; }
        public int CountMainPageProductReviewsInLine { get; set; }

        #endregion

        #region Catalog

        public int CountCategoriesInLine { get; set; }
        public bool CatalogVisibleBriefDescription { get; set; }
        public bool ShowQuickView { get; set; }
        public int ProductsPerPage { get; set; }
        public int CountCatalogProductInLine { get; set; }
        public bool ShowProductsCount { get; set; }
        public bool DisplayCategoriesInBottomMenu { get; set; }
        public bool ShowProductArtNo { get; set; }
        public bool EnableProductRating { get; set; }
        public bool EnableCompareProducts { get; set; }
        public bool EnablePhotoPreviews { get; set; }
        public bool ShowSpinboxInCatalog { get; set; }
        public bool ShowCountPhoto { get; set; }
        public bool ShowOnlyAvalible { get; set; }
        public bool MoveNotAvaliableToEnd { get; set; }
        public bool ShowNotAvaliableLable { get; set; }

        public bool FilterVisibility { get; set; }
        public bool ShowPriceFilter { get; set; }
        public bool ShowProducerFilter { get; set; }
        public bool ShowSizeFilter { get; set; }
        public bool ShowColorFilter { get; set; }
        public bool ShowWarehouseFilter { get; set; }
        public bool ShowPropertiesFilterInProductList { get; set; }
        public bool ShowPropertiesFilterInParentCategories { get; set; }
        public bool ExcludingFilters { get; set; }

        public string SizesHeader { get; set; }
        public string ColorsHeader { get; set; }
        public string ColorsControlType { get; set; }
        public List<SelectListItem> ColorsControlTypes { get; set; }
        public string SizesControlType { get; set; }
        public List<SelectListItem> SizesControlTypes { get; set; }

        public string ColorsViewMode { get; set; }
        public List<SelectListItem> ColorsViewModes { get; set; }
        public int ColorIconWidthCatalog { get; set; }
        public int ColorIconHeightCatalog { get; set; }
        public int ColorIconWidthDetails { get; set; }
        public int ColorIconHeightDetails { get; set; }
        public bool ComplexFilter { get; set; }

        public string BuyButtonText { get; set; }
        public bool DisplayBuyButton { get; set; }
        public string PreOrderButtonText { get; set; }
        public bool DisplayPreOrderButton { get; set; }

        public string DefaultCatalogView { get; set; }
        public bool EnableCatalogViewChange { get; set; }
        public List<SelectListItem> DefaultViewList { get; set; }
        public string DefaultSearchView { get; set; }
        public bool EnableSearchViewChange { get; set; }

        public int BigProductImageWidth { get; set; }
        public int BigProductImageHeight { get; set; }
        public int MiddleProductImageWidth { get; set; }
        public int MiddleProductImageHeight { get; set; }
        public int SmallProductImageWidth { get; set; }
        public int SmallProductImageHeight { get; set; }
        public int XSmallProductImageWidth { get; set; }
        public int XSmallProductImageHeight { get; set; }
        public int BigCategoryImageWidth { get; set; }
        public int BigCategoryImageHeight { get; set; }
        public int SmallCategoryImageWidth { get; set; }
        public int SmallCategoryImageHeight { get; set; }

        #endregion

        #region Product

        public bool DisplayWeight { get; set; }
        public bool DisplayDimensions { get; set; }
        public bool ShowAvailableLableInProduct { get; set; }
        public bool ShowStockAvailability { get; set; }
        public bool ShowProductArtNoOnProductCard { get; set; }
        public bool ShowNotAvailableLableInProduct { get; set; }
        public bool ShowAvailableInWarehouseInProduct { get; set; }
        public bool ShowOnlyAvailableWarehousesInProduct { get; set; }
        public string YaMapsApiKeyFoMapWarehouse { get; set; }
        public bool CompressBigImage { get; set; }
        public bool EnableZoom { get; set; }

        public bool AllowReviews { get; set; }
        public bool ModerateReviews { get; set; }
        public bool ReviewsVoiteOnlyRegisteredUsers { get; set; }
        public bool DisplayReviewsImage { get; set; }
        public bool AllowReviewsImageUploading { get; set; }
        public int ReviewImageWidth { get; set; }
        public int ReviewImageHeight { get; set; }
        
        public List<SelectListItem> WhoAllowReviewsOption { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SettingsDesign.eWhoAllowReviews WhoAllowReviews { get; set; }
        
        public string ShowShippingsMethodsInDetails { get; set; }
        public List<SelectListItem> ShowShippingsMethods { get; set; }
        public int ShippingsMethodsInDetailsCount { get; set; }

        public string RelatedProductName { get; set; }
        public string AlternativeProductName { get; set; }
        public string RelatedProductSourceType { get; set; }
        public string SimilarProductSourceType { get; set; }
        public List<SelectListItem> RelatedProductTypes { get; set; }
        public int RelatedProductsMaxCount { get; set; }
        
        #endregion

        #region Checkout

        public bool ShowProductsPhotoInCheckoutCart { get; set; }
        public int PaymentIconWidth { get; set; }
        public int PaymentIconHeight { get; set; }
        public int ShippingIconWidth { get; set; }
        public int ShippingIconHeight { get; set; }

        #endregion

        #region Brands

        public int BrandLogoWidth { get; set; }
        public int BrandLogoHeight { get; set; }
        public int BrandsPerPage { get; set; }
        public bool ShowProductsInBrand { get; set; }
        public List<SelectListItem> DefaultSortOrderProductInBrandOption { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ESortOrder DefaultSortOrderProductInBrand { get; set; }
        public bool ShowCategoryTreeInBrand { get; set; }

        #endregion

        #region News

        public int NewsImageWidth { get; set; }
        public int NewsImageHeight { get; set; }
        public string NewsMainPageText { get; set; }
        public int NewsPerPage { get; set; }
        public int NewsMainPageCount { get; set; }

        #endregion


        #region CssEditor

        public string CssEditorText { get; set; }

        #endregion

        #region Other

        public List<TemplateSettingSection> OtherSettingsSections { get; set; }
        public List<TemplateSetting> OtherSettings { get; set; }
        //public Dictionary<string, List<TemplateSetting>> OtherSettingsDic { get; set; }
        public List<AdditionalPhone> AdditionalPhones { get; set; }
        public List<SelectItemModel<int>> AdditionalPhoneTypes { get; set; }
        public bool ShowAdditionalPhones { get; set; }

        #endregion

        public bool IsDemo
        {
            get { return Demo.IsDemoEnabled; }
        }

        public bool IsAdmin
        {
            get { return CustomerContext.CurrentCustomer.IsAdmin; }
        }

        public Theme ColorSelected { get; set; }
        public bool ShowOtherSettings { get; set; }
        public bool HideDisplayToolBarBottomOption { get; set; }

        public List<string> HiddenSettings { get; set; }
        public string ReviewsSortingOnMainPage { get; set; }
        public List<SelectItemModel> ReviewsSortingOnMainPages { get; set; }
    }

    public class ImageSelectListOption : TemplateOptionSetting
    {
        public string ImageSrc { get; set; }

        public ImageSelectListOption()
        {
        }

        public ImageSelectListOption(TemplateOptionSetting option)
        {
            Title = option.Title;
            Value = option.Value;
            Image = option.Image;

            var path = SettingsDesign.Template != TemplateService.DefaultTemplateId
                ? "templates/" + SettingsDesign.Template + "/"
                : "";
            ImageSrc = path + Image;
        }
    }
}