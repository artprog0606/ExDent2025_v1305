using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipTariffsResponseModel
    {
        public List<ApiShipTariff> Rows { get; set; }
        public MetaTariff Meta {  get; set; }
    }

    public class ApiShipTariff
    {
        public string Id { get; set; }
        public string ProviderKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AliasName { get; set; }
        public int? WeightMin { get; set; }
        public int? WeightMax { get; set; }
        public int? PickupType { get; set; }
        public int? DeliveryType { get; set; }
    }

    public class MetaTariff
    {
        public int? Total { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
}
