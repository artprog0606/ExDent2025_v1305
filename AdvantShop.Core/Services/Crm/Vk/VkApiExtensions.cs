using AdvantShop.Configuration;
using VkNet;
using VkNet.Model;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public static class VkApiExtensions
    {
        public static VkApi AuthorizeByUserToken(this VkApi vk, int? requestsPerSecond = null)
        {
            vk.Authorize(new ApiAuthParams()
            {
                AccessToken = SettingsVk.UserTokenData.access_token, 
                UserId = SettingsVk.UserId,
            });
            
            if (requestsPerSecond != null)
                vk.RequestsPerSecond = requestsPerSecond.Value;
            
            return vk;
        }
        
        public static VkApi AuthorizeByGroupToken(this VkApi vk, int? requestsPerSecond = null)
        {
            vk.Authorize(new ApiAuthParams()
            {
                AccessToken = SettingsVk.TokenGroup, 
                UserId = SettingsVk.UserId
            });
            
            if (requestsPerSecond != null)
                vk.RequestsPerSecond = requestsPerSecond.Value;
            
            return vk;
        }
    }
}