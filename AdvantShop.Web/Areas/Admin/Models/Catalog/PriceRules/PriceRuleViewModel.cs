using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Models.Catalog.PriceRules
{
    public class PriceRuleViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public float Amount { get; set; }
        
        public List<int> CustomerGroupIds { get; set;  }

        public int? PaymentMethodId { get; set; }
        public int? ShippingMethodId { get; set; }

        public List<int> WarehouseIds { get; set; }

        public bool ApplyDiscounts { get; set; }
        
        public bool Enabled { get; set; }
        
        public PriceRuleViewModel(){}
        
        public PriceRuleViewModel(PriceRule priceRule)
        {
            Id = priceRule.Id;
            Name = priceRule.Name;
            SortOrder = priceRule.SortOrder;
            Amount = priceRule.Amount;
            CustomerGroupIds = priceRule.CustomerGroupIds;
            PaymentMethodId = priceRule.PaymentMethodId;
            ShippingMethodId = priceRule.ShippingMethodId;
            WarehouseIds = priceRule.WarehouseIds;
            ApplyDiscounts = priceRule.ApplyDiscounts;
            Enabled = priceRule.Enabled;
        }

        public static explicit operator PriceRule(PriceRuleViewModel priceRule) =>
            new PriceRule()
            {
                Id = priceRule.Id,
                Name = priceRule.Name,
                SortOrder = priceRule.SortOrder,
                Amount = priceRule.Amount,
                CustomerGroupIds = priceRule.CustomerGroupIds ?? new List<int>(),
                PaymentMethodId = priceRule.PaymentMethodId,
                ShippingMethodId = priceRule.ShippingMethodId,
                WarehouseIds = priceRule.WarehouseIds ?? new List<int>(),
                ApplyDiscounts = priceRule.ApplyDiscounts,
                Enabled = priceRule.Enabled
            };
    }
}