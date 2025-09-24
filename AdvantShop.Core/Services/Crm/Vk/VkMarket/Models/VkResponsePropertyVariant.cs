using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkResponsePropertyVariant
    {
        [JsonProperty("variant_id")]
        public long Id { get; set; }
    }
}
