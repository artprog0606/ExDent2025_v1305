using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Customers.CustomerFieldValues;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerFields
{
    public class AdminCustomerFieldModel
    {
        public AdminCustomerFieldModel()
        {
            FieldValues = new List<AdminCustomerFieldValueModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerFieldType FieldType { get; set; }
        public CustomerType CustomerType { get; set; }
        public CustomerFieldAssignment FieldAssignment { get; set; }
        public int SortOrder { get; set; }
        public bool Required { get; set; }
        public bool ShowInRegistration { get; set; }
        public bool ShowInCheckout { get; set; }
        public bool DisableCustomerEditing { get; set; }
        public bool ShowInUserEditing { get; set; }

        public bool Enabled { get; set; }

        public List<AdminCustomerFieldValueModel> FieldValues { get; set; }

        public string FieldTypeFormatted { get { return FieldType.Localize(); } }
        public string CustomerTypeFormatted { get { return CustomerType.Localize(); } }
        public bool HasValues { get { return FieldType == CustomerFieldType.Select; } }
    }
}
