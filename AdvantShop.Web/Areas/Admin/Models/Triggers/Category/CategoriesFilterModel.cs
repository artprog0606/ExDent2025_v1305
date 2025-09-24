using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Triggers.Category
{
    public class CategoriesFilterModel : BaseFilterModel
    {
        public int? SortingFrom { get; set; }
        public int? SortingTo { get; set; }
    }
}
