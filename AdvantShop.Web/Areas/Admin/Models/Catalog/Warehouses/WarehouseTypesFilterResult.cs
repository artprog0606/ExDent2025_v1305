using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class WarehouseTypesFilterResult : FilterResult<WarehouseTypeGridModel> { }
    
    public class WarehouseTypeGridModel
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
    }
}