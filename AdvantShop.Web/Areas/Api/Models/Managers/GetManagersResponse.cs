using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Managers
{
    public class GetManagersResponse : List<ManagerModel>, IApiResponse
    {
        public GetManagersResponse(List<ManagerModel> items)
        {
            this.AddRange(items);
        }
    }
}