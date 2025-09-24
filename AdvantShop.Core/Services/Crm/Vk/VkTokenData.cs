namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkTokenData
    {
        public string refresh_token { get; set; } 
        public string access_token { get; set; } 
        public string id_token { get; set; }
        public string  token_type { get; set; }
        public long expires_in { get; set; }
        public long user_id { get; set; }
        public string state { get; set; }
        public string scope { get; set; }
        public string  device_id { get; set; }
        public string client_id { get; set; }

        public VkTokenData()
        {
            
        }

        public VkTokenData(ExchangeCodeResponse result, string deviceId, string clientId)
        {
            refresh_token = result.refresh_token;
            access_token = result.access_token;
            id_token = result.id_token;
            token_type = result.token_type;
            expires_in = result.expires_in;
            user_id = result.user_id;
            state = result.state;
            scope = result.scope;
            device_id = deviceId;
            client_id = clientId;
        }

        public VkTokenData Refresh(RefreshTokenResponse refreshTokenResponse)
        {
            refresh_token = refreshTokenResponse.refresh_token;
            access_token = refreshTokenResponse.access_token;
            
            return this;
        } 
    }
}