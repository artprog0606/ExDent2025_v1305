using System;
using System.Web.Mvc;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Infrastructure.Filters.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnyCrossOriginResourceSharingAttribute : HttpHeaderAttributeBase
    {
        public override void SetHttpHeadersOnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeaderIfNotExists(HeaderConstants.AccessControlAllowOrigin, "*");
            filterContext.HttpContext.Response.AddHeaderIfNotExists(HeaderConstants.AccessControlAllowMethods, "*");
            filterContext.HttpContext.Response.AddHeaderIfNotExists(HeaderConstants.AccessControlAllowHeaders, "*");
            filterContext.HttpContext.Response.AddHeaderIfNotExists(HeaderConstants.AccessControlAllowCredentials, "true");
        }
    }
}