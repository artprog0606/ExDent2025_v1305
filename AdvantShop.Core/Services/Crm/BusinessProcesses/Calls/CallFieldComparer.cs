using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.Customers;
using System;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum ECallFieldType
    {
        [Localize("Core.Crm.ECallFieldType.None"), FieldUsage(EFieldUsage.None)]
        None = 0,
        [Localize("Core.Crm.ECallFieldType.LastName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 10)]
        LastName = 1,
        [Localize("Core.Crm.ECallFieldType.FirstName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 20)]
        FirstName = 2,
        [Localize("Core.Crm.ECallFieldType.Patronymic"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 30)]
        Patronymic = 3,
        [Localize("Core.Crm.ECallFieldType.CustomerGroup"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 40)]
        CustomerGroup = 4,
        [Localize("Core.Crm.ECallFieldType.CustomerField")]
        CustomerField = 5,
        [Localize("Core.Crm.ECallFieldType.Email"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 50)]
        Email = 6,
        [Localize("Core.Crm.ECallFieldType.Country"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 60)]
        Country = 7,
        [Localize("Core.Crm.ECallFieldType.Region"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 70)]
        Region = 8,
        [Localize("Core.Crm.ECallFieldType.City"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80)]
        City = 9,
        [Localize("Core.Crm.ECallFieldType.Phone"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 45)]
        Phone = 10,
        
        [Localize("Core.Crm.ELeadFieldType.CustomerSegment"), 
         FieldType(EFieldType.Select), 
         FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 110), FieldUsage(EFieldUsage.Filter)]
        CustomerSegment = 11,
        
        [Localize("Core.Crm.ELeadFieldType.CustomerType"), 
         FieldType(EFieldType.Select), 
         FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 115), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerType = 12,
    }

    public class CallFieldComparer : IBizObjectFieldComparer<Call>
    {
        public ECallFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public bool CheckField(Call call)
        {
            var check = Check(call);
            return CompareType == BizObjectFieldCompareType.Equal ? check : !check;
        }

        private bool Check(Call call)
        {
            switch (FieldType)
            {
                case ECallFieldType.LastName:
                    return call.Customers.Any(x => FieldComparer.Check(x.LastName));
                case ECallFieldType.FirstName:
                    return call.Customers.Any(x => FieldComparer.Check(x.FirstName));
                case ECallFieldType.Patronymic:
                    return call.Customers.Any(x => FieldComparer.Check(x.Patronymic));
                case ECallFieldType.CustomerGroup:
                    return call.Customers.Any(x => FieldComparer.Check(x.CustomerGroupId));
                case ECallFieldType.CustomerField:
                    if (!FieldComparer.FieldObjId.HasValue)
                        return true;
                    foreach (var customer in call.Customers)
                    {
                        var customerField = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id).FirstOrDefault(x => x.Id == FieldComparer.FieldObjId.Value);
                        // не возвращаем false, проверяем всех покупателей до
                        if (customerField != null)
                        {
                            if (customerField.FieldType == CustomerFieldType.Date)
                            {
                                DateTime? dateVal;
                                if ((dateVal = customerField.Value.TryParseDateTime(true)).HasValue && FieldComparer.Check(dateVal.Value))
                                    return true;
                            }
                            else if (customerField.FieldType == CustomerFieldType.Number)
                            {
                                float? floatVal;
                                if ((floatVal = customerField.Value.TryParseFloat(true)).HasValue && FieldComparer.Check(floatVal.Value))
                                    return true;
                            }
                            else if (FieldComparer.Check(customerField.Value))
                                return true;
                        }
                    }
                    return false;
                case ECallFieldType.Email:
                    return call.Customers.Any(x => FieldComparer.Check(x.EMail));
                case ECallFieldType.Country:
                    return call.Customers.Any(x => x.Contacts.Any(contact => FieldComparer.Check(contact.Country)));
                case ECallFieldType.Region:
                    return call.Customers.Any(x => x.Contacts.Any(contact => FieldComparer.Check(contact.Region)));
                case ECallFieldType.City:
                    return call.Customers.Any(x => x.Contacts.Any(contact => FieldComparer.Check(contact.City)));
                case ECallFieldType.Phone:
                    return FieldComparer.Check(call.Phone) || call.Customers.Any(x => FieldComparer.Check(x.Phone) || (x.StandardPhone.HasValue && FieldComparer.Check(x.StandardPhone.Value.ToString())));
                default:
                    return false;
            }
        }

        private string _fieldName;
        public string FieldName
        {
            get
            {
                if (_fieldName != null)
                    return _fieldName;
                switch (FieldType)
                {
                    case ECallFieldType.CustomerField:
                        CustomerField customerField;
                        if (FieldComparer != null && FieldComparer.FieldObjId.HasValue &&
                            (customerField = CustomerFieldService.GetCustomerField(FieldComparer.FieldObjId.Value)) != null)
                        {
                            _fieldName = customerField.Name;
                        }
                        else
                            _fieldName = FieldType.Localize();
                        break;
                    default:
                        _fieldName = FieldType.Localize();
                        break;
                }
                return _fieldName;
            }
        }

        private string _fieldValueObjectName;
        public string FieldValueObjectName
        {
            get
            {
                if (_fieldValueObjectName != null)
                    return _fieldValueObjectName;

                if (FieldComparer == null || !FieldComparer.ValueObjId.HasValue)
                {
                    _fieldValueObjectName = string.Empty;
                    return _fieldValueObjectName;
                }

                var fieldValueObjId = FieldComparer.ValueObjId.Value;
                switch (FieldType)
                {
                    case ECallFieldType.CustomerGroup:
                        var customerGroup = CustomerGroupService.GetCustomerGroup(fieldValueObjId);
                        _fieldValueObjectName = customerGroup != null ? customerGroup.GroupName : string.Empty;
                        break;
                    default:
                        _fieldValueObjectName = string.Empty;
                        break;
                }

                return _fieldValueObjectName;
            }
        }

        public bool IsValid()
        {
            if (FieldComparer == null)
                return false;
            if (FieldComparer.FieldObjId.HasValue)
            {
                switch (FieldType)
                {
                    case ECallFieldType.CustomerField:
                        if (CustomerFieldService.GetCustomerField(FieldComparer.FieldObjId.Value) == null)
                            return false;
                        break;
                }
            }

            if (!FieldComparer.ValueObjId.HasValue)
                return true;

            var fieldValueObjId = FieldComparer.ValueObjId.Value;
            switch (FieldType)
            {
                case ECallFieldType.CustomerGroup:
                    return CustomerGroupService.GetCustomerGroup(fieldValueObjId) != null;
            }
            return true;
        }
    }
}
