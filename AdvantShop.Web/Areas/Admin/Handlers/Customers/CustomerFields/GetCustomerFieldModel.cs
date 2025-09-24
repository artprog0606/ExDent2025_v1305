using System.Linq;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Handlers.Customers.CustomerFieldValues;
using AdvantShop.Web.Admin.Models.Customers.CustomerFields;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerFields
{
    public class GetCustomerFieldModel
    {
        private readonly CustomerField _customerField;

        public GetCustomerFieldModel(CustomerField customerField)
        {
            _customerField = customerField;
        }

        public AdminCustomerFieldModel Execute()
        {
            var model = new AdminCustomerFieldModel
            {
                Id = _customerField.Id,
                Name = _customerField.Name,
                FieldType = _customerField.FieldType,
                SortOrder = _customerField.SortOrder,
                Required = _customerField.Required,
                ShowInRegistration = _customerField.ShowInRegistration,
                ShowInCheckout = _customerField.ShowInCheckout,
                DisableCustomerEditing = _customerField.DisableCustomerEditing,
                Enabled = _customerField.Enabled,
                CustomerType = _customerField.CustomerType,
                FieldAssignment = _customerField.FieldAssignment,
                ShowInUserEditing = _customerField.ShowInUserEditing,
            };

            if (model.HasValues)
            {
                model.FieldValues = CustomerFieldService.GetCustomerFieldValues(model.Id).Select(x => new GetCustomerFieldValueModel(x).Execute()).ToList();
            }

            return model;
        }
    }
}
