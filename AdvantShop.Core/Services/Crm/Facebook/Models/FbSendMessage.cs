namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbSendMessage
    {
        public string messaging_type { get; set; }
        public FbSendMessageRecipient recipient { get; set; }
        public FbSendMessageText message { get; set; }
    }

    public class FbSendMessageRecipient
    {
        public string id { get; set; }
    }

    public class FbSendMessageText
    {
        public string text { get; set; }
    }

    public class FbSendResponse
    {
        public string recipient_id { get; set; }
        public string message_id { get; set; }
    }
}
