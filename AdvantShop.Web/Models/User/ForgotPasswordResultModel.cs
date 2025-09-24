using Newtonsoft.Json;

namespace AdvantShop.Models.User
{
    public class ForgotPasswordResultModel : LoginResult
    {
        [JsonProperty("view")]
        public string View { get; set; }

        public ForgotPasswordResultModel(string status) : base(status)
        {
        }

        public ForgotPasswordResultModel(string status, string error) : base(status, error)
        {
        }

        public ForgotPasswordResultModel(string status, string error, bool requestCaptcha) : base(status, error, requestCaptcha)
        {
        }
    }
}