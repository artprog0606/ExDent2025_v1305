using System;
using AdvantShop.Core.Services.Configuration.Settings;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class MobileVersionSettingsModel
    {
        public MobileVersionSettingsModel()
        {
            DefaultViewList = new List<SelectListItem>();

            foreach (ProductViewMode item in Enum.GetValues(typeof(ProductViewMode)))
            {
                if (item != ProductViewMode.Table)
                {
                    DefaultViewList.Add(new SelectListItem()
                        { Text = item.Localize(), Value = ((int)item).ToString() });
                }
            }

            ProductImageTypeList = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = AdvantShop.FilePath.ProductImageType.XSmall.Localize(),
                    Value = ((int)AdvantShop.FilePath.ProductImageType.XSmall).ToString()
                },
                new SelectListItem()
                {
                    Text = AdvantShop.FilePath.ProductImageType.Small.Localize(),
                    Value = ((int)AdvantShop.FilePath.ProductImageType.Small).ToString()
                },
                new SelectListItem()
                {
                    Text = AdvantShop.FilePath.ProductImageType.Middle.Localize(),
                    Value = ((int)AdvantShop.FilePath.ProductImageType.Middle).ToString()
                }
            };

            MainPageCatalogViewList = new List<SelectListItem>();

            foreach (SettingsMobile.eMainPageCatalogView item in Enum.GetValues(
                         typeof(SettingsMobile.eMainPageCatalogView)))
            {
                MainPageCatalogViewList.Add(new SelectListItem()
                    { Text = item.Localize(), Value = ((int)item).ToString() });
            }

            FooterMunuModeList = new List<SelectListItem>();
            foreach (SettingsMobile.eFooterMunuMode item in Enum.GetValues(typeof(SettingsMobile.eFooterMunuMode)))
            {
                FooterMunuModeList.Add(new SelectListItem() { Text = item.Localize(), Value = ((int)item).ToString() });
            }
        }

        public bool Enabled { get; set; }
        public List<SelectListItem> FooterMunuModeList { get; set; }
        public SettingsMobile.eFooterMunuMode FooterMunuMode { get; set; }
        public int MainPageProductCountMobile { get; set; }
        public bool ShowCity { get; set; }
        public bool ShowSlider { get; set; }

        public bool IsProductInfoBlocksAccordion { get; set; }
        public bool ShowUnitCardProductMobile { get; set; }
        public bool DisplayHeaderTitle { get; set; }
        public string HeaderCustomTitle { get; set; }
        public bool IsFullCheckout { get; set; }
        public string Robots { get; set; }
        public string MobileTemlate { get; set; }
        public List<SelectListItem> MobileTemplates { get; set; }
        public string BrowserColor { get; set; }
        public SettingsMobile.eBrowserColorVariants BrowserColorVariantsSelected { get; set; }
        public List<SelectListItem> BrowserColorVariantsList { get; set; }
        public SettingsMobile.eHeaderColorVariants HeaderColorVariantsSelected { get; set; }
        public List<SelectListItem> HeaderColorVariantsList { get; set; }
        public SettingsMobile.eViewCategoriesOnMain ViewCategoriesOnMain { get; set; }
        public List<SelectListItem> ViewCategoriesOnMainList { get; set; }
        public bool ShowAddButton { get; set; }
        public SettingsMobile.eLogoType LogoType { get; set; }
        public List<SelectListItem> LogoTypeList { get; set; }
        public string LogoImgSrc { get; set; }
        public bool ShowMenuLinkAll { get; set; }
        public List<SelectListItem> DefaultViewList { get; private set; }
        public ProductViewMode DefaultCatalogView { get; set; }
        public List<SelectListItem> ProductImageTypeList { get; private set; }
        public AdvantShop.FilePath.ProductImageType ProductImageType { get; set; }
        public List<SelectListItem> MainPageCatalogViewList { get; private set; }
        public SettingsMobile.eMainPageCatalogView MainPageCatalogView { get; set; }
        public bool EnableCatalogViewChange { get; set; }

        public SettingsMobile.eCatalogMenuViewMode CatalogMenuViewMode { get; set; }
        public List<SelectListItem> CatalogMenuViewModeList { get; set; }
        public int BlockProductPhotoHeight { get; set; }
        public int BlockProductPhotoMiddleHeight { get; set; }
        public bool ShowBottomPanel { get; set; }

        public SettingsMobile.eBottomPanelCatalogMenuMode BottomPanelCatalogMenuMode { get; set; }

        public List<SelectListItem> BottomPanelCatalogMenuModeList { get; set; }

        public bool ShowBriefDescription { get; set; }
        public bool ShowBriefDescriptionInMainMobile { get; set; }
        
        public bool ShowNewsOnMainPage { get; set; }
    }
}