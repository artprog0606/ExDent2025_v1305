using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class CheckReferralAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                return;
            }

            if (filterContext.IsChildAction || filterContext.HttpContext == null || !SettingsMain.PartnersActive)
                return;

            var request = filterContext.HttpContext.Request;
            if (request.IsAjaxRequest() || request.HttpMethod == "POST" || Helpers.BrowsersHelper.IsBot())
                return;

            PartnerService.SetReferralCookie(request);
        }
    }
}
