using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Areas.Api.Models.Categories
{
    public class CategoriesFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ExternalId { get; set; }
        
        public bool? Enabled { get; set; }
        public bool? Hidden { get; set; }
        
        public bool? Extended { get; set; }
    }
}