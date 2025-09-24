using AdvantShop.Catalog;

namespace AdvantShop.ViewModel.ProductDetails
{
    public class SizeChartViewModel
    {
        public int Id { get; set; }
        public string ModalHeader { get; set; }
        public string LinkText { get; set; }
        public string Text { get; set; }
        public ESizeChartSourceType SourceType { get; set; }
    }
}
