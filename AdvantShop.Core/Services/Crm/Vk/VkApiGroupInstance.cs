using VkNet;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkApiGroupInstance
    {
        private static VkApi _instance;

        private VkApiGroupInstance()
        {
        }
        
        public static VkApi GetInstance(int? requestsPerSecond = null)
        { 
            return _instance ?? (_instance = new VkApi().AuthorizeByGroupToken(requestsPerSecond));
        }
    }
}