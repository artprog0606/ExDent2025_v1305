using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public sealed class VkAuthError
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }

    public interface IVkAuthError
    {
        VkAuthError Error { get; set; }
    }
}