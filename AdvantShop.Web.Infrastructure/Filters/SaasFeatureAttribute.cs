using AdvantShop.Core.UrlRewriter;
using AdvantShop.Saas;
using System.Web.Mvc;
using System;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Customers;
using System.Web.Routing;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class SaasFeatureInfAttribute : ActionFilterAttribute
    {
        private ESaasProperty saasMarker;

        public SaasFeatureInfAttribute(ESaasProperty marker)
        {
            saasMarker = marker;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction && !SaasDataService.IsEnabledFeature(saasMarker))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer.Enabled && (customer.IsAdmin || customer.IsVirtual || customer.IsModerator || CustomerContext.IsDebug))
                {
                    //filterContext.Result = (ViewResult)new ServiceController().GetFeature(saasMarker.ToString());

                    //filterContext.Result = new TransferResult(UrlService.GetAbsoluteLink("adminv2/service/getfeature/" + saasMarker));

                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "service" }, { "action", "getfeature" },{ "id", "saasMarker" } });
                    //new RedirectResult(UrlService.GetAbsoluteLink("adminv2/service/getfeature/" + saasMarker))

                    //filterContext.Result =(ViewResult) new ServiceController().GetFeature(saasMarker.ToString());
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /*
    
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SaasFeaturesExclusion.IsAvailblePage(filterContext.HttpContext.Request.Url.AbsoluteUri))
            {                
                filterContext.Result = new TransferResult(UrlService.GetAbsoluteLink("adminv2/service/getfeature/" + saasMarker));
                
                filterContext.RouteData.Values.Add("clentcontroller", "jopa");
            }

            base.OnActionExecuting(filterContext);
        }
        
        private ActionResult View(object p, object model)
        {
            throw new NotImplementedException();
        }
         */
    }
}
