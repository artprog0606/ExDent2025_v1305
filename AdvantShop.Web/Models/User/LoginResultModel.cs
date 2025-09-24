using AdvantShop.Extensions;
using Newtonsoft.Json;

namespace AdvantShop.Models.User
{
    public class LoginResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("requestCaptcha")]
        public bool RequestCaptcha { get; set; }

        [JsonProperty("redirectTo")]
        public string RedirectTo { get; set; }

        public LoginResult(string status) : this(status, "")
        {
        }

        public LoginResult(string status, string error) : this(status, error, false)
        {
        }

        public LoginResult(string status, string error, bool requestCaptcha)
        {
            Status = status;
            Error = error;
            RequestCaptcha = requestCaptcha;
        }
    }
}