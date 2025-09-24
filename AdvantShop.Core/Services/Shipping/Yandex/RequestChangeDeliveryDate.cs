using System.Collections.Generic;
using AdvantShop.Shipping.Yandex.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Yandex
{
    public class RequestChangeDeliveryDate
    {
        [JsonProperty("timesOfDelivery")]
        public IDictionary<string,List<IntervalOffer>> TimesOfDelivery { get; set; }
        
        [JsonProperty("datesOfDelivery")]
        public Dictionary<string, List<IntervalOffer>>.KeyCollection DatesOfDelivery { get; set; }
    }
}