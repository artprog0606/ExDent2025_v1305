using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvantShop.Shipping.ShippingYandexDelivery
{
    public class YandexDeliveryListDto
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<YandexDeliveryListItem> Data { get; set; }
    }

    public class YandexDeliveryListItem
    {
        [JsonProperty(PropertyName = "delivery")]
        public YaDeliveryDataDelivery Delivery { get; set; }


        [JsonProperty(PropertyName = "pickupPoints")]
        public List<YaDeliveryPicpoint> PickupPoints { get; set; }

        /// <summary>
        /// Срок доставки. может быть число, может быть строка (1-3)
        /// </summary>
        [JsonProperty(PropertyName = "days")]
        public string Days { get; set; }

        /// <summary>
        /// Название службы доставки
        /// </summary>
        [JsonProperty(PropertyName = "delivery_name")]
        public string DeliveryName { get; set; }

        /// <summary>
        /// Итоговая стоимость доставки для покупателя
        /// </summary>
        [JsonProperty(PropertyName = "costWithRules")]
        public float CostWithRules { get; set; }


        [JsonProperty(PropertyName = "tariffId")]
        public string TariffId { get; set; }

        [JsonProperty(PropertyName = "tariffName")]
        public string TariffName { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }


        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }


        [JsonProperty(PropertyName = "settings")]
        public YaDeliverySettings Settings { get; set; }

    }

    public class YaDeliveryDataDelivery
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class YaDeliveryPicpoint
    {
        public string id { get; set; }
        public string delivery_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string location_name { get; set; }
        public string full_address { get; set; }
        public string has_payment_cash { get; set; }
        public string has_payment_card { get; set; }
    }

    public class YaDeliverySettings
    {
        [JsonProperty(PropertyName = "to_yd_warehouse")]
        public string ToYDWarehouse { get; set; }
    }

    //31.05.18 переписать объекты ответов с зависимостью YandexDeliveryResponse
    public class YandexDeliveryResponse<T>
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }

    public class YandexDeliveryResponseAutocomplete
    {
        [JsonProperty(PropertyName = "suggestions")]
        public List<YandexDeliveryResponseAutocompleteItem> Suggestions { get; set; }
    }

    public class YandexDeliveryResponseAutocompleteItem
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "geo_id")]
        public string GeoId { get; set; }
    }

    public class YandexDeliveryResponseCreateOrder
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Данные ответа
        /// </summary>
        [JsonProperty("data")]
        public CreateOrderData Data { get; set; }
    }

    public class CreateOrderData
    {
        /// <summary>
        /// Данные о заказе
        /// </summary>
        [JsonProperty("order")]
        public CreatedOrder Order { get; set; }
    }

    public class CreatedOrder
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Номер заказа в магазине
        /// </summary>
        [JsonProperty("num")]
        public string Number { get; set; }
    }

    public class GetOrderStatuses
    {
        [JsonProperty("data")]
        public List<YandexOrderStatus> Data { get; set; }
    }

    public class YandexOrderStatus
    {
        [JsonProperty("time")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("uniform_status")]
        public string Status { get; set; }

        [JsonProperty("status")]
        public string StatusComment { get; set; }
    }

    public class GetDeliveries
    {
        [JsonProperty("deliveries")]
        public List<YandexDeliveryDto> Deliveries { get; set; }
    }

    public class YandexDeliveryDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("unique_name")]
        public string UniqueName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("warehouses")]
        public List<Warehous> Warehouses { get; set; }
    }

    public class Warehous
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }


    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}