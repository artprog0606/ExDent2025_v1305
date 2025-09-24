using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    // Docs: https://developers.facebook.com/docs/facebook-login/access-tokens

    public class FacebookAccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "error")]
        public FacebookAccessTokenError Error { get; set; }
    }

    public class FacebookAccessTokenError
    {
        public string message { get; set; }
        public string type { get; set; }
        public string code { get; set; }
        public string fbtrace_id { get; set; }
    }

    /// <summary>
    /// Маркеры доступа Страницы
    /// https://developers.facebook.com/docs/facebook-login/access-tokens
    /// </summary>
    public class FacebookGroupAccessToken
    {
        [JsonProperty(PropertyName = "data")]
        public List<FacebookGroupItemAccessToken>  Data { get; set; }
    }

    public class FacebookGroupItemAccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "perms")]
        public List<string> Permissions { get; set; }
    }
}
