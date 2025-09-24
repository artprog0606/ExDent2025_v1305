using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Models.Catalog;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class GetCatalogFilterResponse : List<FilterItemModel>, IApiResponse
    {
        public GetCatalogFilterResponse(List<FilterItemModel> items)
        {
            this.AddRange(items);
        }
    }
}