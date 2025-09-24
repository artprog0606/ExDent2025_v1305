using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.FilePath;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Admin.Models.Settings.Templates;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Templates
{
    public class GetTemplateSettings : ICommandHandler<SettingsTemplateModel>
    {
        private SettingsTemplate _settingsTemplate;

        public GetTemplateSettings() => _settingsTemplate = new SettingsTemplate();

        public SettingsTemplateModel Execute()
        {
            var model = new SettingsTemplateModel();

            #region Common

            model.MainPageMode = _settingsTemplate.MainPageMode;
            model.MainPageModeOptions = _settingsTemplate.MainPageModeOptions;

            model.MenuStyle = _settingsTemplate.MenuStyle;
            model.MenuStyleOptions = _settingsTemplate.MenuStyleOptions;
            
            model.FontStyle = _settingsTemplate.FontStyle;
            model.FontStyleOptions = _settingsTemplate.FontStyleOptions;

            model.FontSize = _settingsTemplate.FontSize;
            model.FontSizeOptions = _settingsTemplate.FontSizeOptions;
            
            model.TitleStyle = _settingsTemplate.TitleStyle;
            model.TitleStyleOptions = _settingsTemplate.TitleStyleOptions;
            
            model.TitleSize = _settingsTemplate.TitleSize;
            model.TitleSizeOptions = _settingsTemplate.TitleSizeOptions;
            
            model.TitleWeight = _settingsTemplate.TitleWeight;
            model.TitleWeightOptions = _settingsTemplate.TitleWeightOptions;
            
            model.SearchBlockLocation = _settingsTemplate.SearchBlockLocation;
            model.SearchBlockLocationOptions = _settingsTemplate.SearchBlockLocationOptions;
            
            model.TopPanel = _settingsTemplate.TopPanel;
            model.TopPanelOptions = _settingsTemplate.TopPanelOptions;
            
            model.Header = _settingsTemplate.Header;
            model.HeaderOptions = _settingsTemplate.HeaderOptions;
            
            model.TopMenu = _settingsTemplate.TopMenu;
            model.TopMenuOptions = _settingsTemplate.TopMenuOptions;

            // model.MobileAppBannerVisibility = TryGetTemplateSettingValue("MobileAppBannerVisibility").TryParseBool();
            model.AllowChooseDarkTheme = SettingsDesign.AllowChooseDarkTheme;
            model.UseAnotherForDarkTheme = SettingsDesign.UseAnotherForDarkTheme;
            model.RecentlyViewVisibility = _settingsTemplate.RecentlyViewVisibility;
            model.WishListVisibility = _settingsTemplate.WishListVisibility;
            model.IsStoreClosed = SettingsMain.IsStoreClosed;
            model.EnableInplace = SettingsMain.EnableInplace;
            model.DisplayToolBarBottom = SettingsDesign.DisplayToolBarBottom;
            model.AutodetectCity = SettingsDesign.AutodetectCity;
            model.HideCityInTopPanel = SettingsDesign.HideCityInTopPanel;

            if (SettingsDesign.DefaultCityIdIfNotAutodetect.HasValue && SettingsDesign.DefaultCityIdIfNotAutodetect != 0)
            {
                var city = CityService.GetCity(SettingsDesign.DefaultCityIdIfNotAutodetect.Value);
                if (city != null)
                {
                    model.DefaultCityIdIfNotAutodetect = SettingsDesign.DefaultCityIdIfNotAutodetect;
                    model.DefaultCityIfNotAutodetect = city.Name;

                    var region = RegionService.GetRegion(city.RegionId);
                    var country = CountryService.GetCountry(region.CountryId);
                    model.DefaultCityDescription = $"{country.Name}, {region.Name}";
                }
            } else
            {
                model.DefaultCityIfNotAutodetect = SettingsDesign.DefaultCityIfNotAutodetect;
            }

            model.AdditionalHeadMetaTag = SettingsSEO.CustomMetaString;
            model.ShowUserAgreementText = SettingsCheckout.IsShowUserAgreementTextValue;
            model.AgreementDefaultChecked = SettingsCheckout.AgreementDefaultChecked;
            model.UserAgreementText = SettingsCheckout.UserAgreementText;
            model.ShowUserAgreementForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter;
            model.UserAgreementForPromotionalNewsletter = SettingsDesign.UserAgreementForPromotionalNewsletter;
            model.SetUserAgreementForPromotionalNewsletterChecked = SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked;
            model.DisplayCityBubbleType = SettingsDesign.DisplayCityBubbleType;
            model.DisplayCityBubbleTypes = Enum.GetValues(typeof(SettingsDesign.EDisplayCityBubbleType))
                                                .Cast<SettingsDesign.EDisplayCityBubbleType>()
                                                .Select(x => new SelectListItem { Text = x.Localize(), Value = ((int)x).ToString() })
                                                .ToList();
            model.HideCountriesInZoneDialog = SettingsDesign.HideCountriesInZoneDialog;
            model.HideSearchInZoneDialog = SettingsDesign.HideSearchInZoneDialog;
            model.ShowCookiesPolicyMessage = SettingsNotifications.ShowCookiesPolicyMessage;
            model.CookiesPolicyMessage = SettingsNotifications.CookiesPolicyMessage;

            model.TopMenuVisibility = _settingsTemplate.TopMenuVisibility;

            model.DarkThemeLogoImgSrc =
                    !string.IsNullOrEmpty(SettingsMain.DarkThemeLogoName)
                        ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.DarkThemeLogoName, true)
                        : "../images/nophoto_small.png";
            model.UseAdaptiveRootCategory = SettingsCatalog.UseAdaptiveRootCategory;
            model.LimitedCategoryMenu = SettingsCatalog.LimitedCategoryMenu;
            model.ShowPriceInMiniCart = _settingsTemplate.ShowPriceInMiniCart;
            
            model.CartAddType = SettingsDesign.CartAddType;

            #endregion

            #region Main page

            model.CarouselVisibility = _settingsTemplate.CarouselVisibility;
            model.CarouselAnimationSpeed = _settingsTemplate.CarouselAnimationSpeed;
            model.CarouselAnimationDelay = _settingsTemplate.CarouselAnimationDelay;

            model.MainPageProductsVisibility = _settingsTemplate.MainPageProductsVisibility;
            model.CountMainPageProductInSection = _settingsTemplate.CountMainPageProductInSection;
            model.CountMainPageProductInLine = _settingsTemplate.CountMainPageProductInLine;
            
            model.BrandCarouselVisibility = _settingsTemplate.BrandCarouselVisibility;
            model.NewsVisibility = _settingsTemplate.NewsVisibility;
            model.NewsSubscriptionVisibility = _settingsTemplate.NewsSubscriptionVisibility;
            model.CheckOrderVisibility = _settingsTemplate.CheckOrderVisibility;
            model.GiftSertificateVisibility = _settingsTemplate.GiftSertificateVisibility;

            model.MainPageCategoriesVisibility = _settingsTemplate.MainPageCategoriesVisibility;
            model.CountMainPageCategoriesInSection = _settingsTemplate.CountMainPageCategoriesInSection;
            model.CountMainPageCategoriesInLine = _settingsTemplate.CountMainPageCategoriesInLine;
            model.MainPageVisibleBriefDescription = SettingsMain.MainPageVisibleBriefDescription;
            
            model.MainPageProductReviewsVisibility = _settingsTemplate.MainPageProductReviewsVisibility;
            model.CountMainPageProductReviewsInSection = _settingsTemplate.CountMainPageProductReviewsInSection;
            model.CountMainPageProductReviewsInLine = _settingsTemplate.CountMainPageProductReviewsInLine;
            
            model.ReviewsSortingOnMainPage = (int)SettingsCatalog.ReviewsSortingOnMainPage;
            model.ReviewsSortingOnMainPages = new List<SelectListItem>();

            foreach (ReviewsSortingOnMainPage item in Enum.GetValues(typeof(ReviewsSortingOnMainPage)))
                model.ReviewsSortingOnMainPages.Add(new SelectListItem() { Text = item.Localize(), Value = ((int)item).ToString() });

            #endregion

            #region Catalog

            model.CountCategoriesInLine = _settingsTemplate.CountCategoriesInLine;
            model.CountCatalogProductInLine = _settingsTemplate.CountCatalogProductInLine;
            model.CatalogVisibleBriefDescription = SettingsCatalog.CatalogVisibleBriefDescription;
            model.ShowQuickView = SettingsCatalog.ShowQuickView;
            model.ProductsPerPage = SettingsCatalog.ProductsPerPage;
            model.ShowProductsCount = SettingsCatalog.ShowProductsCount;
            model.DisplayCategoriesInBottomMenu = SettingsCatalog.DisplayCategoriesInBottomMenu;
            model.ShowProductArtNo = SettingsCatalog.ShowProductArtNo;
            model.EnableProductRating = SettingsCatalog.EnableProductRating;
            model.EnableCompareProducts = SettingsCatalog.EnableCompareProducts;
            model.EnablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews;
            model.ShowCountPhoto = SettingsCatalog.ShowCountPhoto;
            model.ShowOnlyAvalible = SettingsCatalog.ShowOnlyAvalible;
            model.MoveNotAvaliableToEnd = SettingsCatalog.MoveNotAvaliableToEnd;
            model.ShowNotAvaliableLable = SettingsCatalog.ShowNotAvaliableLable;
            model.ShowUnitsInCatalog = SettingsCatalog.ShowUnitsInCatalog;

            model.FilterVisibility = SettingsDesign.FilterVisibility;
            model.ShowPriceFilter = SettingsCatalog.ShowPriceFilter;
            model.ShowProducerFilter = SettingsCatalog.ShowProducerFilter;
            model.ShowSizeFilter = SettingsCatalog.ShowSizeFilter;
            model.ShowWarehouseFilter = SettingsCatalog.ShowWarehouseFilter;
            model.ShowPropertiesFilterInProductList = SettingsCatalog.ShowPropertiesFilterInProductList;
            model.ShowPropertiesFilterInParentCategories = SettingsCatalog.ShowPropertiesFilterInParentCategories;
            model.ExcludingFilters = SettingsCatalog.ExcludingFilters;
            model.ShowSpinboxInCatalog = SettingsCatalog.ShowSpinboxInCatalog;
            model.ShowColorFilter = SettingsCatalog.ShowColorFilter;
            model.SizesHeader = SettingsCatalog.SizesHeader;
            model.ColorsHeader = SettingsCatalog.ColorsHeader;
            model.ColorsViewMode = SettingsCatalog.ColorsViewMode;

            model.ColorsViewModes = new List<SelectListItem>();

            foreach (ColorsViewMode item in Enum.GetValues(typeof(ColorsViewMode)))
            {
                model.ColorsViewModes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            model.ColorIconWidthCatalog = SettingsPictureSize.ColorIconWidthCatalog;
            model.ColorIconHeightCatalog = SettingsPictureSize.ColorIconHeightCatalog;
            model.ColorIconWidthDetails = SettingsPictureSize.ColorIconWidthDetails;
            model.ColorIconHeightDetails = SettingsPictureSize.ColorIconHeightDetails;
            model.ComplexFilter = SettingsCatalog.ComplexFilter;

            model.BuyButtonText = SettingsCatalog.BuyButtonText;
            model.DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
            model.PreOrderButtonText = SettingsCatalog.PreOrderButtonText;
            model.DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

            model.DefaultCatalogView = SettingsCatalog.DefaultCatalogView;
            model.EnableCatalogViewChange = SettingsCatalog.EnabledCatalogViewChange;

            model.DefaultSearchView = SettingsCatalog.DefaultSearchView;
            model.EnableSearchViewChange = SettingsCatalog.EnabledSearchViewChange;

            model.DefaultViewList = new List<SelectListItem>();
            
            var options = _settingsTemplate.UnsupportedViewListTypes;
            var unsupportedViewListTypes = options.Select(x => x.Text.Parse<ProductViewMode>()).ToList();

            foreach (ProductViewMode item in Enum.GetValues(typeof(ProductViewMode)))
            {
                if (item != ProductViewMode.Single && !unsupportedViewListTypes.Contains(item))
                {
                    model.DefaultViewList.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
                }
            }


            model.BigProductImageWidth = _settingsTemplate.BigProductImageWidth;
            model.BigProductImageHeight = _settingsTemplate.BigProductImageHeight;

            model.MiddleProductImageWidth = _settingsTemplate.MiddleProductImageWidth;
            model.MiddleProductImageHeight = _settingsTemplate.MiddleProductImageHeight;

            model.SmallProductImageWidth = _settingsTemplate.SmallProductImageWidth;
            model.SmallProductImageHeight = _settingsTemplate.SmallProductImageHeight;

            model.XSmallProductImageWidth = _settingsTemplate.XSmallProductImageWidth;
            model.XSmallProductImageHeight = _settingsTemplate.XSmallProductImageHeight;

            model.BigCategoryImageWidth = _settingsTemplate.BigCategoryImageWidth;
            model.BigCategoryImageHeight = _settingsTemplate.BigCategoryImageHeight;

            model.SmallCategoryImageWidth = _settingsTemplate.SmallCategoryImageWidth;
            model.SmallCategoryImageHeight = _settingsTemplate.SmallCategoryImageHeight;

            #endregion

            #region Product

            model.DisplayWeight = SettingsCatalog.DisplayWeight;
            model.DisplayDimensions = SettingsCatalog.DisplayDimensions;
            model.ShowStockAvailability = SettingsCatalog.ShowStockAvailability;
            model.ShowProductArtNoOnProductCard = SettingsCatalog.ShowProductArtNoOnProductCard;
            //model.CompressBigImage = SettingsCatalog.CompressBigImage;
            model.EnableZoom = SettingsDesign.EnableZoom;
            model.AllowReviews = SettingsCatalog.AllowReviews;
            model.ModerateReviews = SettingsCatalog.ModerateReviews;
            model.ReviewsVoiteOnlyRegisteredUsers = SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers;
            model.DisplayReviewsImage = SettingsCatalog.DisplayReviewsImage;
            model.AllowReviewsImageUploading = SettingsCatalog.AllowReviewsImageUploading;
            model.ReviewImageWidth = SettingsPictureSize.ReviewImageWidth;
            model.ReviewImageHeight = SettingsPictureSize.ReviewImageHeight;
            
            model.ShowShippingsMethodsInDetails = SettingsDesign.ShowShippingsMethodsInDetails;
            model.ShowShippingsMethods = new List<SelectListItem>();
            foreach (SettingsDesign.eShowShippingsInDetails item in Enum.GetValues(typeof(SettingsDesign.eShowShippingsInDetails)))
            {
                model.ShowShippingsMethods.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            model.ShippingsMethodsInDetailsCount = SettingsDesign.ShippingsMethodsInDetailsCount;
            model.RelatedProductName = SettingsCatalog.RelatedProductName;
            model.AlternativeProductName = SettingsCatalog.AlternativeProductName;
            model.RelatedProductSourceType = SettingsDesign.RelatedProductSourceType;
            model.SimilarProductSourceType = SettingsDesign.SimilarProductSourceType;
            model.RelatedProductTypes = new List<SelectListItem>();
            foreach (SettingsDesign.eRelatedProductSourceType item in Enum.GetValues(typeof(SettingsDesign.eRelatedProductSourceType)))
            {
                if (item != SettingsDesign.eRelatedProductSourceType.FromCurrentCategory)
                    model.RelatedProductTypes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.RelatedProductTypes.Insert(2, new SelectListItem
            {
                Text = SettingsDesign.eRelatedProductSourceType.FromCurrentCategory.Localize(),
                Value = SettingsDesign.eRelatedProductSourceType.FromCurrentCategory.ToString()
            });

            model.RelatedProductsMaxCount = SettingsCatalog.RelatedProductsMaxCount;

            model.WhoAllowReviewsOption = new List<SelectListItem>();
            foreach (SettingsDesign.eWhoAllowReviews item in Enum.GetValues(typeof(SettingsDesign.eWhoAllowReviews)))
            {
                model.WhoAllowReviewsOption.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.WhoAllowReviews = SettingsDesign.WhoAllowReviews;

            model.ShowNotAvailableLableInProduct = SettingsCatalog.ShowNotAvaliableLableInProduct;
            model.ShowAvailableLableInProduct = SettingsCatalog.ShowAvaliableLableInProduct;
            model.ShowMarketplaceButton = SettingsDesign.ShowMarketplaceButton;
            model.ShowUnitCardProduct = SettingsDesign.ShowUnitCardProduct;
            model.ShowVerificationCheckmarkAtAdminInReviews = SettingsDesign.ShowVerificationCheckmarkAtAdminInReviews;
            
            model.SizesColorsControlTypesOptions = new List<SelectListItem>();
            foreach (SettingsDesign.eSizeColorControlType item in Enum.GetValues(typeof(SettingsDesign.eSizeColorControlType)))
            {
                model.SizesColorsControlTypesOptions.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.ColorsControlType = SettingsDesign.ColorsControlType;
            model.SizesControlType = SettingsDesign.SizesControlType;
            
            model.ShowAvailableInWarehouseInProduct = SettingsCatalog.ShowAvailableInWarehouseInProduct;
            model.ShowOnlyAvailableWarehousesInProduct = SettingsCatalog.ShowOnlyAvailableWarehousesInProduct;
            model.YaMapsApiKeyFoMapWarehouse = SettingsCatalog.YaMapsApiKeyFoMapWarehouse;

            #endregion

            #region Checkout

            model.ShowProductsPhotoInCheckoutCart = _settingsTemplate.ShowProductsPhotoInCheckoutCart;
            model.PaymentIconWidth = _settingsTemplate.PaymentIconWidth;
            model.PaymentIconHeight = _settingsTemplate.PaymentIconHeight;
            model.ShippingIconWidth = _settingsTemplate.ShippingIconWidth;
            model.ShippingIconHeight = _settingsTemplate.ShippingIconHeight;

            #endregion

            #region Brands

            model.BrandLogoWidth = _settingsTemplate.BrandLogoWidth;
            model.BrandLogoHeight = _settingsTemplate.BrandLogoHeight;
            model.BrandsPerPage = SettingsCatalog.BrandsPerPage;
            model.ShowCategoryTreeInBrand = SettingsCatalog.ShowCategoryTreeInBrand;
            model.ShowProductsInBrand = SettingsCatalog.ShowProductsInBrand;
            model.DefaultSortOrderProductInBrandOption = new List<SelectListItem>();
            foreach (ESortOrder item in Enum.GetValues(typeof(ESortOrder)))
            {
                model.DefaultSortOrderProductInBrandOption.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.DefaultSortOrderProductInBrand = SettingsCatalog.DefaultSortOrderProductInBrand;

            #endregion

            #region News

            model.NewsImageWidth = _settingsTemplate.NewsImageWidth;
            model.NewsImageHeight = _settingsTemplate.NewsImageHeight;
            model.NewsPerPage = SettingsNews.NewsPerPage;
            model.NewsMainPageCount = SettingsNews.NewsMainPageCount;

            #endregion

            #region CustomOptions

            model.CustomOptionsImageWidth = _settingsTemplate.CustomOptionsImageWidth;
            model.CustomOptionsImageHeight = _settingsTemplate.CustomOptionsImageHeight;

            #endregion

            #region Other

            model.MainPageText = SettingsNews.MainPageText;

            model.OtherSettingsSections = _settingsTemplate.OtherSettingsSections;

            #endregion

            #region Css editor

            model.CssEditorText = new CssEditorHandler().GetFileContent();

            #endregion
            
            model.HiddenSettings = _settingsTemplate.HiddenSettings;

            return model;
        }
    }
}
