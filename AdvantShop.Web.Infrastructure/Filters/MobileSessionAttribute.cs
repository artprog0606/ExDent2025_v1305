using AdvantShop.Helpers;
using System;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MobileSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext) => MobileHelper.SetMobileSessionCookie();
    }
}
