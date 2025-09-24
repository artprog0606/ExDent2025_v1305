using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Admin.Models.Settings.Templates;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Templates
{
    public class SaveTemplateSettings : ICommandHandler<bool>
    {
        private readonly SettingsTemplateModel _model;
        private readonly SettingsTemplate _settingsTemplate;

        public SaveTemplateSettings(SettingsTemplateModel model)
        {
            _model = model;
            _settingsTemplate = new SettingsTemplate();
        }

        public bool Execute()
        {
            #region Common

            _settingsTemplate.MainPageMode = _model.MainPageMode;
            _settingsTemplate.MenuStyle = _model.MenuStyle;
            _settingsTemplate.FontStyle = _model.FontStyle;
            _settingsTemplate.FontSize = _model.FontSize;
            _settingsTemplate.TitleStyle = _model.TitleStyle;
            _settingsTemplate.TitleSize = _model.TitleSize;
            _settingsTemplate.TitleWeight = _model.TitleWeight;
            _settingsTemplate.SearchBlockLocation = _model.SearchBlockLocation;
            _settingsTemplate.RecentlyViewVisibility = _model.RecentlyViewVisibility;
            _settingsTemplate.WishListVisibility = _model.WishListVisibility;
            // TemplateSettingsProvider.SaveSetting("MobileAppBannerVisibility", _model.MobileAppBannerVisibility);

            _settingsTemplate.TopPanel = _model.TopPanel;
            _settingsTemplate.Header = _model.Header;
            _settingsTemplate.TopMenu = _model.TopMenu;
            _settingsTemplate.TopMenuVisibility = _model.TopMenuVisibility;

            var storeClosedChanged = SettingsMain.IsStoreClosed != _model.IsStoreClosed;

            SettingsDesign.AllowChooseDarkTheme = _model.AllowChooseDarkTheme;
            SettingsDesign.UseAnotherForDarkTheme = (bool)_model.UseAnotherForDarkTheme;
            SettingsMain.IsStoreClosed = _model.IsStoreClosed;
            SettingsMain.EnableInplace = _model.EnableInplace;
            SettingsDesign.DisplayToolBarBottom = _model.DisplayToolBarBottom;
            SettingsDesign.AutodetectCity = _model.AutodetectCity;
            SettingsDesign.HideCityInTopPanel = _model.HideCityInTopPanel;
            
            if(_model.DefaultCityIdIfNotAutodetect != null && _model.DefaultCityIdIfNotAutodetect != 0)
            {
                SettingsDesign.DefaultCityIdIfNotAutodetect = _model.DefaultCityIdIfNotAutodetect;
            } 
            else
            {
                SettingsDesign.DefaultCityIdIfNotAutodetect = _model.DefaultCityIdIfNotAutodetect;
                SettingsDesign.DefaultCityIfNotAutodetect = _model.DefaultCityIfNotAutodetect;
            }

            SettingsSEO.CustomMetaString = _model.AdditionalHeadMetaTag;
            SettingsCheckout.IsShowUserAgreementTextValue = _model.ShowUserAgreementText;
            SettingsCheckout.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            SettingsCheckout.UserAgreementText = _model.UserAgreementText;
            SettingsDesign.DisplayCityBubbleType = _model.DisplayCityBubbleType;
            SettingsDesign.HideSearchInZoneDialog = _model.HideSearchInZoneDialog;
            SettingsDesign.HideCountriesInZoneDialog = _model.HideCountriesInZoneDialog;
            SettingsNotifications.ShowCookiesPolicyMessage = _model.ShowCookiesPolicyMessage;
            SettingsNotifications.CookiesPolicyMessage = _model.CookiesPolicyMessage;
            
            SettingsDesign.ShowUserAgreementForPromotionalNewsletter = _model.ShowUserAgreementForPromotionalNewsletter;
            SettingsDesign.UserAgreementForPromotionalNewsletter = _model.UserAgreementForPromotionalNewsletter;
            SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked = _model.SetUserAgreementForPromotionalNewsletterChecked;
            SettingsDesign.ShowPriceInMiniCart = _model.ShowPriceInMiniCart;
            SettingsDesign.CartAddType = _model.CartAddType;
            
            SettingsCatalog.UseAdaptiveRootCategory = _model.UseAdaptiveRootCategory;
            SettingsCatalog.LimitedCategoryMenu = _model.LimitedCategoryMenu;
            #endregion

            #region Main page

            _settingsTemplate.CarouselVisibility = _model.CarouselVisibility;
            _settingsTemplate.CarouselAnimationSpeed = _model.CarouselAnimationSpeed;
            _settingsTemplate.CarouselAnimationDelay = _model.CarouselAnimationDelay;

            _settingsTemplate.MainPageProductsVisibility = _model.MainPageProductsVisibility;
            _settingsTemplate.CountMainPageProductInSection = _model.CountMainPageProductInSection;
            _settingsTemplate.CountMainPageProductInLine = _model.CountMainPageProductInLine;

            _settingsTemplate.BrandCarouselVisibility = _model.BrandCarouselVisibility;
            _settingsTemplate.NewsVisibility = _model.NewsVisibility;
            _settingsTemplate.NewsSubscriptionVisibility = _model.NewsSubscriptionVisibility;
            _settingsTemplate.CheckOrderVisibility = _model.CheckOrderVisibility;
            _settingsTemplate.GiftSertificateVisibility = _model.GiftSertificateVisibility;

            _settingsTemplate.MainPageCategoriesVisibility = _model.MainPageCategoriesVisibility;
            _settingsTemplate.CountMainPageCategoriesInSection = _model.CountMainPageCategoriesInSection;
            _settingsTemplate.CountMainPageCategoriesInLine = _model.CountMainPageCategoriesInLine;
            SettingsMain.MainPageVisibleBriefDescription = _model.MainPageVisibleBriefDescription;
            
            _settingsTemplate.MainPageProductReviewsVisibility = _model.MainPageProductReviewsVisibility;
            _settingsTemplate.CountMainPageProductReviewsInSection = _model.CountMainPageProductReviewsInSection;
            _settingsTemplate.CountMainPageProductReviewsInLine = _model.CountMainPageProductReviewsInLine;
            SettingsCatalog.ReviewsSortingOnMainPage = (ReviewsSortingOnMainPage)_model.ReviewsSortingOnMainPage;
            
            #endregion

            #region Catalog

            _settingsTemplate.CountCategoriesInLine = _model.CountCategoriesInLine;
            _settingsTemplate.CountCatalogProductInLine = _model.CountCatalogProductInLine;
            SettingsCatalog.CatalogVisibleBriefDescription = _model.CatalogVisibleBriefDescription;
            SettingsCatalog.ShowQuickView = _model.ShowQuickView;
            SettingsCatalog.ProductsPerPage = _model.ProductsPerPage;
            SettingsCatalog.ShowProductsCount = _model.ShowProductsCount;
            SettingsCatalog.DisplayCategoriesInBottomMenu = _model.DisplayCategoriesInBottomMenu;
            SettingsCatalog.ShowProductArtNo = _model.ShowProductArtNo;
            SettingsCatalog.EnableProductRating = _model.EnableProductRating;
            SettingsCatalog.EnableCompareProducts = _model.EnableCompareProducts;
            SettingsCatalog.EnablePhotoPreviews = _model.EnablePhotoPreviews;
            SettingsCatalog.ShowCountPhoto = _model.ShowCountPhoto;
            var showOnlyAvailableChanged = SettingsCatalog.ShowOnlyAvalible != _model.ShowOnlyAvalible;
            SettingsCatalog.ShowOnlyAvalible = _model.ShowOnlyAvalible;
            SettingsCatalog.MoveNotAvaliableToEnd = _model.MoveNotAvaliableToEnd;
            SettingsCatalog.ShowNotAvaliableLable = _model.ShowNotAvaliableLable;
            SettingsCatalog.ShowUnitsInCatalog = _model.ShowUnitsInCatalog;

            SettingsDesign.FilterVisibility = _model.FilterVisibility;
            SettingsCatalog.ShowPriceFilter = _model.ShowPriceFilter;
            SettingsCatalog.ShowProducerFilter = _model.ShowProducerFilter;
            SettingsCatalog.ShowSizeFilter = _model.ShowSizeFilter;
            SettingsCatalog.ShowWarehouseFilter = _model.ShowWarehouseFilter;
            SettingsCatalog.ShowPropertiesFilterInProductList = _model.ShowPropertiesFilterInProductList;
            SettingsCatalog.ShowPropertiesFilterInParentCategories = _model.ShowPropertiesFilterInParentCategories;
            SettingsCatalog.ExcludingFilters = _model.ExcludingFilters;
            SettingsCatalog.ShowSpinboxInCatalog = _model.ShowSpinboxInCatalog;
            SettingsCatalog.ShowColorFilter = _model.ShowColorFilter;
            SettingsCatalog.SizesHeader = _model.SizesHeader;
            SettingsCatalog.ColorsHeader = _model.ColorsHeader;
            SettingsCatalog.ColorsViewMode = _model.ColorsViewMode;

            SettingsPictureSize.ColorIconWidthCatalog = _model.ColorIconWidthCatalog;
            SettingsPictureSize.ColorIconHeightCatalog = _model.ColorIconHeightCatalog;
            SettingsPictureSize.ColorIconWidthDetails = _model.ColorIconWidthDetails;
            SettingsPictureSize.ColorIconHeightDetails = _model.ColorIconHeightDetails;
            SettingsCatalog.ComplexFilter = _model.ComplexFilter;

            SettingsCatalog.BuyButtonText = _model.BuyButtonText;
            SettingsCatalog.DisplayBuyButton = _model.DisplayBuyButton;
            SettingsCatalog.PreOrderButtonText = _model.PreOrderButtonText;
            SettingsCatalog.DisplayPreOrderButton = _model.DisplayPreOrderButton;

            SettingsCatalog.DefaultCatalogView = _model.DefaultCatalogView;
            SettingsCatalog.EnabledCatalogViewChange = _model.EnableCatalogViewChange;

            SettingsCatalog.DefaultSearchView = _model.DefaultSearchView;
            SettingsCatalog.EnabledSearchViewChange = _model.EnableSearchViewChange;

            _settingsTemplate.BigProductImageWidth = _model.BigProductImageWidth;
            _settingsTemplate.BigProductImageHeight = _model.BigProductImageHeight;

            _settingsTemplate.MiddleProductImageWidth = _model.MiddleProductImageWidth;
            _settingsTemplate.MiddleProductImageHeight = _model.MiddleProductImageHeight;

            _settingsTemplate.SmallProductImageWidth = _model.SmallProductImageWidth;
            _settingsTemplate.SmallProductImageHeight = _model.SmallProductImageHeight;

            _settingsTemplate.XSmallProductImageWidth = _model.XSmallProductImageWidth;
            _settingsTemplate.XSmallProductImageHeight = _model.XSmallProductImageHeight;

            _settingsTemplate.BigCategoryImageWidth = _model.BigCategoryImageWidth;
            _settingsTemplate.BigCategoryImageHeight = _model.BigCategoryImageHeight;

            _settingsTemplate.SmallCategoryImageWidth = _model.SmallCategoryImageWidth;
            _settingsTemplate.SmallCategoryImageHeight = _model.SmallCategoryImageHeight;

            #endregion

            #region Product

            SettingsCatalog.DisplayWeight = _model.DisplayWeight;
            SettingsCatalog.DisplayDimensions = _model.DisplayDimensions;
            SettingsCatalog.ShowStockAvailability = _model.ShowStockAvailability;
            SettingsCatalog.ShowProductArtNoOnProductCard = _model.ShowProductArtNoOnProductCard;
            //SettingsCatalog.CompressBigImage = _model.CompressBigImage;
            SettingsDesign.EnableZoom = _model.EnableZoom;
            SettingsCatalog.AllowReviews = _model.AllowReviews;
            SettingsCatalog.ModerateReviews = _model.ModerateReviews;
            SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers = _model.ReviewsVoiteOnlyRegisteredUsers;
            SettingsCatalog.DisplayReviewsImage = _model.DisplayReviewsImage;
            SettingsCatalog.AllowReviewsImageUploading = _model.AllowReviewsImageUploading;
            SettingsPictureSize.ReviewImageWidth = _model.ReviewImageWidth;
            SettingsPictureSize.ReviewImageHeight = _model.ReviewImageHeight;

            SettingsDesign.ShowShippingsMethodsInDetails = _model.ShowShippingsMethodsInDetails;

            SettingsDesign.ShippingsMethodsInDetailsCount = _model.ShippingsMethodsInDetailsCount;
            SettingsCatalog.RelatedProductName = _model.RelatedProductName;
            SettingsCatalog.AlternativeProductName = _model.AlternativeProductName;
            SettingsDesign.RelatedProductSourceType = _model.RelatedProductSourceType;
            SettingsDesign.SimilarProductSourceType = _model.SimilarProductSourceType;
            SettingsCatalog.RelatedProductsMaxCount = _model.RelatedProductsMaxCount;
            SettingsDesign.WhoAllowReviews = _model.WhoAllowReviews;
            SettingsCatalog.ShowNotAvaliableLableInProduct = _model.ShowNotAvailableLableInProduct;
            SettingsCatalog.ShowAvaliableLableInProduct = _model.ShowAvailableLableInProduct;
            SettingsCatalog.ShowAvailableInWarehouseInProduct = _model.ShowAvailableInWarehouseInProduct;
            SettingsCatalog.ShowOnlyAvailableWarehousesInProduct = _model.ShowOnlyAvailableWarehousesInProduct;
            SettingsCatalog.YaMapsApiKeyFoMapWarehouse = _model.YaMapsApiKeyFoMapWarehouse;
            SettingsDesign.ShowMarketplaceButton = _model.ShowMarketplaceButton;
            SettingsDesign.ShowUnitCardProduct = _model.ShowUnitCardProduct;
            SettingsDesign.ShowVerificationCheckmarkAtAdminInReviews = _model.ShowVerificationCheckmarkAtAdminInReviews;
            SettingsDesign.ColorsControlType = _model.ColorsControlType;
            SettingsDesign.SizesControlType = _model.SizesControlType;
            #endregion

            #region Checkout
            _settingsTemplate.ShowProductsPhotoInCheckoutCart = _model.ShowProductsPhotoInCheckoutCart;
            _settingsTemplate.PaymentIconWidth = _model.PaymentIconWidth;
            _settingsTemplate.PaymentIconHeight = _model.PaymentIconHeight;
            _settingsTemplate.ShippingIconWidth = _model.ShippingIconWidth;
            _settingsTemplate.ShippingIconHeight = _model.ShippingIconHeight;

            #endregion

            #region Brands

            _settingsTemplate.BrandLogoWidth = _model.BrandLogoWidth;
            _settingsTemplate.BrandLogoHeight = _model.BrandLogoHeight;
            SettingsCatalog.BrandsPerPage = _model.BrandsPerPage;
            SettingsCatalog.ShowCategoryTreeInBrand = _model.ShowCategoryTreeInBrand;
            SettingsCatalog.ShowProductsInBrand = _model.ShowProductsInBrand;
            SettingsCatalog.DefaultSortOrderProductInBrand = _model.DefaultSortOrderProductInBrand;

            #endregion

            #region News


            _settingsTemplate.NewsImageWidth = _model.NewsImageWidth;
            _settingsTemplate.NewsImageHeight = _model.NewsImageHeight;
            SettingsNews.NewsPerPage = _model.NewsPerPage;
            SettingsNews.NewsMainPageCount = _model.NewsMainPageCount;

            #endregion

            #region CustomOptions

            _settingsTemplate.CustomOptionsImageWidth = _model.CustomOptionsImageWidth;
            _settingsTemplate.CustomOptionsImageHeight = _model.CustomOptionsImageHeight;

            #endregion

            #region Other

            SettingsNews.MainPageText = _model.MainPageText;
            _settingsTemplate.OtherSettingsSections = _model.OtherSettings?
                .Select(os => 
                    new TemplateSettingSection() { Settings = os.Value })
                .ToList() ?? new List<TemplateSettingSection>();

            #endregion

            #region Css editor

            new CssEditorHandler().SaveFileContent(_model.CssEditorText ?? "");

            #endregion
            
            if (showOnlyAvailableChanged)
                CategoryService.RecalculateProductsCountManual();

            CacheManager.RemoveByPattern(CacheNames.MenuPrefix);
            
            if (storeClosedChanged)
                CacheManager.RemoveByPattern(LpConstants.LandingCachePrefix);

            return true;
        }
    }
}
