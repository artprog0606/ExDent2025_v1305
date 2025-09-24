using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses.Customers
{
    public enum ECustomerFieldType
    {
        [Localize("Core.Crm.ELeadFieldType.None"), FieldUsage(EFieldUsage.None)]
        None = 0,

        [Localize("Core.Crm.ELeadFieldType.LastName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 10), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        LastName = 1,
        [Localize("Core.Crm.ELeadFieldType.FirstName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 20), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        FirstName = 2,
        [Localize("Core.Crm.ELeadFieldType.Patronymic"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 30), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Patronymic = 3,
        [Localize("Core.Crm.ELeadFieldType.Email"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 50), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Email = 6,
        [Localize("Core.Crm.ELeadFieldType.Phone"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 40), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Phone = 7,
        [Localize("Core.Crm.ELeadFieldType.Country"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 60), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Country = 8,
        [Localize("Core.Crm.ELeadFieldType.Region"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 70), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Region = 9,
        [Localize("Core.Crm.ELeadFieldType.City"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        City = 10,
        [Localize("Core.Crm.ELeadFieldType.Organization"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Organization = 20,
        [Localize("Core.Crm.ELeadFieldType.Manager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 90), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Manager = 11,
        [Localize("Core.Crm.ELeadFieldType.CustomerGroup"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 100), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerGroup = 4,
        [Localize("Core.Crm.ELeadFieldType.CustomerSegment"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 110), FieldUsage(EFieldUsage.Filter)]
        CustomerSegment = 12,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidSum"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 140), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidSum = 13,
        [Localize("Core.Crm.ELeadFieldType.OrdersCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 150), FieldUsage(EFieldUsage.Filter)]
        OrdersCount = 14,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 160), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidCount = 15,
        [Localize("Core.Crm.ELeadFieldType.ProductsByCustomer"), FieldType(EFieldType.ProductChooser), FieldTypeValue(EFieldType.ProductChooser, "Core.Crm.EFieldType.GroupName.Client", 170), FieldUsage(EFieldUsage.Filter)]
        ProductsByCustomer = 16,
        [Localize("Core.Crm.ELeadFieldType.CategoriesByCustomer"), FieldType(EFieldType.CategoryChooser), FieldTypeValue(EFieldType.CategoryChooser, "Core.Crm.EFieldType.GroupName.Client", 180), FieldUsage(EFieldUsage.Filter)]
        CategoriesByCustomer = 17,
        [Localize("Core.Crm.ELeadFieldType.OpenLeadSalesFunnels"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 190), FieldUsage(EFieldUsage.Filter)]
        OpenLeadSalesFunnels = 18,

        [Localize("Core.Crm.ELeadFieldType.CustomerField"), FieldUsage(EFieldUsage.None)]
        CustomerField = 5,


        [Localize("Core.Crm.EOrderFieldType.Datetime"), FieldType(EFieldType.Datetime), FieldTypeValue(EFieldType.Datetime, "Core.Crm.EFieldType.GroupName.System", 400), FieldUsage(EFieldUsage.Filter)]
        Datetime = 19,
        [Localize("Core.Crm.EOrderFieldType.Time"), FieldType(EFieldType.Time), FieldTypeValue(EFieldType.Time, "Core.Crm.EFieldType.GroupName.System", 410), FieldUsage(EFieldUsage.Filter)]
        Time = 21,
        [Localize("Core.Crm.ELeadFieldType.CustomerType"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 115), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerType = 22,
        [Localize("Core.Crm.ELeadFieldType.BonusAccount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 200), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        BonusAccount = 23
    }


    public class CustomerFieldComparer : IBizObjectFieldComparer<Customer>
    {
        public ECustomerFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public bool CheckField(Customer customer)
        {
            var check = Check(customer);
            return CompareType == BizObjectFieldCompareType.Equal ? check : !check;
        }

        private bool Check(Customer customer)
        {
            switch (FieldType)
            {
                case ECustomerFieldType.LastName:
                    return FieldComparer.Check(customer.LastName);

                case ECustomerFieldType.FirstName:
                    return FieldComparer.Check(customer.FirstName);

                case ECustomerFieldType.Patronymic:
                    return FieldComparer.Check(customer.Patronymic);

                case ECustomerFieldType.CustomerGroup:
                    return FieldComparer.Check(customer.CustomerGroupId);

                case ECustomerFieldType.CustomerField:
                    if (!FieldComparer.FieldObjId.HasValue)
                        return true;
                    var customerField = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id).FirstOrDefault(x => x.Id == FieldComparer.FieldObjId.Value);
                    if (customerField == null)
                        return false;
                    switch (customerField.FieldType)
                    {
                        case CustomerFieldType.Date:
                            var date = customerField.Value.TryParseDateTime(true);
                            return FieldComparer.Check(date);
                        case CustomerFieldType.Number:
                            var floatVal = customerField.Value.TryParseFloat(true);
                            return FieldComparer.Check(floatVal);
                        default:
                            return FieldComparer.Check(customerField.Value);
                    }

                case ECustomerFieldType.Email:
                    return FieldComparer.Check(customer.EMail);

                case ECustomerFieldType.Phone:
                    return FieldComparer.Check(customer.Phone) ||
                           (customer.StandardPhone.HasValue && FieldComparer.Check(customer.StandardPhone.Value.ToString()));

                case ECustomerFieldType.Country:
                {
                    var country = customer.Contacts != null && customer.Contacts.Count > 0
                        ? customer.Contacts[0].Country
                        : null;
                    return FieldComparer.Check(country);
                }

                case ECustomerFieldType.Region:
                {
                    var region = customer.Contacts != null && customer.Contacts.Count > 0
                        ? customer.Contacts[0].Region
                        : null;
                    return FieldComparer.Check(region);
                }

                case ECustomerFieldType.City:
                {
                    var city = customer.Contacts != null && customer.Contacts.Count > 0
                        ? customer.Contacts[0].City
                        : null;
                    return FieldComparer.Check(city);
                }

                case ECustomerFieldType.Organization:
                    return FieldComparer.Check(customer.Organization);

                case ECustomerFieldType.CustomerType:
                    return FieldComparer.Check((int)customer.CustomerType);

                case ECustomerFieldType.Manager:
                    return FieldComparer.Check(customer.ManagerId);

                case ECustomerFieldType.CustomerSegment:
                    return FieldComparer.Check(customer.Id);

                case ECustomerFieldType.OrdersPaidSum:
                    return FieldComparer.Check(customer.Id);

                case ECustomerFieldType.OrdersCount:
                    return FieldComparer.Check(customer.Id);

                case ECustomerFieldType.OrdersPaidCount:
                    return FieldComparer.Check(customer.Id);


                case ECustomerFieldType.ProductsByCustomer:
                {
                    foreach (var productId in OrderService.GetProductIdsByCustomer(customer.Id))
                        if (FieldComparer.Check(productId))
                            return true;
                    return false;
                }

                case ECustomerFieldType.CategoriesByCustomer:
                {
                    foreach (var productId in OrderService.GetProductIdsByCustomer(customer.Id))
                    {
                        var categories = ProductService.GetCategoriesByProductId(productId);

                        foreach (var category in categories)
                            if (FieldComparer.Check(category.CategoryId))
                                return true;
                    }
                    return false;
                }

                case ECustomerFieldType.OpenLeadSalesFunnels:
                    return FieldComparer.Check(customer.Id);

                case ECustomerFieldType.Datetime:
                    return FieldComparer.Check(DateTime.Now);

                case ECustomerFieldType.Time:
                    return FieldComparer.Check(DateTime.Now);

                case ECustomerFieldType.BonusAccount:
                    if (!customer.BonusCardNumber.HasValue)
                        return false;
                    var card = BonusSystemService.GetCard(customer.BonusCardNumber.Value);
                    return FieldComparer.Check((float)card.BonusesTotalAmount);
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
                    case ECustomerFieldType.CustomerField:
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
                    case ECustomerFieldType.CustomerGroup:
                        var customerGroup = CustomerGroupService.GetCustomerGroup(fieldValueObjId);
                        _fieldValueObjectName = customerGroup != null ? customerGroup.GroupName : string.Empty;
                        break;
                    case ECustomerFieldType.Manager:
                        var manager = ManagerService.GetManager(fieldValueObjId);
                        _fieldValueObjectName = manager != null ? manager.FullName : string.Empty;
                        break;
                    case ECustomerFieldType.CustomerSegment:
                        var segment = CustomerSegmentService.Get(fieldValueObjId);
                        _fieldValueObjectName = segment != null ? segment.Name : string.Empty;
                        break;
                    case ECustomerFieldType.OpenLeadSalesFunnels:
                        var salesFunnel = SalesFunnelService.Get(fieldValueObjId);
                        _fieldValueObjectName = salesFunnel != null ? salesFunnel.Name : string.Empty;
                        break;
                    case ECustomerFieldType.CustomerType:
                        _fieldValueObjectName = ((CustomerType)fieldValueObjId).Localize();
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
                    case ECustomerFieldType.CustomerField:
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
                case ECustomerFieldType.CustomerGroup:
                    return CustomerGroupService.GetCustomerGroup(fieldValueObjId) != null;
                case ECustomerFieldType.Manager:
                    return ManagerService.GetManager(fieldValueObjId) != null;
                case ECustomerFieldType.CustomerSegment:
                    return CustomerSegmentService.Get(fieldValueObjId) != null;
            }
            return true;
        }
    }
}
