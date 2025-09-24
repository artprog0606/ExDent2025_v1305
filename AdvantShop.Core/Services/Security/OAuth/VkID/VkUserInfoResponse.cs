using Newtonsoft.Json;

namespace AdvantShop.Security.OAuth
{
    public sealed class VkUserInfoResponse
    {
        [JsonProperty(PropertyName = "user")]
        public VkUserInfo User { get; set; }
    }
    
    public sealed class VkUserInfo
    {
        [JsonProperty(PropertyName = "user_id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
        
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName = "sex")]
        public EVkUserSex Sex { get; set; }
        
        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
    }

    public enum EVkUserSex
    {
        Unknown = 0,
        Female = 1,
        Male = 2,
    }
}