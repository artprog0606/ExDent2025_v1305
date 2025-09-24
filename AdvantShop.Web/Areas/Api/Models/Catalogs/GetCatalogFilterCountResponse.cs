using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Catalogs
{
    public class GetCatalogFilterCountResponse : IApiResponse
    {
        public int ItemsCount { get; }

        public GetCatalogFilterCountResponse(int itemsCount)
        {
            ItemsCount = itemsCount;
        }
    }
}