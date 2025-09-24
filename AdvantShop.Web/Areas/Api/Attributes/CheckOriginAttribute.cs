using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters.Headers;

namespace AdvantShop.Areas.Api.Attributes
{
    public class CheckOriginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var origin = filterContext.HttpContext.Request.Headers["Origin"];

            if (origin != null && origin.Contains("chrome-extension://"))
                filterContext.HttpContext.Response.AddHeaderIfNotExists(HeaderConstants.AccessControlAllowOrigin, origin);
        }
    }
}