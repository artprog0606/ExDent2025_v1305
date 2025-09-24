using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.ImageSearches
{
    public class PexelsSearchResponse
    {
        public int Page { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "total_results")]
        public int TotalResults { get; set; }

        public string Url { get; set; }

        [JsonProperty(PropertyName = "next_page")]
        public string NextPage { get; set; }

        [JsonProperty(PropertyName = "prev_page")]
        public string PrevPage { get; set; }

        [JsonProperty(PropertyName = "photos")]
        public List<PexelsPhoto> Photos { get; set; }
    }

    public class PexelsPhoto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Photographer { get; set; }
        public PexelsPhotoSrc Src { get; set; }
    }

    
    public class PexelsPhotoSrc
    {
        /// <summary>
        /// The size of the original image is given with the attributes width and height.
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// This image has a maximum width of 940px and a maximum height of 650px. It has the aspect ratio of the original image.
        /// </summary>
        public string Large { get; set; }

        /// <summary>
        /// This image has a height of 350px and a flexible width. It has the aspect ratio of the original image.
        /// </summary>
        public string Medium { get; set; }

        /// <summary>
        /// This image has a height of 130px and a flexible width. It has the aspect ratio of the original image.
        /// </summary>
        public string Small { get; set; }

        /// <summary>
        /// This image has a width of 800px and a height of 1200px.
        /// </summary>
        public string Portrait { get; set; }

        /// <summary>
        /// This image has a width of 1200px and height of 627px.
        /// </summary>
        public string Landscape { get; set; }

        /// <summary>
        /// This image has a width of 280px and height of 200px.
        /// </summary>
        public string Tiny { get; set; }
    }
}
