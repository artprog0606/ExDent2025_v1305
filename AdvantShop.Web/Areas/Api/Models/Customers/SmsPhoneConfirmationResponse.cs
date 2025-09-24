using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Customers
{
    
    public class SmsPhoneConfirmationResponse : ApiResponse
    {
        public bool IsCodeSended { get; set; }
    }
}