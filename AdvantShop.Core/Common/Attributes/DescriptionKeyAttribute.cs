using System;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Common.Attributes
{
    public class DescriptionKeyAttribute : Attribute, IAttribute<string>
    {
        private string _key;

        public DescriptionKeyAttribute(string key)
        {
            _key = key;
        }

        public string Value
        {
            get { return LocalizationService.GetResource(_key); }
        }
    }
}