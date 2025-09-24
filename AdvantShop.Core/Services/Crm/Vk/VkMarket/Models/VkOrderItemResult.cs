using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkOrderItemResult : IVkError
    {
        public VkOrderItemResponse Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public class VkOrderItemResponse
    {
        public int Count { get; set; }
        public List<VkOrderItem> Items { get; set; }
    }
}
