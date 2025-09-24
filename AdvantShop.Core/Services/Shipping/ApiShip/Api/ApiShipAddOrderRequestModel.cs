using System.Collections.Generic;

namespace AdvantShop.Shipping.ApiShip.Api
{
    public class ApiShipAddOrderRequestModel
    {
        public ApiShipOrder Order { get; set; }
        public ApiShipCost Cost { get; set; }
        public ApiShipSender Sender { get; set; }
        public ApiShipRecipient Recipient { get; set; }
        public ApiShipContact ReturnAddress { get; set; }
        public List<ApiShipPlaces> Places { get; set; }
        public List<Extra> ExtraParams { get; set; }
    }

    public class ApiShipOrder
    {
        public string ClientNumber { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderConnectId { get; set; }
        public int PickupType { get; set; }
        public int DeliveryType { get; set; }
        public int TariffId { get; set; }
        public int? PointInId { get; set; }
        public int? PointOutId { get; set; }
        public string PickupDate { get; set; }
        public string PickupTimeStart { get; set; }
        public string PickupTimeEnd { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTimeStart { get; set; }
        public string DeliveryTimeEnd { get; set; }
        public string TrackingUrl { get; set; }
        public int? Height { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int? Weight { get; set; }
    }

    public class ApiShipCost
    {
        public float AssessedCost { get; set; }
        public float? DeliveryCost { get; set; }
        public int? DeliveryCostVat { get; set; }
        public float CodCost { get; set; }
        public bool IsDeliveryPayedByRecipient { get; set; }
        public int? PaymentMethod { get; set; } //Способ оплаты заказа: - 1 - Наличные; - 2 - Карта; - 3 - Смешанная оплата(наличные и карта) - 4 - Безналичная оплата (по счету)
        public List<ApiShipCostCostThresholds> DeliveryCostThresholds { get; set; }
    }

    public class ApiShipCostCostThresholds
    {
        public int DeliveryCost { get; set; }
        public int Threshold { get; set; }
    }

    public class ApiShipContact
    {
        public string CountryCode { get; set; }
        public string PostIndex { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CityGuid { get; set; }
        public string Community { get; set; }
        public string CommunityGuid { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Block { get; set; }
        public string Office { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string AddressString { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }

    public class ApiShipSender : ApiShipContact
    {
        public string CompanyInn { get; set; }
        public string BrandName { get; set; }
    }

    public class ApiShipRecipient : ApiShipContact
    {
        public string CompanyInn { get; set; }
        public string AdditionalPhone { get; set; }
    }

    public class ApiShipPlaces
    {
        public int? Height { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int Weight { get; set; }
        public string PlaceNumber { get; set; }
        public string Barcode { get; set; }
        public List<ApiShipItem> Items { get; set; }
    }

    public class ApiShipItem
    {
        public int? Height { get; set; }
        public int? Length { get; set; }
        public int? Width { get; set; }
        public int Weight { get; set; }
        public string Articul { get; set; }
        public string MarkCode { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int? QuantityDelivered { get; set; }
        public float? AssessedCost { get; set; }
        public float? Cost { get; set; }
        public int? CostVat { get; set; }
        public string Barcode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyInn { get; set; }
        public string CompanyPhone { get; set; }
        public string Tnved { get; set; }
        public string Url { get; set; }
    }
}