using System.Collections.Generic;
using AdvantShop.Core.Services.Catalog.Warehouses;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingMethods.Warehouse
{
    public class AddWarehousesHandler : ICommandHandler
    {
        private readonly int _shippingMethodId;
        private readonly List<int> _warehouseIds;

        public AddWarehousesHandler(int shippingMethodId, List<int> warehouseIds)
        {
            _shippingMethodId = shippingMethodId;
            _warehouseIds = warehouseIds;
        }
        
        public void Execute()
        {
            if (_warehouseIds is null
                || _warehouseIds.Count == 0)
                return;

            foreach (var warehouseId in _warehouseIds)
            {
                if (!WarehouseService.Exists(warehouseId))
                    continue;
                
                ShippingWarehouseMappingService.Add(_shippingMethodId, warehouseId);
            }
        }
    }
}