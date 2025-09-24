using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Handlers.Catalog.Import;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Customers
{
    public class GetCustomersSettingsHandler
    {
        public CustomersSettingsModel Execute()
        {
            return new CustomersSettingsModel
            {
                ApplicationId = SettingsVk.ApplicationId,
                IsRegistrationAsLegalEntity = SettingsCustomers.IsRegistrationAsLegalEntity,
                IsRegistrationAsPhysicalEntity = SettingsCustomers.IsRegistrationAsPhysicalEntity,
                ImportCustomersModel = new GetImportCustomersModel().Execute(),
                MinimalOrderPriceForPhysicalEntity = SettingsCustomers.MinimalOrderPriceForPhysicalEntity,
                MinimalOrderPriceForLegalEntity = SettingsCustomers.MinimalOrderPriceForLegalEntity
            };
        }
    }
}
