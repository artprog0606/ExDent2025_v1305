using AdvantShop.SEO;

namespace AdvantShop.Catalog
{
    public class ProductList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
        public bool ShuffleList { get; set; }
        public bool ShowOnMainPage { get; set; }
        public string UrlPath { get; set; }

        public MetaType MetaType
        {
            get { return MetaType.MainPageProducts; }
        }

        private MetaInfo _meta;
        public MetaInfo Meta
        {
            get
            {
                return _meta ??
                       (_meta =
                        MetaInfoService.GetMetaInfo(Id, MetaType) ??
                        MetaInfoService.GetDefaultMetaInfo(MetaType, Name));
            }
            set
            {
                _meta = value;
            }
        }
    }
}