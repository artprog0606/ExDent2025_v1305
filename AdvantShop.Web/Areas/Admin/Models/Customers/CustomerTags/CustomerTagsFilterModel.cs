using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerTags
{
    public class CustomerTagsFilterModel : BaseFilterModel
    {
        public string Name { get; set; }
        public bool? Enabled { get; set; }
    }
}
