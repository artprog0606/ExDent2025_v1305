using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class Feature
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get { return "Feature"; } }

        [JsonProperty("geometry")]
        public PointGeometry Geometry { get; set; }

        /// <summary>
        /// Properties https://tech.yandex.ru/maps/jsapi/doc/2.1/ref/reference/GeoObject-docpage/#GeoObject
        /// </summary>
        [JsonProperty("properties")]
        public PointProperties Properties { get; set; }

        /// <summary>
        /// Options https://tech.yandex.ru/maps/jsapi/doc/2.1/ref/reference/GeoObject-docpage/#GeoObject
        /// </summary>
        [JsonProperty("options")]
        public PointOptions Options { get; set; }
    }
}
