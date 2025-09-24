using AdvantShop.Web.Infrastructure.Admin;
using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog
{
    public enum ECatalogShowMethod
    {
        Normal,
        AllProducts,
        //OnlyInCategories,
        OnlyWithoutCategories
    }

    public class CatalogFilterModel : BaseFilterModel
    {
        /// <summary>
        /// Откуда пришел запрос (search)
        /// </summary>
        public string From { get; set; }

        public ECatalogShowMethod ShowMethod { get; set; }
        
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }


        public float? PriceFrom { get; set; }
        public float? PriceTo { get; set; }

        public float? AmountFrom { get; set; }
        public float? AmountTo { get; set; }

        public int? SortingFrom { get; set; }
        public int? SortingTo { get; set; }

        public string ArtNo { get; set; }

        public string Name { get; set; }

        public bool? HasPhoto { get; set; }

        public bool? Enabled { get; set; }

        public int? BrandId { get; set; }

        public int? ListId { get; set; }

        public int? UnitId { get; set; }

        public string CategorySearch { get; set; }

        public string ExcludeIds { get; set; }

        public string PropertyId { get; set; }

        public string PropertyValueId { get; set; }

        public List<int> Tags { get; set; }

        public float? DiscountFrom { get; set; }
        public float? DiscountTo { get; set; }

        public float? DiscountAmountFrom { get; set; }
        public float? DiscountAmountTo { get; set; }

        public EProductOnMain Type { get; set; }
        public List<string> CreatedByList { get; set; }
        public string BarCode { get; set; }
        
        public bool? IsProductSelector { get; set; }
        
        public List<int> WarehouseIds { get; set; }
    }

    public class CatalogRangeModel
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }
}