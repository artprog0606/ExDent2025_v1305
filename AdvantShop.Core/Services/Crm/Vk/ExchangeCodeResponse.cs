using AdvantShop.Core.Services.Crm.Vk.VkMarket;

namespace AdvantShop.Core.Services.Crm.Vk
{
    /// <summary>
    /// https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/api-integration/api-description#Successful-request-authorization-code
    /// </summary>
    public sealed class ExchangeCodeResponse
    {
        public string refresh_token { get; set; } 
        public string access_token { get; set; } 
        public string id_token { get; set; }
        public string  token_type { get; set; }
        public long expires_in { get; set; }
        public long user_id { get; set; }
        public string state { get; set; }
        public string scope { get; set; }
    }
}