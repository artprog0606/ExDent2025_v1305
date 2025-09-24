using AdvantShop.Catalog;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.SizeChart
{
    public class SizeChartFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public string LinkText { get; set; }
        public string Text { get; set; }
        public ESizeChartSourceType? SourceType { get; set; }
        public bool? Enabled { get; set; }
    }
}
