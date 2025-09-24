using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    // https://developers.facebook.com/docs/graph-api/reference/v2.11/message

    
    public class FbConversationMessageCollection
    {
        public List<FbConversationMessage> Data { get; set; }
    }

    public class FbConversationMessage
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public FbConversationSender From { get; set; }

        public FbConversationSenderCollection To { get; set; }
        
        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; }
    }
}
