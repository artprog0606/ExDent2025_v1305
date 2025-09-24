using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Common.Attributes
{
    public class ShippingKeyAttribute : Attribute, IAttribute<string>
    {
        public ShippingKeyAttribute(string name)
        {
            Value = name;
        }

        public string Value { get; }
    }
    
    public class ShippingAdminModelAttribute : Attribute, IAttribute<string>
    {
        public ShippingAdminModelAttribute(string key)
        {
            Value = key;
        }

        public string Value { get; }
    }
    
    public class PaymentKeyAttribute : Attribute, IAttribute<string>
    {
        public PaymentKeyAttribute(string name)
        {
            Value = name;
        }

        public string Value { get; }
    }
    
    public class PaymentAdminModelAttribute : Attribute, IAttribute<string>
    {
        public PaymentAdminModelAttribute(string key)
        {
            Value = key;
        }

        public string Value { get; }
    }
    
    public class PaymentOrderPayModelAttribute : Attribute, IAttribute<string>
    {
        public PaymentOrderPayModelAttribute(string key)
        {
            Value = key;
        }

        public string Value { get; }
    }

    public class StringNameAttribute : Attribute, IAttribute<string>
    {
        public StringNameAttribute(string name)
        {
            Value = name;
        }

        public string Value { get; }
    }

    public class ActiveAttribute : Attribute, IAttribute<bool>
    {
        public ActiveAttribute(bool active)
        {
            Value = active;
        }

        public bool Value { get; }
    }

    public class ExportFeedKeyAttribute : Attribute, IAttribute<string>
    {
        public ExportFeedKeyAttribute(string name)
        {
            Value = name;
        }

        public string Value { get; }
    }
    
    public class LetterFormatKeyAttribute : Attribute, IAttribute<List<string>>, IFormatKeyAttribute<List<string>>
    {
        public LetterFormatKeyAttribute(string name)
        {
            Value = new List<string>() {name};
        }

        public LetterFormatKeyAttribute(string name1, string name2)
        {
            Value = new List<string>() {name1, name2};
        }
        
        public LetterFormatKeyAttribute(string name1, string name2, string name3)
        {
            Value = new List<string>() {name1, name2, name3};
        }
        
        public List<string> Value { get; }
        
        public string Description { get; set; }
    }
}