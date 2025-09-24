using System.Collections.Generic;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace AdvantShop.ViewModel.Checkout
{
    public sealed class GeoModeDeliveriesResponse
    {
        [JsonProperty("options")]
        public List<BaseShippingOption> Options { get; set; }
        
        [JsonProperty("selectedOption")]
        public BaseShippingOption SelectedOption { get; set; }
    }
}