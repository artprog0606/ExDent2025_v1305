using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Models.Review
{
    public class AddReviewResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }
        
        [JsonProperty("review")]
        public AddReviewItem Review { get; set; }
    }

    public class AddReviewItem
    {
        public int ParentId { get; set; }
        public int ReviewId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int RatioByLikes { get; set; }
        public List<AddReviewItemPhoto> Photos { get; set; }
    }

    public class AddReviewItemPhoto
    {
        [JsonProperty("photoId")]
        public int PhotoId { get; set; }
        
        [JsonProperty("small")]
        public string Small { get; set; }
        
        [JsonProperty("big")]
        public string Big { get; set; }
    }
}