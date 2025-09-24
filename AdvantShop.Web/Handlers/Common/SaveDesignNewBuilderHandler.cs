using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Design;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Customers;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Models.Common;
using AdvantShop.Track;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Common
{
    public class SaveDesignNewBuilderHandler : ICommandHandler
    {
        private readonly DesignNewBuilderModel _model;
        private readonly SettingsTemplate _settingsTemplate;
        
        public SaveDesignNewBuilderHandler(DesignNewBuilderModel model)
        {
            _model = model;
            _settingsTemplate = new SettingsTemplate();
        }

        public void Execute()
        {
            if (Demo.IsDemoEnabled && !CustomerContext.CurrentCustomer.IsAdmin)
            {
                CommonHelper.SetCookie(DesignService.TypeAndPath[eDesign.Theme], _model.CurrentTheme);
                CommonHelper.SetCookie(DesignService.TypeAndPath[eDesign.Background], _model.CurrentBackGround);
                CommonHelper.SetCookie(DesignService.TypeAndPath[eDesign.Color], _model.CurrentColorScheme);
                CommonHelper.SetCookie(DesignConstants.DemoCookie_Design_MainPageMode, _model.MainPageMode);

                return;
            }

            SettingsDesign.Theme = _model.CurrentTheme;
            SettingsDesign.Background = _model.CurrentBackGround;
            SettingsDesign.ColorScheme = _model.CurrentColorScheme;
            SettingsDesign.MainPageMode = (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), _model.MainPageMode);
            

            TrialService.TrackEvent(TrialEvents.ChangeColorScheme, _model.CurrentColorScheme);
            TrialService.TrackEvent(TrialEvents.ChangeBackGround, _model.CurrentBackGround);
            TrialService.TrackEvent(TrialEvents.ChangeTheme, _model.CurrentTheme);
            TrialService.TrackEvent(TrialEvents.ChangeMainPageMode, _model.MainPageMode);

            TrackService.TrackEvent(ETrackEvent.Trial_ChangeDesignTransformer);

            var mobileBrowserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
            if (mobileBrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
            {
                var curColorScheme = DesignService.GetCurrenDesign(eDesign.Color);
                SettingsMobile.BrowserColor = curColorScheme.Color;
            }

            #region Common

            _settingsTemplate.SearchBlockLocation = _model.SearchBlockLocation;
            _settingsTemplate.RecentlyViewVisibility = _model.RecentlyViewVisibility;
            _settingsTemplate.WishListVisibility = _model.WishListVisibility;
            _settingsTemplate.MenuStyle = _model.MenuStyle;
            _settingsTemplate.FontStyle = _model.FontStyle;
            _settingsTemplate.FontSize = _model.FontSize;
            _settingsTemplate.TitleSize = _model.TitleSize;
            _settingsTemplate.TitleStyle = _model.TitleStyle;
            _settingsTemplate.TitleWeight = _model.TitleWeight;
            _settingsTemplate.TopPanel = _model.TopPanel;
            _settingsTemplate.Header = _model.Header;
            _settingsTemplate.TopMenu = _model.TopMenu;
            _settingsTemplate.TopMenuVisibility = _model.TopMenuVisibility;


            SettingsMain.IsStoreClosed = _model.IsStoreClosed;
            SettingsMain.EnableInplace = _model.EnableInplace;
            SettingsDesign.DisplayToolBarBottom = _model.DisplayToolBarBottom;
            SettingsDesign.HideCityInTopPanel = _model.HideCityInTopPanel;
            SettingsDesign.AutodetectCity = _model.AutodetectCity;
            SettingsDesign.DefaultCityIdIfNotAutodetect = _model.AutodetectCity
                ? null
                : _model.DefaultCityIdIfNotAutodetect;
            SettingsSEO.CustomMetaString = _model.AdditionalHeadMetaTag;
            SettingsCheckout.IsShowUserAgreementTextValue = _model.ShowUserAgreementText;
            SettingsCheckout.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            SettingsCheckout.UserAgreementText = _model.UserAgreementText;
            SettingsDesign.DisplayCityBubbleType = _model.DisplayCityBubbleType;
            SettingsNotifications.ShowCookiesPolicyMessage = _model.ShowCookiesPolicyMessage;
            SettingsNotifications.CookiesPolicyMessage = _model.CookiesPolicyMessage;

            var langChanged = SettingsMain.Language != _model.SiteLanguage;
            SettingsMain.Language = _model.SiteLanguage;

            if (langChanged)
            {
                CacheManager.Clean();
                LocalizationService.GenerateJsResourcesFile();
            }

            SettingsDesign.ShowCustomCopyright = _model.CopyrightMode == "custom";
            SettingsDesign.CopyrightText = _model.CopyrightText;

            SettingsDesign.ShowUserAgreementForPromotionalNewsletter = _model.ShowUserAgreementForPromotionalNewsletter;
            SettingsDesign.UserAgreementForPromotionalNewsletter = _model.UserAgreementForPromotionalNewsletter;
            SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked = _model.SetUserAgreementForPromotionalNewsletterChecked;
            
            if (_model.AdditionalPhones != null && _model.AdditionalPhones.Count > 0)
            {
                var i = 0;
                var phones = new List<AdditionalPhone>();
                
                foreach (var phone in _model.AdditionalPhones)
                {
                    i++;

                    if (i != 1 && (string.IsNullOrWhiteSpace(phone.Phone) || string.IsNullOrWhiteSpace(phone.StandardPhone)) && phone.Type != EAdditionalPhoneType.Telegram)
                        continue;

                    phone.StandardPhone = StringHelper.ConvertToMobileStandardPhone(phone.StandardPhone);

                    if (i == 1)
                    {
                        SettingsMain.Phone = phone.Phone;
                        SettingsMain.MobilePhone = phone.StandardPhone;
                        SettingsMain.PhoneDescription = phone.Description;
                    }
                    else
                    {
                        phones.Add(phone);
                    }
                }
                SettingsMain.AdditionalPhones = phones;
            }
            
            SettingsCatalog.UseAdaptiveRootCategory = _model.UseAdaptiveRootCategory;
            SettingsCatalog.LimitedCategoryMenu = _model.LimitedCategoryMenu;
            SettingsDesign.ShowPriceInMiniCart = _model.ShowPriceInMiniCart;
            SettingsDesign.CartAddType = _model.CartAddType;

            #endregion

            #region Main page

            _settingsTemplate.CarouselVisibility = _model.CarouselVisibility;
            _settingsTemplate.CarouselAnimationSpeed = _model.CarouselAnimationSpeed;
            _settingsTemplate.CarouselAnimationDelay = _model.CarouselAnimationDelay;

            _settingsTemplate.MainPageProductsVisibility = _model.MainPageProductsVisibility;
            _settingsTemplate.CountMainPageProductInSection = _model.CountMainPageProductInSection;
            _settingsTemplate.CountMainPageProductInLine = _model.CountMainPageProductInLine;

            _settingsTemplate.NewsVisibility = _model.NewsVisibility;
            _settingsTemplate.NewsSubscriptionVisibility = _model.NewsSubscriptionVisibility;
            _settingsTemplate.CheckOrderVisibility = _model.CheckOrderVisibility;
            _settingsTemplate.GiftSertificateVisibility = _model.GiftSertificateVisibility;
            _settingsTemplate.BrandCarouselVisibility = _model.BrandCarouselVisibility;

            _settingsTemplate.MainPageCategoriesVisibility = _model.MainPageCategoriesVisibility;
            _settingsTemplate.CountMainPageCategoriesInSection = _model.CountMainPageCategoriesInSection;
            _settingsTemplate.CountMainPageCategoriesInLine = _model.CountMainPageCategoriesInLine;
            SettingsMain.MainPageVisibleBriefDescription = _model.MainPageVisibleBriefDescription;
            
            _settingsTemplate.MainPageProductReviewsVisibility = _model.MainPageProductReviewsVisibility;
            _settingsTemplate.CountMainPageProductReviewsInSection = _model.CountMainPageProductReviewsInSection;
            _settingsTemplate.CountMainPageProductReviewsInLine = _model.CountMainPageProductReviewsInLine;
            
            SettingsCatalog.ReviewsSortingOnMainPage = _model.ReviewsSortingOnMainPage.TryParseEnum<ReviewsSortingOnMainPage>();

            #endregion

            #region Catalog

            _settingsTemplate.CountCategoriesInLine = _model.CountCategoriesInLine;
            SettingsCatalog.CatalogVisibleBriefDescription = _model.CatalogVisibleBriefDescription;
            SettingsCatalog.ShowQuickView = _model.ShowQuickView;
            SettingsCatalog.ProductsPerPage = _model.ProductsPerPage;
            _settingsTemplate.CountCatalogProductInLine = _model.CountCatalogProductInLine;
            SettingsCatalog.ShowProductsCount = _model.ShowProductsCount;
            SettingsCatalog.DisplayCategoriesInBottomMenu = _model.DisplayCategoriesInBottomMenu;
            SettingsCatalog.ShowProductArtNo = _model.ShowProductArtNo;
            SettingsCatalog.EnableProductRating = _model.EnableProductRating;
            SettingsCatalog.EnableCompareProducts = _model.EnableCompareProducts;
            SettingsCatalog.EnablePhotoPreviews = _model.EnablePhotoPreviews;
            SettingsCatalog.ShowSpinboxInCatalog = _model.ShowSpinboxInCatalog;
            SettingsCatalog.ShowCountPhoto = _model.ShowCountPhoto;
            var showOnlyAvailableChanged = SettingsCatalog.ShowOnlyAvalible != _model.ShowOnlyAvalible;
            SettingsCatalog.ShowOnlyAvalible = _model.ShowOnlyAvalible;
            SettingsCatalog.MoveNotAvaliableToEnd = _model.MoveNotAvaliableToEnd;
            SettingsCatalog.ShowNotAvaliableLable = _model.ShowNotAvaliableLable;

            SettingsDesign.FilterVisibility = _model.FilterVisibility;
            SettingsCatalog.ShowPriceFilter = _model.ShowPriceFilter;
            SettingsCatalog.ShowProducerFilter = _model.ShowProducerFilter;
            SettingsCatalog.ShowSizeFilter = _model.ShowSizeFilter;
            SettingsCatalog.ShowColorFilter = _model.ShowColorFilter;
            SettingsCatalog.ShowWarehouseFilter = _model.ShowWarehouseFilter;
            SettingsCatalog.ShowPropertiesFilterInProductList = _model.ShowPropertiesFilterInProductList;
            SettingsCatalog.ShowPropertiesFilterInParentCategories = _model.ShowPropertiesFilterInParentCategories;
            SettingsCatalog.ExcludingFilters = _model.ExcludingFilters;

            SettingsCatalog.SizesHeader = _model.SizesHeader;
            SettingsCatalog.ColorsHeader = _model.ColorsHeader;

            SettingsDesign.ColorsControlType = _model.ColorsControlType.ToString().TryParseEnum<SettingsDesign.eSizeColorControlType>();
            SettingsDesign.SizesControlType = _model.SizesControlType.ToString().TryParseEnum<SettingsDesign.eSizeColorControlType>();

            SettingsCatalog.ColorsViewMode = _model.ColorsViewMode.TryParseEnum<ColorsViewMode>();
            SettingsPictureSize.ColorIconWidthCatalog = _model.ColorIconWidthCatalog;
            SettingsPictureSize.ColorIconHeightCatalog = _model.ColorIconHeightCatalog;
            SettingsPictureSize.ColorIconWidthDetails = _model.ColorIconWidthDetails;
            SettingsPictureSize.ColorIconHeightDetails = _model.ColorIconHeightDetails;
            SettingsCatalog.ComplexFilter = _model.ComplexFilter;

            SettingsCatalog.BuyButtonText = _model.BuyButtonText;
            SettingsCatalog.DisplayBuyButton = _model.DisplayBuyButton;
            SettingsCatalog.PreOrderButtonText = _model.PreOrderButtonText;
            SettingsCatalog.DisplayPreOrderButton = _model.DisplayPreOrderButton;

            SettingsCatalog.DefaultCatalogView = _model.DefaultCatalogView.TryParseEnum<ProductViewMode>();
            SettingsCatalog.EnabledCatalogViewChange = _model.EnableCatalogViewChange;
            SettingsCatalog.DefaultSearchView = _model.DefaultSearchView.TryParseEnum<ProductViewMode>();
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
            SettingsCatalog.ShowAvaliableLableInProduct = _model.ShowAvailableLableInProduct;
            SettingsCatalog.ShowStockAvailability = _model.ShowStockAvailability;
            SettingsCatalog.ShowProductArtNoOnProductCard = _model.ShowProductArtNoOnProductCard;
            SettingsCatalog.ShowNotAvaliableLableInProduct = _model.ShowNotAvailableLableInProduct;
            SettingsCatalog.ShowAvailableInWarehouseInProduct = _model.ShowAvailableInWarehouseInProduct;
            SettingsCatalog.ShowOnlyAvailableWarehousesInProduct = _model.ShowOnlyAvailableWarehousesInProduct;
            SettingsCatalog.YaMapsApiKeyFoMapWarehouse = _model.YaMapsApiKeyFoMapWarehouse;

            //SettingsCatalog.CompressBigImage = _model.CompressBigImage;
            SettingsDesign.EnableZoom = _model.EnableZoom;

            SettingsCatalog.AllowReviews = _model.AllowReviews;
            SettingsCatalog.ModerateReviews = _model.ModerateReviews;
            SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers = _model.ReviewsVoiteOnlyRegisteredUsers;
            SettingsCatalog.DisplayReviewsImage = _model.DisplayReviewsImage;
            SettingsCatalog.AllowReviewsImageUploading = _model.AllowReviewsImageUploading;
            SettingsPictureSize.ReviewImageWidth = _model.ReviewImageWidth;
            SettingsPictureSize.ReviewImageHeight = _model.ReviewImageHeight;
            
            SettingsDesign.WhoAllowReviews = _model.WhoAllowReviews;
            
            SettingsDesign.ShowShippingsMethodsInDetails = _model.ShowShippingsMethodsInDetails.TryParseEnum<SettingsDesign.eShowShippingsInDetails>();
            SettingsDesign.ShippingsMethodsInDetailsCount = _model.ShippingsMethodsInDetailsCount;

            SettingsCatalog.RelatedProductName = _model.RelatedProductName;
            SettingsCatalog.AlternativeProductName = _model.AlternativeProductName;
            SettingsDesign.RelatedProductSourceType = _model.RelatedProductSourceType.TryParseEnum<SettingsDesign.eRelatedProductSourceType>();
            SettingsDesign.SimilarProductSourceType = _model.SimilarProductSourceType.TryParseEnum<SettingsDesign.eRelatedProductSourceType>();
            SettingsCatalog.RelatedProductsMaxCount = _model.RelatedProductsMaxCount;

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
            SettingsNews.MainPageText = _model.NewsMainPageText;
            SettingsNews.NewsPerPage = _model.NewsPerPage;
            SettingsNews.NewsMainPageCount = _model.NewsMainPageCount;

            #endregion

            #region CssEditor
            FilePath.FoldersHelper.SaveCSS(_model.CssEditorText, FilePath.CssType.extra);
            #endregion

            #region Other
            
            _settingsTemplate.OtherSettingsSections = _model.OtherSettingsSections;

            #endregion
            
            if (showOnlyAvailableChanged)
                CategoryService.RecalculateProductsCountManual();

            CacheManager.Clean();

            new ScreenshotService().UpdateStoreScreenShotInBackground();
        }
    }
}