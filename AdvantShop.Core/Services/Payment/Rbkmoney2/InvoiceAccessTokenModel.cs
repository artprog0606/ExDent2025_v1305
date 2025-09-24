using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Rbkmoney2
{
    public class InvoiceAccessTokenModel
    {
        /// <summary>
        /// Содержимое токена для доступа
        /// </summary>
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}