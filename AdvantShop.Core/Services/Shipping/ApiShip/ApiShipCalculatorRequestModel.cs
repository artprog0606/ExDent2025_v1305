using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    
    public class ApiShipCalculatorRequestModel
    {
        public ApiShipCalculatorObject to { get; set; }
        public ApiShipCalculatorObject from { get; set; }
        public List<ApiShipCalculatorPlaces> places { get; set; }
        public string pickupDate { get; set; }
        public List<int> pickupTypes { get; set; }
        public List<int> deliveryTypes { get; set; }
        public List<string> providerKeys { get; set; }
        public int assessedCost { get; set; }
        public int codCost { get; set; }
        public bool includeFees { get; set; }
        public int timeout { get; set; }
        public bool skipTariffRules { get; set; }
        public ExtraParam extraParams { get; set; }
        public string promoCode { get; set; }
        public string customCode { get; set; }
        public List<int> tariffIds { get; set; }
        public int pointInId { get; set; }
        public int pointOutId { get; set; }

    }
    public class ApiShipCalculatorObject
    {
        public string countryCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string index { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string addressString { get; set; }
        public string region { get; set; }
        public string city { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cityGuid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double lng { get; set; }
    }

    public class ApiShipCalculatorPlaces
    {
        public int height { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int weight { get; set; }
    }

    public class ExtraParam
    {
        //[JsonProperty(propertyName: extraParamName, NullValueHandling = NullValueHandling.Ignore)]
        [JsonProperty("dpd.providerConnectId", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonProperty(PropertyName = extraParamName, NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        //public static string extraParamName { get; set; }
    }

    
}
