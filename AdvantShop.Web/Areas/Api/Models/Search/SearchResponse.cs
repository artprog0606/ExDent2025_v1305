using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Search
{
    public class SearchResponse: IApiResponse
    {
        public ApiPagination Pager { get; set; }
        
        public List<CatalogProductItem> Products { get; set; }
        
        public List<GetCategoryResponse> Categories { get; set; }
    }
}