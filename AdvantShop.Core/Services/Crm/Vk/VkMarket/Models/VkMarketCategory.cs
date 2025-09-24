using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Models
{
    public class FilterCategoriesResponse : IVkError
    {
        public VkMarketCategoryResponse Response { get; set; }
        public VkApiError Error { get; set; }
    }
    
    public class VkMarketCategoryResponse
    {
        public List<VkMarketCategoryItem> Items { get; set; }
    }
    
    public class VkMarketCategoryItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public VkMarketCategoryView View { get; set; }
    }
    
    public class VkMarketCategoryView
    {
        [JsonProperty("root_path")]
        public List<string> RootPath { get; set; }
    }
}
