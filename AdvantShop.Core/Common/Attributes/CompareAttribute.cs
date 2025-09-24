using System;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Localization;

namespace AdvantShop.Core.Common.Attributes
{
    public class CompareAttribute : Attribute, IAttribute<string>, ICompareAttribute<string, ChangeHistoryParameterType>
    {
        private string _key;
        private ChangeHistoryParameterType _type;
        private readonly bool _notLogValue;

        public CompareAttribute(string key)
        {
            _key = key;
        }

        public CompareAttribute(string key, ChangeHistoryParameterType type)
        {
            _key = key;
            _type = type;
        }

        public CompareAttribute(string key, bool notLogValue)
        {
            _key = key;
            _notLogValue = notLogValue;
        }

        public string Value => LocalizationService.GetResource(_key);

        public ChangeHistoryParameterType Type => _type;

        public bool NotLogValue => _notLogValue;
    }
}