using System.Collections.Generic;
using AdvantShop.Catalog;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class CatalogAllFilter
    {
        public int? ProductsLimit { get; set; }
        
        public bool? ShowHtmlPrice { get; set; }
        
        public ESortOrder? Sorting { get; set; }
        
        public string Search { get; set; }
        
        public List<int> WarehouseIds { get; set; }
    }
}