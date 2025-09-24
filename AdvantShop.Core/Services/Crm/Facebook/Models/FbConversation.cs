using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbConversationCollection
    {
        public List<FbConversation> Data { get; set; }
    }

    /// <summary>
    /// https://developers.facebook.com/docs/graph-api/reference/v2.11/conversation
    /// </summary>
    public class FbConversation
    {
        public string Id { get; set; }
        public string Link { get; set; }

        [JsonProperty(PropertyName = "updated_time")]
        public DateTime UpdatedTime { get; set; }

        [JsonProperty(PropertyName = "message_count")]
        public int MessageCount { get; set; }

        [JsonProperty(PropertyName = "unread_count")]
        public int UnreadCount { get; set; }
        
        [JsonProperty(PropertyName = "can_reply")]
        public bool CanReply { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public FbConversationMessageCollection Messages { get; set; }
    }

    public class FbConversationSenderCollection
    {
        public List<FbConversationSender> Data { get; set; }
    }

    public class FbConversationSender
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
