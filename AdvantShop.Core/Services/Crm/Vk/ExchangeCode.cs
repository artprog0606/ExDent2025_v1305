namespace AdvantShop.Core.Services.Crm.Vk
{
    public sealed class ExchangeCode
    {
        public string code { get; set; } 
        public string state { get; set; } 
        public string code_verifier { get; set; }
        public string  device_id { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
    }
}