using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Filters.Headers
{
    public abstract class HttpHeaderAttributeBase : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            
            SetHttpHeadersOnActionExecuted(filterContext);
        }

        public abstract void SetHttpHeadersOnActionExecuted(ActionExecutedContext filterContext);
    }
}
