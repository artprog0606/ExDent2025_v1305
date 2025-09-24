using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class PointGeometry
    {
        [JsonProperty("type")]
        public string Type { get { return "Point"; } }
        [JsonIgnore]
        public float PointX { get; set; }
        [JsonIgnore]
        public float PointY { get; set; }

        [JsonProperty("coordinates")]
        public float[] Coordinates { get { return new float[] { PointX, PointY }; } }
    }
}
