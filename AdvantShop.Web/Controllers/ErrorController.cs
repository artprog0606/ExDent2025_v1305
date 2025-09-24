using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Models.Error;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Trial;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [ExcludeFilter(typeof(SaasStoreAttribute))]
    public class ErrorController : BaseClientController
    {
        // some error we dont know
        public ActionResult Index()
        {
            SetResponse(HttpStatusCode.InternalServerError);
            return View();
        }

        // 400
        public ActionResult BadRequest()
        {
            SetResponse(HttpStatusCode.BadRequest);
            return View();
        }

        // 404
        public ActionResult NotFound()
        {
            SetResponse(HttpStatusCode.NotFound);

            var showEmptyLayout = false;
            try
            {
                var path = Request.RawUrl.ToLower();
                var index = path.IndexOf('?');
                var pathWithoutQuery = index > 0 ? path.Substring(0, index) : path;

                var ext = FileHelpers.GetExtension(pathWithoutQuery);
                if (!string.IsNullOrEmpty(ext))
                {
                    var list = new List<string>
                    {
                        ".css", ".js", ".jpg", ".jpeg", ".png", ".bmp", ".map", ".ico", ".gif", ".svg", ".webp", ".cur",
                        ".tif", ".tiff", ".woff", ".woff2"
                    };
                    if (list.Contains(ext))
                        return new EmptyResult();

                    // Debug.Log.Error("Not found " + Request.RawUrl, new HttpException(404, "Not found " + Request.RawUrl));
                    var err404 = new Error404(System.Web.HttpContext.Current.Request);
                    Error404Service.AddError404(err404);
                }
            }
            catch (ArgumentException) 
            {
                // сработает на GetExtension, если в пути есть невалидные символы
                // обрабаываем отдельно, потому что падает любой вызов @Html.Action 
                showEmptyLayout = true;
            }
            catch
            {
                // ignored
            }

            SetMetaInformation(T("Error.NotFound.Title"));
            SetNgController(NgControllers.NgControllersTypes.ErrorCtrl);

            if (showEmptyLayout)
                return View("NotFound", "NotFoundEmptyLayout");

            return SettingsDesign.IsEmptyLayout 
                ? View("NotFound",  "_LayoutEmpty", false) 
                : View();
        }

        // 403
        public ActionResult Forbidden()
        {
            SetResponse(HttpStatusCode.Forbidden);
            return View();
        }

        // 500
        public ActionResult InternalServerError()
        {
            SetResponse(HttpStatusCode.InternalServerError);
            return View();
        }


        public ActionResult SessionError(int? errorCode, string errorMsg)
        {
            string errorMessage = "";
            switch (errorCode)
            {
                case 1:
                    errorMessage = Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ?
                        "База данных не доступна.<br/> Проверьте строку подключения в файле 'Web.ConnectionString.config'" :
                        "Database is not available.<br/> Check the connection string in file 'Web.ConnectionString.config'";
                    break;

                case 2:
                    var dbversion = Core.DataBaseService.GetkDBVersionFomDatabase();
                    var configVersion = Core.DataBaseService.GetDbVersionFromConfig();

                    errorMessage = string.Format(Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ? 
                            "Неверная версия базы данных:<br/>Версия в базе данных - '{0}', версия из файла web.config - '{1}'" :
                            "Incorrect database version:<br/>Version from database is '{0}', web.config version is '{1}'", dbversion, configVersion);
                    break;
                case 3:
                    errorMessage = Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ?
                                "Неверная база данных" : "Invalid database";
                    break;
                case 4:
                    errorMessage = Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ?
                        "Некорректная база данных" : "Incorrect dataBase";
                    break;
                default:
                    errorMessage = errorMsg ?? (Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ? "Неизвестная ошибка" :"Unknown error");
                    break;
            }

            var header = Localization.Culture.CurrentCulture == Localization.Culture.SupportLanguage.Russian ? "Внутренняя ошибка сервера" : "Internal Server Error";

            SetResponse(HttpStatusCode.InternalServerError);
            return View(new KeyValuePair<string, string>(header, errorMessage));
        }

        public ActionResult LicCheck()
        {
            if (SettingsLic.ActiveLic && TrialService.IsTrialEnabled)
                return RedirectToRoute("Home");

            if (SettingsLic.ActiveLic && SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.IsCorrect)
                return RedirectToRoute("Home");

            if (!string.IsNullOrEmpty(SettingsLic.LicKey) && ChecLicKey(SettingsLic.LicKey))
            {
                if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.IsCorrect)
                {
                    return View(new LicCheckModel() { Msg = "Check Web.ModeSettings.config" });
                }
                //return RedirectToRoute("Home");
            }

            return View(new LicCheckModel());
        }

        [HttpPost]
        public ActionResult LicCheck(LicCheckModel model)
        {
            if (SettingsLic.ActiveLic && SaasDataService.CurrentSaasData.IsCorrect)
                return RedirectToRoute("Home");

            var viewModel = model;

            if (string.IsNullOrWhiteSpace(model.Key))
                return View(viewModel);

            if (ChecLicKey(model.Key))
            {
                SettingsLic.ActiveLic = true;
                SettingsLic.LicKey = model.Key;
                SaasDataService.GetSaasData(true);
                return RedirectToRoute("Home");
            }
            else
            {
                viewModel.Msg = "key is wrong";
            }

            return View(viewModel);
        }

        private void SetResponse(HttpStatusCode httpStatusCode)
        {
            try
            {
                Response.Clear();
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)httpStatusCode;
                Response.StatusDescription = HttpWorkerRequest.GetStatusDescription((int)httpStatusCode);
            }
            catch
            {
                // ignored
            }
        }

        private bool ChecLicKey(string key)
        {
            return SettingsLic.Activate(key);
        }
    }


    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    [ExcludeFilter(typeof(TechDomainGuardAttribute), typeof(SaasStoreAttribute), typeof(AccessBySettings),
        typeof(CheckReferralAttribute), typeof(MobileAppAttribute), typeof(IsStoreClosedAttribute))]
    public class ErrorExtController : BaseClientController
    {
        public ActionResult TechDomainClosed()
        {
            SettingsDesign.IsMobileTemplate = false;

            Track.TrackService.TrackEvent(Track.ETrackEvent.ClientBlocker_Visited, dontLogBotsUsers: true);
            return View("~/Views/Error/TechDomainClosed.cshtml");
        }

        public ActionResult MobileAppBlocked()
        {
            return View("~/Views/Error/MobileAppBlocked.cshtml");
        }
    }
}