using AdvantShop.Shipping;

namespace AdvantShop.Shipping.ShippingYandexNewDelivery
{
    public class YandexNewDeliveryTemplate : DefaultCargoParams
    {
        public const string AuthorizationToken = "AuthorizationToken";
        public const string ShopId = "ShopId";
        public const string WarehouseId = "WarehouseId";
        public const string CityFrom = "CityFrom";

        public const string WidgetApiKey = "WidgetApiKey";

        public const string TypeViewPoints = "TypeViewPoints";
        public const string YaMapsApiKey = "YaMapsApiKey";

        public const string PriceDisplayType = "PriceDisplayType";

        public const string ExportOrderExternalIdType = "ExportOrderExternalIdType";

        public const string CourierOptionName = "CourierOptionName";
        public const string PickupOptionName = "PickupOptionName";

        #region Statuses

        public const string StatusesSync = "StatusesSync";
        public const string Status_DRAFT = "Status_DRAFT";
        public const string Status_VALIDATING = "Status_VALIDATING";
        public const string Status_VALIDATING_ERROR = "Status_VALIDATING_ERROR";
        public const string Status_DELIVERY_PROCESSING_STARTED = "Status_DELIVERY_PROCESSING_STARTED";
        public const string Status_DELIVERY_TRACK_RECEIVED = "Status_DELIVERY_TRACK_RECEIVED";
        public const string Status_CREATED = "Status_CREATED";
        public const string Status_SENDER_SENT = "Status_SENDER_SENT";
        public const string Status_DELIVERY_LOADED = "Status_DELIVERY_LOADED";
        public const string Status_SENDER_WAIT_FULFILMENT = "Status_SENDER_WAIT_FULFILMENT";
        public const string Status_SENDER_WAIT_DELIVERY = "Status_SENDER_WAIT_DELIVERY";
        public const string Status_DELIVERY_AT_START = "Status_DELIVERY_AT_START";
        public const string Status_DELIVERY_AT_START_SORT = "Status_DELIVERY_AT_START_SORT";
        public const string Status_DELIVERY_TRANSPORTATION = "Status_DELIVERY_TRANSPORTATION";
        public const string Status_DELIVERY_ARRIVED = "Status_DELIVERY_ARRIVED";
        public const string Status_DELIVERY_TRANSPORTATION_RECIPIENT = "Status_DELIVERY_TRANSPORTATION_RECIPIENT";
        public const string Status_DELIVERY_CUSTOMS_ARRIVED = "Status_DELIVERY_CUSTOMS_ARRIVED";
        public const string Status_DELIVERY_CUSTOMS_CLEARED = "Status_DELIVERY_CUSTOMS_CLEARED";
        public const string Status_DELIVERY_STORAGE_PERIOD_EXTENDED = "Status_DELIVERY_STORAGE_PERIOD_EXTENDED";
        public const string Status_DELIVERY_STORAGE_PERIOD_EXPIRED = "Status_DELIVERY_STORAGE_PERIOD_EXPIRED";
        public const string Status_DELIVERY_UPDATED_BY_SHOP = "Status_DELIVERY_UPDATED_BY_SHOP";
        public const string Status_DELIVERY_UPDATED_BY_RECIPIENT = "Status_DELIVERY_UPDATED_BY_RECIPIENT";
        public const string Status_DELIVERY_UPDATED_BY_DELIVERY = "Status_DELIVERY_UPDATED_BY_DELIVERY";
        public const string Status_DELIVERY_ARRIVED_PICKUP_POINT = "Status_DELIVERY_ARRIVED_PICKUP_POINT";
        public const string Status_DELIVERY_TRANSMITTED_TO_RECIPIENT = "Status_DELIVERY_TRANSMITTED_TO_RECIPIENT";
        public const string Status_DELIVERY_DELIVERED = "Status_DELIVERY_DELIVERED";
        public const string Status_DELIVERY_ATTEMPT_FAILED = "Status_DELIVERY_ATTEMPT_FAILED";
        public const string Status_DELIVERY_CAN_NOT_BE_COMPLETED = "Status_DELIVERY_CAN_NOT_BE_COMPLETED";
        public const string Status_RETURN_STARTED = "Status_RETURN_STARTED";
        public const string Status_RETURN_PREPARING = "Status_RETURN_PREPARING";
        public const string Status_RETURN_ARRIVED_DELIVERY = "Status_RETURN_ARRIVED_DELIVERY";
        public const string Status_RETURN_TRANSMITTED_FULFILLMENT = "Status_RETURN_TRANSMITTED_FULFILLMENT";
        public const string Status_SORTING_CENTER_CREATED = "Status_SORTING_CENTER_CREATED";
        public const string Status_SORTING_CENTER_LOADED = "Status_SORTING_CENTER_LOADED";
        public const string Status_SORTING_CENTER_AT_START = "Status_SORTING_CENTER_AT_START";
        public const string Status_SORTING_CENTER_OUT_OF_STOCK = "Status_SORTING_CENTER_OUT_OF_STOCK";
        public const string Status_SORTING_CENTER_AWAITING_CLARIFICATION = "Status_SORTING_CENTER_AWAITING_CLARIFICATION";
        public const string Status_SORTING_CENTER_PREPARED = "Status_SORTING_CENTER_PREPARED";
        public const string Status_SORTING_CENTER_TRANSMITTED = "Status_SORTING_CENTER_TRANSMITTED";
        public const string Status_SORTING_CENTER_RETURN_PREPARING = "Status_SORTING_CENTER_RETURN_PREPARING";
        public const string Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT = "Status_SORTING_CENTER_RETURN_RFF_PREPARING_FULFILLMENT";
        public const string Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT = "Status_SORTING_CENTER_RETURN_RFF_TRANSMITTED_FULFILLMENT";
        public const string Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT = "Status_SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT";
        public const string Status_SORTING_CENTER_RETURN_ARRIVED = "Status_SORTING_CENTER_RETURN_ARRIVED";
        public const string Status_SORTING_CENTER_RETURN_PREPARING_SENDER = "Status_SORTING_CENTER_RETURN_PREPARING_SENDER";
        public const string Status_SORTING_CENTER_RETURN_TRANSFERRED = "Status_SORTING_CENTER_RETURN_TRANSFERRED";
        public const string Status_SORTING_CENTER_RETURN_RETURNED = "Status_SORTING_CENTER_RETURN_RETURNED";
        public const string Status_SORTING_CENTER_CANCELED = "Status_SORTING_CENTER_CANCELED";
        public const string Status_SORTING_CENTER_ERROR = "Status_SORTING_CENTER_ERROR";
        public const string Status_LOST = "Status_LOST";
        public const string Status_CANCELLED = "Status_CANCELLED";
        public const string Status_CANCELLED_USER = "Status_CANCELLED_USER";
        public const string Status_FINISHED = "Status_FINISHED";
        public const string Status_ERROR = "Status_ERROR";

        #endregion
    }

    public class YandexNewDeliveryAdditionalData
    {
        public int tariffId { get; set; }
        public int partnerId { get; set; }
        public string deliveryType { get; set; }
    }
}
