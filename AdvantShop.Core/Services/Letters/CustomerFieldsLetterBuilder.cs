using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdvantShop.Customers;

namespace AdvantShop.Letters
{
    public sealed class CustomerFieldsLetterBuilder : ILetterTemplateBuilder
    {
        private readonly Customer _customer;
        private List<CustomerFieldWithValue> _customerFields;
        
        private static readonly Regex _regexCompiled = new Regex("#([A-Za-z0-9_]*)#", RegexOptions.Compiled);
        
        public CustomerFieldsLetterBuilder(Customer customer) 
        {
            _customer = customer;
        }

        private List<CustomerFieldWithValue> GetCustomerFields()
        {
            return _customerFields ??
                   (_customerFields = CustomerFieldService.GetCustomerFieldsWithValue(_customer.Id));
        }
                
        public Dictionary<string, string> GetFormattedParams(string text)
        {
            var letterParams = new Dictionary<string, string>();
                 
            var matches = _regexCompiled.Matches(text);
            if (matches.Count == 0)
                return letterParams;

            GetCustomerFields();
                 
            foreach (Match match in matches)
            {
                var collectionKey = match.Value;
                     
                var field = _customerFields.Find(x => x.LetterKey.Equals(collectionKey, StringComparison.OrdinalIgnoreCase));
                if (field != null)
                    letterParams.Add(collectionKey, field.Value);
            }
                 
            return letterParams;
        }

        public List<LetterFormatKey> GetKeyDescriptions()
        {
            return
                CustomerFieldService.GetCustomerFields()
                    .Select(x => new LetterFormatKey(x.LetterKey,  "Поле покупателя " + x.Name))
                    .ToList();
        }

        public string FormatText(string text)
        {
            return _regexCompiled.Replace(text, Replace);
        }
        
        private string Replace(Match match)
        {
            GetCustomerFields();
            
            var collectionKey = match.Value;

            var field = _customerFields.Find(x => x.LetterKey.Equals(collectionKey, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                return field.Value;

            return collectionKey;
        }
    }
}