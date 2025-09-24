using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class WarehouseTypesFilterModel : BaseFilterModel
    {
        public string TypeName { get; set; }

        public bool? Enabled { get; set; }

        public int SortingFrom { get; set; }

        public int SortingTo { get; set; }
    }
}