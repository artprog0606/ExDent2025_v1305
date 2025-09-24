using System;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkApiAccessDeniedException : Exception
    {
        public VkApiAccessDeniedException(string message, int code, string error) : base(message)
        {
            Code = code;
            Error = error;
        }
        public int Code { get; }
        public string Error { get; }
    }
}