using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Customers
{
    public class CustomerField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerFieldType FieldType { get; set; }
        public int SortOrder { get; set; }
        public bool Required { get; set; }
        public bool Enabled { get; set; }
        public bool ShowInRegistration { get; set; }
        public bool ShowInCheckout { get; set; }
        public bool DisableCustomerEditing { get; set; }
        public bool ShowInUserEditing { get; set; }
        public CustomerType CustomerType { get; set; }
        public CustomerFieldAssignment FieldAssignment { get;set; }
        
        public string LetterKey =>
            $"#{(FieldAssignment != CustomerFieldAssignment.None ? FieldAssignment.ToString() : "CUSTOMER_FIELD")}_{Id}#";
    }

    public enum CustomerFieldShowMode
    {
        None = 0,
        Registration = 1,
        Checkout = 2,
        MyAccount = 3
    }

    public class CustomerFieldWithValue : CustomerField
    {
        private string _value;
        public string Value
        {
            get
            {
                if (FieldType == CustomerFieldType.Date)
                {
                    var dt = _value.TryParseDateTime(true);
                    return dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : null;
                }
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string ValueDateFormat
        {
            get
            {
                if (FieldType == CustomerFieldType.Date)
                {
                    var dt = _value.TryParseDateTime(true);
                    return dt.HasValue ? dt.Value.ToString("dd.MM.yyyy") : null;
                }
                return null;
            }
        }

        public List<SelectListItem> Values
        {
            get
            {
                if (FieldType == CustomerFieldType.Select)
                {
                    var list = new List<SelectListItem>() { }; /*new SelectListItem() { Text = "---", Value = "" } */

                    var items = CustomerFieldService.GetCustomerFieldValues(Id);
                    if (items != null && items.Count > 0)
                    {
                        list.AddRange(items.Select(x => new SelectListItem()
                        {
                            Text = x.Value,
                            Value = x.Value,
                            Selected = x.Value == Value
                        }));
                    }

                    return list;
                }

                return new List<SelectListItem>();
            }
        }
    }

    public class CustomerFieldValueMapShort
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
