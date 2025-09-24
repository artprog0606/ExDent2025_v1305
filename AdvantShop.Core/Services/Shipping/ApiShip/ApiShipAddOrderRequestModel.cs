using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip
{
    public class ApiShipAddOrderRequestModel
    {
        public ApiShipOrder order { get; set; }
        public ApiShipCost cost { get; set; }
        public ApiShipSender sender { get; set; }
        public ApiShipRecipient recipient { get; set; }
        public ApiShipContact returnAddress { get; set; }
        public List<ApiShipPlaces> places { get; set; }
        public List<Extra> extraParams { get; set; }
    }

    public class ApiShipOrder
    {
        public string clientNumber { get; set; }
        public string barcode { get; set; }
        public string description { get; set; }
        public string providerKey { get; set; }
        public string providerConnectId { get; set; }
        public int pickupType { get; set; }
        public int deliveryType { get; set; }
        public int tariffId { get; set; }
        public int pointInId { get; set; }
        public int pointOutId { get; set; }
        public string pickupDate { get; set; }
        public string pickupTimeStart { get; set; }
        public string pickupTimeEnd { get; set; }
        public string deliveryDate { get; set; }
        public string deliveryTimeStart { get; set; }
        public string deliveryTimeEnd { get; set; }
        public string trackingUrl { get; set; }
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int? weight { get; set; }
    }

    public class ApiShipCost
    {
        public int assessedCost { get; set; }
        public int? deliveryCost { get; set; }
        public int? deliveryCostVat { get; set; }
        public int codCost { get; set; }
        public bool isDeliveryPayedByRecipient { get; set; }
        public int? paymentMethod { get; set; } //Способ оплаты заказа: - 1 - Наличные; - 2 - Карта; - 3 - Смешанная оплата(наличные и карта) - 4 - Безналичная оплата (по счету)
        public List<ApiShipCostCostThresholds> deliveryCostThresholds { get; set; }
    }
    public class ApiShipCostCostThresholds
    {
        public int deliveryCost { get; set; }
        public int threshold { get; set; }
    }

    public class ApiShipContact
    {
        public string countryCode { get; set; }
        public string postIndex { get; set; }
        public string region { get; set; }
        public string area { get; set; }
        public string city { get; set; }
        public string cityGuid { get; set; }
        public string community { get; set; }
        public string communityGuid { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string block { get; set; }
        public string office { get; set; }
        public float lat { get; set; }
        public float lng { get; set; }
        public string addressString { get; set; }
        public string companyName { get; set; }
        public string contactName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string comment { get; set; }
    }

    public class ApiShipSender : ApiShipContact
    {
        public string companyInn { get; set; }
        public string brandName { get; set; }
    }

    public class ApiShipRecipient : ApiShipContact
    {
        public string companyInn { get; set; }
        public string additionalPhone { get; set; }
    }

    public class ApiShipPlaces
    {
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int weight { get; set; }
        public string placeNumber { get; set; }
        public string barcode { get; set; }
        public List<ApiShipItem> items { get; set; }
    }

    public class ApiShipItem
    {
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int weight { get; set; }
        public string articul { get; set; }
        public string markCode { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public int? quantityDelivered { get; set; }
        public int? assessedCost { get; set; }
        public float? cost { get; set; }
        public int? costVat { get; set; }
        public string barcode { get; set; }
        public string companyName { get; set; }
        public string companyInn { get; set; }
        public string companyPhone { get; set; }
        public string tnved { get; set; }
        public string url { get; set; }
    }
}