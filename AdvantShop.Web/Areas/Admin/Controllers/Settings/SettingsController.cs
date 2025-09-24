using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Settings.ClearDatas;
using AdvantShop.Web.Admin.Handlers.Settings.Common;
using AdvantShop.Web.Admin.Handlers.Settings.Mobiles;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.ViewModels.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Tools.QrCode;
using AdvantShop.Core.Services.Configuration.Settings;
using System.Collections.Specialized;
using System.Web;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsController : BaseAdminController
    {
        #region Common settings

        public ActionResult Index()
        {
            var model = new GetCommonSettings().Execute();

            SetMetaInformation(T("Admin.Settings.Index.SettingsTitle"));
            return View("Common/Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(CommonSettingsModel model)
        {
            var isMobile = SettingsDesign.IsMobileTemplate;

            var referrerParams = new System.Web.Routing.RouteValueDictionary();
            if (HttpContext.Request.UrlReferrer != null)
            {
                var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                foreach (var nvKey in nv.AllKeys)
                {
                    if (referrerParams.ContainsKey(nvKey) is false)
                        referrerParams.Add(nvKey, nv.Get(nvKey));
                }
            }

            if (ModelState.IsValid)
            {
                var handler = new SaveCommonSettings(model);
                handler.Execute();

                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                    foreach (var error in modelState.Errors)
                    {
                        ShowMessage(NotifyType.Error, error.ErrorMessage);
                    }
            }

            var area = RouteData.DataTokens["area"];


            if (isMobile)
            {
                if(area != null && area.ToString().ToLower() != "adminv3")
                {
                    return RedirectToAction("Index");
                } 
                else
                {
                    return RedirectToAction("Common", referrerParams);
                }
            }
            else
            {
                return RedirectToAction(area != null && area.ToString().ToLower() != "adminv3" ? "Index" : "Common");
            }
        }

        #region Common

        [HttpPost]        
        public JsonResult GetRegions(int countryId)
        {
            return JsonOk(new GetRegions(countryId).Execute());
        }


        public ActionResult Common()
        {
            var model = new GetCommonSettings().Execute();

            SetMetaInformation(T("Admin.Settings.Index.SettingsTitle"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCtrl);

            return View("Common/CommonIndex", model);
        }
        
        #endregion

        #region Logo
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogo()
        {
            var result = new UploadLogoPictureHandler().Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogoByLink(string fileLink)
        {
            var result = new UploadLogoPictureByLinkHandler(fileLink).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLogo()
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
            SettingsMain.LogoImageName = string.Empty;
            SettingsMain.IsLogoImageSvg = false;
            SettingsMain.LogoImageWidth = 0;
            SettingsMain.LogoImageHeight = 0;
            return JsonOk(new { picture = "../images/nophoto_small.png" });
        }
        #endregion

        #region DarkThemeLogo

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadDarkThemeLogo()
        {
            var result = new UploadLogoPictureHandler(isDarkThemeLogo: true).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UploadDarkThemeLogoByLink(string fileLink)
        {
            var result = new UploadLogoPictureByLinkHandler(fileLink, isDarkThemeLogo: true).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteDarkThemeLogo()
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.DarkThemeLogoName));
            SettingsMain.DarkThemeLogoName = string.Empty;
            SettingsMain.DarkThemeLogoImageWidth = 0;
            SettingsMain.DarkThemeLogoImageHeight = 0;
            return JsonOk(new { picture = "../images/nophoto_small.png" });
        }

        #endregion

        #region LogoMobile
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogoMobile()
        {
            var uploadLogoPictureHandler = new UploadLogoPictureHandler(true);
            var result = uploadLogoPictureHandler.Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogoMobileByLink(string fileLink)
        {
            var uploadLogoPictureByLinkHandler = new UploadLogoPictureByLinkHandler(fileLink, true);
            var result = uploadLogoPictureByLinkHandler.Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLogoMobile()
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMobile.LogoImageName));
            SettingsMobile.LogoImageName = string.Empty;
            SettingsMobile.LogoImageWidth = 0;
            SettingsMobile.LogoImageHeight = 0;
            return JsonOk(new { picture = "../images/nophoto_small.png" });
        }
        #endregion

        #region LogoBlog
        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogoBlog()
        {
            var result = new UploadLogoPictureHandler(isLogoBlog:true).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UploadLogoBlogByLink(string fileLink)
        {
            var result = new UploadLogoPictureByLinkHandler(fileLink, isLogoBlog: true).Execute();

            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddLogo_AdminArea);

            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLogoBlog()
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoBlogImageName));
            SettingsMain.LogoBlogImageName = string.Empty;
            return JsonOk(new { picture = "../images/nophoto_small.png" });
        }
        #endregion

        #region Favicon

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFavicon()
        {
            var result = new UploadFaviconPictureHandler().Execute();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddFavicon);
            return result.Result
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UploadFaviconByLink(string fileLink)
        {
            var result = new UploadFaviconPictureByLinkHandler(fileLink).Execute();
            Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_AddFavicon);
            return result.Result 
                ? JsonOk(new { picture = result.Picture, pictureId = result.PictureId })
                : JsonError(result.Error);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFavicon()
        {
            if (SettingsMain.FaviconImageName != "favicon.icon")
            {
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));
            }
            
            SettingsMain.FaviconImageName = string.Empty;
            SettingsMain.IsFaviconImageSvg = false;
            return JsonOk(new { picture = "../images/nophoto_small.png" });
        }

        #endregion

        #endregion

        #region PaymentMethods

        public ActionResult PaymentMethods()
        {
            SetMetaInformation(T("Admin.Settings.PaymentMethods.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCtrl);

            // if (!SettingsCongratulationsDashboard.PaymentDone)
            // {
            //     SettingsCongratulationsDashboard.PaymentDone = true;
            //     if (SettingsCongratulationsDashboard.ShippingDone)
            //         Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_ShippingPaymentDone);
            // }

            return View("Payments/PaymentMethods", new PaymentMethodsSettingsModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentMethods(PaymentMethodsSettingsModel model)
        {
            return PaymentMethods();
        }

        #endregion

        #region ShippingMethods

        public ActionResult ShippingMethods()
        {
            SetMetaInformation(T("Admin.Settings.ShippingMethods.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCtrl);

            // if (!SettingsCongratulationsDashboard.ShippingDone)
            // {
            //     SettingsCongratulationsDashboard.ShippingDone = true;
            //     if (SettingsCongratulationsDashboard.PaymentDone)
            //         Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_ShippingPaymentDone);
            // }

            return View("Shippings/ShippingMethods", new ShippingMethodsSettingsModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ShippingMethods(ShippingMethodsSettingsModel model)
        {
            return ShippingMethods();
        }

        #endregion

        #region MobileVersion

        [SaasFeature(ESaasProperty.MobileVersion)]
        [SalesChannel(ESalesChannelType.Store)]
        public ActionResult MobileVersion()
        {
            var model = new LoadSaveMobileSettingsHandler().Get();

            SetMetaInformation(T("Admin.Settings.MobileVersion.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsMobileCtrl);

            return View("Mobile/MobileVersion", model);
        }

        [SaasFeature(ESaasProperty.MobileVersion)]
        public JsonResult GetMobileTemplateSettings(string template)
        {
            return Json(new LoadSaveMobileSettingsHandler().GetSettings(template));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SaasFeature(ESaasProperty.MobileVersion)]
        [SalesChannel(ESalesChannelType.Store)]
        public ActionResult MobileVersion(MobileVersionSettingsModel model)
        {
            new LoadSaveMobileSettingsHandler(model).Save();

            ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
            return RedirectToAction("MobileVersion");
        }

        public ActionResult GetMobileVersionQrCode()
        {
            var url = @UrlService.GetUrl();
            var stream = QrCodeService.GetQrCode(url, 4);

            return new FileStreamResult(stream, "image/png");
        }

        #endregion

        #region Users

        public ActionResult UsersSettings()
        {
            SetMetaInformation(T("Admin.Settings.Users.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsUsersCtrl);

            var saasData = SaasDataService.CurrentSaasData;
            var employeesCount = EmployeeService.GetEmployeeCount();
            var employeesLimit = SaasDataService.IsSaasEnabled
                ? saasData.EmployeesCount
                : int.MaxValue;

            var model = new UsersSettingsViewModel
            {
                UsersViewModel = new UsersViewModel
                {
                    ManagersCount = ManagerService.GetManagersCount(),
                    ManagersLimitation = SaasDataService.IsSaasEnabled,
                    ManagersLimit = SaasDataService.IsSaasEnabled ? saasData.EmployeesCount : 0,

                    EmployeesCount = employeesCount,
                    EmployeesLimit = employeesLimit,
                    EnableEmployees = !SaasDataService.IsSaasEnabled || (SaasDataService.IsSaasEnabled && employeesCount < employeesLimit),

                    EnableManagersModule = true,
                },

                ManagersOrderConstraint = SettingsManager.ManagersOrderConstraint,
                ManagersLeadConstraint = SettingsManager.ManagersLeadConstraint,
                ManagersCustomerConstraint = SettingsManager.ManagersCustomerConstraint,
                ManagersTaskConstraint = SettingsManager.ManagersTaskConstraint,
                ActiveTwoFactorAuthModule = SettingsManager.ActiveTwoFactorAuthModule,
            };

            return View("Users/UsersSettings", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UsersSettings(UsersSettingsViewModel model)
        {
            SettingsManager.ManagersOrderConstraint = model.ManagersOrderConstraint;
            SettingsManager.ManagersLeadConstraint = model.ManagersLeadConstraint;
            SettingsManager.ManagersCustomerConstraint = model.ManagersCustomerConstraint;
            SettingsManager.ManagersTaskConstraint = model.ManagersTaskConstraint;
            SettingsManager.ActiveTwoFactorAuthModule = model.ActiveTwoFactorAuthModule;

            ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));

            var isMobile = SettingsDesign.IsMobileTemplate;

            var referrerParams = new System.Web.Routing.RouteValueDictionary();
            if (HttpContext.Request.UrlReferrer != null)
            {
                var nv = HttpUtility.ParseQueryString(HttpContext.Request.UrlReferrer.Query); //Преобразует урлхвост в NameValueCollection

                foreach (var nvKey in nv.AllKeys)
                {
                    if (referrerParams.ContainsKey(nvKey) is false)
                        referrerParams.Add(nvKey, nv.Get(nvKey));
                }
            }

            if (isMobile)
            {
                return RedirectToAction("Userssettings", referrerParams);
            }
            else
            {
                return UsersSettings();
            }

        }

        #endregion

        #region Files

        public ActionResult Files()
        {
            SetMetaInformation(T("Admin.Settings.System.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCtrl);

            return View("Files/Files", new FilesSettingsModel());
        }

        #endregion
        
        #region ClearTrialData

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearTrialData()
        {
            new ClearTrialDataHandler().Execute();
            return Json(new {result = true});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ClearData(ClearDataViewModel model)
        {
            new ClearDataSettingsHandler(model).Execute();
            return Json(new {result = true});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ConvertToStandardPhone(string phone)
        {
            phone = StringHelper.RemoveHTML(phone);

            var phoneFormatted = StringHelper.ConvertToStandardPhone(phone, true, true).ToString();
            if (phone.StartsWith("+") || (phoneFormatted.StartsWith("7") && phoneFormatted.Length == 11))
                phoneFormatted = "+" + phoneFormatted;

            return JsonOk(phoneFormatted);
        }
        #endregion
    }
}
