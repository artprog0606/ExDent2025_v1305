using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Crm.OK.Domain
{
    #region Responses
    public class OkBaseResponse
    {
        [JsonProperty(propertyName: "error_code")]
        public int ErrorCode { get; set; }
        [JsonProperty(propertyName: "error_msg")]
        public string ErrorMsg { get; set; }
    }
    public class OkGroupResponse : OkBaseResponse
    {
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
        [JsonProperty(propertyName: "group_id")]
        public string GroupID { get; set; }
    }
    public class OkChatResponse : OkBaseResponse
    {
        [JsonProperty(propertyName: "type")]
        public string Type { get; set; }
        [JsonProperty(propertyName: "status")]
        public string Status { get; set; }
        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }
        [JsonProperty(propertyName: "advert_id")]
        public string AdvertId { get; set; }
    }
    public class OkSendMessageResponse : OkBaseResponse
    {
        [JsonProperty(propertyName: "message_id")]
        public string MessageId { get; set; }
    }
    public class OkSendDirectMessageResponse : OkBaseResponse
    {
        [JsonProperty(propertyName: "success")]
        public List<bool> Success { get; set; }
        [JsonProperty(propertyName: "chat_ids")]
        public List<string> ChatIds { get; set; }
    }
    #endregion
    #region Requests
    public class OkSendMessageRequest
    {
        [JsonProperty(propertyName: "recipient")]
        public OkRecipientRequest Recipient { get; set; }
        [JsonProperty(propertyName: "message")]
        public OkMessageRequest Message { get; set; }

        public OkSendMessageRequest() { }
        public OkSendMessageRequest(string text, string chatID = null, string userId = null)
        {
            Recipient = new OkRecipientRequest
            {
                ChatId = chatID,
                UserId = userId
            };
            Message = new OkMessageRequest() { Text = text };
        }
    }
    public class OkMessageRequest
    {
        [JsonProperty(propertyName: "text")]
        public string Text { get; set; }
    }
    public class OkRecipientRequest
    {
        [JsonProperty(propertyName: "chat_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ChatId { get; set; }
        [JsonProperty(propertyName: "user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }
    }
    public class OkUpdateStatusRequest
    {
        [JsonProperty(propertyName: "recipient")]
        public OkRecipientRequest Recipient { get; set; }
        [JsonProperty(propertyName: "sender_action")]
        public string ChatStatus { get; set; }

        public OkUpdateStatusRequest() { }
        public OkUpdateStatusRequest(string chatId, OkChatStatus status)
        {
            this.Recipient = new OkRecipientRequest() { ChatId = chatId };
            this.ChatStatus = status.ToString();
        }
    }
    #endregion
    #region OkWebhook
    public class OkWebhookNotification
    {
        [JsonProperty(propertyName: "sender")]
        public OkWebhookSender Sender { get; set; }
        [JsonProperty(propertyName: "recipient")]
        public OkWebhookRecipient Recipient { get; set; }
        [JsonProperty(propertyName: "message")]
        public OkWebhookMessage Message { get; set; }
        [JsonProperty(propertyName: "timestamp")]
        public long Timestemp { get; set; }
    }

    public class OkWebhookSender
    {
        [JsonProperty(propertyName: "user_id")]
        public string UserID { get; set; }
        [JsonProperty(propertyName: "name")]
        public string Name { get; set; }
    }

    public class OkWebhookRecipient
    {
        [JsonProperty(propertyName: "chat_id")]
        public string ChatID { get; set; }
    }

    public class OkWebhookMessage
    {
        [JsonProperty(propertyName: "mid")]
        public string MessageID { get; set; }
        [JsonProperty(propertyName: "text")]
        public string Text { get; set; }
        [JsonProperty(propertyName: "seq")]
        public string Seq { get; set; }
        [JsonProperty(propertyName: "attachments")]
        public List<OkWebhookMessageAttachments> Attachments { get; set; }
    }

    public class OkWebhookMessageAttachments
    {
        [JsonProperty(propertyName: "type")]
        public AttachmentType Type { get; set; }
        [JsonProperty(propertyName: "payload")]
        public OkWebhookMessageAttachmentPayload Payload { get; set; }
    }

    public class OkWebhookMessageAttachmentPayload
    {
        [JsonProperty(propertyName: "url")]
        public string Url { get; set; }
    }

    public class OkWebhookSystemMessage
    {
        [JsonProperty(propertyName: "ty")]
        public string Type { get; set; }
    }
    #endregion

    public enum AttachmentType
    {
        IMAGE,
        VIDEO,
        AUDIO,
        SHARE,
        FILE
    }

    public enum OkChatStatus
    {
        mark_seen,
        typing_on,
        sending_photo
    }
}