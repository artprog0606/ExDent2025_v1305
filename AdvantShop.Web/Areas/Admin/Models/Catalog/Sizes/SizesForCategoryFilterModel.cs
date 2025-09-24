using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.Sizes
{
    public class SizesForCategoryFilterModel : BaseFilterModel
    {
        public string SizeName { get; set; }
        public string SizeNameForCategory { get; set; }
        public int CategoryId { get; set; }
    }
}
