using System.Web.Mvc;
using AdvantShop.Core.Services.Admin;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class AdminAreaRedirectAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction ||
                (filterContext.RequestContext.HttpContext != null && filterContext.RequestContext.HttpContext.Request.RequestType == "POST"))
                return;
            
            var url = filterContext.RequestContext.HttpContext.Request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            var urlWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

            if (urlWhitoutQuery.Contains("adminv2")) //&& AdminAreaTemplate.Template != "adminv2")
            {
                filterContext.Result = new RedirectResult(url.Replace("adminv2", "adminv3")); // AdminAreaTemplate.Template
            }
        }
    }
}
