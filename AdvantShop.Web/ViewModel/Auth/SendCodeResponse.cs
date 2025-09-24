using Newtonsoft.Json;

namespace AdvantShop.ViewModel.Auth
{
    public class SendCodeResponse
    {
        [JsonProperty("secondsToRetry")]
        public int SecondsToRetry { get; set; }
    }
}