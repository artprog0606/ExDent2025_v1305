using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Rbkmoney2
{
    public class Rbkmoney2Response
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }

    public class InvoiceCreatedResponse : Rbkmoney2Response
    {
        [JsonProperty("invoice")]
        public InvoiceModel Invoice { get; set; }

        [JsonProperty("invoiceAccessToken")]
        public InvoiceAccessTokenModel InvoiceAccessToken { get; set; }
    }


}