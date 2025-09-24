namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public sealed class VkOrderByIdResult : IVkError
    {
        public VkOrderByIdResponse Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public sealed class VkOrderByIdResponse
    {
        public VkOrder Order { get; set; }
    }
}