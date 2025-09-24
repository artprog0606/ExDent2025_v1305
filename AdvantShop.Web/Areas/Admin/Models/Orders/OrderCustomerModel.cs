using AdvantShop.Customers;
using AdvantShop.Orders;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public class OrderCustomerModel
    {
        public int OrderId { get; set; }

        public OrderCustomer OrderCustomer { get; set; }

        public List<CustomerFieldWithValue> CustomerFields { get; set; }
    }
}
