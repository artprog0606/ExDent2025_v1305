using System.Web.Mvc;
using AdvantShop.App.Landing.Domain;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.App.Landing.Filters
{
    /// <summary>
    /// Check if exist landing page by "lpId". Only for Json.
    /// </summary>
    public class CheckLpAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var lpId = filterContext.RequestContext.HttpContext.Request["lpId"];

            Lp lp = null;

            if (!string.IsNullOrEmpty(lpId))
            {
                lp = new LpService().Get(lpId.TryParseInt());
                if (lp != null)
                    LpService.CurrentLanding = lp;
            }

            if (lp == null)
                filterContext.Result = new JsonNetResult() {Data = new {result = false, errors = new[] {"wrong lp"}}};
        }
    }
}
