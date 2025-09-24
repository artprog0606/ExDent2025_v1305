using System.Web.Mvc;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Infrastructure.Filters
{
    /// <summary>
    /// Guard admin mobile app from leaving admin panel
    /// </summary>
    public class AdminMobileAppGuardAttribute : ActionFilterAttribute
    {
        public bool Disable { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            if (Disable)
                return;

            if (MobileHelper.IsMobileAdminApp())
            {
                filterContext.Result = new RedirectResult(UrlService.GetAdminUrl(useAdminAreaTemplates: false));
            }
        }
    }
}