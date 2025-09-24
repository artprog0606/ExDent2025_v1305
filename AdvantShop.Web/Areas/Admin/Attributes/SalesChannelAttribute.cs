using System.Web.Mvc;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Controllers.Shared;

namespace AdvantShop.Web.Admin.Attributes
{
    public class SalesChannelAttribute : ActionFilterAttribute
    {
        private readonly ESalesChannelType _type;

        public SalesChannelAttribute(ESalesChannelType type)
        {
            _type = type;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                var channel = SalesChannelService.GetByType(_type);
                if (channel == null || !channel.Enabled)
                {
                    var customer = CustomerContext.CurrentCustomer;
                    if (customer.Enabled && (customer.IsAdmin || customer.IsVirtual || customer.IsModerator || CustomerContext.IsDebug))
                    {
                        filterContext.Result = (ViewResult) new ServiceController().GetSalesChannel(_type);
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
