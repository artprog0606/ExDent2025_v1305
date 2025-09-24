using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Controllers.Shared;

namespace AdvantShop.Web.Admin.Attributes
{
    public class SaasFeatureAttribute : ActionFilterAttribute
    {
        private ESaasProperty _saasMarker;
        private bool _partial;

        public SaasFeatureAttribute(ESaasProperty marker)
        {
            _saasMarker = marker;
        }

        public SaasFeatureAttribute(ESaasProperty marker, bool partial)
        {
            _saasMarker = marker;
            _partial = partial;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction && !SaasDataService.IsEnabledFeature(_saasMarker))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer.Enabled && (customer.IsAdmin || customer.IsVirtual || customer.IsModerator || CustomerContext.IsDebug))
                {
                    if (_partial)
                        filterContext.Result = (PartialViewResult)new ServiceController().GetFeature(_saasMarker.ToString(), partial: true);
                    else
                        filterContext.Result = (ViewResult)new ServiceController().GetFeature(_saasMarker.ToString());
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
