using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class PointProperties
    {
        [JsonProperty("hintContent")]
        public string HintContent { get; set; }

        [JsonProperty("balloonContentHeader")]
        public string BalloonContentHeader { get; set; }

        [JsonProperty("balloonContentBody")]
        public string BalloonContentBody { get; set; }

        [JsonProperty("balloonContentFooter")]
        public string BalloonContentFooter { get; set; }
    }
}
