using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders.Marking
{
    public class MarkingOrderItemModel
    {
        public int OrderItemId { get; set; }
        public List<string> Codes {get; set;}
        public string Name { get; set; }
    }
}
