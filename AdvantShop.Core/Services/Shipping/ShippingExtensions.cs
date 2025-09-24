using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Shipping;

namespace AdvantShop.Core.Services.Shipping
{
    public static class ShippingExtensions
    {
        public static int? GetWarehouseId(this BaseShippingOption shipping)
        {
            var warehouseId = GetWarehouseIds(shipping)?.FirstOrDefault();
            
            return warehouseId is default(int) // FirstOrDefault can return default(int)
                ? default
                : warehouseId;
        }
        
        public static IEnumerable<int> GetWarehouseIds(this BaseShippingOption shipping)
        {
            if (shipping != null)
            {
                var pickPoint = shipping.GetOrderPickPoint();
                
                if (pickPoint?.WarehouseIds?.Count > 0)
                    return pickPoint.WarehouseIds;

                if (shipping.Warehouses?.Count > 0)
                    return shipping.Warehouses;
            }

            return default;
        }
        
        public static string KeyAttribute(this BaseShipping obj)
        {
            return AttributeHelper.GetAttributeValue<ShippingKeyAttribute, string>(obj);
        }

    }
}