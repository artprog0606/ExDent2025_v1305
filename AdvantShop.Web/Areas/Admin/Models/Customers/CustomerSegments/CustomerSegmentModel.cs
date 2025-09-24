using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Customers.CustomerSegments
{
    public class CustomerSegmentModel
    {
        public bool IsEditMode { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerSegmentFilter SegmentFilter { get; set; }

        public List<string> Categories { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Cities { get; set; }


        private List<CustomerFieldWithValue> _customerFields;
        public List<CustomerFieldWithValue> CustomerFields
        {
            get
            {
                if (_customerFields != null)
                    return _customerFields;

                _customerFields = CustomerFieldService.GetCustomerFieldsWithValue(Guid.Empty) ??
                                  new List<CustomerFieldWithValue>();

                if (SegmentFilter != null && SegmentFilter.CustomerFields != null)
                {
                    foreach (var customerField in _customerFields)
                    {
                        var field = SegmentFilter.CustomerFields.FirstOrDefault(x => x.Key == customerField.Id);
                        if (!field.Equals(default(KeyValuePair<int, string>)))
                        {
                            customerField.Value = field.Value;
                        }
                    }
                }

                return _customerFields;
            }
            set { _customerFields = value; }
        }

        public List<SelectListItem> CustomerTypes { get; set; }

        public bool ShowCustomerType { get; set; }

        public IEnumerable<SelectListItem> OrderTypes { get; set; }

        public CustomerSegmentModel()
        {
            Categories = new List<string>();
            Countries = new List<string>();
            Cities = new List<string>();
            OrderTypes = Enum.GetValues(typeof(OrderType))
                        .Cast<OrderType>()
                        .Select(x => new SelectListItem
                        {
                            Text = x.Localize(),
                            Value = x.ToString()
                        });
        }
    }
}
