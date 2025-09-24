using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Files;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.Mobiles
{
    public class LoadSaveMobileSettingsHandler
    {
        private readonly MobileVersionSettingsModel _model;

        public LoadSaveMobileSettingsHandler() { }

        public LoadSaveMobileSettingsHandler(MobileVersionSettingsModel model) : this()
        {
            _model = model;
        }

        public MobileVersionSettingsModel Get()
        {
            var _browserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
            var _headerColorVariantsSelected = SettingsMobile.HeaderColorVariantsSelected.TryParseEnum(SettingsMobile.eHeaderColorVariants.ColorScheme);
            var _viewCategoriesOnMainList = SettingsMobile.ViewCategoriesOnMain.TryParseEnum(SettingsMobile.eViewCategoriesOnMain.Default);
            var _logoType = SettingsMobile.LogoType.TryParseEnum(SettingsMobile.eLogoType.Text);
            var _catalogMenuViewMode = SettingsMobile.CatalogMenuViewMode.TryParseEnum(SettingsMobile.eCatalogMenuViewMode.RootCategories);
            var _bottomPanelCatalogMenuMode = SettingsMobile.BottomPanelCatalogMenuMode.TryParseEnum(SettingsMobile.eBottomPanelCatalogMenuMode.RootCategories);
            var model = new MobileVersionSettingsModel()
            {
                Enabled = SettingsMobile.IsMobileTemplateActive,
                IsFullCheckout = SettingsMobile.IsFullCheckout,

                MainPageProductCountMobile = SettingsMobile.MainPageProductsCount,
                FooterMunuMode = SettingsMobile.FooterMunuMode,
                ShowCity = SettingsMobile.DisplayCity,
                ShowSlider = SettingsMobile.DisplaySlider,
                IsProductInfoBlocksAccordion = SettingsMobile.IsProductInfoBlocksAccordion,
                ShowUnitCardProductMobile = SettingsMobile.ShowUnitCardProductMobile,
                ShowBottomPanel = SettingsMobile.ShowBottomPanel,
                DisplayHeaderTitle = SettingsMobile.DisplayHeaderTitle,
                HeaderCustomTitle = SettingsMobile.HeaderCustomTitle,

                MobileTemlate = SettingsDesign.MobileTemplate ?? "",
                MobileTemplates = GetMobileTemplates(),
                BrowserColor = SettingsMobile.BrowserColor,
                ShowAddButton = SettingsMobile.ShowAddButton,
                ShowMenuLinkAll = SettingsMobile.ShowMenuLinkAll,

                BrowserColorVariantsSelected = _browserColorVariantsSelected,
                BrowserColorVariantsList = Enum.GetValues(typeof(SettingsMobile.eBrowserColorVariants)).Cast<SettingsMobile.eBrowserColorVariants>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _browserColorVariantsSelected
                }).ToList(),

                HeaderColorVariantsSelected = _headerColorVariantsSelected,

                HeaderColorVariantsList = Enum.GetValues(typeof(SettingsMobile.eHeaderColorVariants)).Cast<SettingsMobile.eHeaderColorVariants>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _headerColorVariantsSelected
                }).ToList(),

                ViewCategoriesOnMain = _viewCategoriesOnMainList,

                ViewCategoriesOnMainList = Enum.GetValues(typeof(SettingsMobile.eViewCategoriesOnMain)).Cast<SettingsMobile.eViewCategoriesOnMain>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _viewCategoriesOnMainList
                }).ToList(),

                LogoType = SettingsMobile.LogoType.TryParseEnum(SettingsMobile.eLogoType.Text),
                LogoTypeList = Enum.GetValues(typeof(SettingsMobile.eLogoType)).Cast<SettingsMobile.eLogoType>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _logoType
                }).ToList(),

                LogoImgSrc = !string.IsNullOrEmpty(SettingsMobile.LogoImageName)
                    ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMobile.LogoImageName, true)
                    : "../images/nophoto_small.png",


                DefaultCatalogView = SettingsMobile.DefaultCatalogView,
                ProductImageType = SettingsMobile.ProductImageType,
                MainPageCatalogView = SettingsMobile.MainPageCatalogView,
                EnableCatalogViewChange = SettingsMobile.EnableCatalogViewChange,
                CatalogMenuViewMode = _catalogMenuViewMode,
                CatalogMenuViewModeList = Enum.GetValues(typeof(SettingsMobile.eCatalogMenuViewMode)).Cast<SettingsMobile.eCatalogMenuViewMode>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _catalogMenuViewMode
                }).ToList(),
                BottomPanelCatalogMenuMode = _bottomPanelCatalogMenuMode,
                BottomPanelCatalogMenuModeList = Enum.GetValues(typeof(SettingsMobile.eBottomPanelCatalogMenuMode)).Cast<SettingsMobile.eBottomPanelCatalogMenuMode>()
                .Select(x => new SelectListItem
                {
                    Text = x.Localize(),
                    Value = ((int)x).ToString(),
                    Selected = x == _bottomPanelCatalogMenuMode
                }).ToList(),
                BlockProductPhotoHeight = SettingsMobile.BlockProductPhotoHeight,
                BlockProductPhotoMiddleHeight = SettingsMobile.BlockProductPhotoMiddleHeight,
                ShowBriefDescription =  SettingsMobile.ShowBriefDescription,
                ShowBriefDescriptionInMainMobile = SettingsMobile.ShowBriefDescriptionInMainMobile,
                ShowNewsOnMainPage = SettingsMobile.ShowNewsOnMainPage
            };
            
            var robotsTxt = FileInDbService.Get("/areas/mobile/robots.txt");
            model.Robots = robotsTxt != null && robotsTxt.Content != null
                ? Encoding.UTF8.GetString(robotsTxt.Content)
                : "";

            return model;
        }

        public MobileVersionSettingsModel GetSettings(string template)
        {
            if (template == null)
                template = "";

            return new MobileVersionSettingsModel()
            {
                MainPageProductCountMobile = SettingsMobile.GetMainPageProductsCount(template),
                FooterMunuMode = SettingsMobile.GetFooterMunuMode(template),
                ShowCity = SettingsMobile.GetDisplayCity(template),
                ShowSlider = SettingsMobile.GetDisplaySlider(template),
                IsProductInfoBlocksAccordion = SettingsMobile.GetProductInfoBlocksMode(template),
                ShowUnitCardProductMobile = SettingsMobile.ShowUnitCardProductMobile,
                ShowBottomPanel = SettingsMobile.GetShowBottomPanel(template),
                BottomPanelCatalogMenuMode = SettingsMobile.GetBottomPanelCatalogMenuMode(template).TryParseEnum(SettingsMobile.eBottomPanelCatalogMenuMode.RootCategories),
                DisplayHeaderTitle = SettingsMobile.GetDisplayHeaderTitle(template),
                HeaderCustomTitle = SettingsMobile.GetHeaderCustomTitle(template),

                BrowserColor = SettingsMobile.GetBrowserColor(template),
                ShowAddButton = SettingsMobile.GetShowAddButton(template),
                ShowMenuLinkAll = SettingsMobile.GetShowMenuLinkAll(template),

                BrowserColorVariantsSelected = 
                    SettingsMobile.GetBrowserColorVariantsSelected(template).TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme),

                HeaderColorVariantsSelected =
                    SettingsMobile.GetHeaderColorVariantsSelected(template).TryParseEnum(SettingsMobile.eHeaderColorVariants.ColorScheme),
                
                ViewCategoriesOnMain =
                    SettingsMobile.GetViewCategoriesOnMain(template).TryParseEnum(SettingsMobile.eViewCategoriesOnMain.Default),

                LogoType = SettingsMobile.GetLogoType(template).TryParseEnum(SettingsMobile.eLogoType.Text),
                
                LogoImgSrc = !string.IsNullOrEmpty(SettingsMobile.GetLogoImageName(template))
                    ? FoldersHelper.GetPath(FolderType.Pictures, SettingsMobile.GetLogoImageName(template), true)
                    : "../images/nophoto_small.png",

                DefaultCatalogView = SettingsMobile.GetDefaultCatalogView(template),
                ProductImageType = SettingsMobile.GetProductImageType(template),
                EnableCatalogViewChange = SettingsMobile.GetEnableCatalogViewChange(template),

                CatalogMenuViewMode = SettingsMobile.GetCatalogMenuViewMode(template).TryParseEnum(SettingsMobile.eCatalogMenuViewMode.RootCategories),
                BlockProductPhotoHeight = SettingsMobile.GetBlockProductPhotoHeight(template),
                BlockProductPhotoMiddleHeight = SettingsMobile.GetBlockProductPhotoMiddleHeight(template),
                ShowBriefDescription = SettingsMobile.ShowBriefDescription,
                ShowBriefDescriptionInMainMobile = SettingsMobile.ShowBriefDescriptionInMainMobile,
                ShowNewsOnMainPage = SettingsMobile.ShowNewsOnMainPage
            };
        }

        public void Save()
        {
            SettingsMobile.IsMobileTemplateActive = _model.Enabled;
            SettingsMobile.IsFullCheckout = _model.IsFullCheckout;
            SettingsMobile.FooterMunuMode = _model.FooterMunuMode;
            SettingsDesign.MobileTemplate = _model.MobileTemlate ?? ""; // save first, cause settings depends on template

            SettingsMobile.MainPageProductsCount = Convert.ToInt32(_model.MainPageProductCountMobile);
            SettingsMobile.DisplayCity = _model.ShowCity;
            SettingsMobile.DisplaySlider = _model.ShowSlider;
            SettingsMobile.IsProductInfoBlocksAccordion = _model.IsProductInfoBlocksAccordion;
            SettingsMobile.ShowUnitCardProductMobile = _model.ShowUnitCardProductMobile;
            SettingsMobile.ShowBottomPanel = _model.ShowBottomPanel;
            SettingsMobile.BottomPanelCatalogMenuMode = _model.BottomPanelCatalogMenuMode.ToString();
            SettingsMobile.DisplayHeaderTitle = _model.DisplayHeaderTitle;
            SettingsMobile.HeaderCustomTitle = _model.HeaderCustomTitle ?? string.Empty;
            SettingsMobile.BrowserColorVariantsSelected = _model.BrowserColorVariantsSelected.ToString();
            SettingsMobile.HeaderColorVariantsSelected = _model.HeaderColorVariantsSelected.ToString();
            SettingsMobile.ViewCategoriesOnMain = _model.ViewCategoriesOnMain.ToString();
            SettingsMobile.LogoType = _model.LogoType.ToString();
            SettingsMobile.ShowAddButton = _model.ShowAddButton;
            SettingsMobile.ShowMenuLinkAll = _model.ShowMenuLinkAll;

            SettingsMobile.DefaultCatalogView = _model.DefaultCatalogView;
            SettingsMobile.ProductImageType = _model.ProductImageType;
            SettingsMobile.MainPageCatalogView = _model.MainPageCatalogView;
            SettingsMobile.EnableCatalogViewChange = _model.EnableCatalogViewChange;
            SettingsMobile.ShowBriefDescription = _model.ShowBriefDescription;
            SettingsMobile.ShowBriefDescriptionInMainMobile = _model.ShowBriefDescriptionInMainMobile;
            SettingsMobile.ShowNewsOnMainPage = _model.ShowNewsOnMainPage;

            if (_model.BrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.None)
            {
                SettingsMobile.BrowserColor = "";
            }
            else if (_model.BrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
            {
                var colorscheme = DesignService.GetCurrenDesign(eDesign.Color);
                if (colorscheme != null)
                    SettingsMobile.BrowserColor = colorscheme.Color ?? "";
            }
            else
            {
                SettingsMobile.BrowserColor = _model.BrowserColor ?? "";
            }

            SettingsMobile.CatalogMenuViewMode = _model.CatalogMenuViewMode.ToString();
            SettingsMobile.BlockProductPhotoHeight = _model.BlockProductPhotoHeight;
            SettingsMobile.BlockProductPhotoMiddleHeight = _model.BlockProductPhotoMiddleHeight;
            
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog + "_mobile_menu_");
            
            CommonHelper.DeleteCookie("mobile_viewmode");
            
            if (FileInDbService.IsExist("/areas/mobile/robots.txt"))
                FileInDbService.Update("/areas/mobile/robots.txt", Encoding.UTF8.GetBytes(_model.Robots ?? ""));
            else
            {
                if (!new FileExtensionContentTypeHelper().TryGetContentType("/areas/mobile/robots.txt", out var contentType))
                    contentType = "text/plain";

                FileInDbService.Add(new FileInDb()
                {
                    Name = "robots.txt",
                    Path = "/areas/mobile/robots.txt",
                    ContentType = contentType,
                    Content = Encoding.UTF8.GetBytes(_model.Robots ?? ""),
                    Charset = "utf-8"
                });
            }
        }

        private List<SelectListItem> GetMobileTemplates()
        {
            var mobileTemplates = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = LocalizationService.GetResource("Admin.Settings.Mobiles.MobileTemplate.Classic"),
                    Value = ""
                }
            };
            
            try
            {
                var mobileTemplatesDirPath = HostingEnvironment.MapPath("~/Areas/Mobile/Templates/");
                if (Directory.Exists(mobileTemplatesDirPath))
                {
                    foreach (var tplDir in Directory.GetDirectories(mobileTemplatesDirPath))
                    {
                        var config = tplDir + "\\config.json";
                        if (!File.Exists(config))
                            continue;

                        var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(config));

                        var key = tplDir.Split('\\').LastOrDefault();

                        if (key != null && settings.ContainsKey("name"))
                            mobileTemplates.Add(new SelectListItem() { Text = settings["name"], Value = key });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return mobileTemplates;
        }
    }
}
