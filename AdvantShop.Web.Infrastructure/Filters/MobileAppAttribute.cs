using System;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Infrastructure.Filters
{
    /// <summary>
    /// Check for the support of the mobile application by the saas tariff
    /// </summary>
    public class MobileAppAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
                return;

            if (filterContext.IsChildAction)
                return;
            
            if (filterContext.RequestContext.HttpContext.Request.IsMobileAppNotAvailable())
            {
                filterContext.Result = new TransferResult("~/errorext/mobileAppBlocked");
            }
        }
    }
}
