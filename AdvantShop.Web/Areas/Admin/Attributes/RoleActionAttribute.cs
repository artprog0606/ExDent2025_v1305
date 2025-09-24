using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Controllers.Shared;

namespace AdvantShop.Web.Admin.Attributes
{
    [Flags]
    public enum EAuthErrorType
    {
        None = 0,
        Json = 1,
        Action = 2,
        View = 4,
        PartialView = 8
    }

    public enum EAuthKeysComparer : byte
    {
        Or = 0,
        And = 1
    }

    public class SkipRoleFilteringAttribute : ActionFilterAttribute
    {
    }

    public class AuthAttribute : ActionFilterAttribute
    {
        private List<RoleAction> _rolesActionKeys;
        private readonly EAuthErrorType _errorType;
        private readonly EAuthKeysComparer _keysComparer = EAuthKeysComparer.Or;

        public AuthAttribute()
        {
            _rolesActionKeys = new List<RoleAction>();
            _errorType = EAuthErrorType.Json | EAuthErrorType.Action | EAuthErrorType.View;
        }

        public AuthAttribute(params RoleAction[] keys)
        {
            _rolesActionKeys = new List<RoleAction>(keys);
            _errorType = EAuthErrorType.Json | EAuthErrorType.Action | EAuthErrorType.View;
        }
        
        public AuthAttribute(EAuthKeysComparer keysComparer, params RoleAction[] keys)
        {
            _keysComparer = keysComparer;
            _rolesActionKeys = new List<RoleAction>(keys);
            _errorType = EAuthErrorType.Json | EAuthErrorType.Action | EAuthErrorType.View;
        }

        public AuthAttribute(EAuthErrorType errorType, params RoleAction[] keys)
        {
            _errorType = errorType;
            _rolesActionKeys = new List<RoleAction>(keys);
        }

        public List<RoleAction> RolesAction
        {
            get => _rolesActionKeys;
            set => _rolesActionKeys = value;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator && !HasRole(customer, out var requiredRole))
            {
                var customAttributes =
                    filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipRoleFilteringAttribute), false);
                if (customAttributes.Length == 1)
                    return;

                var controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                var action = filterContext.RouteData.Values["action"].ToString().ToLower();

                if (!(controller.Equals("account") && action.Equals("login")))
                {
                    if (!filterContext.IsChildAction)
                    {
                        var role = requiredRole ?? RoleAction.None; //_rolesActionKeys.Any() ? _rolesActionKeys[0] : RoleAction.None;
                        var request = filterContext.RequestContext.HttpContext.Request;
                        
                        if (request.IsAjaxRequest() && _errorType.HasFlag(EAuthErrorType.Json))
                        {
                            filterContext.Result = new ServiceController().RoleAccessIsDeniedJson(role);
                        }
                        else if (_errorType.HasFlag(EAuthErrorType.PartialView))
                        {
                            filterContext.Result =
                                (PartialViewResult)new ServiceController().RoleAccessIsDenied(role, partial: true);
                        }
                        else
                            filterContext.Result = (ViewResult)new ServiceController().RoleAccessIsDenied(role);
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                    }
                }
            }
        }

        private bool HasRole(Customer customer, out RoleAction? requiredRole)
        {
            requiredRole = null;
            var customerRoles = RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id);

            if (_keysComparer == EAuthKeysComparer.Or)
            {
                foreach (var actionKey in _rolesActionKeys)
                {
                    if (actionKey != RoleAction.None && customerRoles.Any(item => item.Role == actionKey))
                        return true;
                }

                if (_rolesActionKeys.Count > 0)
                    requiredRole = _rolesActionKeys[0];

                return false;
            }
            
            if (_keysComparer == EAuthKeysComparer.And)
            {
                foreach (var role in _rolesActionKeys)
                {
                    if (customerRoles.Find(x => x.Role != role) == null)
                    {
                        requiredRole = role;
                        return false;
                    }
                }

                return true;
            }
            
            return false;
        }
    }
}