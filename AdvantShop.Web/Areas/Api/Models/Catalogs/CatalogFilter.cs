using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class CatalogFilter
    {
        public int? CategoryId { get; set; }

        public string Url { get; set; }

        public int? Page { get; set; }

        public List<int> BrandIds { get; set; }

        public List<int> SizeIds { get; set; }

        public List<int> ColorIds { get; set; }

        public float? PriceFrom { get; set; }

        public float? PriceTo { get; set; }

        public List<List<int>> PropertyLists { get; set; }
        
        public List<CatalogFilterPropertyRange> PropertyRanges { get; set; }

        public bool Available { get; set; }

        public ESortOrder? Sorting { get; set; }

        public string Search { get; set; }
        
        public bool? ShowHtmlPrice { get; set; }
        
        /// <summary>
        /// Фильтр по складу (который в фильтре в категории слева). Не путать с глобальным WarehouseContext.CurrentWarehouseIds
        /// </summary>
        public List<int> WarehouseIds { get; set; }

        public static explicit operator CategoryModel(CatalogFilter filter)
        {
            return new CategoryModel()
            {
                CategoryId = filter.CategoryId,
                Url = filter.Url,
                Page = filter.Page,
                Available = filter.Available,
                Brand = filter.BrandIds != null ? string.Join(",", filter.BrandIds) : null,
                Color = filter.ColorIds != null ? string.Join(",", filter.ColorIds) : null,
                Size = filter.SizeIds != null ? string.Join(",", filter.SizeIds) : null,
                Prop = filter.PropertyLists != null
                    ? string.Join("-", filter.PropertyLists.Select(x => string.Join(",", x)))
                    : null,
                PropertyRanges = filter.PropertyRanges,
                Sort = filter.Sorting,
                PriceFrom = filter.PriceFrom,
                PriceTo = filter.PriceTo,
                SearchQuery = filter.Search,
                Warehouse = filter.WarehouseIds != null ? string.Join(",", filter.WarehouseIds) : null
            };
        }
            
    }
}