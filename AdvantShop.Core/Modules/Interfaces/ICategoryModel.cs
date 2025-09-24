using AdvantShop.Catalog;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface ICategoryModel
    {
        int? CategoryId { get; set; }

        string Url { get; set; }

        string TagUrl { get; set; }
        int TagId { get; set; }

        bool Indepth { get; set; }

        int? Page { get; set; }

        string Brand { get; set; }

        string Size { get; set; }

        string Color { get; set; }

        float? PriceFrom { get; set; }

        float? PriceTo { get; set; }

        string Prop { get; set; }

        bool Available { get; set; }

        //public string Preorder { get; set; }

        ESortOrder? Sort { get; set; }

        string ViewMode { get; set; }

        string SearchQuery { get; set; }
    }
}
