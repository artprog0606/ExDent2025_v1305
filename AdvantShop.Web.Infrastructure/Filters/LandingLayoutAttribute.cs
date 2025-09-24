using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Customers;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class LandingLayoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            
            if (LandingHelper.IsLandingDomain(filterContext.HttpContext.Request.Url, out _))
            {
                filterContext.RouteData.DataTokens.Add("MasterLayout", "_LayoutEmpty");
            }
        }
    }
}
