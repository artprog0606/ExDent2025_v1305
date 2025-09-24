using System.Collections.Generic;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Webhook.Models.Api;
using AdvantShop.Web.Infrastructure.Api;

namespace AdvantShop.Areas.Api.Models.Orders
{
    public class GetOrdersMeResponse : IApiResponse
    {
        public List<OrderModel> Orders { get; }
        public ApiPagination Pager { get; }

        public GetOrdersMeResponse(EntitiesFilterResult<OrderModel> result)
        {
            Orders = result.DataItems;

            Pager = new ApiPagination()
            {
                CurrentPage = result.PageIndex,
                TotalCount = result.TotalItemsCount,
                TotalPageCount = result.TotalPageCount,
                Count = result.DataItems?.Count ?? 0
            };
        }
    }
}