using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipPointsModel
    {
        public List<ApiShipPoint> rows { get; set; }
        public MetaTariff meta { get; set; }
    }

    public class ApiShipPoint
    {
        public int id { get; set; }
        public string providerKey { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string postIndex { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionType { get; set; }
        public string city { get; set; }
        public string cityGuid { get; set; }
        public string cityType { get; set; }
        public string community { get; set; }
        public string communityGuid { get; set; }
        public string communityType { get; set; }
        public string area { get; set; }
        public string street { get; set; }
        public string streetType { get; set; }
        public string house { get; set; }
        public string block { get; set; }
        public string office { get; set; }
        public string address { get; set; }
        public string url { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string timetable { get; set; }
        public WorkTimes worktime { get; set; }
        public int? fittingRoom { get; set; }
        public int? cod { get; set; }
        public int? paymentCash { get; set; }
        public int? paymentCard { get; set; }
        public int? multiplaceDeliveryAllowed { get; set; }
        public int? availableOperation { get; set; }
        public int? type { get; set; }
        public string description { get; set; }
        public List<string> photos { get; set; }
        public List<Metro> metro { get; set; }
        public List<Extra> extra { get; set; }
        public Limits limits { get; set; }
        public bool enabled { get; set; }

        public string DeliveryTime { get; set; }

    }

    public class WorkTimes
    {
        [JsonProperty("1")]
        public string Mon { get; set; }
        [JsonProperty("2")]
        public string Tue { get; set; }
        [JsonProperty("3")]
        public string Wed { get; set; }
        [JsonProperty("4")]
        public string Thu { get; set; }
        [JsonProperty("5")]
        public string Fri { get; set; }
        [JsonProperty("6")]
        public string Sat { get; set; }
        [JsonProperty("7")]
        public string Sun { get; set; }
    }

    public class Metro
    {
        public string name { get; set; }
        public decimal? distance { get; set; }
        public string line { get; set; }
    }

    public class Extra
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Limits
    {
        public int? maxSizeA { get; set; }
        public int? maxSizeB { get; set; }
        public int? maxSizeC { get; set; }
        public int? maxSizeSum { get; set; }
        public int? maxWeight { get; set; }
        public int? minWeight { get; set; }
        public decimal? maxCod { get; set; }
        public int? maxVolume { get; set; }
    }
}