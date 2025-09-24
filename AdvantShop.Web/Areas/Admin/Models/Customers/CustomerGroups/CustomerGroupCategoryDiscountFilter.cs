using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerGroups
{
    public class CustomerGroupCategoryDiscountFilter : BaseFilterModel
    {
        public int CustomerGroupId { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public float? Discount { get; set; }
    }
}