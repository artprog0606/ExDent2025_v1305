using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.App.Landing.Models.Pictures
{
    public class UploadPictureResult
    {
        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "processedPictures")]
        public Dictionary<string, string> ProcessedPictures { get; set; }
    }
}
