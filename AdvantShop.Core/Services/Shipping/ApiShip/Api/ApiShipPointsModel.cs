using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipPointsModel
    {
        public List<ApiShipPoint> Rows { get; set; }
        public MetaTariff Meta { get; set; }
    }

    public class ApiShipPoint
    {
        public int Id { get; set; }
        public string ProviderKey { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PostIndex { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string RegionType { get; set; }
        public string City { get; set; }
        public string CityGuid { get; set; }
        public string CityType { get; set; }
        public string Community { get; set; }
        public string CommunityGuid { get; set; }
        public string CommunityType { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        public string StreetType { get; set; }
        public string House { get; set; }
        public string Block { get; set; }
        public string Office { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Timetable { get; set; }
        public WorkTimes Worktime { get; set; }
        public int? FittingRoom { get; set; }
        public int? Cod { get; set; }
        public int? PaymentCash { get; set; }
        public int? PaymentCard { get; set; }
        public int? MultiplaceDeliveryAllowed { get; set; }
        public int? AvailableOperation { get; set; }
        public int? Type { get; set; }
        public string Description { get; set; }
        public List<string> Photos { get; set; }
        public List<Metro> Metro { get; set; }
        public List<Extra> Extra { get; set; }
        public Limits Limits { get; set; }
        public bool Enabled { get; set; }
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
        public string Name { get; set; }
        public decimal? Distance { get; set; }
        public string Line { get; set; }
    }

    public class Extra
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Limits
    {
        public int? MaxSizeA { get; set; }
        public int? MaxSizeB { get; set; }
        public int? MaxSizeC { get; set; }
        public int? MaxSizeSum { get; set; }
        public int? MaxWeight { get; set; }
        public int? MinWeight { get; set; }
        public decimal? MaxCod { get; set; }
        public int? MaxVolume { get; set; }
    }
}