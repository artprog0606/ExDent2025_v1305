using AdvantShop.Core.Services.Crm;

namespace AdvantShop.Core.Services.Webhook.Models
{
    public class WebhookLeadEventModel : WebhookModel
    {
        public WebhookLeadEventModel() { }

        public WebhookLeadEventModel(int objId, LeadEventType eventType)
        {
            ObjId = objId;
            EventType = eventType;
        }

        public int ObjId { get; set; }

        public LeadEventType EventType { get; set; }
    }
}
