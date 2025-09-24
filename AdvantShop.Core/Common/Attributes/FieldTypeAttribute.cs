using System;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Triggers;

namespace AdvantShop.Core.Common.Attributes
{
    public class FieldTypeAttribute : Attribute, IAttribute<EFieldType>
    {
        private EFieldType _type;

        public FieldTypeAttribute(EFieldType type)
        {
            _type = type;
        }

        public EFieldType Value
        {
            get { return _type; }
        }
    }

    public class FieldTypeValueAttribute : Attribute, IAttribute<FieldTypeValue>
    {
        private FieldTypeValue _fieldTypeValue;

        public FieldTypeValueAttribute(EFieldType type, string groupName, int sortOrder)
        {
            _fieldTypeValue = new FieldTypeValue(type, groupName, sortOrder);
        }

        public FieldTypeValue Value
        {
            get { return _fieldTypeValue; }
        }
    }


    public class FieldUsageAttribute : Attribute, IAttribute<EFieldUsage>
    {
        private readonly EFieldUsage _usage;

        public FieldUsageAttribute(EFieldUsage usage)
        {
            _usage = usage;
        }

        public EFieldUsage Value
        {
            get { return _usage; }
        }

        public ETriggerEventType[] UseForEdit { get; set; }

        public ETriggerEventType[] UseForFilter { get; set; }
    }


    public class FieldTypeValue
    {
        public EFieldType Type { get; private set; }
        public string GroupName { get; private set; }
        public int SortOrder { get; private set; }

        public FieldTypeValue(EFieldType type, string groupName, int sortOrder)
        {
            Type = type;
            GroupName = LocalizationService.GetResource(groupName);
            SortOrder = sortOrder;
        }
    }
}