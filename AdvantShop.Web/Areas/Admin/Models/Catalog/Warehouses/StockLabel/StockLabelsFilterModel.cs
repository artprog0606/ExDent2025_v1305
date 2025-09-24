using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses.StockLabel
{
    public class StockLabelsFilterModel : BaseFilterModel
    {
        public string Name { get; set; }

        public int? AmountUpToFrom { get; set; }

        public int? AmountUpToTo { get; set; }
    }
}