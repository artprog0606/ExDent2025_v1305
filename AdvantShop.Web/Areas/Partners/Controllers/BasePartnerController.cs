using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using AdvantShop.Areas.Partners.Attributes;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Filters.Headers;

namespace AdvantShop.Areas.Partners.Controllers
{
    [PartnerAuth]
    [SessionState(SessionStateBehavior.Disabled)]
    [XFrameOptions(XFrameOptionsPolicy.Deny)]
    [XXssProtection(XXssProtectionPolicy.FilterDisabled, false)]
    [AccessBySettings(EProviderSetting.PartnersActive, ETypeRedirect.Error404)]
    public class BasePartnerController : BaseController
    {
        protected void SetNgController(NgControllers.NgControllersTypes controllerName)
        {
            LayoutExtensions.NgController = controllerName;
        }

        protected ActionResult Error404()
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "NotFound");
            routeData.DataTokens.Add("area", "Partners");

            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(HttpContext, routeData));
            return new EmptyResult();
        }

        protected void ShowErrorMessages()
        {
            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    ShowMessage(NotifyType.Error, string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage);
        }
    }
}