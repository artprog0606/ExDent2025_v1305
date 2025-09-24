using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.MainPageProducts
{
    public class MainPageProductsModel
    {
        public EProductOnMain Type { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool ShowOnMainPage { get; set; }
        public bool UseDefaultMeta { get; set; }
        public string H1 { get; set; }
        public string Title { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public bool? DisplayLatestProductsInNewOnMainPage { get; set; }
        public bool ShuffleList { get; set; }
        public int Sorting { get; set; }
    }
}