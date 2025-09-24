using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.Services.IPTelephony.CallBack;
using AdvantShop.Core.Services.SEO.MetaData;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Handlers.Common;
using AdvantShop.Handlers.Menu;
using AdvantShop.Models.Common;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Trial;
using AdvantShop.ViewModel.Common;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Repository;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.ViewModel.Shared;
using Debug = AdvantShop.Diagnostics.Debug;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Statistic;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.CDN;
using AdvantShop.Areas.Mobile.Handlers.Footer;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.SEO;
using AdvantShop.ViewCommon;
using AdvantShop.Web.Admin.Attributes;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class CommonController : BaseClientController
    {
        public ActionResult ClosedStore()
        {
            if (!SettingsMain.IsStoreClosed)
                return RedirectToRoute("Home");

            SettingsDesign.IsMobileTemplate = false;

            SetMetaInformation(new MetaInfo(T("Common.ClosedStore.Title")));
            return View();
        }

        public JsonResult GetDesign()
        {
            return Json(new GetDesignHandler().Get());
        }

        public JsonResult GetDesignBuilder()
        {
            var model = new GetDesignNewBuilderHandler().Execute();
            return Json(model);
        }


        public ActionResult AdditionalPhones()
        {
            var phones = new GetAdditionalPhones().Execute().Phones;

            if (string.IsNullOrEmpty(SettingsMain.Phone))
                return new EmptyResult();

            return PartialView(phones);
        }

        [ChildActionOnly]
        public ActionResult CartAdd(CartAddViewModel model)
        {
            return PartialView("_CartAdd", model);
        }
        
        [ChildActionOnly]
        public ActionResult NewBuilderButton()
        {
            var customer = CustomerContext.CurrentCustomer;

            var isShowOnLoad = false;

            var cookieValue = CommonHelper.GetCookieString("trialBuilderShowed").TryParseBool(true);

            if ((cookieValue == null || cookieValue == false) && CustomerContext.CurrentCustomer.IsAdmin && TrialService.IsTrialEnabled)
            {
                CommonHelper.SetCookie("trialBuilderShowed", "true", true);
                isShowOnLoad = true;
            }

            if ((customer.Enabled && customer.IsAdmin) || Demo.IsDemoEnabled)
                return PartialView("~/Views/Common/DesignBuilderButton.cshtml", isShowOnLoad);

            return new EmptyResult();
        }

        public void SetCurrency(string currencyIso)
        {
            CurrencyService.CurrentCurrency = CurrencyService.Currency(currencyIso);
        }

        public JsonResult SaveDesign(string theme, string colorscheme, string structure, string background)
        {
            if (CustomerContext.CurrentCustomer.CustomerRole != Role.Administrator && !Demo.IsDemoEnabled &&
                !TrialService.IsTrialEnabled || theme.IsNullOrEmpty() || colorscheme.IsNullOrEmpty() || background.IsNullOrEmpty())
            {
                return Json("error", "text");
            }

            new SaveDesignHandler().Save(theme, colorscheme, structure, background);

            if (!SettingsCongratulationsDashboard.DesignDone)
            {
                SettingsCongratulationsDashboard.DesignDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DesignDone);
            }

            return Json("success", "text");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDesignNewBuilder(DesignNewBuilderModel data)
        {
            if (CustomerContext.CurrentCustomer.CustomerRole != Role.Administrator && !Demo.IsDemoEnabled && !TrialService.IsTrialEnabled)
            {
                return JsonError();
            }

            try
            {
                new SaveDesignNewBuilderHandler(data).Execute();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError();
            }

            if (!SettingsCongratulationsDashboard.DesignDone)
            {
                SettingsCongratulationsDashboard.DesignDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DesignDone);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult DebugJs(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Debug.Log.Error(HttpUtility.HtmlEncode(message));

            return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult Logo(LogoModel logo)
        {
            if(SettingsDesign.AllowChooseDarkTheme && SettingsDesign.UseAnotherForDarkTheme)
            {
                var themeImgSrc = HttpContext.Request.Cookies.Get("cookieThemeImgKey") != null ? HttpUtility.UrlDecode(HttpContext.Request.Cookies.Get("cookieThemeImgKey").Value.ToString()) : "";
                logo.ImgSource = themeImgSrc == "" ? logo.ImgSource : themeImgSrc;
            }
            
            if (!logo.Visible)
                return new EmptyResult();

            var isInplace = InplaceEditorService.CanUseInplace(RoleAction.Settings);

            if (logo.ImgSource.IsNullOrEmpty() && InplaceEditorService.CanUseInplace(RoleAction.Settings))
            {
                logo.ImgSource = UrlService.GetUrl("images/nophoto-logo.png");
            }

            var alt = !string.IsNullOrEmpty(logo.ImgAlt) ? string.Format(" alt=\"{0}\"", logo.ImgAlt) : string.Empty;
            var cssClass = !string.IsNullOrEmpty(logo.CssClass) ? " " + logo.CssClass : string.Empty;

            logo.LogoGeneratorEditOnPageLoad = Request["logoGeneratorEditOnPageLoad"].TryParseBool();

            logo.LogoGeneratorEnabled = !SettingsDesign.IsMobileTemplate &&
                                        CustomerContext.CurrentCustomer.CustomerRole == Role.Administrator &&
                                        (isInplace || logo.LogoGeneratorEditOnPageLoad);

            if (!string.IsNullOrEmpty(logo.ImgSource))
            {
                var imgWidth = SettingsMain.LogoImageWidth > 0 ? SettingsMain.LogoImageWidth : SettingsDesign.LogoImageWidth;
                var imgHeight = SettingsMain.LogoImageHeight > 0 ? SettingsMain.LogoImageHeight : SettingsDesign.LogoImageHeight;
                var style = string.Format("style=\"height: auto;width: {0};\"",
                    logo.IsSvg ? "100%" : (imgWidth > 0 ?  $"min({imgWidth}px, 100%)" : "auto"));

                logo.Html = string.Format(
                    "<img src=\"{0}\"{1} class=\"site-head-logo-picture{2}\" {3} {4} {5} id=\"{6}\" {7}/>",
                    logo.ImgSource, alt, cssClass,
                    Extensions.InplaceExtensions.InplaceImageLogo(),
                    logo.LogoGeneratorEnabled ? "data-logo-generator-preview-img" : "",
                    !logo.LogoGeneratorEnabled && !InplaceEditorService.CanUseInplace(RoleAction.Settings) && imgWidth > 0 && imgHeight > 0 ? $"width=\"{imgWidth}\" height=\"{imgHeight}\""
                        : "",
                    logo.ImgId,
                    style);
            }

            logo.DisplayHref = Request.Path != "/";

            return PartialView("Logo", logo);
        }

        public JsonResult GetAllLogo ()
        {
            LogoModel logoLight = new LogoModel();
            DarkLogoModel logoDark = new DarkLogoModel();
            bool useAnotherForDarkTheme =  SettingsDesign.UseAnotherForDarkTheme && SettingsDesign.AllowChooseDarkTheme;

            if (logoLight.ImgSource.IsNullOrEmpty() && InplaceEditorService.CanUseInplace(RoleAction.Settings))
            {
                logoLight.ImgSource = UrlService.GetUrl("images/nophoto-logo.png");
            }

            if (logoDark.ImgSource.IsNullOrEmpty() && InplaceEditorService.CanUseInplace(RoleAction.Settings))
            {
                logoDark.ImgSource = UrlService.GetUrl("images/nophoto-logo.png");
            }

            return Json(new { light = logoLight, dark = logoDark , useAnotherForDarkTheme = useAnotherForDarkTheme });
        }


        [ChildActionOnly]
        public ActionResult Favicon(FaviconModel faviconModel, string imgSource) => 
            PartialView("Favicon", new FaviconHandler(faviconModel, imgSource, Request.Browser.Browser).Execute());

        [ChildActionOnly]
        public ActionResult MenuTop(bool alwaysRender = false)
        {
            if (!alwaysRender)
            {
                if (GetMainPageMode() != SettingsDesign.eMainPageMode.Default)
                    return new EmptyResult();
            }

            var model = new MenuTopHanlder().GetTopMenuItems();
            return PartialView("MenuTop", model);
        }

        [ChildActionOnly]
        public ActionResult MenuBottom(bool isShowSocial = true)
        {
            var model = new MenuBottomHanlder().Get();
            model.IsShowSocial = isShowSocial;
            model.IsAccordion = SettingsMobile.FooterMunuMode.ToString() == "Accordion" && SettingsDesign.IsMobileTemplate;
            model.IsBonusActive = BonusSystem.IsActive;
            if (model.IsBonusActive)
            {
                model.BonusLinkHref = CustomerContext.CurrentCustomer.RegistredUser ? "myaccount#tab=bonusTab" : "getbonuscard";
            }
            return PartialView("MenuBottom", model);
        }

        public ActionResult Copyright()
        {
            return PartialView("Copyright",
                new CopyrightModel
                {
                    Text = CopyrightService.GetFormattedCopyrightText(ECopyrightType.Site)
                });
        }

        [ChildActionOnly]
        public ActionResult MobileAppLinks()
        {
            var model = new FooterHandler().Get();
            return PartialView("MobileAppLinks", model);
        }


        [ChildActionOnly]
        public ActionResult ToolBarBottom(ToolBarBottomViewModel toolbarModel)
        {
            if (!SettingsDesign.DisplayToolBarBottom || MobileHelper.IsMobileBrowser())
                return new EmptyResult();

            var controller = Request.RequestContext.RouteData.Values["controller"] as string;

            toolbarModel.isCart = controller == "Cart";
            toolbarModel.ShowConfirmButton = controller != "Checkout";

            return PartialView(toolbarModel);
        }


        [ChildActionOnly]
        public ActionResult Telephony()
        {
            if (SettingsSocialWidget.IsActive)
                return new EmptyResult();

            var callBack = IPTelephonyOperator.Current.CallBack;
            if (callBack == null || !callBack.Enabled || (Saas.SaasDataService.IsSaasEnabled && !Saas.SaasDataService.CurrentSaasData.HaveTelephony))
                return new EmptyResult();

            var cookieValue = CommonHelper.GetCookieString("telephonyUserMode");

            var model = new TelephonyViewModel
            {
                TimeInterval = SettingsTelephony.CallBackTimeInterval,
                IsWorkTime = callBack.IsWorkTime(),
                ShowMode =
                    cookieValue.IsNotEmpty()
                        ? cookieValue.TryParseEnum<ECallBackShowMode>()
                        : SettingsTelephony.CallBackShowMode
            };


            return PartialView(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CallBack(string phone, bool check = false)
        {
            var callBack = IPTelephonyOperator.Current.CallBack;
            if (callBack == null || !callBack.Enabled || phone.IsNullOrEmpty())
                return new JsonResult();

            var result = callBack.MakeRequest(phone, check);
            return Json(result);
        }

        [ChildActionOnly]
        public ActionResult CookiesPolicy()
        {
            var cookieName = string.Format("{0}_CookiesPopicyAccepted", SettingsMain.SiteUrlPlain);

            if (!SettingsNotifications.ShowCookiesPolicyMessage || CommonHelper.GetCookieString(HttpUtility.UrlEncode(cookieName)) == "true"
                || BrowsersHelper.IsBot() || Request.IsLighthouse())
                return new EmptyResult();

            return PartialView((object)cookieName);
        }

        [ChildActionOnly]
        public ActionResult MetaData()
        {
            if (!SettingsSEO.OpenGraphEnabled)
                return new EmptyResult();

            var ogModelContext = MetaDataContext.CurrentObject;

            if (ogModelContext == null)
                return PartialView(new OpenGraphModel());

            var ogModel = new OpenGraphModel()
            {
                SiteName = ogModelContext.SiteName,
                Url = ogModelContext.Url,
                Type = ogModelContext.Type,
                Image = ogModelContext.Image
            };

            return PartialView(ogModel);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckOrder(string orderNumber)
        {
            if (!string.IsNullOrEmpty(orderNumber))
                return Json(new { StatusName = T("Checkout.CheckOrder.StatusCommentNotFound") });

            var statusInf = OrderService.GetStatusInfo(orderNumber);
            return
                Json(statusInf != null
                    ? new
                    {
                        StatusName = statusInf.Hidden ? statusInf.PreviousStatus : statusInf.Status,
                        StatusComment = statusInf.Comment
                    }
                    : new
                    {
                        StatusName = T("Checkout.CheckOrder.StatusCommentNotFound"),
                        StatusComment = ""
                    });
        }

        [ChildActionOnly]
        public ActionResult GoogleAnalytics()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            return
                Content(
                    new GoogleAnalyticsString(SettingsSEO.GoogleAnalyticsNumber, SettingsSEO.GoogleAnalyticsEnabled)
                        .GetGoogleAnalyticsString(SettingsSEO.GoogleAnalyticsEnableDemogrReports));
        }


        [ChildActionOnly]
        public ActionResult TopPanel()
        {
            return PartialView(new TopPanelHandler().Get());
        }

        public ActionResult MyAccount()
        {
            return PartialView(new TopPanelHandler().Get());
        }

        [ChildActionOnly]
        public ActionResult Preview()
        {
            return PartialView(new TopPanelHandler().Get());
        }

        [ChildActionOnly]
        public ActionResult BreadCrumbs(List<BreadCrumbs> breadCrumbs)
        {
            if (breadCrumbs == null || breadCrumbs.Count == 0)
                return new EmptyResult();

            return PartialView(breadCrumbs);
        }

        [ChildActionOnly]
        public ActionResult Rating(int objId, double rating, string url, bool readOnly = true, string binding = null)
        {
            return PartialView("_Rating", new RatingViewModel(rating)
            {
                ObjId = objId,
                Url = url,
                ReadOnly = readOnly,
                Binding = binding
            });
        }

        [ChildActionOnly]
        public ActionResult ZonePopover()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            var cookieValue = CommonHelper.GetCookieString("zonePopoverVisible").ToLower();
            var settingValue = SettingsDesign.DisplayCityBubbleType;
            var settingValueString = settingValue == SettingsDesign.EDisplayCityBubbleType.NoDisplay 
                                        ? "false"
                                        : "true";
            var displayPopup = false;
            var expiresDate = new TimeSpan(364, 0, 0, 0, 0);

            if (string.IsNullOrEmpty(cookieValue) || cookieValue != settingValueString)
            {
                // Внутри меняется глобальный Last-Modified, но т.к. ZonePopover грузится в ту же секунду, что и страница, браузер считает страницу не измененной ...
                CommonHelper.SetCookie("zonePopoverVisible", settingValueString, expiresDate, httpOnly: false, setOnMainDomain: true);
                displayPopup = settingValue != SettingsDesign.EDisplayCityBubbleType.NoDisplay;
            }

            if (!displayPopup)
                return new EmptyResult();

            if (settingValue == SettingsDesign.EDisplayCityBubbleType.DisplayZonePopover)
            {
                var zoneCity = IpZoneContext.CurrentZone.City == string.Empty ? SettingsDesign.DefaultCityIfNotAutodetect : IpZoneContext.CurrentZone.City;
                return PartialView(new ZonePopoverViewModel() { City = zoneCity });
            }
                
            
            if (settingValue == SettingsDesign.EDisplayCityBubbleType.DisplayZoneDialog)
                return Content("<div data-zone-dialog-trigger show-immediately=\"true\"></div>");
            
            return new EmptyResult();
        }

        [HttpGet]
        public JsonResult GetZoneDialogSettings()
        {
            return Json(new
            {
                hideCountries = SettingsDesign.HideCountriesInZoneDialog,
                hideSearch = SettingsDesign.HideSearchInZoneDialog
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ResetLastModified()
        {
            // ... сбрасываем глобальный Last-Modified после отображения всплывашки
            Core.Common.DataModificationFlag.ResetLastModified();
            return Content(null);
        }

        [ChildActionOnly]
        public ActionResult Captcha(string ngModel, string ngModelSource = null, string captchaId = null, CaptchaMode? captchaMode = null, int? codeLength = null, string captchaCode = null)
        {
            var model = new CaptchaViewModel()
            {
                CaptchaId = captchaId,
                NgModel = ngModel,
                NgModelSource = ngModelSource,
                CaptchaMode = captchaMode != null ? captchaMode.Value : SettingsMain.CaptchaMode,
                CodeLength = codeLength != null ? codeLength.Value : SettingsMain.CaptchaLength,
                CaptchaCode = captchaCode
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MenuGeneral()
        {
            var currentMode = GetMainPageMode();
            
            var menuHandler = new MenuHandler();

            var model = new MenuViewModel
            {
                ViewMode = SettingsDesign.eMenuStyle.Classic,
                DisplayProductsCount = SettingsCatalog.ShowProductsCount
            };
            
            if(currentMode != SettingsDesign.eMainPageMode.Default)
            {
                model.MenuItems = menuHandler.GetMenuItems();
                model.isMainMenu = true;
            }
            else
            {
                var menuItemModel = menuHandler.GetCatalogMenuItems(0, limited: true);
                model.MenuItems = menuItemModel.SubItems;
                model.RootUrlPath = menuItemModel.UrlPath;
                model.MenuItemsLimited = menuItemModel.SubItemsLimited;
                model.isMainMenu = false;
            }

            return PartialView("MenuGeneral", model);

        }

        [ChildActionOnly]
        public ActionResult MenuBlock()
        {
            var model = new MenuBlockViewModel();
            
            switch (GetMainPageMode())
            {
                case SettingsDesign.eMainPageMode.Default:
                    model.Layout = "_ColumnsOne";
                    break;
                case SettingsDesign.eMainPageMode.TwoColumns:
                    model.Layout = "_ColumnsTwo";
                    break;
                case SettingsDesign.eMainPageMode.ThreeColumns:
                    model.Layout = "_ColumnsThree";
                    break;
            }

            return PartialView("_Menu", model);
        }

        [ChildActionOnly]
        public ActionResult SocialButtons()
        {
            if (!SettingsSocial.SocialShareEnabled || SettingsSocial.SocialShareCustomCode.IsNullOrEmpty())
                return new EmptyResult();
            
            return PartialView(new SocialButtonsViewModel());
        }

        [ChildActionOnly]
        public ActionResult DiscountByTime()
        {
            var discountsByTime = DiscountByTimeService.GetCurrentDiscountsByTime();
            var popupTextList = new List<string>();
            var timeNow = DateTime.Now.TimeOfDay;
            foreach (var discountByTime in discountsByTime)
            {
                var cookieName = "discountbytime" + discountByTime.Id;
                if (discountByTime.ShowPopup 
                    && discountByTime.TimeFrom <= timeNow
                    && discountByTime.TimeTo > timeNow
                    && CommonHelper.GetCookieString(cookieName).IsNullOrEmpty())
                {
                    CommonHelper.SetCookie(cookieName, "true", new TimeSpan(12, 0, 0), true);
                    popupTextList.Add(discountByTime.PopupText);
                }
            }
            if (popupTextList.Count > 0)
                return PartialView("DiscountByTime", popupTextList);
            return new EmptyResult();
        }

        public ActionResult CancelTemplatePreview()
        {
            if (CustomerContext.CurrentCustomer.IsAdmin)
            {
                SettingsDesign.PreviewTemplate = null;
                CacheManager.Clean();
            }

            var url = Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : SettingsMain.SiteUrl;

            return Redirect(url);
        }

        public ActionResult ApplyTemplate()
        {
            var previewTemplate = SettingsDesign.PreviewTemplate;

            if (CustomerContext.CurrentCustomer.IsAdmin && previewTemplate != null)
            {
                SettingsDesign.ChangeTemplate(previewTemplate);
                CacheManager.Clean();
            }

            var url = Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : SettingsMain.SiteUrl;

            return Redirect(url);
        }

        public ActionResult LiveCounter()
        {
            return Content("");
        }

        [ChildActionOnly]
        public ActionResult Inplace(bool enabled, string inplaceMinAsset = "inplaceMin", string inplaceMaxAsset = "inplaceMax", string  areaBundle = "")
        {

            var model = new InplaceViewModel()
            {
                Enabled = enabled,
                InplaceMinAsset = inplaceMinAsset,
                InplaceMaxAsset = inplaceMaxAsset,
                AreaBundle = areaBundle
            };

            return PartialView(model);
        }

        [HttpPost]
        [Auth(RoleAction.Store)]
        public JsonResult ResizePictures()
        {
            if (CommonStatistic.IsRun) 
                return Json(new CommandResult() { Result = false, Error = T("Admin.Designs.NotPossibleToCompressPhotos") });

            try
            {
                CommonStatistic.StartNew(() =>
                {
                    CommonStatistic.TotalRow = PhotoService.GetCountPhotos(0, PhotoType.Product);
                    Helpers.FileHelpers.ResizeAllProductPhotos();
                },
                    "settingstemplate#?settingsTab=catalog",
                    "Пережатие фотографии товаров");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Design_ResizePictures);

            return Json(new CommandResult() { Result = true });
        }
        
        [HttpPost]
        [Auth(RoleAction.Store)]
        public JsonResult ResizeCategoryPictures(CategoryImageType type)
        {
            if (CommonStatistic.IsRun) 
                return Json(new CommandResult() { Result = false, Error = T("Admin.Designs.NotPossibleToCompressPhotos") });

            try
            {
                CommonStatistic.StartNew(() =>
                    {
                        FileHelpers.ResizeCategoryPhotos(type);
                    },
                    "settingstemplate#?settingsTab=catalog",
                    "Пережатие фотографии товаров");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return Json(new CommandResult() { Result = true });
        }

        public ActionResult Counter(bool force = false)
        {
            if ((TrialService.IsTrialEnabled && CustomerContext.CurrentCustomer.IsAdmin) || force)
                return Content(TrialService.TrialCounter);
            else
                return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult TrialBuilder()
        {
            return new EmptyResult();
        }

        public ActionResult SvgSprite(string name, string cssClass, string svgAttributes, string spriteFileName, string areaName)
        {
            var model = new SvgSprite(name, cssClass, svgAttributes, spriteFileName, areaName);

            return PartialView("SvgSprite", model);
        }

        public ActionResult FontsStyles(List<string> fileNameSourceList)
        {
            List<string> files = CacheManager.Get<List<string>>("fontStylesExist_" + SettingsDesign.Template + (SettingsDesign.IsMobileTemplate ? "_Mobile":""), () =>
            {
                return fileNameSourceList.Where(x => System.IO.File.Exists(Server.MapPath(x + ".css"))).ToList();
            });

            if (files.Count == 0)
            {
                return new EmptyResult();
            }

            bool useCDNFonts = CDNService.UseCDN(eCDN.Fonts);

            var model = new FontsViewModel()
            {
                UseCDNFonts = useCDNFonts,
                FileNameSourceList = files
            };

            return PartialView("_FontsStyles", model);
        }
        
        public ActionResult ZoneDialogTrigger()
        {
            var customer = CustomerContext.CurrentCustomer;
            var mainCustomerContact = customer.Contacts.Find(x => x.IsMain) ?? customer.Contacts.FirstOrDefault();
            var isShowPickPoint = TemplateSettingsProvider.Items["ShowDeliveryInDeliveryWidgetOnMain"].TryParseBool();
            var model = new ZoneDialogTrigger()
            {
                CurrentContact = mainCustomerContact,
                CurrentCity = IpZoneContext.CurrentZone.City,
                EnablesAddressList = customer.RegistredUser && isShowPickPoint,
            };
            return PartialView("ZoneDialogTrigger", model);
        }
    }

    // Common controller with session state
    public partial class CommonExtController : BaseClientController
    {
        [HttpPost]
        public ActionResult GetCaptchaHtml(string ngModel, string ngModelSource = null, string captchaId = null, string captchaCode = null)
        {
            var model = new CaptchaViewModel()
            {
                NgModel = ngModel,
                NgModelSource = ngModelSource,
                CaptchaId = captchaId,
                CaptchaCode = captchaCode
            };

            return PartialView("~/Views/Common/Captcha.cshtml", model);
        }
    }
}