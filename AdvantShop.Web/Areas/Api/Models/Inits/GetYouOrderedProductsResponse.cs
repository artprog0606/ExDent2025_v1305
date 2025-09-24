using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Catalogs;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Inits
{
    public class GetYouOrderedProductsResponse : List<CatalogProductItem>, IApiResponse
    {
        public GetYouOrderedProductsResponse(List<CatalogProductItem> items)
        {
            this.AddRange(items);
        }
    }
}