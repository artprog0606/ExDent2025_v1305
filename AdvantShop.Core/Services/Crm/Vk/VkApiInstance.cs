using VkNet;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkApiInstance
    {
        private static VkApi _instance;

        private VkApiInstance()
        {
        }
        
        public static VkApi GetInstance(int? requestsPerSecond = null)
        { 
            return _instance ?? (_instance = new VkApi().AuthorizeByUserToken(requestsPerSecond));
        }
    }
}