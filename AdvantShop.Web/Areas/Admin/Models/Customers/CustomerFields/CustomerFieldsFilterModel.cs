using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerFields
{
    public class CustomerFieldsFilterModel : BaseFilterModel<int>
    {
        public string Name { get; set; }
        public CustomerFieldType? FieldType { get; set; }
        public bool? Required { get; set; }
        public bool? ShowInRegistration { get; set; }
        public bool? ShowInCheckout { get; set; }
        public bool? ShowInUserEditing { get; set; }
        public bool? Enabled { get; set; }
        public CustomerType? CustomerType { get; set; }
    }
}
