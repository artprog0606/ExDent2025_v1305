using System.Collections.Generic;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping;

namespace AdvantShop.Areas.Api.Models.Deliveries.v2
{
    internal sealed class Delivery
    {
        public int Id { get; }
        public string Type { get; }
        public string Name { get; }
        public string Description { get; }
        public List<int> WarehouseIds { get; }
        public List<DeliveryPoint> Points { get; set; }

        public Delivery(ShippingMethod method)
        {
            Id = method.ShippingMethodId;
            Type = method.ShippingType;
            Name = method.Name;
            Description = !string.IsNullOrWhiteSpace(method.Description) ? method.Description : null;
            
            WarehouseIds = ShippingWarehouseMappingService.GetByMethod(method.ShippingMethodId);
        }
    }
}