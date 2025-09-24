using Newtonsoft.Json;

namespace AdvantShop.ViewModel.User
{
    public class ConfirmCodeModel
    {
        [JsonProperty("redirectTo")]
        public string RedirectTo { get; set; }
    }
}