using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Facebook.Models
{
    public class FbPostCommentCollection
    {
        public List<FbPostComment> Data { get; set; }
        public FbPostSummary Summary { get; set; }
    }

    public class FbPostComment
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public FbUserShort From { get; set; }

        [JsonProperty(PropertyName = "created_time")]
        public DateTime CreatedTime { get; set; }
        
        [JsonProperty(PropertyName = "parent")]
        public FbPostComment Parent { get; set; }
    }

    public class FbPostSummary
    {
        public FbPostCommentOrder Order { get; set; }
        
        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }

        [JsonProperty(PropertyName = "can_comment")]
        public bool CanComment { get; set; }
    }
    

    /// <summary>
    /// Order in which comments were returned.
    /// </summary>
    public enum FbPostCommentOrder
    {
        /// <summary>
        /// The most interesting comments are sorted first.
        /// </summary>
        ranked,

        /// <summary>
        /// Comments sorted by the oldest comments first.
        /// </summary>
        chronological,

        /// <summary>
        /// Comments sorted by the newest comments first.
        /// </summary>
        reverse_chronological
    }
}
