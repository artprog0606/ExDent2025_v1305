using AdvantShop.Configuration;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Models.Customers
{
    public class CustomersFilterResult : FilterResult<IAdminCustomerResult>
    {
        public CustomersFilterResult()
        {
            ShowCustomerType = SettingsCustomers.IsRegistrationAsLegalEntity && SettingsCustomers.IsRegistrationAsPhysicalEntity;
        }
        public bool ShowCustomerType { get; set; }
    }
}
