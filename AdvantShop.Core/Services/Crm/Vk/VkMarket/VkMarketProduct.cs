using System.Collections.Generic;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using VkNet.Enums;
using VkNet.Model;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket
{
    public class VkMarketGetProductsResponse : IVkError
    {
        public VkApiError Error { get; set; }
        public VkMarketProductResponse Response { get; set; }
    }
    
    public class VkMarketProductResponse
    {
        public int Count { get; set; }
        public List<VkMarketProduct> Items { get; set; }
    }
    
    public class VkMarketProduct : Market
    {
        /// <summary>
        /// Список всех вариантов товара
        /// </summary>
        [JsonProperty("variants")]
        public List<VkMarketVariant> Variants { get; set; }
        
        /// <summary>
        /// Варианты для данного товара
        /// </summary>
        [JsonProperty("variants_grid")]
        public List<VkMarketVariantGridItem> VariantsGrid { get; set; }
        
        /// <summary>
        /// Id группировки
        /// </summary>
        [JsonProperty("variants_grouping_id")]
        public long VariantsGroupingId { get; set; }
        
        /// <summary>
        /// Главный вариант
        /// </summary>
        [JsonProperty("is_main_variant")]
        public bool IsMainVariant { get; set; }
        
        /// <summary>
        /// Список всех свойст (цвет/размер и их значения)
        /// </summary>
        [JsonProperty("properties")]
        public List<VkMarketProperty> Properties { get; set; }
        
        /// <summary>
        /// Свойства этого товара 
        /// </summary>
        [JsonProperty("property_values")]
        public List<VkMarketProductPropertyValue> ProductPropertyValues { get; set; }
        
        /// <summary>
        /// Список альбомов товара
        /// </summary>
        [JsonProperty("albums_ids")]
        public List<long> AlbumIds { get; set; }
        
        public string Sku { get; set; }
    }

    public class VkMarketVariant
    {
        [JsonProperty("item_id")]
        public long ItemId { get; set; }
        
        [JsonProperty("availability")]
        public ProductAvailability Availability { get; set; }
        
        [JsonProperty("variant_ids")]
        public List<long> VariantIds { get; set; }
    }

    public class VkMarketVariantGridItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("variants")]
        public List<VkMarketVariantGridItemVariant> Variants { get; set; }
    }
    
    public class VkMarketVariantGridItemVariant
    {
        [JsonProperty("variant_id")]
        public long VariantId { get; set; }
        
        [JsonProperty("item_id")]
        public long ItemId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("is_selected")]
        public bool IsSelected { get; set; }
    }

    public class VkMarketProperty
    {
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        [JsonProperty("variants")]
        public List<VkMarketPropertyValue> PropertyValues { get; set; }
    }

    public class VkMarketPropertyValue
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class VkMarketProductPropertyValue
    {
        [JsonProperty("variant_id")]
        public long VariantId { get; set; }
        
        [JsonProperty("variant_name")]
        public string VariantName { get; set; }
        
        [JsonProperty("property_name")]
        public string PropertyName { get; set; }
    }
}