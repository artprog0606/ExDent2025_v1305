using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.Products
{
    public class ProductPropertyGroupApi
    {
        [JsonIgnore]
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public List<ProductPropertyApi> Properties { get; set; }
    }

    public class ProductPropertyApi
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}