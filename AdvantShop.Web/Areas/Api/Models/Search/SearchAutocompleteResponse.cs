using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Areas.Api.Models.Categories;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Search
{
    public sealed class SearchAutocompleteResponse : IApiResponse
    {
        public List<CatalogProductItem> Products { get; }
        public List<GetCategoryResponse> Categories { get; }

        public SearchAutocompleteResponse(List<CatalogProductItem> products, List<GetCategoryResponse> categories)
        {
            Products = products;
            Categories = categories;
        }
    }
}