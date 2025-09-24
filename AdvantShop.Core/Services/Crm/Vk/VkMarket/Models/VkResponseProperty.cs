using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkResponseProperty
    {
        [JsonProperty("property_id")]
        public long PropertyId { get; set; }
    }

    public class VkResponseProperties
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public List<VkResponsePropertyItem> Items { get; set; }
    }

    public class VkResponsePropertyItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public List<VkResponsePropertyItemVariant> Variants { get; set; }
    }

    public class VkResponsePropertyItemVariant
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
