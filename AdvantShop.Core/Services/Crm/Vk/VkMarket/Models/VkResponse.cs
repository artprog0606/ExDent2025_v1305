namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class VkResponse<T> : IVkError
    {
        public T Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public interface IVkError
    {
        VkApiError Error { get; set; }
    }
}
