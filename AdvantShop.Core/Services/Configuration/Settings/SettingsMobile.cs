//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Design;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsMobile
    {
        public enum eBrowserColorVariants
        {
            [Localize("Admin.Settings.Mobile.BrowserColorVariantsNotSet")]
            None = 0,
            [Localize("Admin.Settings.Mobile.BrowserColorVariantsAsColorScheme")]
            ColorScheme = 1,
            [Localize("Admin.Settings.Mobile.BrowserColorVariantsCustomColor")]
            CustomColor = 2
        }

        public enum eHeaderColorVariants
        {
            [Localize("Admin.Settings.Mobile.HeaderColorVariantAsColorScheme")]
            ColorScheme = 0,
            [Localize("Admin.Settings.Mobile.HeaderColorVariantWhite")]
            White = 1
        }

        public enum eViewCategoriesOnMain
        {
            [Localize("Admin.Settings.Mobile.ViewCategoriesOnMainNotOutput")]
            None = 0,
            [Localize("Admin.Settings.Mobile.ViewCategoriesOnMainWithoutIcons")]
            Default = 1,
            [Localize("Admin.Settings.Mobile.ViewCategoriesOnMainWithIcons")]
            WithIcons = 2,
            [Localize("Admin.Settings.Mobile.ViewCategoriesOnMainBlocksMode")]
            BlocksMode = 3,
        }

        public enum eLogoType
        {
            [Localize("Admin.Settings.Mobile.LogoTypeText")]
            Text = 0,
            [Localize("Admin.Settings.Mobile.LogoTypeFromDesktop")]
            Desktop = 1,
            [Localize("Admin.Settings.Mobile.LogoTypeCustom")]
            Mobile = 2,
        }


        public enum eCatalogMenuViewMode
        {
            [Localize("Admin.Settings.Mobile.CatalogMenuViewModeRootCategories")]
            RootCategories = 0,
            [Localize("Admin.Settings.Mobile.CatalogMenuViewModeLink")]
            Link = 1
        }

        public enum eBottomPanelCatalogMenuMode
        {
            [Localize("Admin.Settings.Mobile.BottomPanelCatalogMenuModeRootCategories")]
            RootCategories = 0,
            [Localize("Admin.Settings.Mobile.BottomPanelCatalogMenuModeLink")]
            Link = 1
        }



        public enum eMainPageCatalogView
        {
            [Localize("Admin.Settings.Mobile.MainPageViewModeHorizontal")]
            Horizontal = 0,
            [Localize("Admin.Settings.Mobile.MainPageViewModeVertical")]
            Vertical = 1,
            [Localize("Admin.Settings.Mobile.MainPageViewModeNone")]
            None = 2
        }

        public enum eFooterMunuMode
        {
            [Localize("Admin.Settings.Mobile.FooterMunuModeAccordion")]
            Accordion = 0,
            [Localize("Admin.Settings.Mobile.FooterMunuModeDeployed")]
            Deployed = 1,
        }

        public static bool IsMobileTemplateActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsMobileTemplateActive"]);
            set => SettingProvider.Items["IsMobileTemplateActive"] = value.ToString();
        }

        public static bool IsFullCheckout
        {
            get => Convert.ToBoolean(SettingProvider.Items["Mobile_IsFullCheckout"]);
            set => SettingProvider.Items["Mobile_IsFullCheckout"] = value.ToString();
        }

        public static int CountLinesProductName => SettingProvider.Items["Mobile_CountLinesProductName"].TryParseInt(3);

        #region Mobile settings by theme and template

        private static string DefaultTheme = "";

        private static string GetPrefix(string name, string theme = null)
        {
            if (theme == null)
                theme = SettingsDesign.MobileTemplate;

            return $"Mobile_{name}{(!string.IsNullOrEmpty(theme) ? "_" + theme : "")}";
        }

        private static string GetSettingValue(string name, string theme = null)
        {
            var nameByTheme = GetPrefix(name, theme);
            var value = TemplateSettingsProvider.GetSettingValue(nameByTheme) ??
                        TemplateSettingsProvider.GetSettingValue(nameByTheme, TemplateService.DefaultTemplateId) ??
                        "";

            return value;
        }

        public static int MainPageProductsCount
        {
            get => GetMainPageProductsCount();
            set => TemplateSettingsProvider.Items[GetPrefix("MainPageProductsCount")] = value.ToString();
        }

        public static int GetMainPageProductsCount(string theme = null)
        {
            var value = GetSettingValue("MainPageProductsCount", theme);
            return
                string.IsNullOrEmpty(value)
                    ? MainPageProductsCount = GetSettingValue("MainPageProductsCount", DefaultTheme).TryParseInt()
                    : Convert.ToInt32(value);
        }

        public static bool DisplayCity
        {
            get => GetDisplayCity();
            set => TemplateSettingsProvider.Items[GetPrefix("DisplayCity")] = value.ToString();
        }

        public static bool GetDisplayCity(string theme = null)
        {
            var value = GetSettingValue("DisplayCity", theme);
            return
                string.IsNullOrEmpty(value)
                    ? DisplayCity = GetSettingValue("DisplayCity", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }

        public static eFooterMunuMode FooterMunuMode
        {
            get => GetFooterMunuMode();
            set => TemplateSettingsProvider.Items[GetPrefix("FooterMunuMode")] = ((int)value).ToString();
        }

        public static eFooterMunuMode GetFooterMunuMode(string theme = null)
        {
            return (eFooterMunuMode)GetSettingValue("FooterMunuMode", theme).TryParseInt();
        }

        public static bool DisplaySlider
        {
            get => GetDisplaySlider();
            set => TemplateSettingsProvider.Items[GetPrefix("DisplaySlider")] = value.ToString();
        }

        public static bool GetDisplaySlider(string theme = null)
        {
            var value = GetSettingValue("DisplaySlider", theme);
            return
                string.IsNullOrEmpty(value)
                    ? DisplaySlider = GetSettingValue("DisplaySlider", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }
        
        public static bool ShowBottomPanel
        {
            get => GetShowBottomPanel();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowBottomPanel")] = value.ToString();
        }

        public static bool GetShowBottomPanel(string theme = null)
        {
            var value = GetSettingValue("ShowBottomPanel", theme);
            return
                string.IsNullOrEmpty(value)
                    ? ShowBottomPanel = GetSettingValue("ShowBottomPanel", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }

        public static bool ShowNewsOnMainPage
        {
            get => GetShowNewsOnMainPage();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowNewsOnMainPage")] = value.ToString();
        }

        public static bool GetShowNewsOnMainPage(string theme = null)
        {
            var value = GetSettingValue("ShowNewsOnMainPage", theme);
            return
                string.IsNullOrEmpty(value)
                    ? ShowNewsOnMainPage = GetSettingValue("ShowNewsOnMainPage", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }
        
        public static bool IsProductInfoBlocksAccordion
        {
            get => GetProductInfoBlocksMode();
            set => TemplateSettingsProvider.Items[GetPrefix("IsProductInfoBlocksAccordion")] = value.ToString();
        }

        public static bool GetProductInfoBlocksMode(string theme = null)
        {
            var value = GetSettingValue("IsProductInfoBlocksAccordion", theme);
            return
                string.IsNullOrEmpty(value)
                    ? IsProductInfoBlocksAccordion = GetSettingValue("IsProductInfoBlocksAccordion", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }

        public static bool ShowUnitCardProductMobile
        {
            get => TemplateSettingsProvider.Items["ShowUnitCardProductMobile"].TryParseBool(true) ?? true;
            set => TemplateSettingsProvider.Items["ShowUnitCardProductMobile"] = value.ToString();
        }

        public static string BottomPanelCatalogMenuMode
        {
            get => GetBottomPanelCatalogMenuMode();
            set => TemplateSettingsProvider.Items[GetPrefix("BottomPanelCatalogMenuMode")] = value.ToString();
        }

        public static string GetBottomPanelCatalogMenuMode(string theme = null)
        {
            return GetSettingValue("BottomPanelCatalogMenuMode", theme);
        }

        public static bool DisplayHeaderTitle
        {
            get => GetDisplayHeaderTitle();
            set => TemplateSettingsProvider.Items[GetPrefix("DisplayHeaderTitle")] = value.ToString();
        }

        public static bool GetDisplayHeaderTitle(string theme = null)
        {
            var value = GetSettingValue("DisplayHeaderTitle");
            return
                string.IsNullOrEmpty(value)
                    ? DisplayHeaderTitle = GetSettingValue("DisplayHeaderTitle", DefaultTheme).TryParseBool()
                    : Convert.ToBoolean(value);
        }

        public static string HeaderCustomTitle
        {
            get => GetHeaderCustomTitle();
            set => TemplateSettingsProvider.Items[GetPrefix("HeaderCustomTitle")] = value;
        }

        public static string GetHeaderCustomTitle(string theme = null)
        {
            var value = GetSettingValue("HeaderCustomTitle", theme);
            return value ?? (HeaderCustomTitle = GetSettingValue("HeaderCustomTitle", DefaultTheme));
        }

        public static string BrowserColor
        {
            get => GetBrowserColor();
            set => TemplateSettingsProvider.Items[GetPrefix("BrowserColor")] = value;
        }

        public static string GetBrowserColor(string theme = null)
        {
            var value = GetSettingValue("BrowserColor", theme);
            return value ?? (BrowserColor = GetSettingValue("BrowserColor", DefaultTheme));
        }

        public static string BrowserColorVariantsSelected
        {
            get => GetBrowserColorVariantsSelected();
            set => TemplateSettingsProvider.Items[GetPrefix("BrowserColorVariantsSelected")] = value;
        }

        public static string GetBrowserColorVariantsSelected(string theme = null)
        {
            var value = GetSettingValue("BrowserColorVariantsSelected", theme);
            return value ??
                   (BrowserColorVariantsSelected = GetSettingValue("BrowserColorVariantsSelected", DefaultTheme));
        }

        public static string HeaderColorVariantsSelected
        {
            get => GetHeaderColorVariantsSelected();
            set => TemplateSettingsProvider.Items[GetPrefix("HeaderColorVariantsSelected")] = value;
        }

        public static string GetHeaderColorVariantsSelected(string theme = null)
        {
            var value = GetSettingValue("HeaderColorVariantsSelected", theme);
            return value ??
                   (HeaderColorVariantsSelected = GetSettingValue("HeaderColorVariantsSelected", DefaultTheme));
        }

        public static string ViewCategoriesOnMain
        {
            get => GetViewCategoriesOnMain();
            set => TemplateSettingsProvider.Items[GetPrefix("ViewCategoriesOnMain")] = value;
        }

        public static string GetViewCategoriesOnMain(string theme = null)
        {
            var value = GetSettingValue("ViewCategoriesOnMain", theme);
            return value ??
                   (ViewCategoriesOnMain = GetSettingValue("ViewCategoriesOnMain", DefaultTheme));
        }


        public static bool ShowAddButton
        {
            get => GetShowAddButton();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowAddButton")] = value.ToString();
        }

        public static bool GetShowAddButton(string theme = null)
        {
            return GetSettingValue("ShowAddButton", theme).TryParseBool();
        }

        public static string LogoType
        {
            get => GetLogoType();
            set => TemplateSettingsProvider.Items[GetPrefix("LogoType")] = value;
        }

        public static string GetLogoType(string theme = null)
        {
            return GetSettingValue("LogoType", theme);
        }

        public static string LogoImageName
        {
            get => GetLogoImageName();
            set => TemplateSettingsProvider.Items[GetPrefix("LogoImageName")] = value;
        }

        public static string GetLogoImageName(string theme = null)
        {
            return GetSettingValue("LogoImageName", theme);
        }

        public static int LogoImageWidth
        {
            get => GetLogoImageWidth();
            set => TemplateSettingsProvider.Items[GetPrefix("LogoImageWidth")] = value.ToString();
        }

        public static int GetLogoImageWidth(string theme = null)
        {
            var value = (GetSettingValue("LogoImageWidth", theme) ??
                         GetSettingValue("LogoImageWidth", DefaultTheme)).TryParseInt();

            if (value <= 0)
                value = 0;

            return value;
        }


        public static int LogoImageHeight
        {
            get => GetLogoImageHeight();
            set => TemplateSettingsProvider.Items[GetPrefix("LogoImageHeight")] = value.ToString();
        }

        public static int GetLogoImageHeight(string theme = null)
        {
            var value = (GetSettingValue("LogoImageHeight", theme) ??
                         GetSettingValue("LogoImageHeight", DefaultTheme)).TryParseInt();

            if (value <= 0)
                value = 0;

            return value;
        }

        public static bool ShowMenuLinkAll
        {
            get => GetShowMenuLinkAll();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowMenuLinkAll")] = value.ToString();
        }

        public static bool GetShowMenuLinkAll(string theme = null)
        {
            return GetSettingValue("ShowMenuLinkAll", theme).TryParseBool();
        }

        public static ProductViewMode DefaultCatalogView
        {
            get => GetDefaultCatalogView();
            set => TemplateSettingsProvider.Items[GetPrefix("DefaultCatalogView")] = ((int)value).ToString();
        }

        public static ProductViewMode GetDefaultCatalogView(string theme = null)
        {
            return (ProductViewMode)GetSettingValue("DefaultCatalogView", theme).TryParseInt();
        }
        
        public static AdvantShop.FilePath.ProductImageType ProductImageType
        {
            get => GetProductImageType();
            set => TemplateSettingsProvider.Items[GetPrefix("ProductImageType")] = ((int)value).ToString();
        }

        public static AdvantShop.FilePath.ProductImageType GetProductImageType(string theme = null)
        {
            return (AdvantShop.FilePath.ProductImageType)GetSettingValue("ProductImageType", theme).TryParseInt();
        }

        public static eMainPageCatalogView MainPageCatalogView
        {
            get => GetMainPageCatalogView();
            set => TemplateSettingsProvider.Items[GetPrefix("MainPageCatalogView")] = ((int)value).ToString();
        }

        public static eMainPageCatalogView GetMainPageCatalogView(string theme = null)
        {
            return (eMainPageCatalogView)GetSettingValue("MainPageCatalogView", theme).TryParseInt();
        }

        public static bool EnableCatalogViewChange
        {
            get => GetEnableCatalogViewChange();
            set => TemplateSettingsProvider.Items[GetPrefix("EnableCatalogViewChange")] = value.ToString();
        }

        public static bool GetEnableCatalogViewChange(string theme = null)
        {
            return GetSettingValue("EnableCatalogViewChange", theme).TryParseBool();
        }

        public static string CatalogMenuViewMode
        {
            get => GetCatalogMenuViewMode();
            set => TemplateSettingsProvider.Items[GetPrefix("CatalogMenuViewMode")] = value.ToString();
        }

        public static string GetCatalogMenuViewMode(string theme = null)
        {
            return GetSettingValue("CatalogMenuViewMode", theme);
        }


        public static int BlockProductPhotoHeight
        {
            get => GetBlockProductPhotoHeight();
            set => TemplateSettingsProvider.Items[GetPrefix("BlockProductPhotoHeight")] = value.ToString();
        }
        
        public static int GetBlockProductPhotoHeight(string theme = null)
        {
            var value = GetSettingValue("BlockProductPhotoHeight", theme).TryParseInt();
            return value > 0 ? value : 180;
        }

        public static int BlockProductPhotoMiddleHeight
        {
            get => GetBlockProductPhotoMiddleHeight();
            set => TemplateSettingsProvider.Items[GetPrefix("BlockProductPhotoMiddleHeight")] = value.ToString();
        }
        
        public static int GetBlockProductPhotoMiddleHeight(string theme = null)
        {
            var value = GetSettingValue("BlockProductPhotoMiddleHeight", theme).TryParseInt();
            return value > 0 ? value : 395;
        }      
        
        public static bool ShowBriefDescription
        {
            get => GetShowBriefDescription();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowBriefDescription")] = value.ToString();
        }

        public static bool GetShowBriefDescription(string theme = null)
        {
            return GetSettingValue("ShowBriefDescription", theme).TryParseBool();
        }
        public static bool ShowBriefDescriptionInMainMobile
        {
            get => GetShowBriefDescriptionInMainMobile();
            set => TemplateSettingsProvider.Items[GetPrefix("ShowBriefDescriptionInMainMobile")] = value.ToString();
        }

        public static bool GetShowBriefDescriptionInMainMobile(string theme = null)
        {
            return GetSettingValue("ShowBriefDescriptionInMainMobile", theme).TryParseBool();
        }
        

        #endregion

        #region MobileApp

        public static bool MobileAppActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["MobileApp_Active"]);
            set => SettingProvider.Items["MobileApp_Active"] = value.ToString();
        }
        public static string MobileAppName
        {
            get => SettingProvider.Items["MobileApp_Name"];
            set => SettingProvider.Items["MobileApp_Name"] = value;
        }
        public static string MobileAppShortName
        {
            get => SettingProvider.Items["MobileApp_ShortName"];
            set => SettingProvider.Items["MobileApp_ShortName"] = value;
        }
        public static string MobileAppAppleAppStoreLink
        {
            get => SettingProvider.Items["MobileApp_AppleAppStoreLink"];
            set => SettingProvider.Items["MobileApp_AppleAppStoreLink"] = value;
        }
        public static string MobileAppGooglePlayMarket
        {
            get => SettingProvider.Items["MobileApp_GooglePlayMarket"];
            set => SettingProvider.Items["MobileApp_GooglePlayMarket"] = value;
        }
        public static string MobileAppIconImageName
        {
            get => SettingProvider.Items["MobileApp_IconImageName"];
            set => SettingProvider.Items["MobileApp_IconImageName"] = value;
        }
        public static bool MobileAppShowBadges
        {
            get => Convert.ToBoolean(SettingProvider.Items["MobileApp_ShowBadges"]);
            set => SettingProvider.Items["MobileApp_ShowBadges"] = value.ToString();
        }
        public static string MobileAppManifestName
        {
            get => SettingProvider.Items["MobileApp_ManifestName"];
            set => SettingProvider.Items["MobileApp_ManifestName"] = value;
        }

        #endregion
    }
}
