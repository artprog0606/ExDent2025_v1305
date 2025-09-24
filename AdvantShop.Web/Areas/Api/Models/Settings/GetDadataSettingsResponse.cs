using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Settings
{
    public class GetDadataSettingsResponse : IApiResponse
    {
        public bool IsActive { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}