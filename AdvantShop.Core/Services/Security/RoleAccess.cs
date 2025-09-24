//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Customers;

namespace AdvantShop.Security
{
    public class RoleAccess
    {
        private static readonly Dictionary<string, RoleAction> dictionary = new Dictionary<string, RoleAction>
        {
        };

        public static bool Check(Customer customer, string currentPage)
        {
            if (customer.CustomerRole != Role.Moderator || currentPage.Contains("default.aspx"))
                return true;

            var page = currentPage.Split(new[] {'?'}).First().Split(new[] {'/'}).Last();

            //ckeditor\plugins\fileman\asp_net
            if (page == "main.ashx")
                return
                    RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id)
                        .Any(x => x.Role == RoleAction.Store || x.Role == RoleAction.Landing);

            if (dictionary.ContainsKey(page))
            {
                RoleAction key = dictionary[page];
                return
                    RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id)
                                     .Any(item => item.Role == key);
            }

            return false;
        }
    }
}