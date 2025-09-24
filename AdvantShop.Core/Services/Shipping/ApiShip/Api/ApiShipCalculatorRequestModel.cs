using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{    
    public class ApiShipCalculatorRequestModel
    {
        public ApiShipCalculatorObject To { get; set; }
        public ApiShipCalculatorObject From { get; set; }
        public List<ApiShipCalculatorPlaces> Places { get; set; }
        public string PickupDate { get; set; }
        public List<int> PickupTypes { get; set; }
        public List<int> DeliveryTypes { get; set; }
        public List<string> ProviderKeys { get; set; }
        public int AssessedCost { get; set; }
        public int CodCost { get; set; }
        public bool IncludeFees { get; set; }
        public int Timeout { get; set; }
        public bool SkipTariffRules { get; set; }
        public ExtraParam ExtraParams { get; set; }
        public string PromoCode { get; set; }
        public string CustomCode { get; set; }
        public List<int> TariffIds { get; set; }
        //public int PointInId { get; set; }
        //public int PointOutId { get; set; }

    }
    public class ApiShipCalculatorObject
    {
        public string CountryCode { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Index { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AddressString { get; set; }
        public string Region { get; set; }
        public string City { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string CityGuid { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public double Lat { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public double Lng { get; set; }
    }

    public class ApiShipCalculatorPlaces
    {
        public int Height { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Weight { get; set; }
    }

    public class ExtraParam
    {
        //[JsonProperty(propertyName: extraParamName, NullValueHandling = NullValueHandling.Ignore)]
        [JsonProperty("dpd.providerConnectId", NullValueHandling = NullValueHandling.Ignore)]
        //[JsonProperty(PropertyName = extraParamName, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        //public static string extraParamName { get; set; }
    }

    
}
