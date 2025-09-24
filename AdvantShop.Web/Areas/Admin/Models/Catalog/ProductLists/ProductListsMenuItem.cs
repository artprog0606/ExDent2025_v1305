using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.ProductLists
{
    public class ProductListsMenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool ShuffleList { get; set; }
        public bool ShowOnMainPage { get; set; }
        public EProductOnMain Type { get; set; }
        public string TypeStr { get { return Type.ToString().ToLower(); } }
        public bool DisplayLatestProductsInNewOnMainPage { get; set; }
        public string Link { get; set; }
    }
}
