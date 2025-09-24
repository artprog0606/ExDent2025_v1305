using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Web.Admin.Controllers.Shared;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Filters.Headers;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Controllers
{
    [AdminAuth]
    [AdminAreaRedirect]
    [SessionState(SessionStateBehavior.Disabled)]
    [XFrameOptions(XFrameOptionsPolicy.Deny)]
    [XXssProtection(XXssProtectionPolicy.FilterDisabled, false)]
    public partial class BaseAdminController : BaseController
    {
        public BaseAdminController()
        {
            var useMobileLayout = System.Web.HttpContext.Current.Request["useMobileLayout"];
            ViewBag.useMobileLayout = useMobileLayout != null ? useMobileLayout.TryParseBool() : useMobileLayout.TryParseBool();
        }
        protected void SetNgController(NgControllers.NgControllersTypes controllerName)
        {
            LayoutExtensions.NgController = controllerName;
        }

        protected ActionResult Error404(bool partial = false)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", partial ? "NotFoundPartial" : "NotFound");
            routeData.DataTokens.Add("area", AdminAreaTemplate.IsAdminv3() ? "AdminV3" : "AdminV2");

            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(HttpContext, routeData));
            return new EmptyResult();
        }

        protected string RenderPartialToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        protected void ShowErrorMessages()
        {
            foreach (var modelState in ViewData.ModelState.Values)
                foreach (var error in modelState.Errors)
                    ShowMessage(NotifyType.Error, string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage);
        }
    }
}