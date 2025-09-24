using System.Web.Mvc;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Security;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class CustomerEnabledAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CustomerContext.CurrentCustomer.Enabled) return;
            
            AuthorizeService.SignOut();
            filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("/login"));
        }
    }
}