using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbWebhookModel
    {
        public string @object { get; set; }
        public List<FbWebhookEntry> Entry { get; set; }
    }

    public class FbWebhookEntry
    {
        public List<FbWebhookMessaging> Messaging { get; set; }
    }

    public class FbWebhookMessaging
    {
        public FbWebhookMessageSender Sender { get; set; }
        public FbWebhookMessageSender Recipient { get; set; }
        public long Timestamp { get; set; }
        public FbWebhookMessage Message { get; set; }
    }


    public class FbWebhookMessageSender
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messages
    /// </summary>
    public class FbWebhookMessage
    {
        public string Mid { get; set; }
        public string Text { get; set; }
        public List<FbWebhookMessageAttachment> Attachments { get; set; }
    }

    public class FbWebhookMessageAttachment
    {
        public string Type { get; set; }
        public FbWebhookMessageAttachmentUrl Payload { get; set; }
    }

    public class FbWebhookMessageAttachmentUrl
    {
        public string Url { get; set; }
    }

}
