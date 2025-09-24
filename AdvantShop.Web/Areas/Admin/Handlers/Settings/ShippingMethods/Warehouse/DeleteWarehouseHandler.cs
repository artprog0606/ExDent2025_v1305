using AdvantShop.Core.Services.Shipping;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingMethods.Warehouse
{
    public class DeleteWarehouseHandler : ICommandHandler
    {
        private readonly int _shippingMethodId;
        private readonly int _warehouseId;

        public DeleteWarehouseHandler(int shippingMethodId, int warehouseId)
        {
            _shippingMethodId = shippingMethodId;
            _warehouseId = warehouseId;
        }
        public void Execute()
        {
            ShippingWarehouseMappingService.Delete(_shippingMethodId, _warehouseId);
        }
    }
}