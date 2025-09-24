using System;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Services;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Areas.Api.Attributes
{
    /// <summary>
    /// Authorization attribute for API methods with required CustomerContext.CurrentCustomer.
    /// Set in response header X-API-USER-ID
    /// </summary>
    public class AuthUserApiAttribute: ActionFilterAttribute
    {
        public bool UseCookie { get; set; }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authApiUserId = (filterContext.HttpContext.Request.Headers["X-API-USER-ID"] ?? "").TryParseGuid();
            var authApiUserKey = filterContext.HttpContext.Request.Headers["X-API-USER-KEY"];
            var apiAuthorizeService = new ApiAuthorizeService();

            if (authApiUserId == Guid.Empty)
            {
                var customer = apiAuthorizeService.GetNotExistCustomer();

                SetCustomer(customer);
                
                authApiUserId = customer.Id;
                filterContext.HttpContext.Response.Headers.Remove("X-API-USER-ID");
                filterContext.HttpContext.Response.Headers.Add("X-API-USER-ID", authApiUserId.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(authApiUserKey))
            {
                if (!TrySetCustomerByUserKey(authApiUserId, authApiUserKey))
                    filterContext.Result = ErrorResult("Invalid X-API-USER-KEY or X-API-USER-ID");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!UseCookie)
                filterContext.HttpContext.Response.Cookies.Clear();
        }

        private bool TrySetCustomerByUserKey(Guid customerId, string authApiUserKey)
        {
            var customer = CustomerService.GetCustomer(customerId);
            if (customer == null) 
                return false;
            
            if (authApiUserKey != new ApiAuthorizeService().CreateUserKey(customer))
                return false;

            SetCustomer(customer);
            return true;
        }

        private void SetCustomer(Customer customer)
        {
            HttpContext.Current.Items["CustomerContext"] = customer; // set CustomerContext.CurrentCustomer
        }

        private ActionResult ErrorResult(string error)
        {
            return new JsonNetResult {Data = new ApiError(error)};
        }
    }
}