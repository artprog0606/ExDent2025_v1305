using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerGroups
{
    public partial class AdminCustomerGroupModel
    {
        public int CustomerGroupId { get; set; }
        public string GroupName { get; set; }
        public float GroupDiscount { get; set; }
        public float MinimumOrderPrice { get; set; }
        public int CustomersCount { get; set; }

        public bool CanBeDeleted => CustomersCount == 0 && CustomerGroupId != CustomerGroupService.DefaultCustomerGroup;
    }
}
