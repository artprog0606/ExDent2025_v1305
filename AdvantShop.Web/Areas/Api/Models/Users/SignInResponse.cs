using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class SignInResponse : IApiResponse
    {
        public string UserKey { get; set; }
        public string UserId { get; set; }
        public GetCustomerResponse Customer { get; set; }
    }
}