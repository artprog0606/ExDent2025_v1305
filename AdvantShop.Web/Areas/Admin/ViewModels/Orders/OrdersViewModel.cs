using System.Collections.Generic;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.ViewModels.Orders
{
    public class OrdersViewModel
    {
        public OrdersPreFilterType? PreFilter { get; set; }

        public bool EnableMangers { get; set; }

        public List<OrderStatus> OrderStatuses { get; set; }

        public int? StatusId { get; set; }
    }
}
