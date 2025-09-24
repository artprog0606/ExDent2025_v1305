using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Units
{
    public class UnitsFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool? WithOutMeasureType { get; set; }
    }
}