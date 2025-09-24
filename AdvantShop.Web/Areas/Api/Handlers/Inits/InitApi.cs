using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Areas.Api.Handlers.Modules;
using AdvantShop.Areas.Api.Handlers.Settings;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Areas.Api.Models.Inits;
using AdvantShop.Areas.Api.Models.Settings;
using AdvantShop.Areas.Api.Models.Shared;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Core.Services.Shop;
using AdvantShop.Customers;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Shipping.PointDelivery;
using AdvantShop.Track;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Inits
{
    public sealed class InitApi : AbstractCommandHandler<InitApiResponse>
    {
        private readonly InitApiModel _model;

        private readonly List<string> _defaultSettings = new List<string>()
        {
            "ShopName",
            "ShopURL",
            "ProductsPerPage",
            "SizesHeader",
            "ColorsHeader",
            "BuyButtonText",
            "Phone",
            "MobilePhone"
        };

        private readonly List<string> _stopSettings = new List<string>() {"LicKey"};

        public InitApi(InitApiModel model)
        {
            _model = model;
        }
        
        protected override InitApiResponse Handle()
        {
            var customer = CustomerContext.CurrentCustomer;
            var currency = CurrencyService.CurrentCurrency;

            var result = new InitApiResponse()
            {
                Customer = new GetCustomerResponse(customer),
                Currency = new CurrencyApi(currency),
                Location = IpZoneContext.CurrentZone,
                Settings = GetSettings(),
                Dadata = new GetDadataSettings().Execute(),
                Contact = new ContactInformation(),
                Carousel = CarouselApiService.GetListByExpirationDate()
                                             .Select(x => new CarouselItem(x))
                                             .ToList(),
                Products = new MainPageProductsInit()
                {
                    YouOrderedProducts =
                        customer.RegistredUser && SettingsApiAuth.ShowYouOrderedBlockOnMainPage
                            ? new GetYouOrderedProducts().Execute()
                            : null
                },
                Stories = new GetStories().Execute(),
                OptionalSettings = 
                    AttachedModules.GetModuleInstances<IApiSettings>()?
                        .SelectMany(x => x.GetSettings())
                        .Where(x => x != null)
                        .ToList(),
            };

            TrackEvents();

            return result;
        }

        private SettingsResponse GetSettings()
        {
            var items = new List<SettingItemResponse>();

            foreach (var setting in _defaultSettings)
            {
                items.Add(new SettingItemResponse(setting, SettingProvider.Items[setting]));
            }

            if (!string.IsNullOrWhiteSpace(_model.Settings))
            {
                items.AddRange(
                    _model.Settings.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !_defaultSettings.Contains(x, StringComparer.OrdinalIgnoreCase) && 
                                         !_stopSettings.Contains(x, StringComparer.OrdinalIgnoreCase))
                        .Select(x => new SettingItemResponse(x, SettingProvider.Items[x]))
                );
            }

            var logo = !string.IsNullOrEmpty(SettingsMain.LogoImageName)
                ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false)
                : null;
            
            items.Add(new SettingItemResponse("Logo", logo));

            var colorScheme = DesignService.GetCurrenDesign(eDesign.Color);
            
            items.Add(new SettingItemResponse("ColorScheme", colorScheme?.Name));
            items.Add(new SettingItemResponse("ColorSchemesColor", colorScheme?.Color));
            items.Add(new SettingItemResponse("ColorSchemesButtonTextColor", colorScheme?.ButtonTextColor ?? "fff"));
            
            items.Add(new SettingItemResponse("MobileAppTheme", SettingsApiAuth.MobileAppTheme.ToString().ToCamelCase()));
            items.Add(new SettingItemResponse("MobileAppUseStoreColorScheme", SettingsApiAuth.MobileAppUseStoreColorScheme));
            items.Add(new SettingItemResponse("MobileAppMainColor", SettingsApiAuth.MobileAppMainColor));
            items.Add(new SettingItemResponse("MobileAppButtonTextColor", SettingsApiAuth.MobileAppButtonTextColor));

            var changeBackgroundColor = SettingsApiAuth.ChangeBackgroundColor;
            
            items.Add(new SettingItemResponse("ChangeBackgroundColor", changeBackgroundColor));
            items.Add(new SettingItemResponse("BackgroundColor", changeBackgroundColor ? SettingsApiAuth.BackgroundColor : null));
            
            var changeHeaderBackgroundColor = SettingsApiAuth.ChangeHeaderBackgroundColor;
            
            items.Add(new SettingItemResponse("ChangeHeaderBackgroundColor", changeHeaderBackgroundColor));
            items.Add(new SettingItemResponse("HeaderBackgroundColor", changeHeaderBackgroundColor ? SettingsApiAuth.HeaderBackgroundColor : null));
            items.Add(new SettingItemResponse("HeaderTextColor", changeHeaderBackgroundColor ? SettingsApiAuth.HeaderTextColor : null));
            
            var changeBottomMenuBackgroundColor = SettingsApiAuth.ChangeBottomMenuBackgroundColor;
            
            items.Add(new SettingItemResponse("ChangeBottomMenuBackgroundColor", changeBottomMenuBackgroundColor));
            items.Add(new SettingItemResponse("BottomMenuBackgroundColor", changeBottomMenuBackgroundColor ? SettingsApiAuth.BottomMenuBackgroundColor : null));
            items.Add(new SettingItemResponse("BottomMenuTextColor", changeBottomMenuBackgroundColor ? SettingsApiAuth.BottomMenuTextColor : null));
            items.Add(new SettingItemResponse("BottomMenuActiveItemColor", changeBottomMenuBackgroundColor ? SettingsApiAuth.BottomMenuActiveItemColor : null));
            
            var changeTileBackgroundColor = SettingsApiAuth.ChangeTileBackgroundColor;
            
            items.Add(new SettingItemResponse("ChangeTileBackgroundColor", changeTileBackgroundColor));
            items.Add(new SettingItemResponse("TileBackgroundColor", changeTileBackgroundColor ? SettingsApiAuth.TileBackgroundColor : null));
            
            items.Add(new SettingItemResponse("ShowBonusCardQrCode", SettingsApiAuth.ShowBonusCardQrCode));
            items.Add(new SettingItemResponse("BonusCardQrCodeMode", SettingsApiAuth.ShowBonusCardQrCode ? SettingsApiAuth.BonusCardQrCodeMode.ToString().ToCamelCase() : null));
            
            items.Add(new SettingItemResponse("HideShoppingCart", SettingsApiAuth.HideShoppingCart));
            
            items.Add(new SettingItemResponse("MobileAppSignInPicture", 
                !string.IsNullOrEmpty(SettingsApiAuth.SignInPicture)
                    ? FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsApiAuth.SignInPicture, false)
                    : null));
            items.Add(new SettingItemResponse("MobileAppCartPicture", 
                !string.IsNullOrEmpty(SettingsApiAuth.CartPicture)
                    ? FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsApiAuth.CartPicture, false)
                    : null));
            
            
            items.Add(new SettingItemResponse("MobileAppLogo", 
                !string.IsNullOrEmpty(SettingsApiAuth.MobileAppLogo)
                    ? FoldersHelper.GetPath(FolderType.MobileAppIcon, SettingsApiAuth.MobileAppLogo, false)
                    : null));
            
            items.Add(new SettingItemResponse("ShowRating", SettingsApiAuth.ShowRating));
            items.Add(new SettingItemResponse("ShowSku", SettingsApiAuth.ShowSku));
            items.Add(new SettingItemResponse("ShowCatalogFilter", SettingsApiAuth.ShowCatalogFilter));
            items.Add(new SettingItemResponse("MobileAppMainPageMode", SettingsApiAuth.MobileAppMainPageMode.ToString().ToCamelCase()));
            items.Add(new SettingItemResponse("CategoryViewModeOnMainPage", SettingsApiAuth.CategoryViewModeOnMainPage.ToString().ToCamelCase()));
            
            items.Add(new SettingItemResponse("ShowCatalogPanel", SettingsApiAuth.ShowCatalogPanel));
            items.Add(new SettingItemResponse("ShowAvailability", SettingsApiAuth.ShowAvailability));
            items.Add(new SettingItemResponse("ShowReviews", SettingsApiAuth.ShowReviews));
            items.Add(new SettingItemResponse("ShowCitySelection", SettingsApiAuth.ShowCitySelection));
            items.Add(new SettingItemResponse("ForceCitySelection", SettingsApiAuth.ForceCitySelection));
            items.Add(new SettingItemResponse("ProhibitSelectingCitiesNotFromPresetList", SettingsApiAuth.ProhibitSelectingCitiesNotFromPresetList));
            items.Add(new SettingItemResponse("ShowAuthScreenOnStart", SettingsApiAuth.ShowAuthScreenOnStart));
            items.Add(new SettingItemResponse("MobileAppProductViewMode", SettingsApiAuth.MobileAppProductViewMode.ToString().ToCamelCase()));
            
            items.Add(new SettingItemResponse("ShowProductBriefDescriptionInCatalog", SettingsApiAuth.ShowProductBriefDescriptionInCatalog));
            items.Add(new SettingItemResponse("ProductsCountByCategoryForAllCategories", SettingsApiAuth.ProductsCountByCategoryForAllCategories));
            
            items.Add(new SettingItemResponse("ShowAmountSpinBoxInsteadOfAddToCart", SettingsApiAuth.ShowAmountSpinBoxInsteadOfAddToCart));
            
            items.Add(new SettingItemResponse("ShowPriceOnAddToCart", SettingsApiAuth.ShowPriceOnAddToCart));
            items.Add(new SettingItemResponse("ProductPhotosWithRoundedCorners", SettingsApiAuth.ProductPhotosWithRoundedCorners));
            
            items.Add(new SettingItemResponse("ShowPostCode", SettingsApiAuth.ShowPostCode));
            items.Add(new SettingItemResponse("ShowLogoInCenter", SettingsApiAuth.ShowLogoInCenter));
            items.Add(new SettingItemResponse("ShowBlockTitlesInProductDetails", SettingsApiAuth.ShowBlockTitlesInProductDetails));
            items.Add(new SettingItemResponse("OfficeAddressesBlockName", SettingsApiAuth.OfficeAddressesBlockName));
            items.Add(new SettingItemResponse("ShowSearch", SettingsApiAuth.ShowSearch));
            items.Add(new SettingItemResponse("ShowSearchByBarcode", SettingsApiAuth.ShowSearchByBarcode));
            items.Add(new SettingItemResponse("ShowActiveOrdersOnMainPage", SettingsApiAuth.ShowActiveOrdersOnMainPage));
            items.Add(new SettingItemResponse("ShowYouOrderedBlockOnMainPage", SettingsApiAuth.ShowYouOrderedBlockOnMainPage));
            items.Add(new SettingItemResponse("ShowWishList", SettingsApiAuth.ShowWishList));
            items.Add(new SettingItemResponse("ShowDeliveriesInProductDetails", SettingsApiAuth.ShowDeliveriesInProductDetails));
            items.Add(new SettingItemResponse("CheckOccurrenceOfAddressInDeliveryArea", SettingsApiAuth.CheckOccurrenceOfAddressInDeliveryArea));

            items.Add(new SettingItemResponse("DisplayReviewsImage", SettingsCatalog.DisplayReviewsImage));
            items.Add(new SettingItemResponse("AllowReviewsImageUploading", SettingsCatalog.AllowReviewsImageUploading));
            items.Add(new SettingItemResponse("ModerateReviews", SettingsCatalog.ModerateReviews));
            items.Add(new SettingItemResponse("ReviewImageWidth", SettingsPictureSize.ReviewImageWidth));
            items.Add(new SettingItemResponse("ReviewImageHeight", SettingsPictureSize.ReviewImageHeight));
            
            items.Add(new SettingItemResponse("TextOnMainTitle", SettingsApiAuth.TextOnMainTitle));
            items.Add(new SettingItemResponse("TextOnMainText", SettingsApiAuth.TextOnMainText));
            items.Add(new SettingItemResponse("CarouselShowSectionInBottomNavigationMenu", SettingsApiAuth.CarouselShowSectionInBottomNavigationMenu));

            items.Add(new SettingItemResponse("IsPhysicalEntityEnabled", SettingsCustomers.IsRegistrationAsPhysicalEntity));
            items.Add(new SettingItemResponse("IsLegalEntityEnabled", SettingsCustomers.IsRegistrationAsLegalEntity));
            
            items.Add(new SettingItemResponse("ShowUserAgreementForPromotionalNewsletter", SettingsDesign.ShowUserAgreementForPromotionalNewsletter));
            items.Add(new SettingItemResponse("SetUserAgreementForPromotionalNewsletterChecked", SettingsDesign.SetUserAgreementForPromotionalNewsletterChecked));
            items.Add(new SettingItemResponse("UserAgreementForPromotionalNewsletter", SettingsDesign.UserAgreementForPromotionalNewsletter));
            
            items.Add(new SettingItemResponse("AppMetricaEnabled", SettingsApiAuth.AppMetricaEnabled));
            items.Add(new SettingItemResponse("AppMetricaApiKey", SettingsApiAuth.AppMetricaApiKey));

            items.Add(new SettingItemResponse("StoryViewMode", SettingsApiAuth.StoryViewMode.ToString().ToCamelCase()));
            items.Add(new SettingItemResponse("StorySizeMode", SettingsApiAuth.StorySizeMode.ToString().ToCamelCase()));
            
            
            items.Add(new SettingItemResponse("ShowDeliveryInDeliveryWidgetOnMain", SettingsApiAuth.ShowDeliveryInDeliveryWidgetOnMain));
            var showSelfDeliveryInDeliveryWidgetOnMain = SettingsApiAuth.ShowSelfDeliveryInDeliveryWidgetOnMain;
            items.Add(new SettingItemResponse("ShowSelfDeliveryInDeliveryWidgetOnMain", showSelfDeliveryInDeliveryWidgetOnMain));
            items.Add(new SettingItemResponse("ShowInHouseInDeliveryWidgetOnMain", SettingsApiAuth.ShowInHouseInDeliveryWidgetOnMain));
            items.Add(new SettingItemResponse("InHouseMethodName", SettingsApiAuth.InHouseMethodName));
            items.Add(new SettingItemResponse("InHouseMethodText", SettingsApiAuth.InHouseMethodText));
            
            items.Add(new SettingItemResponse("DefaultSelfDeliveryId", 
                                                showSelfDeliveryInDeliveryWidgetOnMain 
                                                    ? SettingsApiAuth.DefaultSelfDeliveryId 
                                                    : default(int?)));
            items.Add(new SettingItemResponse("DefaultSelfDeliveryPointMethodId", 
                                                showSelfDeliveryInDeliveryWidgetOnMain 
                                                    ? SettingsApiAuth.DefaultSelfDeliveryPointMethodId 
                                                    : default(int?)));
            items.Add(new SettingItemResponse("DefaultSelfDeliveryPointStringId", 
                                                showSelfDeliveryInDeliveryWidgetOnMain 
                                                    ? SettingsApiAuth.DefaultSelfDeliveryPointStringId 
                                                    : null));
            items.Add(new SettingItemResponse("DefaultSelfDeliveryName", showSelfDeliveryInDeliveryWidgetOnMain ? GetDefaultSelfDeliveryName() : null));
            
            items.Add(new SettingItemResponse("ShowChoosingDeliveryWhenOpeningApplication", SettingsApiAuth.ShowChoosingDeliveryWhenOpeningApplication));
            items.Add(new SettingItemResponse("TryGetNearestPickPoint", SettingsApiAuth.TryGetNearestPickPoint));
            
            
            var sellerCountryId = SettingsMain.SellerCountryId;
            var sellerCountry = CountryService.GetCountry(sellerCountryId);
            
            items.Add(new SettingItemResponse("SellerCountryId", sellerCountryId));
            items.Add(new SettingItemResponse("SellerCountryName", sellerCountry?.Name));
            items.Add(new SettingItemResponse("SellerCountryDialCode", sellerCountry?.DialCode));
            items.Add(new SettingItemResponse("SellerCountryIso2", sellerCountry?.Iso2));
            items.Add(new SettingItemResponse("SellerCountryIso3", sellerCountry?.Iso3));
            items.Add(new SettingItemResponse("AllowOrderCheckout", ShopService.AllowCheckoutNow()));
            items.Add(new SettingItemResponse("OrderCheckoutDisabledMessage", SettingsCheckout.NotAllowCheckoutText));

            
            items.Add(new SettingItemResponse("DefaultSelfDeliveryPointId", 
                showSelfDeliveryInDeliveryWidgetOnMain 
                    ? SettingsApiAuth.DefaultSelfDeliveryPointId 
                    : default(int?)));
            
            items.Add(new SettingItemResponse("ShowAvailableInWarehouseInProduct", SettingsCatalog.ShowAvailableInWarehouseInProduct));
            
            items.Add(new SettingItemResponse("ShowFeedback", SettingsApiAuth.ShowFeedback));
            items.Add(new SettingItemResponse("FeedbackLeadFunnelId", SettingsApiAuth.FeedbackLeadFunnelId));
            items.Add(new SettingItemResponse("FeedbackHintText", SettingsApiAuth.FeedbackHintText));
            
            items.Add(new SettingItemResponse("RegistrationIsProhibited", SettingsMain.RegistrationIsProhibited));

            var bookingTablesModuleActive = AttachedModules.GetModuleById("BookingTables", true) != null;
            items.Add(new SettingItemResponse("BookingTablesModuleActive", bookingTablesModuleActive));
            
            items.Add(new SettingItemResponse("UserAddressRequired", SettingsApiAuth.UserAddressRequired));
            items.Add(new SettingItemResponse("ShowAddressInProfile", SettingsApiAuth.ShowAddressInProfile));
            
            items.Add(new SettingItemResponse("ActiveWarehouseCount", WarehouseService.GetList(true).Count));
            
            items.Add(new SettingItemResponse("DefineCityBasedOnGeodata", SettingsApiAuth.DefineCityBasedOnGeodata));

            var defaultCityIdIfNotAutodetect = SettingsDesign.DefaultCityIdIfNotAutodetect;

            var cityIfNotAutodetect =
                defaultCityIdIfNotAutodetect != null
                    ? CityService.GetCity(defaultCityIdIfNotAutodetect.Value)
                    : null;

            var regionIfNotAutodetect =
                cityIfNotAutodetect != null
                    ? RegionService.GetRegion(cityIfNotAutodetect.RegionId)
                    : null;

            var countryIfNotAutodetect =
                regionIfNotAutodetect != null
                    ? CountryService.GetCountry(regionIfNotAutodetect.CountryId)
                    : null;
            
            items.Add(new SettingItemResponse("CityIdIfNotAutodetect", cityIfNotAutodetect?.CityId));
            items.Add(new SettingItemResponse("CityNameIfNotAutodetect", cityIfNotAutodetect?.Name));
            
            items.Add(new SettingItemResponse("RegionIdIfNotAutodetect", regionIfNotAutodetect?.RegionId));
            items.Add(new SettingItemResponse("RegionNameIfNotAutodetect", regionIfNotAutodetect?.Name));
            
            items.Add(new SettingItemResponse("CountryIdIfNotAutodetect", countryIfNotAutodetect?.CountryId));
            items.Add(new SettingItemResponse("CountryNameIfNotAutodetect", countryIfNotAutodetect?.Name));
            
            items.Add(new SettingItemResponse("ColorsViewMode", SettingsApiAuth.ColorsViewMode.ToString().ToCamelCase()));
            
            items.Add(new SettingItemResponse("ShowPriceRuleAmountTableInProduct", SettingsPriceRules.ShowAmountsTableInProduct));
            
            items.Add(new SettingItemResponse("AuthByCodeActive", SettingsOAuth.AuthByCodeActive));
            items.Add(new SettingItemResponse("AuthByCodeMethod", SettingsOAuth.AuthByCodeMethod.ToString()));
            items.Add(new SettingItemResponse("AuthCallMode", 
                SettingsOAuth.AuthByCodeMethod == EAuthByCodeMode.Call 
                    ? SettingsAuthCall.AuthCallMode.ToString()
                    : null));
            items.Add(new SettingItemResponse("AuthByCodeMethodDescription", new PhoneConfirmationService().GetHintDescription()));

            return new SettingsResponse(items);
        }

        private string GetDefaultSelfDeliveryName()
        {
            try
            {
                if (SettingsApiAuth.DefaultSelfDeliveryId != null)
                {
                    var method = ShippingMethodService.GetShippingMethod(SettingsApiAuth.DefaultSelfDeliveryId.Value);
                    return method?.Name;
                }
            
                if (SettingsApiAuth.DefaultSelfDeliveryPointMethodId != null && 
                    SettingsApiAuth.DefaultSelfDeliveryPointStringId.IsNotEmpty())
                {
                    var name = CacheManager.Get("CachedPointDeliveryName", 3, () =>
                    {
                        var method =
                            ShippingMethodService.GetShippingMethod(SettingsApiAuth.DefaultSelfDeliveryPointMethodId.Value);
                        
                        if (method == null) 
                            return "";

                        var shippingParams = new ShippingCalculationParameters()
                        {
                            IsFromAdminArea = true,
                            PreOrderItems = new List<PreOrderItem>(),
                            Currency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3) ??
                                       CurrencyService.BaseCurrency
                        };
                        var option =
                            new PointDelivery(method, shippingParams)
                                .CalculateOptions(CalculationVariants.All)
                                .SelectMany(x => ((PointDeliveryOption) x).ShippingPoints)
                                .FirstOrDefault(x => x.Id == SettingsApiAuth.DefaultSelfDeliveryPointStringId);

                        return option?.Address ?? "";
                    });

                    return !string.IsNullOrEmpty(name) ? name : null;
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            
            return null;
        }

        private void TrackEvents()
        {
            if (_model.Device.IsNotEmpty())
                TrackService.TrackEvent(ETrackEvent.MobileApp_Device, _model.Device);
            
            if (_model.Os.IsNotEmpty())
                TrackService.TrackEvent(ETrackEvent.MobileApp_Device, _model.Os);
            
            if (_model.AppVersion.IsNotEmpty())
                TrackService.TrackEvent(ETrackEvent.MobileApp_Device, _model.AppVersion);

            if (_model.Device.IsNotEmpty() ||
                _model.Os.IsNotEmpty() ||
                _model.AppVersion.IsNotEmpty())
            {
                var ip = HttpContext.Current.TryGetIp();
                if (ip.IsNotEmpty() && ip != "127.0.0.1" && ip != "::1")
                {
                    TrackService.TrackEvent(ETrackEvent.MobileApp_Ip, ip);
                }
            }
        }
    }
}