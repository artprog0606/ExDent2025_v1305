using AdvantShop.Web.Infrastructure.Admin;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public class OrdersFilterResult : FilterResult<OrderItemsModel>
    {
        public OrdersFilterResult()
        {
            OrdersCount = new Dictionary<int, int>();
        }

        public Dictionary<int, int> OrdersCount { get; set; }
        public Dictionary<OrdersPreFilterType, int> OrdersCountPreFilter { get; set; }
    }
}
