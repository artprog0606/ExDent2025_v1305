using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Shipping.ApiShip.Map
{
    public class PointOptions
    {
        [JsonProperty("preset")]
        public string Preset { get; set; }
    }
}
