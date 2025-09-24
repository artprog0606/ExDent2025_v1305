using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Events;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [TechDomainGuard]
    [AccessBySettings(EProviderSetting.StoreActive, ETypeRedirect.LandingSite)]
    [IsStoreClosed]
    [LogUserActivity]
    [CheckReferral]
    [MobileSession]
    [Warehouse]
    public class BaseMobileController : BaseController
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
            if (Helpers.BrowsersHelper.IsBot()) return;

            var @event = new Event
            {
                Name = name,
                Url = url,
                EvenType = type
            };

            var logger = LoggingManager.GetEventLogger();
            logger.LogEvent(@event);
        }
    }
}