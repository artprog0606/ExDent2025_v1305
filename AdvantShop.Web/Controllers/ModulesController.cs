using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class ModulesController : BaseClientController
    {
        public ActionResult RenderModules(string key, object routeValues = null)
        {
            if (DebugMode.IsDebugMode(eDebugMode.Modules))
                return Content("");

            var model = ModulesExtensions.GetModuleRoutes(key, routeValues);
            return PartialView("_Module", model);
        }
    }
}