using System;
using System.IO;
using System.Web.Mvc;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Filters.Headers;

namespace AdvantShop.App.Landing.Controllers
{
    [TechDomainGuard]
    [AdminMobileAppGuard]
    [AccessBySettings(EProviderSetting.ActiveLandingPage, ESaasProperty.HaveLandingFunnel, ETypeRedirect.Error404)]
    [CheckReferral]
    [XXssProtection(XXssProtectionPolicy.FilterEnabled, true)]
    [XFrameOptions(XFrameOptionsPolicy.SameOrigin)]
    [LogUserActivity]
    [MobileApp]
    public class LandingBaseController : BaseController
    {
        protected void SetNgController(NgControllers.NgControllersTypes controllerName)
        {
            LayoutExtensions.NgController = controllerName;
        }

        protected ActionResult Error404()
        {
            System.Web.HttpContext.Current.Server.TransferRequest("~/landing/landing/error404page");
            return new EmptyResult();
        }

        protected bool ViewExist(string name)
        {
            var viewResult = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return viewResult.View != null;
        }

        protected string RenderPartialToString(string viewName, object model)
        {
            try
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
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return null;
            }
        }
    }
}
