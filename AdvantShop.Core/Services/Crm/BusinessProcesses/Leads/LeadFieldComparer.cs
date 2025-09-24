using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum ELeadFieldType
    {
        [Localize("Core.Crm.ELeadFieldType.None"), FieldUsage(EFieldUsage.None)]
        None = 0,

        [Localize("Core.Crm.ELeadFieldType.LastName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 10), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        LastName = 1,
        [Localize("Core.Crm.ELeadFieldType.FirstName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 20), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        FirstName = 2,
        [Localize("Core.Crm.ELeadFieldType.Patronymic"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 30), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Patronymic = 3,
        [Localize("Core.Crm.ELeadFieldType.Phone"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 40), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Phone = 7,
        [Localize("Core.Crm.ELeadFieldType.Email"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 50), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Email = 6,
        [Localize("Core.Crm.ELeadFieldType.Country"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 60), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Country = 8,
        [Localize("Core.Crm.ELeadFieldType.Region"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 70), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Region = 9,
        [Localize("Core.Crm.ELeadFieldType.City"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        City = 10,
        [Localize("Core.Crm.ELeadFieldType.Organization"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Organization = 16,
        [Localize("Core.Crm.ELeadFieldType.Manager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 90), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerManager = 21,
        [Localize("Core.Crm.ELeadFieldType.CustomerGroup"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 100), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerGroup = 4,
        [Localize("Core.Crm.ELeadFieldType.CustomerSegment"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 110), FieldUsage(EFieldUsage.Filter)]
        CustomerSegment = 22,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidSum"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 120), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidSum = 23,
        [Localize("Core.Crm.ELeadFieldType.OrdersCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 130), FieldUsage(EFieldUsage.Filter)]
        OrdersCount = 24,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 140), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidCount = 25,
        [Localize("Core.Crm.ELeadFieldType.ProductsByCustomer"), FieldType(EFieldType.ProductChooser), FieldTypeValue(EFieldType.ProductChooser, "Core.Crm.EFieldType.GroupName.Client", 150), FieldUsage(EFieldUsage.Filter)]
        ProductsByCustomer = 26,
        [Localize("Core.Crm.ELeadFieldType.CategoriesByCustomer"), FieldType(EFieldType.CategoryChooser), FieldTypeValue(EFieldType.CategoryChooser, "Core.Crm.EFieldType.GroupName.Client", 160), FieldUsage(EFieldUsage.Filter)]
        CategoriesByCustomer = 27,

        [Localize("Core.Crm.ELeadFieldType.CustomerType"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 115), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerType = 33,

        [Localize("Core.Crm.ELeadFieldType.OpenLeadSalesFunnels"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 170), FieldUsage(EFieldUsage.Filter)]
        OpenLeadSalesFunnels = 28,

        [Localize("Core.Crm.ELeadFieldType.CustomerField"), FieldUsage(EFieldUsage.None)]
        CustomerField = 5,

        [Localize("Core.Crm.ELeadFieldType.SalesFunnel"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Lead", 200), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        SalesFunnel = 15,
        [Localize("Core.Crm.ELeadFieldType.Manager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Lead", 210), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Manager = 20,
        [Localize("Core.Crm.ELeadFieldType.LeadSum"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Lead", 220), FieldUsage(EFieldUsage.Filter)]
        LeadSum = 12,
        [Localize("Core.Crm.ELeadFieldType.Products"), FieldType(EFieldType.ProductChooser), FieldTypeValue(EFieldType.ProductChooser, "Core.Crm.EFieldType.GroupName.Lead", 230), FieldUsage(EFieldUsage.Filter)]
        Products = 18,
        [Localize("Core.Crm.ELeadFieldType.Categories"), FieldType(EFieldType.CategoryChooser), FieldTypeValue(EFieldType.CategoryChooser, "Core.Crm.EFieldType.GroupName.Lead", 240), FieldUsage(EFieldUsage.Filter)]
        Categories = 19,
        [Localize("Core.Crm.ELeadFieldType.Source"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Lead", 250), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Source = 11,
        [Localize("Core.Crm.ELeadFieldType.Title"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Lead", 270), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Title = 17,
        [Localize("Core.Crm.ELeadFieldType.Description"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Lead", 280), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Description = 14,
        [Localize("Core.Crm.ELeadFieldType.IsFromAdminArea"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Lead", 290), FieldUsage(EFieldUsage.Filter)]
        IsFromAdminArea = 13,

        [Localize("Core.Crm.ELeadFieldType.DealStatus"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Lead", 300), FieldUsage(EFieldUsage.Filter)]
        DealStatus = 30,


        [Localize("Core.Crm.EOrderFieldType.Datetime"), FieldType(EFieldType.Datetime), FieldTypeValue(EFieldType.Datetime, "Core.Crm.EFieldType.GroupName.System", 400), FieldUsage(EFieldUsage.Filter)]
        Datetime = 29,
        [Localize("Core.Crm.EOrderFieldType.Time"), FieldType(EFieldType.Time), FieldTypeValue(EFieldType.Time, "Core.Crm.EFieldType.GroupName.System", 410), FieldUsage(EFieldUsage.Filter)]
        Time = 31,

        [Localize("Core.Crm.ELeadFieldType.LeadField"), FieldUsage(EFieldUsage.None)]
        LeadField = 32,

        [Localize("Core.Crm.ELeadFieldType.BonusAccount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 420), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        BonusAccount = 34,
    }

    public class LeadFieldComparer : IBizObjectFieldComparer<Lead>
    {
        public ELeadFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public bool CheckField(Lead lead)
        {
            var check = Check(lead);
            return CompareType == BizObjectFieldCompareType.Equal ? check : !check;
        }

        private bool Check(Lead lead)
        {
            switch (FieldType)
            {
                case ELeadFieldType.LastName:
                    return FieldComparer.Check(lead.LastName) || (lead.Customer != null && FieldComparer.Check(lead.Customer.LastName));

                case ELeadFieldType.FirstName:
                    return FieldComparer.Check(lead.FirstName) || (lead.Customer != null && FieldComparer.Check(lead.Customer.FirstName));

                case ELeadFieldType.Patronymic:
                    return FieldComparer.Check(lead.Patronymic) || (lead.Customer != null && FieldComparer.Check(lead.Customer.Patronymic));

                case ELeadFieldType.CustomerGroup:
                    return lead.Customer != null && FieldComparer.Check(lead.Customer.CustomerGroupId);

                case ELeadFieldType.CustomerField:
                    if (!FieldComparer.FieldObjId.HasValue)
                        return true;
                    var customerField = lead.CustomerId.HasValue
                        ? CustomerFieldService.GetCustomerFieldsWithValue(lead.CustomerId.Value).FirstOrDefault(x => x.Id == FieldComparer.FieldObjId.Value)
                        : null;
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

                case ELeadFieldType.Email:
                    return FieldComparer.Check(lead.Email) || (lead.Customer != null && FieldComparer.Check(lead.Customer.EMail));

                case ELeadFieldType.Phone:
                    return FieldComparer.Check(lead.Phone) || (lead.Customer != null && 
                        (FieldComparer.Check(lead.Customer.Phone) || (lead.Customer.StandardPhone.HasValue && FieldComparer.Check(lead.Customer.StandardPhone.Value.ToString()))));

                case ELeadFieldType.Country:
                    return FieldComparer.Check(lead.Country) || lead.Customer != null && lead.Customer.Contacts.Any(x => FieldComparer.Check(x.Country));

                case ELeadFieldType.Region:
                    return FieldComparer.Check(lead.Region) || lead.Customer != null && lead.Customer.Contacts.Any(x => FieldComparer.Check(x.Region));

                case ELeadFieldType.City:
                    return FieldComparer.Check(lead.City) || lead.Customer != null && lead.Customer.Contacts.Any(x => FieldComparer.Check(x.City));

                case ELeadFieldType.Source:
                    return FieldComparer.Check(lead.OrderSourceId);

                case ELeadFieldType.LeadSum:
                    return FieldComparer.Check(lead.Sum);

                case ELeadFieldType.IsFromAdminArea:
                    return FieldComparer.Check(lead.IsFromAdminArea);

                case ELeadFieldType.Description:
                    return FieldComparer.Check(lead.Description);

                case ELeadFieldType.Title:
                    return FieldComparer.Check(lead.Title);

                case ELeadFieldType.SalesFunnel:
                    return FieldComparer.Check(lead.SalesFunnelId);

                case ELeadFieldType.Organization:
                    return FieldComparer.Check(lead.Organization) || (lead.Customer != null && FieldComparer.Check(lead.Customer.Organization));

                case ELeadFieldType.CustomerType:
                    return FieldComparer.Check((int)lead.CustomerType);

                case ELeadFieldType.Products:
                {
                    foreach (var item in lead.LeadItems)
                        if (item.ProductId != null && FieldComparer.Check(item.ProductId.Value))
                            return true;
                    return false;
                }

                case ELeadFieldType.Categories:
                {
                    foreach (var item in lead.LeadItems)
                    {
                        if (item.ProductId == null)
                            continue;

                        var categories = ProductService.GetCategoriesByProductId(item.ProductId.Value);

                        foreach (var category in categories)
                            if (FieldComparer.Check(category.CategoryId))
                                return true;
                    }
                    return false;
                }

                case ELeadFieldType.Manager:
                    return FieldComparer.Check(lead.ManagerId);

                case ELeadFieldType.CustomerManager:
                    var c = lead.Customer != null ? CustomerService.GetCustomer(lead.Customer.Id) : null;
                    return FieldComparer.Check(c != null ? c.ManagerId : null);

                case ELeadFieldType.CustomerSegment:
                    return lead.Customer != null && FieldComparer.Check(lead.Customer.Id);

                case ELeadFieldType.OrdersPaidSum:
                    return lead.Customer != null && FieldComparer.Check(lead.Customer.Id);

                case ELeadFieldType.OrdersCount:
                    return lead.Customer != null && FieldComparer.Check(lead.Customer.Id);

                case ELeadFieldType.OrdersPaidCount:
                    return lead.Customer != null && FieldComparer.Check(lead.Customer.Id);


                case ELeadFieldType.ProductsByCustomer:
                {
                    if (lead.Customer != null)
                    {
                        foreach (var productId in OrderService.GetProductIdsByCustomer(lead.Customer.Id))
                            if (FieldComparer.Check(productId))
                                return true;
                    }
                    return false;
                }

                case ELeadFieldType.CategoriesByCustomer:
                {
                    if (lead.Customer != null)
                    {
                        foreach (var productId in OrderService.GetProductIdsByCustomer(lead.Customer.Id))
                        {
                            var categories = ProductService.GetCategoriesByProductId(productId);

                            foreach (var category in categories)
                                if (FieldComparer.Check(category.CategoryId))
                                    return true;
                        }
                    }
                    return false;
                }

                case ELeadFieldType.OpenLeadSalesFunnels:
                    return lead.CustomerId != null && FieldComparer.Check(lead.CustomerId.Value);

                case ELeadFieldType.Datetime:
                    return FieldComparer.Check(DateTime.Now);

                case ELeadFieldType.Time:
                    return FieldComparer.Check(DateTime.Now);

                case ELeadFieldType.DealStatus:
                    return FieldComparer.Check(lead.DealStatusId);

                case ELeadFieldType.LeadField:
                    if (!FieldComparer.FieldObjId.HasValue)
                        return true;
                    var leadField = LeadFieldService.GetLeadFieldsWithValue(lead.Id).FirstOrDefault(x => x.Id == FieldComparer.FieldObjId.Value);
                    if (leadField == null)
                        return false;
                    switch(leadField.FieldType)
                    {
                        case LeadFieldType.Date:
                            var date = leadField.Value.TryParseDateTime(true);
                            return FieldComparer.Check(date);
                        case LeadFieldType.Number:
                            var floatVal = leadField.Value.TryParseFloat(true);
                            return FieldComparer.Check(floatVal);
                        default:
                            return FieldComparer.Check(leadField.Value);
                    }

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
                    case ELeadFieldType.CustomerField:
                        CustomerField customerField;
                        if (FieldComparer != null && FieldComparer.FieldObjId.HasValue &&
                            (customerField = CustomerFieldService.GetCustomerField(FieldComparer.FieldObjId.Value)) != null)
                        {
                            _fieldName = customerField.Name;
                        }
                        break;
                    case ELeadFieldType.LeadField:
                        LeadField leadField;
                        if (FieldComparer != null && FieldComparer.FieldObjId.HasValue &&
                            (leadField = LeadFieldService.GetLeadField(FieldComparer.FieldObjId.Value)) != null)
                        {
                            _fieldName = leadField.Name;
                        }
                        break;
                }
                return _fieldName ?? (_fieldName = FieldType.Localize());
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
                    case ELeadFieldType.CustomerGroup:
                        var customerGroup = CustomerGroupService.GetCustomerGroup(fieldValueObjId);
                        _fieldValueObjectName = customerGroup != null ? customerGroup.GroupName : string.Empty;
                        break;
                    case ELeadFieldType.Source:
                        var orderSource = OrderSourceService.GetOrderSource(fieldValueObjId);
                        _fieldValueObjectName = orderSource != null ? orderSource.Name : string.Empty;
                        break;
                    case ELeadFieldType.SalesFunnel:
                    case ELeadFieldType.OpenLeadSalesFunnels:
                        var salesFunnel = SalesFunnelService.Get(fieldValueObjId);
                        _fieldValueObjectName = salesFunnel != null ? salesFunnel.Name : string.Empty;
                        break;
                    case ELeadFieldType.Manager:
                    case ELeadFieldType.CustomerManager:
                        var manager = ManagerService.GetManager(fieldValueObjId);
                        _fieldValueObjectName = manager != null ? manager.FullName : string.Empty;
                        break;

                    case ELeadFieldType.DealStatus:
                        var dealStatus = DealStatusService.Get(fieldValueObjId);
                        _fieldValueObjectName = dealStatus != null ? dealStatus.Name : string.Empty;
                        break;

                    case ELeadFieldType.CustomerSegment:
                        var segment = CustomerSegmentService.Get(fieldValueObjId);
                        _fieldValueObjectName = segment != null ? segment.Name : string.Empty;
                        break;

                    case ELeadFieldType.CustomerType:
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
                    case ELeadFieldType.CustomerField:
                        if (CustomerFieldService.GetCustomerField(FieldComparer.FieldObjId.Value) == null)
                            return false;
                        break;
                    case ELeadFieldType.LeadField:
                        if (LeadFieldService.GetLeadField(FieldComparer.FieldObjId.Value) == null)
                            return false;
                        break;
                }
            }

            if (!FieldComparer.ValueObjId.HasValue)
                return true;

            var fieldValueObjId = FieldComparer.ValueObjId.Value;
            switch (FieldType)
            {
                case ELeadFieldType.CustomerGroup:
                    return CustomerGroupService.GetCustomerGroup(fieldValueObjId) != null;
                case ELeadFieldType.Source:
                    return OrderSourceService.GetOrderSource(fieldValueObjId) != null;
                case ELeadFieldType.SalesFunnel:
                    return SalesFunnelService.Get(fieldValueObjId) != null;
                case ELeadFieldType.Manager:
                case ELeadFieldType.CustomerManager:
                    return ManagerService.GetManager(fieldValueObjId) != null;
            }
            return true;
        }
    }
}
