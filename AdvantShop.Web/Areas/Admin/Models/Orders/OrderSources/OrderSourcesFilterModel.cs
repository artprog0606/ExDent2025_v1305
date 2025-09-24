using AdvantShop.Core.Services.Orders;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Orders.OrderSources
{
    public class OrderSourcesFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public OrderType? Type { get; set; }
        public bool? Main { get; set; }
        public int? OrdersCountFrom { get; set; }
        public int? OrdersCountTo { get; set; }
        public int ? LeadsCountFrom { get; set; }
        public int ? LeadsCountTo { get; set; }
    }
}
