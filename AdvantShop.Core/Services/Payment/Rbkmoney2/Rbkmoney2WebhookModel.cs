using System;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Rbkmoney2
{
    public enum ERbkmoney2WebhookType
    {
        None,
        InvoiceCreated,
        InvoicePaid,
        InvoiceCancelled,
        InvoiceFulfilled,
        PaymentStarted,
        PaymentProcessed,
        PaymentCaptured,
        PaymentCancelled,
        PaymentRefunded,
        PaymentFailed,
        CustomerCreated,
        CustomerDeleted,
        CustomerReady,
        CustomerBindingStarted,
        CustomerBindingSucceeded,
        CustomerBindingFailed
    }

    public class Rbkmoney2WebhookModel
    {
        [JsonProperty("eventID")]
        public string EventId { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime OccuredAt { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("eventType")]
        public ERbkmoney2WebhookType EventType { get; set; }

        [JsonProperty("invoice")]
        public InvoiceModel Invoice { get; set; }
    }
}