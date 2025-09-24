using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.CustomerGroups
{
    public class GetCustomerGroupsResponse : List<CustomerGroupModel>, IApiResponse
    {
        public GetCustomerGroupsResponse(List<CustomerGroupModel> items)
        {
            this.AddRange(items);
        }
    }
}