using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Infrastructure.Admin;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public class OrderItemsFilterModel : BaseFilterModel
    {
        public int OrderId { get; set; }
        public string EnableHiding { get; set; }
        private OrderItemsColumns _showColumns;

        public OrderItemsColumns ShowColumns
        {
            get
            {
                return _showColumns
                       ?? (_showColumns = EnableHiding.IsNullOrEmpty()
                           ? null
                           : JsonConvert.DeserializeObject<OrderItemsColumns>(EnableHiding));
            }
        }
    }

    public class OrderItemsColumns
    {
        public bool PriceWhenOrdering { get; set; }
        public bool DiscountWhenOrdering { get; set; }
        public bool IsCustomPrice { get; set; }
        public bool Stocks { get; set; }
    }
}
