using System.Web.Mvc;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Areas.Api.Attributes
{
    public class BonusSystemAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!BonusSystem.IsActive)
            {
                filterContext.Result = new JsonNetResult { Data = new ApiError("Бонусная система не активна") };
            }
        }
    }
}