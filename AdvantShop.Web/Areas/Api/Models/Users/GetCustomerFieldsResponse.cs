using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class GetCustomerFieldsResponse : List<CustomerFieldModel>, IApiResponse
    {
        public GetCustomerFieldsResponse(List<CustomerFieldModel> fields)
        {
            this.AddRange(fields);
        }
    }
}