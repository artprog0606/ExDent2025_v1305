using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class WarehouseStock
    {
        public int OfferId { get; set; }
        public int WarehouseId { get; set; }
        [Compare("Core.Catalog.WarehouseStock.Quantity")]
        public float Quantity { get; set; }
    }
}