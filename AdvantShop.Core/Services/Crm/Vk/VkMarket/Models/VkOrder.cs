using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    // docs: https://vk.com/dev/market.getGroupOrders
    // docs: Заказ - Объект, содержащий информацию о заказе https://dev.vk.com/ru/reference/objects/market-order

    public enum VkOrderStatus
    {
        New = 0,
        Conform = 1,
        Gather = 2,
        Delivered = 3,
        Complete = 4,
        Canceled = 5,
        Returned = 6
    }

    public sealed class VkOrderResult : IVkError
    {
        public VkOrderResponse Response { get; set; }
        public VkApiError Error { get; set; }
    }

    public sealed class VkOrderResponse
    {
        public int Count { get; set; }
        public List<VkOrder> Items { get; set; }
    }

    public sealed class VkOrder
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public int DateUnix { get; set; }

        /// <summary>
        /// статус заказа:
        /// 0 - Новый,
        /// 1 - Согласуется,
        /// 2 - Собирается,
        /// 3 - Доставляется,
        /// 4 - Выполнен,
        /// 5 - Отменен,
        /// 6 - Возвращен.
        /// </summary>
        public VkOrderStatus Status { get; set; }

        [JsonProperty(PropertyName = "items_count")]
        public int ItemsCount { get; set; }

        [JsonProperty(PropertyName = "total_price")]
        public VkOrderPrice TotalPrice { get; set; }

        [JsonProperty(PropertyName = "track_number")]
        public string TrackNumber { get; set; }

        [JsonProperty(PropertyName = "track_link")]
        public string TrackLink { get; set; }

        public string Comment { get; set; }

        public VkOrderDelivery Delivery { get; set; }

        public VkOrderRecipient Recipient { get; set; }

        [JsonProperty(PropertyName = "preview_order_items")]
        public List<VkOrderItem> OrderItems { get; set; }
        
        public VkOrderPayment Payment { get; set; }
        
        [JsonProperty(PropertyName = "price_details")]
        public List<VkOrderPriceDetails> PriceDetails { get; set; }
    }

    public sealed class VkOrderPrice
    {
        /// <summary>
        /// стоимость в сотых долях единицы валюты
        /// </summary>
        public string Amount { get; set; }

        public VkOrderCurrency Currency { get; set; }

        /// <summary>
        /// строковое представление стоимости заказа
        /// </summary>
        public string Text { get; set; }
    }

    public sealed class VkOrderCurrency
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public sealed class VkOrderDelivery
    {
        public string Address { get; set; }
        public string Type { get; set; }
        
        /// <summary>
        /// default, service - сервис доставки (сдек), self - самовывоз
        /// </summary>
        [JsonProperty(PropertyName = "type_id")]
        public string TypeId { get; set; }
        
        [JsonProperty(PropertyName = "track_number")]
        public string TrackNumber { get; set; }
        
        [JsonProperty(PropertyName = "track_link")]
        public string TrackLink { get; set; }
        
        [JsonProperty(PropertyName = "delivery_point")]
        public object DeliveryPoint { get; set; } 
    }

    public sealed class VkOrderRecipient
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        
        [JsonProperty(PropertyName = "display_text")]
        public string DisplayText { get; set; }
    }

    public sealed class VkOrderPayment
    {
        public string Status { get; set; }
        
        [JsonProperty(PropertyName = "payment_status")]
        public string PaymentStatus { get; set; }
    }
    
    public sealed class VkOrderPriceDetails
    {
        public string Title { get; set; }
        public VkOrderPrice Price { get; set; }
    }

    public sealed class VkOrderItem
    {
        [JsonProperty(PropertyName = "item_id")]
        public int ItemId { get; set; }
        
        public VkOrderItemPrice Price { get; set; }

        public int Quantity { get; set; }

        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "item")]
        public VkOrderItemItem Item { get; set; }
    }

    public sealed class VkOrderItemPrice
    {
        public string Amount { get; set; }
        public VkOrderCurrency Currency { get; set; }
    }

    public sealed class VkOrderItemItem
    {
        [JsonProperty(PropertyName = "property_values")]
        public List<VkOrderItemPropertyValue> PropertyValues { get; set; }
    }

    public sealed class VkOrderItemPropertyValue
    {
        [JsonProperty(PropertyName = "variant_id")]
        public long VariantId { get; set; }

        [JsonProperty(PropertyName = "variant_name")]
        public string VariantName { get; set; }

        [JsonProperty(PropertyName = "property_name")]
        public string PropertyName { get; set; }
    }
}
