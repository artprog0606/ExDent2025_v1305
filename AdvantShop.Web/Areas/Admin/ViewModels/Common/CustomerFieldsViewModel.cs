using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.ViewModels.Common
{
    public class CustomerFieldsViewModel
    {
        public CustomerFieldsViewModel() { }

        public CustomerFieldsViewModel(List<CustomerFieldWithValue> customerFields, string modelName = "",
                                        string ngModelName = "", string onSelectFunc = "", string ngVariableVisible = null, bool ignoreRequired = false)
        {
            CustomerFields = customerFields;
            ModelName = modelName;
            NgModelName = ngModelName;
            OnSelectFunc = onSelectFunc;
            NgVariableVisible = ngVariableVisible;
            IgnoreRequired = ignoreRequired;
        }

        public List<CustomerFieldWithValue> CustomerFields { get; set; }
        public List<CustomerFieldWithValue> FilteredCustomerFields { get; set; }
        public string ModelName { get; set; }

        public string NgModelName { get; set; }

        public string OnSelectFunc { get; set; }

        public string NgVariableVisible { get; set; }

        public bool IgnoreRequired { get; set; }

        public string CustomerFieldsSerialized 
        { 
            get 
            {
                var fields = FilteredCustomerFields ?? CustomerFields;
                return fields != null ? JsonConvert.SerializeObject(fields) : "[]"; 
            } 
        }

        public string GetName()
        {
            return GetName(null, string.Empty);
        }

        public string GetName(int? index, string field)
        {
            var prefix = NgModelName.IsNotEmpty() ? string.Format("{0}.", NgModelName) : string.Empty;
            var postfix = field.IsNotEmpty() ? string.Format(".{0}", field) : string.Empty;
            var indexPart = index.HasValue ? string.Format("[{0}]", index.Value) : string.Empty;
            return string.Format("{0}customerfieldsJs{1}{2}", prefix, indexPart, postfix);
        }
    }
}
