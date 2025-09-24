using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Customers
{
    public class SmsPhoneConfirmationCodeModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }

    public class SmsPhoneConfirmationCodeResponse : IApiResponse
    {
        public bool Confirmed { get; }

        public SmsPhoneConfirmationCodeResponse(bool confirmed)
        {
            Confirmed = confirmed;
        }
    }
}