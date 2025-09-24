using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkResponseGroup
    {
        [JsonProperty("item_group_id")]
        public long GroupId { get; set; }
    }
}
