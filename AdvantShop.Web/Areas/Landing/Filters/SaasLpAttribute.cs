using System.Web.Mvc;
using AdvantShop.Saas;

namespace AdvantShop.App.Landing.Filters
{
    public class SaasLpAttribute : ActionFilterAttribute
    {
        private ESaasProperty saasMarker;

        public SaasLpAttribute()
        {
            saasMarker = ESaasProperty.LandingPage;
        }

        public SaasLpAttribute(ESaasProperty marker)
        {
            saasMarker = marker;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SaasDataService.IsEnabledFeature(saasMarker))
            {
                filterContext.Result = new RedirectResult("/service/getfeature?id=" + saasMarker);
            }
        }
    }
}