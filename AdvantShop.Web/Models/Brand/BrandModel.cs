using AdvantShop.Catalog;

namespace AdvantShop.Models.Brand
{
    public class BrandModel
    {
        public int BrandId { get; set; }

        public int? Page { get; set; }

        public bool InDepth { get; set; }

        public ESortOrder? Sort { get; set; }

        public string ViewMode { get; set; }
    }
}