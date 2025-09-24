using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class WarehousesFilterResult : FilterResult<WarehouseGridModel> { }

    public class WarehouseGridModel
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal Amount { get; set; }
        public int CountWarehouses { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
        public string TypeName { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public bool UseInOrders { get; set; }
        public int DefaultWarehouse { get; set; }

        public bool CanDelete =>
            Amount == 0
            && CountWarehouses > 1
            && UseInOrders is false
            && WarehouseId != DefaultWarehouse;
    }
}