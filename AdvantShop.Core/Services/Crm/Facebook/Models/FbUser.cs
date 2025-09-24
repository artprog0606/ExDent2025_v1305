using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbUserShort
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class FbUser
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string Gender { get; set; }
    }
}
