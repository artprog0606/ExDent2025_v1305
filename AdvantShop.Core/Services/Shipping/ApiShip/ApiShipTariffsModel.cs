using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipTariffsResponseModel
    {
        public List<ApiShipTariff> rows { get; set; }
        public MetaTariff meta {  get; set; }
    }

    public class ApiShipTariff
    {
        public string id { get; set; }
        public string providerKey { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string aliasName { get; set; }
        public int? weightMin { get; set; }
        public int? weightMax { get; set; }
        public int? pickupType { get; set; }
        public int? deliveryType { get; set; }
    }

    public class MetaTariff
    {
        public int? total { get; set; }
        public int? offset { get; set; }
        public int? limit { get; set; }
    }
}
