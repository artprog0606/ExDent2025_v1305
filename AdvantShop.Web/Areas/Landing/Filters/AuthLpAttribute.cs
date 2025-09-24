using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Customers;

namespace AdvantShop.App.Landing.Filters
{
    public class AuthLpAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator && !HasRole(customer))
            {
                HttpContext.Current.Server.TransferRequest("error/notfound");
            }
        }

        private bool HasRole(Customer customer)
        {
            var customerRolesActions = RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id);

            return customerRolesActions.Any(item => item.Role == RoleAction.Landing);
        }
    }
}
