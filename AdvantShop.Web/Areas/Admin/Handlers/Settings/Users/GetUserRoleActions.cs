using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Settings.Users;

namespace AdvantShop.Web.Admin.Handlers.Settings.Users
{
    public class GetUserRoleActions
    {
        private readonly Guid? _customerId;

        public GetUserRoleActions(Guid? customerId)
        {
            _customerId = customerId;
        }

        public List<UserRoleActionModel> Execute()
        {
            var model = new List<UserRoleActionModel>();

            var customerRoleActions = _customerId.HasValue
                ? RoleActionService.GetRoleActionsByCustomerId(_customerId.Value)
                : new List<RoleAction>();

            var customer = _customerId.HasValue ? CustomerService.GetCustomer(_customerId.Value) : null;

            foreach (RoleAction roleAction in Enum.GetValues(typeof(RoleAction)))
            {
                if (roleAction == RoleAction.None)
                    continue;

                model.Add(new UserRoleActionModel
                {
                    Key = roleAction.ToString(),
                    Name = roleAction.Localize(),
                    Enabled = _customerId.HasValue && ((customer != null && customer.IsAdmin) || customerRoleActions.Any(x => x == roleAction))
                });
            }

            return model;
        }
    }
}
