using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses.StockLabel
{
    public class StockLabelsFilterResult : FilterResult<StockLabelGridModel> { }

    public class StockLabelGridModel
    {
        public int LabelId { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Color { get; set; }
        public float AmountUpTo { get; set; }
    }
}