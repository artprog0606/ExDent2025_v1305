using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.App.Landing.Models.Catalogs
{
    public class ProductsByCategoryModel
    {
        public int CategoryId { get; set; }
        public int CountPerPage { get; set; }
        public int Page { get; set; }

        public ESortOrder? Sort { get; set; }
        public bool? Indepth { get; set; }


        public List<int> Brand { get; set; }
        public List<int> Size { get; set; }
        public List<int> Color { get; set; }
        public List<List<int>> Prop { get; set; }
        public List<PropRangeItem> PropRange { get; set; }

        public float? PriceFrom { get; set; }
        public float? PriceTo { get; set; }


        public bool HideFilterByPrice { get; set; }
        public bool HideFilterByBrand { get; set; }
        public bool HideFilterByColor { get; set; }
        public bool HideFilterBySize { get; set; }
        public bool HideFilterByProperty { get; set; }
    }

    public class PropRangeItem
    {
        public int Id { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
    }
}
