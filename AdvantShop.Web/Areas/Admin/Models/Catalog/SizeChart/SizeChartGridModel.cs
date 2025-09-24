using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.SizeChart
{
    public class SizeChartGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LinkText { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
    }
}
