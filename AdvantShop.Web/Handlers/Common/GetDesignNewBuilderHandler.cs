using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Design;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Models.Common;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.Common
{
    public class GetDesignNewBuilderHandler : ICommandHandler<DesignNewBuilderModel>
    {
        private SettingsTemplate _settingsTemplate;

        public GetDesignNewBuilderHandler() => _settingsTemplate = new SettingsTemplate();

        public DesignNewBuilderModel Execute()
        {
            var model = new DesignNewBuilderModel();
            var isDemoEnabled = Demo.IsDemoEnabled;

            #region Common

            var themeCookie = CommonHelper.GetCookieString(DesignService.TypeAndPath[eDesign.Theme]);
            model.CurrentTheme =
                isDemoEnabled && !string.IsNullOrWhiteSpace(themeCookie)
                        ? themeCookie
                        : SettingsDesign.Theme;
            model.Themes = DesignService.GetDesigns(eDesign.Theme);

            var backGroundCookie = CommonHelper.GetCookieString(DesignService.TypeAndPath[eDesign.Background]);
            model.CurrentBackGround =
                isDemoEnabled && !string.IsNullOrWhiteSpace(backGroundCookie)
                        ? backGroundCookie
                        : SettingsDesign.Background;
            model.Backgrounds = DesignService.GetDesigns(eDesign.Background);

            var colorSchemeCookie = CommonHelper.GetCookieString(DesignService.TypeAndPath[eDesign.Color]);
            model.CurrentColorScheme =
                isDemoEnabled && !string.IsNullOrWhiteSpace(colorSchemeCookie)
                        ? colorSchemeCookie
                        : SettingsDesign.ColorScheme;
            model.Colors = DesignService.GetDesigns(eDesign.Color);

            foreach (var color in model.Colors)
            {
                color.ColorCode = "#" + color.Color;
            }

            model.ColorSelected = model.Colors.FirstOrDefault(x => x.Name == model.CurrentColorScheme);

            var setting = _settingsTemplate.MainPageModeSetting;
            if (setting != null)
            {
                var mainPageModeCookie = CommonHelper.GetCookieString(DesignConstants.DemoCookie_Design_MainPageMode);
                model.MainPageMode = 
                    isDemoEnabled && !string.IsNullOrWhiteSpace(mainPageModeCookie)
                    ? mainPageModeCookie
                    : setting.Value;
                model.MainPageModeType = setting.DataType;

                if (setting.DataType == "ImageSelectList")
                    model.MainPageModeImageOptions = setting.Options.Select(x => new ImageSelectListOption(x)).ToList();
                else
                    model.MainPageModeOptions = _settingsTemplate.MainPageModeOptions;
            }
            
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

            var topPanelSetting = _settingsTemplate.TopPanelSetting;
            if (topPanelSetting != null)
            {
                model.TopPanel = topPanelSetting.Value;
                model.TopPanelOptions = topPanelSetting.Options.Select(x => new ImageSelectListOption(x)).ToList();
            }
            
            var headerSetting = _settingsTemplate.HeaderSetting;
            if (headerSetting != null)
            {
                model.Header = headerSetting.Value;
                model.HeaderOptions = headerSetting.Options.Select(x => new ImageSelectListOption(x)).ToList();
            }

            var topMenuSetting = _settingsTemplate.TopMenuSetting;
            if (topMenuSetting != null)
            {
                model.TopMenu = topMenuSetting.Value;
                model.TopMenuOptions = topMenuSetting.Options.Select(x => new ImageSelectListOption(x)).ToList();
            }

            model.RecentlyViewVisibility = _settingsTemplate.RecentlyViewVisibility;
            model.WishListVisibility = _settingsTemplate.WishListVisibility;
            // if (ModulesRepository.IsInstallModule("MobileApp"))
            // {
            //     model.MobileAppBannerVisibility = TryGetTemplateSettingValue("MobileAppBannerVisibility").TryParseBool();
            // }
            // else
            // {
            //     model.MobileAppBannerVisibility = null;
            // }

            model.IsStoreClosed = SettingsMain.IsStoreClosed;
            model.EnableInplace = SettingsMain.EnableInplace;
            model.DisplayToolBarBottom = SettingsDesign.DisplayToolBarBottom;
            model.AutodetectCity = SettingsDesign.AutodetectCity;
            model.HideCityInTopPanel = SettingsDesign.HideCityInTopPanel;
            
            if (SettingsDesign.DefaultCityIdIfNotAutodetect.HasValue)
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
            }

            model.AdditionalHeadMetaTag = SettingsSEO.CustomMetaString;

            model.ShowUserAgreementText = SettingsCheckout.IsShowUserAgreementTextValue;
            model.AgreementDefaultChecked = SettingsCheckout.AgreementDefaultChecked;
            model.UserAgreementText = SettingsCheckout.UserAgreementText;

            model.DisplayCityBubbleType = SettingsDesign.DisplayCityBubbleType;
            model.DisplayCityBubbleTypes = Enum.GetValues(typeof(SettingsDesign.EDisplayCityBubbleType))
                                                .Cast<SettingsDesign.EDisplayCityBubbleType>()
                                                .Select(x => new SelectListItem { Text = x.Localize(), Value = ((int)x).ToString() })
                                                .ToList();
            model.ShowCookiesPolicyMessage = SettingsNotifications.ShowCookiesPolicyMessage;
            model.CookiesPolicyMessage = SettingsNotifications.CookiesPolicyMessage;

            model.SiteLanguage = SettingsMain.Language;
            model.Languages = LanguageService.GetList().Select(x => new SelectListItem { Text = x.Name, Value = x.LanguageCode }).ToList();
            if (model.Languages.Count == 0)
                model.Languages.Add(new SelectListItem { Text = "Нет языков" });

            model.TopMenuVisibility = _settingsTemplate.TopMenuVisibility;

            model.ShowAdditionalPhones = _settingsTemplate.ShowAdditionalPhones;

            model.HideDisplayToolBarBottomOption = _settingsTemplate.HideDisplayToolBarBottomOption;

            model.CopyrightMode = SettingsDesign.ShowCustomCopyright ? "custom" : "default";

            model.CopyrightModeVariants = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = LocalizationService.GetResource("Admin.Settings.SystemSettings.CopyrightMode.Default"),
                    Value = "default",
                    Selected = !SettingsDesign.ShowCustomCopyright
                },
                new SelectListItem
                {
                    Text = LocalizationService.GetResource("Admin.Settings.SystemSettings.CopyrightMode.Custom"),
                    Value = "custom",
                    Selected = SettingsDesign.ShowCustomCopyright
                }
            };
            model.CopyrightText = SettingsDesign.CopyrightText;

            model.ShowUserAgreementForPromotionalNewsletter = SettingsDesign.ShowUserAgreementForPromotionalNewsletter;
            model.UserAgreementForPromotionalNewsletter = SettingsDesign.UserAgreementForPromotionalNewsletter;
            model.SetUserAgreementForPromotionalNewsletterChecked = SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked;

            model.UseAdaptiveRootCategory = SettingsCatalog.UseAdaptiveRootCategory;
            model.LimitedCategoryMenu = SettingsCatalog.LimitedCategoryMenu;
            
            model.ShowPriceInMiniCart = _settingsTemplate.ShowPriceInMiniCart;
            model.CartAddType = SettingsDesign.CartAddType;
            #endregion

            #region Main Page

            model.CarouselVisibility = _settingsTemplate.CarouselVisibility;
            model.CarouselAnimationSpeed = _settingsTemplate.CarouselAnimationSpeed;
            model.CarouselAnimationDelay = _settingsTemplate.CarouselAnimationDelay;

            model.MainPageProductsVisibility = _settingsTemplate.MainPageProductsVisibility;
            model.CountMainPageProductInSection = _settingsTemplate.CountMainPageProductInSection;
            model.CountMainPageProductInLine = _settingsTemplate.CountMainPageProductInLine;

            model.NewsVisibility = _settingsTemplate.NewsVisibility;
            model.NewsSubscriptionVisibility = _settingsTemplate.NewsSubscriptionVisibility;
            model.CheckOrderVisibility = _settingsTemplate.CheckOrderVisibility;
            model.GiftSertificateVisibility = _settingsTemplate.GiftSertificateVisibility;
            model.BrandCarouselVisibility = _settingsTemplate.BrandCarouselVisibility;

            model.MainPageCategoriesVisibility = _settingsTemplate.MainPageCategoriesVisibility;
            model.CountMainPageCategoriesInSection = _settingsTemplate.CountMainPageCategoriesInSection;
            model.CountMainPageCategoriesInLine = _settingsTemplate.CountMainPageCategoriesInLine;
            model.MainPageVisibleBriefDescription = SettingsMain.MainPageVisibleBriefDescription;

            model.MainPageProductReviewsVisibility = _settingsTemplate.MainPageProductReviewsVisibility;
            model.CountMainPageProductReviewsInSection = _settingsTemplate.CountMainPageProductReviewsInSection;
            model.CountMainPageProductReviewsInLine = _settingsTemplate.CountMainPageProductReviewsInLine;

            model.ReviewsSortingOnMainPage = SettingsCatalog.ReviewsSortingOnMainPage.ToString();
            model.ReviewsSortingOnMainPages = 
                Enum.GetValues(typeof(ReviewsSortingOnMainPage))
                .Cast<ReviewsSortingOnMainPage>()
                .Select(x => new SelectItemModel(x.Localize(), x.ToString()))
                .ToList();

            #endregion

            #region Catalog

            model.CountCategoriesInLine = _settingsTemplate.CountCategoriesInLine;

            model.ShowQuickView = SettingsCatalog.ShowQuickView;
            model.ProductsPerPage = SettingsCatalog.ProductsPerPage;
            model.CatalogVisibleBriefDescription = SettingsCatalog.CatalogVisibleBriefDescription;
            model.CountCatalogProductInLine = _settingsTemplate.CountCatalogProductInLine;
            model.ShowProductsCount = SettingsCatalog.ShowProductsCount;
            model.DisplayCategoriesInBottomMenu = SettingsCatalog.DisplayCategoriesInBottomMenu;
            model.ShowProductArtNo = SettingsCatalog.ShowProductArtNo;
            model.EnableProductRating = SettingsCatalog.EnableProductRating;
            model.EnableCompareProducts = SettingsCatalog.EnableCompareProducts;
            model.EnablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews;
            model.ShowSpinboxInCatalog = SettingsCatalog.ShowSpinboxInCatalog;
            model.ShowCountPhoto = SettingsCatalog.ShowCountPhoto;
            model.ShowOnlyAvalible = SettingsCatalog.ShowOnlyAvalible;
            model.MoveNotAvaliableToEnd = SettingsCatalog.MoveNotAvaliableToEnd;
            model.ShowNotAvaliableLable = SettingsCatalog.ShowNotAvaliableLable;

            model.FilterVisibility = SettingsDesign.FilterVisibility;
            model.ShowPriceFilter = SettingsCatalog.ShowPriceFilter;
            model.ShowProducerFilter = SettingsCatalog.ShowProducerFilter;
            model.ShowSizeFilter = SettingsCatalog.ShowSizeFilter;
            model.ShowColorFilter = SettingsCatalog.ShowColorFilter;
            model.ShowWarehouseFilter = SettingsCatalog.ShowWarehouseFilter;
            model.ShowPropertiesFilterInProductList = SettingsCatalog.ShowPropertiesFilterInProductList;
            model.ShowPropertiesFilterInParentCategories = SettingsCatalog.ShowPropertiesFilterInParentCategories;
            model.ExcludingFilters = SettingsCatalog.ExcludingFilters;

            model.SizesHeader = SettingsCatalog.SizesHeader;
            model.ColorsHeader = SettingsCatalog.ColorsHeader;
            model.ColorsControlType = SettingsDesign.ColorsControlType.ToString();
            model.ColorsControlTypes = new List<SelectListItem>();
            foreach (SettingsDesign.eSizeColorControlType item in Enum.GetValues(typeof(SettingsDesign.eSizeColorControlType)))
            {
                model.ColorsControlTypes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.SizesControlType = SettingsDesign.SizesControlType.ToString();
            model.SizesControlTypes = new List<SelectListItem>();
            foreach (SettingsDesign.eSizeColorControlType item in Enum.GetValues(typeof(SettingsDesign.eSizeColorControlType)))
            {
                model.SizesControlTypes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.ColorsViewMode = SettingsCatalog.ColorsViewMode.ToString();
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

            model.DefaultCatalogView = SettingsCatalog.DefaultCatalogView.ToString();
            model.EnableCatalogViewChange = SettingsCatalog.EnabledCatalogViewChange;
            model.DefaultSearchView = SettingsCatalog.DefaultSearchView.ToString();
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
            model.ShowAvailableLableInProduct = SettingsCatalog.ShowAvaliableLableInProduct;
            model.ShowStockAvailability = SettingsCatalog.ShowStockAvailability;
            model.ShowProductArtNoOnProductCard = SettingsCatalog.ShowProductArtNoOnProductCard;
            model.ShowNotAvailableLableInProduct = SettingsCatalog.ShowNotAvaliableLableInProduct;
            model.ShowAvailableInWarehouseInProduct = SettingsCatalog.ShowAvailableInWarehouseInProduct;
            model.ShowOnlyAvailableWarehousesInProduct = SettingsCatalog.ShowOnlyAvailableWarehousesInProduct;
            model.YaMapsApiKeyFoMapWarehouse = SettingsCatalog.YaMapsApiKeyFoMapWarehouse;

            //model.CompressBigImage = SettingsCatalog.CompressBigImage;
            model.EnableZoom = SettingsDesign.EnableZoom;

            model.AllowReviews = SettingsCatalog.AllowReviews;
            model.ModerateReviews = SettingsCatalog.ModerateReviews;
            model.ReviewsVoiteOnlyRegisteredUsers = SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers;
            model.DisplayReviewsImage = SettingsCatalog.DisplayReviewsImage;
            model.AllowReviewsImageUploading = SettingsCatalog.AllowReviewsImageUploading;
            model.ReviewImageWidth = SettingsPictureSize.ReviewImageWidth;
            model.ReviewImageHeight = SettingsPictureSize.ReviewImageHeight;
            model.WhoAllowReviewsOption = new List<SelectListItem>();
            foreach (SettingsDesign.eWhoAllowReviews item in Enum.GetValues(typeof(SettingsDesign.eWhoAllowReviews)))
            {
                model.WhoAllowReviewsOption.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.WhoAllowReviews = SettingsDesign.WhoAllowReviews;
            model.ShowShippingsMethodsInDetails = SettingsDesign.ShowShippingsMethodsInDetails.ToString();
            model.ShowShippingsMethods = new List<SelectListItem>();
            foreach (SettingsDesign.eShowShippingsInDetails item in Enum.GetValues(typeof(SettingsDesign.eShowShippingsInDetails)))
            {
                model.ShowShippingsMethods.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.ShippingsMethodsInDetailsCount = SettingsDesign.ShippingsMethodsInDetailsCount;

            model.RelatedProductName = SettingsCatalog.RelatedProductName;
            model.AlternativeProductName = SettingsCatalog.AlternativeProductName;
            model.RelatedProductSourceType = SettingsDesign.RelatedProductSourceType.ToString();
            model.SimilarProductSourceType = SettingsDesign.SimilarProductSourceType.ToString();
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
            model.NewsMainPageText = SettingsNews.MainPageText;
            model.NewsPerPage = SettingsNews.NewsPerPage;
            model.NewsMainPageCount = SettingsNews.NewsMainPageCount;

            #endregion

            #region CssEditor
            model.CssEditorText = FilePath.FoldersHelper.ReadCss(FilePath.CssType.extra);
            #endregion

            #region Other

            model.OtherSettingsSections = _settingsTemplate.OtherSettingsSections;

            //model.OtherSettingsDic = new Dictionary<string, List<TemplateSetting>>();

            //foreach (var section in model.OtherSettingsSections)
            //{
            //    model.OtherSettingsDic.Add(section.Name, section.Settings);
            //}


            model.ShowOtherSettings = model.OtherSettingsSections != null; //&& model.OtherSettingsSections.Any(x => x.IsOther);

            #endregion


            var additionalPhones = new GetAdditionalPhones(true).Execute();
            model.AdditionalPhones = additionalPhones.Phones;
            model.AdditionalPhoneTypes = additionalPhones.Types;
            
            model.HiddenSettings = _settingsTemplate.HiddenSettings;

            return model;
        }
    }
}