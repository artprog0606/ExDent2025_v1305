using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class CatalogResponse : IApiResponse
    {
        public GetCategoryResponse Category { get; set; }
        
        public List<SubCategoryItem> SubCategories { get; set; }
        
        public ApiPagination Pager { get; set; }
        
        public List<CatalogProductItem> Products { get; set; }
    }
}