using System;
using System.Collections.Generic;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewDeliveryLocationDto
    {
        [JsonProperty(PropertyName = "geoId")]
        public int GeoId { get; set; }
    }

    public class YandexNewDeliveryListItem
    {
        [JsonProperty(PropertyName = "tariffId")]
        public int TariffId { get; set; }

        [JsonProperty(PropertyName = "tariffName")]
        public string TariffName { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public YaNewDeliveryCost Cost { get; set; }

        [JsonProperty(PropertyName = "delivery")]
        public YaNewDelivery Delivery { get; set; }

        [JsonProperty(PropertyName = "pickupPointIds")]
        public List<long> PickupPointIds { get; set; }

        [JsonProperty(PropertyName = "services")]
        public List<YaNewDeliveryService> Services { get; set; }

        [JsonProperty(PropertyName = "shipments")]
        public List<YaNewDeliveryShipment> Shipments { get; set; }
    }

    public class YaNewDeliveryCost
    {
        [JsonProperty(PropertyName = "delivery")]
        public float Cost { get; set; }

        [JsonProperty(PropertyName = "deliveryForSender")]
        public float CostForSender { get; set; }

        [JsonProperty(PropertyName = "deliveryForCustomer")]
        public float CostForCustomer { get; set; }
    }

    public class YaNewDelivery
    {
        [JsonProperty(PropertyName = "partner")]
        public YaNewDeliveryPartner Partner { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "calculatedDeliveryDateMin")]
        public string CalculatedDeliveryDateMin { get; set; }

        [JsonProperty(PropertyName = "calculatedDeliveryDateMax")]
        public string CalculatedDeliveryDateMax { get; set; }
    }

    public class YaNewDeliveryPartner
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }


    public class YaNewDeliveryService
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public float Cost { get; set; }

        [JsonProperty(PropertyName = "customerPay")]
        public bool CustomerPay { get; set; }
    }

    public class YaNewDeliveryShipment
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "warehouse")]
        public YaNewDeliveryShipmentWarehouse Warehouse { get; set; }

        [JsonProperty(PropertyName = "partner")]
        public YaNewDeliveryShipmentPartner Partner { get; set; }
    }

    public class YaNewDeliveryShipmentWarehouse
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
    }

    public class YaNewDeliveryShipmentPartner
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
    }

    public class YandexNewDeliveryProductItemDto
    {
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "assessedValue")]
        public float AssessedValue { get; set; }

        [JsonProperty(PropertyName = "tax")]
        public string Tax { get; set; }
        
        public YandexNewDeliveryProductItemDto() { }
        public YandexNewDeliveryProductItemDto(OrderItem item)
        {
            ExternalId = item.ArtNo;
            Name = item.Name;

            Count = (int)Math.Round(item.Amount);

            Price = item.Price;
            AssessedValue = item.Price * item.Amount;

            var tax = "NO_VAT";
            if (item.TaxType == TaxType.Vat0)
                tax = "VAT_0";
            if (item.TaxType == TaxType.Vat10)
                tax = "VAT_10";
            if (item.TaxType == TaxType.Vat20)
                tax = "VAT_20";
            Tax = tax;
        }
    }

    public class YandexNewDeliveryRecipientDto
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "middleName", NullValueHandling = NullValueHandling.Ignore)]
        public string MiddleName { get; set; }

        [JsonProperty(PropertyName = "lastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "address")]
        public object Address { get; set; }

        [JsonProperty(PropertyName = "pickupPointId", NullValueHandling = NullValueHandling.Ignore)]
        public long? PickupPointId { get; set; }
    }

    public class YandexNewDeliveryOrderStatuses
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }

        [JsonProperty(PropertyName = "statuses")]
        public List<YaNewDeliveryStatus> Statuses { get; set; }
    }

    public class YaNewDeliveryStatus
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "datetime")]
        public DateTime Datetime { get; set; }
    }


    public class YandexNewDeliveryPickupPointDto
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "partnerId")]
        public int PartnerId { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "instruction")]
        public string Instruction { get; set; }

        [JsonProperty(PropertyName = "address")]
        public YaNewDeliveryPickupPointAddress Address { get; set; }

        [JsonProperty(PropertyName = "supportedFeatures")]
        public YaNewDeliveryPickupPointSupportedFeatures SupportedFeatures { get; set; }

        [JsonProperty("phones")]
        public List<YaNewDeliveryPickupPointPhone> Phones { get; set; }
        
        [JsonProperty("schedule")]
        public List<YaNewDeliveryPickupPointSchedule> Schedule { get; set; }
    }

    public class YaNewDeliveryPickupPointSchedule
    {
        [JsonProperty("day")]
        public byte Day { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class YaNewDeliveryPickupPointPhone
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("internalNumber")]
        public string InternalNumber { get; set; }
    }

    public class YaNewDeliveryPickupPointAddress
    {
        [JsonProperty(PropertyName = "locationId")]
        public int LocationId { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "addressString")]
        public string AddressString { get; set; }

        [JsonProperty(PropertyName = "shortAddressString")]
        public string ShortAddressString { get; set; }
    }

    public class YaNewDeliveryPickupPointSupportedFeatures
    {
        [JsonProperty(PropertyName = "cash")]
        public bool Cash { get; set; }

        [JsonProperty(PropertyName = "prepay")]
        public bool Prepay { get; set; }

        [JsonProperty(PropertyName = "card")]
        public bool Card { get; set; }

        [JsonProperty(PropertyName = "orderReturn")]
        public bool OrderReturn { get; set; }
    }

    public class YandexNewDeliveryOrderDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }
        
        [JsonProperty(PropertyName = "deliveryType")]
        public string DeliveryType { get; set; }
        
        [JsonProperty(PropertyName = "deliveryServiceExternalId")]
        public string DeliveryServiceExternalId { get; set; }
    }
    
    #region Errors

    public class YandexNewDeliveryErrorDto
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    #endregion
}
