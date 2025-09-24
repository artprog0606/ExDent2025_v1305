using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipCalculatorResponseModel : ApiShipErrorModel
    {
        public List<ApiShipDeliveryToDoor> DeliveryToDoor {  get; set; }
        public List<ApiShipDeliveryToPoint> DeliveryToPoint { get; set; }

    }

    public class ApiShipDeliveryToDoor
    {
        public string ProviderKey { get; set; }
        public List<ApiShipTariffToDoor> Tariffs { get; set; }
    }

    public class ApiShipTariffToDoor
    {
        public string TariffProviderId { get; set; }
        public int? TariffId { get; set; }
        public string TariffName { get; set; }
        public List<int> PickupTypes { get; set; }
        public List<int> DeliveryTypes { get; set; }
        public decimal? DeliveryCost { get; set; }
        public decimal? DeliveryCostOriginal { get; set; }
        public bool? FeesIncluded { get; set; }
        public decimal? InsuranceFee { get; set; }
        public decimal? CashServiceFee { get; set; }
        public int? DaysMax { get; set; }
        public int? DaysMin { get; set; }
        [JsonIgnore]
        public string ProviderKey { get; set; }
    }

    public class ApiShipDeliveryToPoint
    {
        public string ProviderKey { get; set; }
        public List<ApiShipTariffToPoint> Tariffs { get; set; }
    }

    public class ApiShipTariffToPoint : ApiShipTariffToDoor
    {
        public List<int> PointIds { get; set; }
    }
}
