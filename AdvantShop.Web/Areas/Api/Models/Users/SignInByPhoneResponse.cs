using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class SignInByPhoneResponse : IApiResponse
    {
        public bool IsCodeSended { get; set; }
    }
}