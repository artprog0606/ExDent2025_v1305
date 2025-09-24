using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Design;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Events;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Filters.Headers;

namespace AdvantShop.Controllers
{
    [TechDomainGuard]
    [AdminMobileAppGuard]
    [MobileApp]
    [AccessBySettings]
    [IsStoreClosed]
    [LogUserActivity]
    [XXssProtection(XXssProtectionPolicy.FilterEnabled, true)]
    [XFrameOptions(XFrameOptionsPolicy.SameOrigin)]
    [CheckReferral]
    [Warehouse]
    [CustomerEnabled]
    public abstract class BaseClientController : BaseController
    {
        protected ActionResult Error404()
        {
            Debug.Log.Error(new HttpException(404, $"Path '{HttpContext.Request.RawUrl}' not found."));

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "NotFound");

            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(HttpContext, routeData));
            return new EmptyResult();
        }

        protected void SetNgController(NgControllers.NgControllersTypes controllerName)
        {
            LayoutExtensions.NgController = controllerName;
        }

        protected void WriteLog(string name, string url, ePageType type)
        {
            if (Helpers.BrowsersHelper.IsBot())
                return;

            var @event = new Event
            {
                Name = name,
                Url = url,
                EvenType = type
            };

            var loger = LoggingManager.GetEventLogger();
            loger.LogEvent(@event);
        }

        protected ActionResult RedirectToReferrerOnPost(string action)
        {
            if (Request.GetUrlReferrer() != null)
                return Redirect(Request.GetUrlReferrer().ToString());

            return RedirectToAction(action);
        }

        protected SettingsDesign.eMainPageMode GetMainPageMode()
        {
            var mainPageModeRawFromRequest = Request["mainPageMode"].IsNotEmpty()? Request["mainPageMode"] : null;
            
            var isCompleteParse = false;
            SettingsDesign.eMainPageMode mainPageModeFromRequest = SettingsDesign.eMainPageMode.Default;
            
            if (mainPageModeRawFromRequest != null)
            {
                isCompleteParse = Enum.TryParse<SettingsDesign.eMainPageMode>(mainPageModeRawFromRequest, out mainPageModeFromRequest);
            }
            
            return isCompleteParse  && DebugMode.IsDebugMode(eDebugMode.CriticalCss) 
                ? mainPageModeFromRequest 
                : !Demo.IsDemoEnabled || !CommonHelper.GetCookieString(DesignConstants.DemoCookie_Design_MainPageMode).IsNotEmpty()
                ? SettingsDesign.MainPageMode
                : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString(DesignConstants.DemoCookie_Design_MainPageMode));
        }
    }
}