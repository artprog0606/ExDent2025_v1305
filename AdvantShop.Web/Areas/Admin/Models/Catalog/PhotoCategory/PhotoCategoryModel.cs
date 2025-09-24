using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Catalog.PhotoCategory
{
    public class PhotoCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
    }

    public class PhotoCategoryFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        
        public bool? Enabled { get; set; }
    }
}
