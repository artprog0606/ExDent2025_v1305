using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Users
{
    public class UpdateFcmTokenResponse : IApiResponse
    {
        public bool Result { get; }

        public UpdateFcmTokenResponse(bool result)
        {
            Result = result;
        }
    }
}