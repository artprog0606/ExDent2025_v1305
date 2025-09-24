//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Payment.Alfabank
{
    public class Receipt
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? OrderCreationDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CustomerDetails CustomerDetails { get; set; }

        [JsonProperty(Required = Required.Always)]
        public CartItems CartItems { get; set; }
    }

    public class CartItems
    {
        [JsonProperty(Required = Required.Always)]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonProperty(Required = Required.Always)]
        public long PositionId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ItemDetails ItemDetails { get; set; }

        [JsonProperty(Required = Required.Always)]
        public BaseQuantity Quantity { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long ItemAmount { get; set; }

        [JsonProperty(PropertyName = "itemCurrency", NullValueHandling = NullValueHandling.Ignore)]
        public string ItemCurrencyISO { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ItemCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Discount Discount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AgentInterest AgentInterest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Tax Tax { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long ItemPrice { get; set; }

        public ItemAttributes ItemAttributes { get; set; }
    }

    public class BaseQuantity
    {
        [JsonProperty(Required = Required.Always)]
        public float Value { get; set; }
    }
    
    public class Quantity<T> : BaseQuantity
    {
        // [JsonProperty(Required = Required.Always)]
        public T Measure { get; set; }
    }

    public class Tax
    {
        [JsonProperty(Required = Required.Always)]
        public byte TaxType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? TaxSum { get; set; }
    }

    public class AgentInterest
    {
        [JsonProperty(Required = Required.Always)]
        public string InterestType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long InterestValue { get; set; }
    }

    public class Discount
    {
        [JsonProperty(Required = Required.Always)]
        public string DiscountType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long DiscountValue { get; set; }
    }

    public class ItemDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ItemDetailsParam> ItemDetailsParams { get; set; }
    }

    public class ItemDetailsParam
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Value { get; set; }
    }

    public class CustomerDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? Phone { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Contact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DeliveryInfo DeliveryInfo { get; set; }
    }

    public class DeliveryInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DeliveryType { get; set; }

        [JsonProperty(PropertyName = "country", Required = Required.Always)]
        public string CountryISO { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string City { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string PostAddress { get; set; }
    }

    public class ItemAttributes
    {
        public ItemAttribute[] Attributes { get; set; }
    }

    public class ItemAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
