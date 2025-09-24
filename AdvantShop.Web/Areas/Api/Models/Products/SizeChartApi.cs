using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Products
{
    public sealed class SizeChartApi
    {
        public int Id { get; }
        public string LinkText { get; }
        public string ModalHeader { get; }
        public string Type { get; }
        public string Text { get; }

        public SizeChartApi(SizeChart sizeChart)
        {
            Id = sizeChart.Id;
            ModalHeader = sizeChart.ModalHeader;
            LinkText = sizeChart.LinkText;
            Type = sizeChart.SourceType.ToString();
            Text = sizeChart.Text;
        }
    }
}