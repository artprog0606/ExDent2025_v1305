using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class FeatureCollection
    {
        [JsonProperty("type")]
        public string Type { get { return "FeatureCollection"; } }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }
}
