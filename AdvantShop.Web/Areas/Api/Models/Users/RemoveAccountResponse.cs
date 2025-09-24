using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class RemoveAccountResponse : IApiResponse
    {
        public bool Result { get; }

        public RemoveAccountResponse(bool result)
        {
            Result = result;
        }
    }
}