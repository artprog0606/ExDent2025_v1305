using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class GetRelatedProductsResponse : List<CatalogProductItem>, IApiResponse
    {
        public GetRelatedProductsResponse(List<CatalogProductItem> items)
        {
            this.AddRange(items);
        }
    }
}