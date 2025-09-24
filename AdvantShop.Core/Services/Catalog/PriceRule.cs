using System.Collections.Generic;

namespace AdvantShop.Core.Services.Catalog
{
    public class PriceRule
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public float Amount { get; set; }
        
        private List<int> _customerGroupIds;
        public List<int> CustomerGroupIds
        {
            get => _customerGroupIds ?? (_customerGroupIds = PriceRuleService.GetCustomerGroupIds(Id));
            set => _customerGroupIds = value ?? new List<int>();
        }

        public int? PaymentMethodId { get; set; }
        public int? ShippingMethodId { get; set; }

        private List<int> _warehouseIds;
        public List<int> WarehouseIds
        {
            get => _warehouseIds ?? (_warehouseIds = PriceRuleService.GetWarehouseIds(Id));
            set => _warehouseIds = value ?? new List<int>();
        }

        public bool ApplyDiscounts { get; set; }
        
        public bool Enabled { get; set; }
    }
    

    public class OfferPriceRule
    {
        public int OfferId { get; set; }

        public int PriceRuleId { get; set; }

        public float? PriceByRule { get; set; }

        public string Name { get; set; }

        public float Amount { get; set; }
        
        private List<int> _customerGroupIds;
        public List<int> CustomerGroupIds  => 
            _customerGroupIds ?? (_customerGroupIds = PriceRuleService.GetCustomerGroupIdsCached(PriceRuleId));

        public int? PaymentMethodId { get; set; }
        
        public int? ShippingMethodId { get; set; }
        
        private List<int> _warehouseIds;
        public List<int> WarehouseIds  => 
            _warehouseIds ?? (_warehouseIds = PriceRuleService.GetWarehouseIdsCached(PriceRuleId));

        public bool ApplyDiscounts { get; set; }
    }
}
