using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Infrastructure.Filters
{
    /// <summary>
    /// Check trial on technical domain
    /// </summary>
    public class TechDomainGuardAttribute : ActionFilterAttribute
    {

        public bool Disable { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
                return;

            if (filterContext.IsChildAction)
                return;

            if (Disable)
                return;

            var request = filterContext.RequestContext.HttpContext.Request;
            if (request.IsTechDomainClosed())
            {
                filterContext.Result = new TransferResult("~/errorext/techdomainclosed");
            }
        }
    }
}