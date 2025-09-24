using System.Collections.Generic;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    public class YandexDeliveryTemplate : DefaultCargoParams
    {
        public const string ClientId = "ClientId";
        public const string SenderId = "SenderId";
        public const string RequisiteId = "RequisiteId";
        public const string WarehouseId = "WarehouseId";

        public const string ApiKeys = "ApiKeys";
        public const string ApiData = "ApiData";
        public const string SearchDeliveryListKey = "SearchDeliveryListKey";
        public const string CreateOrderKey = "CreateOrderKey";
        
        public const string CityFrom = "CityFrom";
        public const string SecretKeyDelivery = "SecretKeyDelivery";
        public const string SecretKeyCreateOrder = "SecretKeyCreateOrder";
        public const string WidgetCode = "WidgetCode";
        public const string IsActive = "IsActive";
        
        public const string ShowAssessedValue = "ShowAssessedValue";

        public const string StatusesSync = "StatusesSync";
        public const string Status_DRAFT = "Status_DRAFT";
        public const string Status_CREATED = "Status_CREATED";
        public const string Status_SENDER_SENT = "Status_SENDER_SENT";
        public const string Status_DELIVERY_LOADED = "Status_DELIVERY_LOADED";
        public const string Status_SENDER_WAIT_FULFILMENT = "Status_SENDER_WAIT_FULFILMENT";
        public const string Status_SENDER_WAIT_DELIVERY = "Status_SENDER_WAIT_DELIVERY";
        public const string Status_FULFILMENT_LOADED = "Status_FULFILMENT_LOADED";
        public const string Status_FULFILMENT_ARRIVED = "Status_FULFILMENT_ARRIVED";
        public const string Status_FULFILMENT_PREPARED = "Status_FULFILMENT_PREPARED";
        public const string Status_FULFILMENT_TRANSMITTED = "Status_FULFILMENT_TRANSMITTED";
        public const string Status_DELIVERY_AT_START = "Status_DELIVERY_AT_START";
        public const string Status_DELIVERY_TRANSPORTATION = "Status_DELIVERY_TRANSPORTATION";
        public const string Status_DELIVERY_ARRIVED = "Status_DELIVERY_ARRIVED";
        public const string Status_DELIVERY_TRANSPORTATION_RECIPIENT = "Status_DELIVERY_TRANSPORTATION_RECIPIENT";
        public const string Status_DELIVERY_ARRIVED_PICKUP_POINT = "Status_DELIVERY_ARRIVED_PICKUP_POINT";
        public const string Status_DELIVERY_DELIVERED = "Status_DELIVERY_DELIVERED";
        public const string Status_RETURN_PREPARING = "Status_RETURN_PREPARING";
        public const string Status_RETURN_ARRIVED_DELIVERY = "Status_RETURN_ARRIVED_DELIVERY";
        public const string Status_RETURN_ARRIVED_FULFILMENT = "Status_RETURN_ARRIVED_FULFILMENT";
        public const string Status_RETURN_PREPARING_SENDER = "Status_RETURN_PREPARING_SENDER";
        public const string Status_RETURN_RETURNED = "Status_RETURN_RETURNED";
        public const string Status_RETURN_TRANSMITTED_FULFILMENT = "Status_RETURN_TRANSMITTED_FULFILMENT";
        public const string Status_LOST = "Status_LOST";
        public const string Status_UNEXPECTED = "Status_UNEXPECTED";
        public const string Status_CANCELED = "Status_CANCELED";
        public const string Status_ERROR = "Status_ERROR";
    }

    public class YaDeliveryConfigParams
    {
        public string client_id { get; set; }
        public List<string> sender_ids { get; set; }
        public List<string> warehouse_ids { get; set; }
        public List<string> requisite_ids { get; set; }
    }

    public class YaDeliveryJsonConfigParams
    {
        public YaDeliveryJsonConfigParamItem client { get; set; }
        public List<YaDeliveryJsonConfigParamItem> warehouses { get; set; }
        public List<YaDeliveryJsonConfigParamItem> senders { get; set; }
        public List<YaDeliveryJsonConfigParamItem> requisites { get; set; }
    }

    public class YaDeliveryJsonConfigParamItem
    {
        public string id { get; set; }
        public string name { get; set; }
    }


    public class YandexDeliveryAdditionalData
    {
        public int direction { get; set; }
        public int delivery { get; set; }
        public int price { get; set; }
        public int tariffId { get; set; }
        public int? to_ms_warehouse { get; set; }
    }
}
