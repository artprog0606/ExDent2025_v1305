using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbPostCollection
    {
        public List<FbPost> Data { get; set; }
    }

    public class FbPost
    {
        public string Id { get; set; }

        public string Message { get; set; }

        [JsonProperty(PropertyName = "updated_time")]
        public DateTime UpdatedTime { get; set; }

        public string Link { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public FbPostCommentCollection Comments { get; set; }
    }
}
