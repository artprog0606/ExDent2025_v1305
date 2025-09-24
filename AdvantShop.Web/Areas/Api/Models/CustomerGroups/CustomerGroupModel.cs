using AdvantShop.Customers;
using Newtonsoft.Json;

namespace AdvantShop.Areas.Api.Models.CustomerGroups
{
    public sealed class CustomerGroupModel
    {
        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("discountPercent")]
        public float DiscountPercent { get; }
        
        [JsonProperty("minimumOrderPrice")]
        public float MinimumOrderPrice { get; }
        
        public CustomerGroupModel(CustomerGroup group)
        {
            Id = group.CustomerGroupId;
            Name = group.GroupName;
            DiscountPercent = group.GroupDiscount;
            MinimumOrderPrice = group.MinimumOrderPrice;
        }
    }
}