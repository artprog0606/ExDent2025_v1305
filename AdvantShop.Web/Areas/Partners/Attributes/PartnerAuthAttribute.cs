using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Areas.Partners.Attributes
{
    public class PartnerAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            var partner = PartnerContext.CurrentPartner;
            var urlHelper = new UrlHelper(filterContext.RequestContext, RouteTable.Routes);

            if (partner != null)
            {
                if (!partner.Enabled)
                    filterContext.Result = new RedirectResult(urlHelper.AbsoluteActionUrl("Blocked", "Account", new { area = "Partners" }));
                else if (!partner.RegistrationComplete)
                    filterContext.Result = new RedirectResult(urlHelper.AbsoluteActionUrl("finishregistration", "account", new { area = "partners" }));
                return;
            }

            var controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
            var url = urlHelper.AbsoluteRouteUrl("Partners_Login");

            var request = filterContext.RequestContext.HttpContext.Request;
            var referrer = request.Url != null ? request.Url.ToString() : "";

            if (referrer.Contains(CommonHelper.GetParentDomain()) && !referrer.Contains("login") && !referrer.EndsWith("partners"))
                url += "?from=" + filterContext.RequestContext.HttpContext.Request.RawUrl;

            filterContext.Result = new RedirectResult(url);
        }
    }
}