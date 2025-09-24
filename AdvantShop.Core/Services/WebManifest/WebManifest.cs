using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.WebManifest
{
    public class WebManifest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("start_url")]
        public string StartUrl { get; set; }

        [JsonProperty("display")]
        public string DisplayType { get; set; }

        [JsonProperty("prefer_related_applications ")]
        public bool PreferRelatedApps { get; set; }

        [JsonProperty("icons")]
        public List<WebManifestIcon> Icons { get; set; }

        [JsonProperty("related_applications", NullValueHandling = NullValueHandling.Ignore)]
        public List<RelatedApp> RelatedApps { get; set; }

        public WebManifest()
        {
            StartUrl = "/";
            DisplayType = "standalone";
            Icons = new List<WebManifestIcon>();
        }
    }

    public class WebManifestIcon
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("sizes")]
        public string Sizes { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class RelatedApp
    {
        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}
