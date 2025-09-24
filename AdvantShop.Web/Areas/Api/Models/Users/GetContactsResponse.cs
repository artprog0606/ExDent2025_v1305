using System.Collections.Generic;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class GetContactsResponse : List<CustomerContactModel>, IApiResponse
    {
        public GetContactsResponse(List<CustomerContactModel> contacts)
        {
            this.AddRange(contacts);
        }
    }
}