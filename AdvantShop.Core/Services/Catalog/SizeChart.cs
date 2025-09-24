using AdvantShop.Core.Common.Attributes;
using System.Collections.Generic;

namespace AdvantShop.Catalog
{
    public enum ESizeChartSourceType
    {
        [Localize("Core.Catalog.SizeChartSourceType.Text")]
        Text = 0,
        [Localize("Core.Catalog.SizeChartSourceType.Link")]
        Link = 1
    }
    public enum ESizeChartEntityType
    {
        Product = 0,
        Category = 1
    }

    public class SizeChart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModalHeader { get; set; }
        public string LinkText { get; set; }
        public string Text { get; set; }
        public ESizeChartSourceType SourceType { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
    }

    public class SizeChartPropertyValue
    {
        public int PropertyValueId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValueName { get; set; }
    }
}
