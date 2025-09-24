using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsTemplate
    {
        private List<TemplateSetting> _settings;

        public SettingsTemplate() =>
            _settings = TemplateSettingsProvider.GetTemplateSettingsBox().Settings 
                                ?? new List<TemplateSetting>();
        
        #region Common

        public string MainPageMode
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MainPageMode", _settings);
            set => TemplateSettingsProvider.SaveSetting("MainPageMode", value);
        }
        
        public List<SelectListItem> MainPageModeOptions => 
            TemplateSettingsProvider.GetTemplateOptions("MainPageMode", _settings);

        public TemplateSetting MainPageModeSetting =>
            TemplateSettingsProvider.TryGetTemplateSetting("MainPageMode", _settings);
        
        public string MenuStyle
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MenuStyle", _settings);
            set => TemplateSettingsProvider.SaveSetting("MenuStyle", value);
        }
        
        public List<SelectListItem> MenuStyleOptions => 
            TemplateSettingsProvider.GetTemplateOptions("MenuStyle", _settings);
        
        public string FontStyle
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("FontStyle", _settings);
            set => TemplateSettingsProvider.SaveSetting("FontStyle", value);
        }
        
        public List<SelectListItem> FontStyleOptions => 
            TemplateSettingsProvider.GetTemplateOptions("FontStyle", _settings);
        
        public string FontSize
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("FontSize", _settings);
            set => TemplateSettingsProvider.SaveSetting("FontSize", value);
        }
        
        public List<SelectListItem> FontSizeOptions => 
            TemplateSettingsProvider.GetTemplateOptions("FontSize", _settings);
        
        public string TitleStyle
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TitleStyle", _settings);
            set => TemplateSettingsProvider.SaveSetting("TitleStyle", value);
        }
        
        public List<SelectListItem> TitleStyleOptions => 
            TemplateSettingsProvider.GetTemplateOptions("TitleStyle", _settings);
        
        public string TitleSize
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TitleSize", _settings);
            set => TemplateSettingsProvider.SaveSetting("TitleSize", value);
        }
        
        public List<SelectListItem> TitleSizeOptions => 
            TemplateSettingsProvider.GetTemplateOptions("TitleSize", _settings);
        
        public string TitleWeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TitleWeight", _settings);
            set => TemplateSettingsProvider.SaveSetting("TitleWeight", value);
        }
        
        public List<SelectListItem> TitleWeightOptions => 
            TemplateSettingsProvider.GetTemplateOptions("TitleWeight", _settings);
        
        public string SearchBlockLocation
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("SearchBlockLocation", _settings);
            set => TemplateSettingsProvider.SaveSetting("SearchBlockLocation", value);
        }
        
        public List<SelectListItem> SearchBlockLocationOptions => 
            TemplateSettingsProvider.GetTemplateOptions("SearchBlockLocation", _settings);

        public string TopPanel
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TopPanel", _settings);
            set => TemplateSettingsProvider.SaveSetting("TopPanel", value);
        }
        
        public List<SelectListItem> TopPanelOptions => 
            TemplateSettingsProvider.GetTemplateOptions("TopPanel", _settings);
        
        public TemplateSetting TopPanelSetting =>
            TemplateSettingsProvider.TryGetTemplateSetting("TopPanel", _settings);
        
        public string Header
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("Header", _settings);
            set => TemplateSettingsProvider.SaveSetting("Header", value);
        }
        
        public List<SelectListItem> HeaderOptions => 
            TemplateSettingsProvider.GetTemplateOptions("Header", _settings);
        
        public TemplateSetting HeaderSetting =>
            TemplateSettingsProvider.TryGetTemplateSetting("Header", _settings);
        
        public string TopMenu
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TopMenu", _settings);
            set => TemplateSettingsProvider.SaveSetting("TopMenu", value);
        }
        
        public List<SelectListItem> TopMenuOptions => 
            TemplateSettingsProvider.GetTemplateOptions("TopMenu", _settings);
        
        public TemplateSetting TopMenuSetting =>
            TemplateSettingsProvider.TryGetTemplateSetting("TopMenu", _settings);
        
        public bool RecentlyViewVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("RecentlyViewVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("RecentlyViewVisibility", value.ToString());
        }

        public bool WishListVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("WishListVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("WishListVisibility", value.ToString());
        }

        public bool TopMenuVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("TopMenuVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("TopMenuVisibility", value.ToString());
        }
        
        public bool ShowPriceInMiniCart
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("ShowPriceInMiniCart", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("ShowPriceInMiniCart", value.ToString());
        }
        
        public bool ShowAdditionalPhones
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("ShowAdditionalPhones", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("ShowAdditionalPhones", value.ToString());
        }
        
        public bool HideDisplayToolBarBottomOption
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("HideDisplayToolBarBottomOption", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("HideDisplayToolBarBottomOption", value.ToString());
        }

        #endregion

        #region Main page

        public bool CarouselVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CarouselVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("CarouselVisibility", value.ToString());
        }
        
        public int CarouselAnimationSpeed
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CarouselAnimationSpeed", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CarouselAnimationSpeed", value.ToString());
        }
        
        public int CarouselAnimationDelay
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CarouselAnimationDelay", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CarouselAnimationDelay", value.ToString());
        }
        
        public bool MainPageProductsVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MainPageProductsVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("MainPageProductsVisibility", value.ToString());
        }
        
        public int CountMainPageProductInSection
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageProductInSection", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageProductInSection", value.ToString());
        }
        
        public int CountMainPageProductInLine
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageProductInLine", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageProductInLine", value.ToString());
        }
        
        public bool BrandCarouselVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BrandCarouselVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("BrandCarouselVisibility", value.ToString());
        }
        
        public bool NewsVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("NewsVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("NewsVisibility", value.ToString());
        }
        
        public bool NewsSubscriptionVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("NewsSubscriptionVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("NewsSubscriptionVisibility", value.ToString());
        }
        
        public bool CheckOrderVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CheckOrderVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("CheckOrderVisibility", value.ToString());
        }
        
        public bool GiftSertificateVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("GiftSertificateVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("GiftSertificateVisibility", value.ToString());
        }
        
        public bool MainPageCategoriesVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MainPageCategoriesVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("MainPageCategoriesVisibility", value.ToString());
        }
        
        public int CountMainPageCategoriesInSection
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageCategoriesInSection", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageCategoriesInSection", value.ToString());
        }
        
        public int CountMainPageCategoriesInLine
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageCategoriesInLine", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageCategoriesInLine", value.ToString());
        }
        
        public bool MainPageProductReviewsVisibility
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MainPageProductReviewsVisibility", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("MainPageProductReviewsVisibility", value.ToString());
        }
        
        public int CountMainPageProductReviewsInSection
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageProductReviewsInSection", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageProductReviewsInSection", value.ToString());
        }
        
        public int CountMainPageProductReviewsInLine
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountMainPageProductReviewsInLine", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountMainPageProductReviewsInLine", value.ToString());
        }

        #endregion

        #region Catalog

        public int CountCategoriesInLine
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountCategoriesInLine", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountCategoriesInLine", value.ToString());
        }
        
        public int CountCatalogProductInLine
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CountCatalogProductInLine", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CountCatalogProductInLine", value.ToString());
        }

        public List<SelectListItem> UnsupportedViewListTypes =>
            TemplateSettingsProvider.GetTemplateOptions("UnsupportedViewListTypes", _settings);
        
        public int BigProductImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BigProductImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BigProductImageWidth", value.ToString());
        }
        
        public int BigProductImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BigProductImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BigProductImageHeight", value.ToString());
        }
        
        public int MiddleProductImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MiddleProductImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("MiddleProductImageWidth", value.ToString());
        }
        
        public int MiddleProductImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("MiddleProductImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("MiddleProductImageHeight", value.ToString());
        }
        
        public int SmallProductImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("SmallProductImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("SmallProductImageWidth", value.ToString());
        }
        
        public int SmallProductImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("SmallProductImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("SmallProductImageHeight", value.ToString());
        }
        
        public int XSmallProductImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("XSmallProductImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("XSmallProductImageWidth", value.ToString());
        }
        
        public int XSmallProductImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("XSmallProductImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("XSmallProductImageHeight", value.ToString());
        }
        
        public int BigCategoryImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BigCategoryImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BigCategoryImageWidth", value.ToString());
        }
        
        public int BigCategoryImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BigCategoryImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BigCategoryImageHeight", value.ToString());
        }
        
        public int SmallCategoryImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("SmallCategoryImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("SmallCategoryImageWidth", value.ToString());
        }
        
        public int SmallCategoryImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("SmallCategoryImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("SmallCategoryImageHeight", value.ToString());
        }

        #endregion

        #region Checkout

        public bool ShowProductsPhotoInCheckoutCart
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("ShowProductsPhotoInCheckoutCart", _settings).TryParseBool();
            set => TemplateSettingsProvider.SaveSetting("ShowProductsPhotoInCheckoutCart", value.ToString());
        }
        
        public int PaymentIconWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("PaymentIconWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("PaymentIconWidth", value.ToString());
        }
        
        public int PaymentIconHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("PaymentIconHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("PaymentIconHeight", value.ToString());
        }
        
        public int ShippingIconWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("ShippingIconWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("ShippingIconWidth", value.ToString());
        }
        
        public int ShippingIconHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("ShippingIconHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("ShippingIconHeight", value.ToString());
        }

        #endregion

        #region Brands

        public int BrandLogoWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BrandLogoWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BrandLogoWidth", value.ToString());
        }
        
        public int BrandLogoHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("BrandLogoHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("BrandLogoHeight", value.ToString());
        }

        #endregion
        
        #region News
        
        public int NewsImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("NewsImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("NewsImageWidth", value.ToString());
        }
        
        public int NewsImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("NewsImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("NewsImageHeight", value.ToString());
        }
        
        #endregion

        #region CustomOptions

        public int CustomOptionsImageWidth
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CustomOptionsImageWidth", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CustomOptionsImageWidth", value.ToString());
        }
        
        public int CustomOptionsImageHeight
        {
            get => TemplateSettingsProvider.TryGetTemplateSettingValue("CustomOptionsImageHeight", _settings).TryParseInt();
            set => TemplateSettingsProvider.SaveSetting("CustomOptionsImageHeight", value.ToString());
        }

        #endregion

        #region Other

        public List<TemplateSettingSection> OtherSettingsSections
        {
            get => 
                _settings
                    .Where(additionalSetting => additionalSetting.IsAdditional)
                    .GroupBy(additionalSetting => additionalSetting.SectionName)
                    .Select(additionalSetting => 
                        new TemplateSettingSection()
                        {
                            Key = additionalSetting.Key,
                            Name = Enum.TryParse(additionalSetting.Key, out ETemplateSettingSection sectionType) 
                                ? sectionType.Localize() 
                                : additionalSetting.Key,
                            Settings = _settings
                                .Where(x => 
                                    x.IsAdditional 
                                    && x.SectionName == additionalSetting.Key)
                                .ToList()
                        }
                    )
                    .ToList();
            set => 
                value.ForEach(tss =>
                    tss.Settings.ForEach(TemplateSettingsProvider.SaveSetting));
        }

        #endregion

        public List<string> HiddenSettings
        {
            get => TemplateSettingsProvider.GetHiddenSettings();
        }
    }
}