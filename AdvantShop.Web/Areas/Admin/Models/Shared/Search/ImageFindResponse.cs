using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Shared.Search
{
    public class ImageFindResponse
    {
        [JsonProperty("items")]
        public List<ImageFindItem> Items { get; set; }

        [JsonProperty("errors")]
        public List<ImageFindError> Errors { get; set; }
    }

    public class ImageFindItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contentUrl")]
        public string ContentUrl { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }

    public class ImageFindError
    {
        public string Message { get; set; }
    }
}
