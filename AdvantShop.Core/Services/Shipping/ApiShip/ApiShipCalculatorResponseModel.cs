using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipCalculatorResponseModel
    {
        public List<ApiShipDeliveryToDoor> deliveryToDoor {  get; set; }
        public List<ApiShipDeliveryToPoint> deliveryToPoint { get; set; }

    }

    public class ApiShipDeliveryToDoor
    {
        public string providerKey { get; set; }
        public List<ApiShipTariffToDoor> tariffs { get; set; }
    }

    public class ApiShipTariffToDoor
    {
        public string tariffProviderId { get; set; }
        public int? tariffId { get; set; }
        public string tariffName { get; set; }
        public List<int> pickupTypes { get; set; }
        public List<int> deliveryTypes { get; set; }
        public decimal? deliveryCost { get; set; }
        public decimal? deliveryCostOriginal { get; set; }
        public bool feesIncluded { get; set; }
        public int? insuranceFee { get; set; }
        public int? cashServiceFee { get; set; }
        public int? daysMax { get; set; }
        public int? daysMin { get; set; }
    }

    public class ApiShipDeliveryToPoint
    {
        public string providerKey { get; set; }
        public List<ApiShipTariffToPoint> tariffs { get; set; }
    }

    public class ApiShipTariffToPoint : ApiShipTariffToDoor
    {
        public List<int> pointIds { get; set; }
    }
}
