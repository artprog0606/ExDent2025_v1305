using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;

namespace AdvantShop.Customers
{
    public static class CustomerExtensions
    {
        public static string GetFullName(this Customer customer)
        {
            return new List<string>
            {
                customer.LastName,
                customer.FirstName,
                customer.Patronymic
            }.Where(x => x.IsNotEmpty()).Distinct().AggregateString(" ");
        }

        public static string GetShortName(this Customer customer)
        {
            var result = "";
            result += customer.LastName;

            if (!string.IsNullOrEmpty(customer.FirstName) && customer.FirstName != customer.LastName)
                result += (result != "" ? " " : "") + customer.FirstName;
            
            return result;
        }

        public static string GetFullName(this OrderCustomer customer)
        {
            return new List<string>
            {
                customer.LastName,
                customer.FirstName,
                customer.Patronymic
            }.Where(x => x.IsNotEmpty()).Distinct().AggregateString(" ");
        }
    }
}
