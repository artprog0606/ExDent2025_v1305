using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Customers
{
    public class SaveCustomersSettingsHandler
    {
        private CustomersSettingsModel _model;

        public SaveCustomersSettingsHandler(CustomersSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            if (_model.IsRegistrationAsLegalEntity)
            {
                CustomerFieldService.AddCustomerFieldForLegalEntity();
                CustomerFieldService.EnabledCustomerFieldByCustomerType(CustomerType.LegalEntity);
                if (!_model.IsRegistrationAsPhysicalEntity)
                    CustomerFieldService.DisabledCustomerFieldByCustomerType(CustomerType.PhysicalEntity);
                else
                    CustomerFieldService.EnabledCustomerFieldByCustomerType(CustomerType.PhysicalEntity);
            }
            else
            {
                CustomerFieldService.EnabledCustomerFieldByCustomerType(CustomerType.PhysicalEntity);
                CustomerFieldService.DisabledCustomerFieldByCustomerType(CustomerType.LegalEntity);
            }
            SettingsCustomers.IsRegistrationAsLegalEntity = _model.IsRegistrationAsLegalEntity;
            SettingsCustomers.IsRegistrationAsPhysicalEntity = _model.IsRegistrationAsPhysicalEntity;
            SettingsCustomers.MinimalOrderPriceForPhysicalEntity = _model.MinimalOrderPriceForPhysicalEntity;
            SettingsCustomers.MinimalOrderPriceForLegalEntity = _model.MinimalOrderPriceForLegalEntity;
        }
    }
}
