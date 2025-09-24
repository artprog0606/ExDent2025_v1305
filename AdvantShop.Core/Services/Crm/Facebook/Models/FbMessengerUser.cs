using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbMessengerUser
    {
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "profile_pic")]
        public string ProfilePic { get; set; }

        public string Gender { get; set; }
    }
}
