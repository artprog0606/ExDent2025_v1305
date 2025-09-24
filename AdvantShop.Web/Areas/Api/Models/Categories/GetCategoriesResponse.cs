using AdvantShop.Core.Services.Api;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public class GetCategoriesItem : CategoryBaseModel
    {
        [JsonProperty(Order = 10)]
        public int Level { get; set; }
        
        [JsonProperty(Order = 20)]
        public int ChildCategoriesCount { get; set; }
        
        [JsonProperty(Order = 30)]
        public int ProductsCount { get; set; }
        
        [JsonProperty(Order = 40)]
        public int ProductsCountInCategory { get; set; }
    }
    
    public class GetCategoriesItemExtended : CategoryModel
    {
        [JsonProperty(Order = 10)]
        public int Level { get; set; }
        
        [JsonProperty(Order = 20)]
        public int ChildCategoriesCount { get; set; }
        
        [JsonProperty(Order = 30)]
        public int ProductsCount { get; set; }
        
        [JsonProperty(Order = 40)]
        public int ProductsCountInCategory { get; set; }
    }

    public class GetCategoriesResponse : ApiPaginationResponse, IApiResponse
    {
        [JsonProperty("categories")]
        public List<ICategoryApi> Categories { get; set; }
    }
}