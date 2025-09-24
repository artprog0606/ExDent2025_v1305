using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class WarehousesFilterModel : BaseFilterModel
    {
        public string WarehouseName { get; set; }

        public int? CityId { get; set; }
        public int? TypeId { get; set; }
        public bool? Enabled { get; set; }

        public int SortingFrom { get; set; }

        public int SortingTo { get; set; }
    }
}