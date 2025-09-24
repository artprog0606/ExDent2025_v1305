using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Vk
{
    public class VkResponse : ApiResponse
    {
        public VkResponse()
        {
        }

        public VkResponse(ApiStatus status, string errors) : base(status, errors)
        {
        }

        public object Data { get; set; }
    }
}