using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.PropertyValues
{
    public class PropertyValuesFilterModel : BaseFilterModel
    {
        public int PropertyId { get; set; }
        public string Name { get; set; }
    }
}
