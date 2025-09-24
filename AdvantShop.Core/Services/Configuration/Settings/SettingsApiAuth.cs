using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsApiAuth
    {
        public static string TextOnMainTitle
        {
            get => SettingProvider.Items["Api_TextOnMain_Title"];
            set => SettingProvider.Items["Api_TextOnMain_Title"] = value;
        }
        
        public static string TextOnMainText
        {
            get => SettingProvider.Items["Api_TextOnMain_Text"];
            set => SettingProvider.Items["Api_TextOnMain_Text"] = value;
        }
        
        public static string ContactAddress
        {
            get => SettingProvider.Items["Api_Contact_Address"];
            set => SettingProvider.Items["Api_Contact_Address"] = value;
        }
        
        public static string ContactAddressMap
        {
            get => SettingProvider.Items["Api_Contact_Address_Map"];
            set => SettingProvider.Items["Api_Contact_Address_Map"] = value;
        }
        
        public static string ContactPhone
        {
            get => SettingProvider.Items["Api_Contact_Phone"];
            set => SettingProvider.Items["Api_Contact_Phone"] = value;
        }
        
        public static string ContactEmail
        {
            get => SettingProvider.Items["Api_Contact_Email"];
            set => SettingProvider.Items["Api_Contact_Email"] = value;
        }
        
        public static string ContactWorkSchedule
        {
            get => SettingProvider.Items["Api_Contact_WorkSchedule"];
            set => SettingProvider.Items["Api_Contact_WorkSchedule"] = value;
        }
        
        public static string ContactAboutCompany
        {
            get => SettingProvider.Items["Api_Contact_AboutCompany"];
            set => SettingProvider.Items["Api_Contact_AboutCompany"] = value;
        }
        
        
        public static MobileAppTheme MobileAppTheme
        {
            get => (MobileAppTheme) Convert.ToInt32(SettingProvider.Items["Api_MobileApp_Theme"]);
            set => SettingProvider.Items["Api_MobileApp_Theme"] = ((int)value).ToString();
        }
        
        public static string MobileAppMainColor
        {
            get => SettingProvider.Items["Api_MobileApp_MainColor"] ?? "#0088cc";
            set => SettingProvider.Items["Api_MobileApp_MainColor"] = value;
        }
        
        public static string MobileAppButtonTextColor
        {
            get => SettingProvider.Items["Api_MobileApp_ButtonTextColor"] ?? "fff";
            set => SettingProvider.Items["Api_MobileApp_ButtonTextColor"] = value;
        }
        
        public static bool MobileAppUseStoreColorScheme
        {
            get => Convert.ToBoolean(SettingProvider.Items["Api_MobileApp_UseStoreColorScheme"]);
            set => SettingProvider.Items["Api_MobileApp_UseStoreColorScheme"] = value.ToString();
        }
        
        public static bool ChangeHeaderBackgroundColor
        {
            get => Convert.ToBoolean(SettingProvider.Items["Api_MobileApp_ChangeHeaderBackgroundColor"]);
            set => SettingProvider.Items["Api_MobileApp_ChangeHeaderBackgroundColor"] = value.ToString();
        }
        
        public static string HeaderBackgroundColor
        {
            get => SettingProvider.Items["Api_MobileApp_HeaderBackgroundColor"] ?? "ffffff";
            set => SettingProvider.Items["Api_MobileApp_HeaderBackgroundColor"] = value;
        }
        
        public static string HeaderTextColor
        {
            get => SettingProvider.Items["Api_MobileApp_HeaderTextColor"];
            set => SettingProvider.Items["Api_MobileApp_HeaderTextColor"] = value;
        }
        
        public static bool ChangeBottomMenuBackgroundColor
        {
            get => Convert.ToBoolean(SettingProvider.Items["Api_MobileApp_ChangeBottomMenuBackgroundColor"]);
            set => SettingProvider.Items["Api_MobileApp_ChangeBottomMenuBackgroundColor"] = value.ToString();
        }
        
        public static string BottomMenuBackgroundColor
        {
            get => SettingProvider.Items["Api_MobileApp_BottomMenuBackgroundColor"] ?? "ffffff";
            set => SettingProvider.Items["Api_MobileApp_BottomMenuBackgroundColor"] = value;
        }
        
        public static string BottomMenuTextColor
        {
            get => SettingProvider.Items["Api_MobileApp_BottomMenuTextColor"];
            set => SettingProvider.Items["Api_MobileApp_BottomMenuTextColor"] = value;
        }
        
        public static string BottomMenuActiveItemColor
        {
            get => SettingProvider.Items["Api_MobileApp_BottomMenuActiveItemColor"];
            set => SettingProvider.Items["Api_MobileApp_BottomMenuActiveItemColor"] = value;
        }
        
        public static bool ChangeBackgroundColor
        {
            get => Convert.ToBoolean(SettingProvider.Items["Api_MobileApp_ChangeBackgroundColor"]);
            set => SettingProvider.Items["Api_MobileApp_ChangeBackgroundColor"] = value.ToString();
        }
        
        public static string BackgroundColor
        {
            get => SettingProvider.Items["Api_MobileApp_BackgroundColor"] ?? "f2f1f6";
            set => SettingProvider.Items["Api_MobileApp_BackgroundColor"] = value;
        }
        
        public static bool ChangeTileBackgroundColor
        {
            get => Convert.ToBoolean(SettingProvider.Items["Api_MobileApp_ChangeTileBackgroundColor"]);
            set => SettingProvider.Items["Api_MobileApp_ChangeTileBackgroundColor"] = value.ToString();
        }
        
        public static string TileBackgroundColor
        {
            get => SettingProvider.Items["Api_MobileApp_TileBackgroundColor"] ?? "ffffff";
            set => SettingProvider.Items["Api_MobileApp_TileBackgroundColor"] = value;
        }
        
        public static bool CarouselShowSectionInBottomNavigationMenu
        {
            get => SettingProvider.Items["Api_MobileApp_CarouselShowSectionInBottomNavigationMenu"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_CarouselShowSectionInBottomNavigationMenu"] = value.ToString();
        }
        
        
        public static string SignInPicture
        {
            get => SettingProvider.Items["Api_MobileApp_SignInPicture"];
            set => SettingProvider.Items["Api_MobileApp_SignInPicture"] = value;
        }
        
        public static string CartPicture
        {
            get => SettingProvider.Items["Api_MobileApp_CartPicture"];
            set => SettingProvider.Items["Api_MobileApp_CartPicture"] = value;
        }
        
        
        public static string MobileAppLogo
        {
            get => SettingProvider.Items["Api_MobileApp_MobileAppLogo"];
            set => SettingProvider.Items["Api_MobileApp_MobileAppLogo"] = value;
        }
        
        public static string MobileAppIcon
        {
            get => SettingProvider.Items["Api_MobileApp_MobileAppIcon"];
            set => SettingProvider.Items["Api_MobileApp_MobileAppIcon"] = value;
        }
        
        public static bool ShowRating
        {
            get => SettingProvider.Items["Api_MobileApp_ShowRating"].TryParseBool(true) ?? SettingsCatalog.EnableProductRating;
            set => SettingProvider.Items["Api_MobileApp_ShowRating"] = value.ToString();
        }
        
        public static bool ShowSku
        {
            get => SettingProvider.Items["Api_MobileApp_ShowSku"].TryParseBool(true) ?? SettingsCatalog.ShowProductArtNo;
            set => SettingProvider.Items["Api_MobileApp_ShowSku"] = value.ToString();
        }
        
        public static bool ShowCatalogFilter
        {
            get => SettingProvider.Items["Api_MobileApp_ShowCatalogFilter"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowCatalogFilter"] = value.ToString();
        }
        
        public static MobileAppMainPageMode MobileAppMainPageMode
        {
            get => (MobileAppMainPageMode) Convert.ToInt32(SettingProvider.Items["Api_MobileApp_MainPageMode"]);
            set => SettingProvider.Items["Api_MobileApp_MainPageMode"] = ((int)value).ToString();
        }
        
        public static bool ShowCatalogPanel
        {
            get => SettingProvider.Items["Api_MobileApp_ShowCatalogPanel"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowCatalogPanel"] = value.ToString();
        }
        
        public static bool ShowAvailability
        {
            get => SettingProvider.Items["Api_MobileApp_ShowAvailability"].TryParseBool(true) ?? SettingsCatalog.ShowAvaliableLableInProduct;
            set => SettingProvider.Items["Api_MobileApp_ShowAvailability"] = value.ToString();
        }

        public static bool ShowReviews
        {
            get => SettingProvider.Items["Api_MobileApp_ShowReviews"].TryParseBool(true) ?? SettingsCatalog.AllowReviews;
            set => SettingProvider.Items["Api_MobileApp_ShowReviews"] = value.ToString();
        }

        public static bool ShowCitySelection
        {
            get => SettingProvider.Items["Api_MobileApp_ShowCitySelection"].TryParseBool(true) ?? SettingsDesign.AutodetectCity;
            set => SettingProvider.Items["Api_MobileApp_ShowCitySelection"] = value.ToString();
        }
        
        public static bool ForceCitySelection
        {
            get => SettingProvider.Items["Api_MobileApp_ForceCitySelection"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ForceCitySelection"] = value.ToString();
        }
        
        public static bool ProhibitSelectingCitiesNotFromPresetList
        {
            get => SettingProvider.Items["Api_MobileApp_ProhibitSelectingCitiesNotFromPresetList"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ProhibitSelectingCitiesNotFromPresetList"] = value.ToString();
        }
        
        public static MobileAppProductViewMode MobileAppProductViewMode
        {
            get => (MobileAppProductViewMode) Convert.ToInt32(SettingProvider.Items["Api_MobileApp_ProductViewMode"]);
            set => SettingProvider.Items["Api_MobileApp_ProductViewMode"] = ((int)value).ToString();
        }
        
        public static bool ShowAmountSpinBoxInsteadOfAddToCart
        {
            get => SettingProvider.Items["Api_MobileApp_ShowAmountSpinBoxInsteadOfAddToCart"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowAmountSpinBoxInsteadOfAddToCart"] = value.ToString();
        }
        
        public static bool AppMetricaEnabled
        {
            get => SettingProvider.Items["Api_MobileApp_AppMetrica_Enabled"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_AppMetrica_Enabled"] = value.ToString();
        }
        
        public static string AppMetricaApiKey
        {
            get => SettingProvider.Items["Api_MobileApp_AppMetrica_ApiKey"];
            set => SettingProvider.Items["Api_MobileApp_AppMetrica_ApiKey"] = value;
        }
        
        public static string SmsRetrieverHashCode
        {
            get => SettingProvider.Items["Api_MobileApp_SmsRetrieverHashCode"];
            set => SettingProvider.Items["Api_MobileApp_SmsRetrieverHashCode"] = value;
        }
        
        public static bool ShowPostCode
        {
            get => SettingProvider.Items["Api_MobileApp_ShowPostCode"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowPostCode"] = value.ToString();
        }
        
        public static bool ShowLogoInCenter
        {
            get => SettingProvider.Items["Api_MobileApp_ShowLogoInCenter"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowLogoInCenter"] = value.ToString();
        }
        
        public static bool ShowBlockTitlesInProductDetails
        {
            get => SettingProvider.Items["Api_MobileApp_ShowBlockTitlesInProductDetails"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowBlockTitlesInProductDetails"] = value.ToString();
        }
        
        public static string OfficeAddressesBlockName
        {
            get => SettingProvider.Items["Api_MobileApp_OfficeAddressesBlockName"] ?? 
                   LocalizationService.GetResource("Api.OfficeAddressesBlockName");
            set => SettingProvider.Items["Api_MobileApp_OfficeAddressesBlockName"] = value;
        }

        public static bool ShowSearch
        {
            get => SettingProvider.Items["Api_MobileApp_ShowSearch"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowSearch"] = value.ToString();
        }
        
        public static bool ShowSearchByBarcode
        {
            get => SettingProvider.Items["Api_MobileApp_ShowSearchByBarcode"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowSearchByBarcode"] = value.ToString();
        }
        
        public static bool ShowActiveOrdersOnMainPage
        {
            get => SettingProvider.Items["Api_MobileApp_ShowActiveOrdersOnMainPage"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowActiveOrdersOnMainPage"] = value.ToString();
        }
        
        public static bool ShowWishList
        {
            get => SettingProvider.Items["Api_MobileApp_ShowWishList"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowWishList"] = value.ToString();
        }
        
        public static bool ShowDeliveriesInProductDetails
        {
            get => SettingProvider.Items["Api_MobileApp_ShowDeliveriesInProductDetails"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowDeliveriesInProductDetails"] = value.ToString();
        }
        
        public static bool CheckOccurrenceOfAddressInDeliveryArea
        {
            get => SettingProvider.Items["Api_MobileApp_CheckOccurrenceOfAddressInDeliveryArea"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_CheckOccurrenceOfAddressInDeliveryArea"] = value.ToString();
        }
        
        public static bool ShowPriceOnAddToCart
        {
            get => SettingProvider.Items["Api_MobileApp_ShowPriceOnAddToCart"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowPriceOnAddToCart"] = value.ToString();
        }
        
        public static bool ProductPhotosWithRoundedCorners
        {
            get => SettingProvider.Items["Api_MobileApp_ProductPhotosWithRoundedCorners"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ProductPhotosWithRoundedCorners"] = value.ToString();
        }
        
        public static bool ShowYouOrderedBlockOnMainPage
        {
            get => SettingProvider.Items["Api_MobileApp_ShowYouOrderedBlockOnMainPage"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowYouOrderedBlockOnMainPage"] = value.ToString();
        }

        public static MobileAppStoryViewMode StoryViewMode
        {
            get => (MobileAppStoryViewMode)Convert.ToInt32(SettingProvider.Items["Api_MobileApp_StoryViewMode"]);
            set => SettingProvider.Items["Api_MobileApp_StoryViewMode"] = ((int)value).ToString();
        }

        public static MobileAppStorySizeMode StorySizeMode
        {
            get => (MobileAppStorySizeMode)Convert.ToInt32(SettingProvider.Items["Api_MobileApp_StorySizeMode"]);
            set => SettingProvider.Items["Api_MobileApp_StorySizeMode"] = ((int)value).ToString();
        }
        
        public static bool ShowDeliveryInDeliveryWidgetOnMain
        {
            get => SettingProvider.Items["Api_MobileApp_ShowDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowDeliveryInDeliveryWidgetOnMain"] = value.ToString();
        }
        
        public static bool ShowSelfDeliveryInDeliveryWidgetOnMain
        {
            get => SettingProvider.Items["Api_MobileApp_ShowSelfDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowSelfDeliveryInDeliveryWidgetOnMain"] = value.ToString();
        }
        
        public static bool ShowInHouseInDeliveryWidgetOnMain
        {
            get => SettingProvider.Items["Api_MobileApp_ShowInHouseInDeliveryWidgetOnMain"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowInHouseInDeliveryWidgetOnMain"] = value.ToString();
        }
        
        public static string InHouseMethodName
        {
            get => SettingProvider.Items["Api_MobileApp_InHouseMethodName"] ?? 
                   LocalizationService.GetResource("Api.InHouseMethodName");
            set => SettingProvider.Items["Api_MobileApp_InHouseMethodName"] = value;
        }
        
        public static string InHouseMethodText
        {
            get => SettingProvider.Items["Api_MobileApp_InHouseMethodText"] ?? 
                   LocalizationService.GetResource("Api.InHouseMethodText");
            set => SettingProvider.Items["Api_MobileApp_InHouseMethodText"] = value;
        }

        public static List<int> InHouseShippingMethodIds
        {
            get
            {
                var ids = SettingProvider.Items["Api_MobileApp_InHouseShippingMethodIds"];
                if (string.IsNullOrEmpty(ids))
                    return new List<int>();

                return JsonConvert.DeserializeObject<List<int>>(ids);
            }
            set => SettingProvider.Items["Api_MobileApp_InHouseShippingMethodIds"] =
                value != null ? JsonConvert.SerializeObject(value) : null;
        }
        
        public static int? DefaultSelfDeliveryId
        {
            get => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryId"].TryParseInt(true);
            set => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryId"] = value.ToString();
        }
        
        public static int? DefaultSelfDeliveryPointMethodId
        {
            get => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointMethodId"].TryParseInt(true);
            set => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointMethodId"] = value.ToString();
        }
        
        [Obsolete("Use DefaultSelfDeliveryPointStringId. Used in mobile apps (~12.0.4)")]
        public static int? DefaultSelfDeliveryPointId
        {
            get => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointId"].TryParseInt(true);
            set => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointId"] = value.ToString();
        }
        
        public static string DefaultSelfDeliveryPointStringId
        {
            get => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointStringId"];
            set => SettingProvider.Items["Api_MobileApp_DefaultSelfDeliveryPointStringId"] = value ?? string.Empty;
        }
        
        public static MobileAppCategoryViewMode CategoryViewModeOnMainPage
        {
            get => (MobileAppCategoryViewMode) Convert.ToInt32(SettingProvider.Items["Api_MobileApp_CategoryViewModeOnMainPage"]);
            set => SettingProvider.Items["Api_MobileApp_CategoryViewModeOnMainPage"] = ((int)value).ToString();
        }
        
        public static bool ShowProductBriefDescriptionInCatalog
        {
            get => SettingProvider.Items["Api_MobileApp_ShowProductBriefDescriptionInCatalog"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowProductBriefDescriptionInCatalog"] = value.ToString();
        }
            
        public static int ProductsCountByCategoryForAllCategories
        {
            get => SettingProvider.Items["Api_MobileApp_ProductsCountByCategoryForAllCategories"].TryParseInt(SettingsCatalog.ProductsPerPage);
            set => SettingProvider.Items["Api_MobileApp_ProductsCountByCategoryForAllCategories"] = value.ToString();
        }
        
        public static MobileAppSalesMode? MobileAppSalesMode
        {
            get
            {
                var value = SettingProvider.Items["Api_MobileApp_SalesMode"];
                if (string.IsNullOrEmpty(value))
                    return null;
                return (MobileAppSalesMode) Convert.ToInt32(value);
            }
            set => SettingProvider.Items["Api_MobileApp_SalesMode"] = value != null ? ((int) value).ToString() : null;
        }
        
        public static bool ShowChoosingDeliveryWhenOpeningApplication
        {
            get => SettingProvider.Items["Api_MobileApp_ShowChoosingDeliveryWhenOpeningApplication"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowChoosingDeliveryWhenOpeningApplication"] = value.ToString();
        }
        
        public static bool ShowAuthScreenOnStart
        {
            get => SettingProvider.Items["Api_MobileApp_ShowAuthScreenOnStart"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_ShowAuthScreenOnStart"] = value.ToString();
        }
        
        public static bool TryGetNearestPickPoint
        {
            get => SettingProvider.Items["Api_MobileApp_TryGetNearestPickPoint"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_TryGetNearestPickPoint"] = value.ToString();
        }
        
        public static bool ShowBonusCardQrCode
        {
            get => SettingProvider.Items["Api_MobileApp_ShowBonusCardQrCode"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowBonusCardQrCode"] = value.ToString();
        }
        
        public static bool ShowFeedback
        {
            get => SettingProvider.Items["Api_ShowFeedback"].TryParseBool();
            set => SettingProvider.Items["Api_ShowFeedback"] = value.ToString();
        }
        
        public static int? FeedbackLeadFunnelId
        {
            get => SettingProvider.Items["Api_FeedbackLeadFunnelId"].TryParseInt(true);
            set => SettingProvider.Items["Api_FeedbackLeadFunnelId"] = value.ToString();
        }
        
        public static string FeedbackHintText
        {
            get => SettingProvider.Items["Api_FeedbackHintText"];
            set => SettingProvider.Items["Api_FeedbackHintText"] = value;
        }
        
        public static bool UserAddressRequired
        {
            get => SettingProvider.Items["Api_MobileApp_UserAddressRequired"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_UserAddressRequired"] = value.ToString();
        }
        
        public static bool ShowAddressInProfile
        {
            get => SettingProvider.Items["Api_MobileApp_ShowAddressInProfile"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["Api_MobileApp_ShowAddressInProfile"] = value.ToString();
        }
        
        public static bool DefineCityBasedOnGeodata
        {
            get => SettingProvider.Items["Api_MobileApp_DefineCityBasedOnGeodata"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_DefineCityBasedOnGeodata"] = value.ToString();
        }
        
        public static ColorsViewMode ColorsViewMode
        {
            get
            {
                var value = SettingProvider.Items["Api_MobileApp_ColorsViewMode"];
                if (value == null)
                    return SettingsCatalog.ColorsViewMode;

                return (ColorsViewMode)int.Parse(value);
            }
            set => SettingProvider.Items["Api_MobileApp_ColorsViewMode"] = ((int)value).ToString();
        }
        
        public static BonusCardQrCodeMode BonusCardQrCodeMode
        {
            get => (BonusCardQrCodeMode) Convert.ToInt32(SettingProvider.Items["Api_MobileApp_BonusCardQrCodeMode"]);
            set => SettingProvider.Items["Api_MobileApp_BonusCardQrCodeMode"] = ((int)value).ToString();
        }
        
        public static bool HideShoppingCart
        {
            get => SettingProvider.Items["Api_MobileApp_HideShoppingCart"].TryParseBool();
            set => SettingProvider.Items["Api_MobileApp_HideShoppingCart"] = value.ToString();
        }
    }

    public enum MobileAppTheme
    {
        [Localize("Core.SettingsApiAuth.MobileAppMainPageMode.Light")]
        Light = 0,
        
        [Localize("Core.SettingsApiAuth.MobileAppMainPageMode.Dark")]
        Dark = 1
    }
    
    public enum MobileAppMainPageMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppMainPageMode.MainCategories")]
        MainCategories = 0,
        
        [Localize("Core.SettingsApiAuth.MobileAppMainPageMode.AllCategoriesWithProducts")]
        AllCategoriesWithProducts = 1
    }
    
    public enum MobileAppProductViewMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppProductViewMode.Tile")]
        Tile = 0,
        
        [Localize("Core.SettingsApiAuth.MobileAppProductViewMode.List")]
        List = 1,
        
        [Localize("Core.SettingsApiAuth.MobileAppProductViewMode.Detail")]
        Detail = 2,
    }

    public enum MobileAppStoryViewMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppStoryViewMode.Circle")]
        Circle = 0,

        [Localize("Core.SettingsApiAuth.MobileAppStoryViewMode.Square")]
        Square = 1,
    }

    public enum MobileAppStorySizeMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppStorySizeMode.Middle")]
        Middle = 0,

        [Localize("Core.SettingsApiAuth.MobileAppStorySizeMode.Big")]
        Big = 1,
    }
    
    public enum MobileAppCategoryViewMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppCategoryViewMode.Tile")]
        Tile = 0,

        [Localize("Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithPhoto")]
        ListWithPhoto = 1,

        [Localize("Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithoutPhoto")]
        ListWithoutPhoto = 2,
    }

    public enum MobileAppSalesMode
    {
        [Localize("Core.SettingsApiAuth.MobileAppSalesMode.Shop")]
        Shop = 0,
        
        [Localize("Core.SettingsApiAuth.MobileAppSalesMode.Resto")]
        Resto = 1 
    }
    
    public enum BonusCardQrCodeMode
    {
        [Localize("Core.SettingsApiAuth.BonusCardQrCodeMode.BonusCardNumber")]
        BonusCardNumber = 0,
        
        [Localize("Core.SettingsApiAuth.BonusCardQrCodeMode.Phone")]
        Phone = 1
    }
}