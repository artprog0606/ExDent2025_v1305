using System.Web.Mvc;
using AdvantShop.Customers;
using log4net;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class LogActivityAttribute : ActionFilterAttribute
    {
        private static readonly ILog Log = LogManager.GetLogger("LogActivity");

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction || filterContext.HttpContext == null)
                return;

            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            if (request.RequestType == "GET" && response.ContentType == "application/json")
                return;

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsVirtual)
                return;

            Log.Info(
                string.Format("{0} {1} {2}",
                    !string.IsNullOrEmpty(customer.EMail)
                        ? customer.EMail
                        : customer.FirstName + " " + customer.LastName,
                    request.RequestType,
                    request.RawUrl));
        }
    }
}
