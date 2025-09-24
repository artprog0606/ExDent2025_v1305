using System;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum EOrderFieldType
    {
        [Localize("Core.Crm.EOrderFieldType.None"), FieldUsage(EFieldUsage.None)]
        None = 0,

        [Localize("Core.Crm.EOrderFieldType.LastName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 10), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        LastName = 1,
        [Localize("Core.Crm.EOrderFieldType.FirstName"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 20), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        FirstName = 2,
        [Localize("Core.Crm.EOrderFieldType.Patronymic"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 30), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Patronymic = 3,
        [Localize("Core.Crm.EOrderFieldType.Phone"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 40), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Phone = 7,
        [Localize("Core.Crm.EOrderFieldType.Email"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 50), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Email = 6,
        [Localize("Core.Crm.EOrderFieldType.Country"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 60), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Country = 8,
        [Localize("Core.Crm.EOrderFieldType.Region"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 70), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Region = 9,
        [Localize("Core.Crm.EOrderFieldType.City"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        City = 10,
        [Localize("Core.Crm.ELeadFieldType.Organization"), FieldType(EFieldType.Text), FieldTypeValue(EFieldType.Text, "Core.Crm.EFieldType.GroupName.Client", 80), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Organization = 31,
        [Localize("Core.Crm.ELeadFieldType.Manager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 90), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerManager = 20,
        [Localize("Core.Crm.EOrderFieldType.CustomerGroup"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 100), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerGroup = 4,
        [Localize("Core.Crm.ELeadFieldType.CustomerSegment"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 110), FieldUsage(EFieldUsage.Filter)]
        CustomerSegment = 21,
        [Localize("Core.Crm.EOrderFieldType.CustomerType"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 115), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        CustomerType = 36,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidSum"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 120), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidSum = 22,
        [Localize("Core.Crm.ELeadFieldType.OrdersCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 130), FieldUsage(EFieldUsage.Filter)]
        OrdersCount = 23,
        [Localize("Core.Crm.ELeadFieldType.OrdersPaidCount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 140), FieldUsage(EFieldUsage.Filter)]
        OrdersPaidCount = 24,
        [Localize("Core.Crm.ELeadFieldType.ProductsByCustomer"), FieldType(EFieldType.ProductChooser), FieldTypeValue(EFieldType.ProductChooser, "Core.Crm.EFieldType.GroupName.Client", 150), FieldUsage(EFieldUsage.Filter)]
        ProductsByCustomer = 25,
        [Localize("Core.Crm.ELeadFieldType.CategoriesByCustomer"), FieldType(EFieldType.CategoryChooser), FieldTypeValue(EFieldType.CategoryChooser, "Core.Crm.EFieldType.GroupName.Client", 160), FieldUsage(EFieldUsage.Filter)]
        CategoriesByCustomer = 26,
        [Localize("Core.Crm.ELeadFieldType.OpenLeadSalesFunnels"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Client", 170), FieldUsage(EFieldUsage.Filter)]
        OpenLeadSalesFunnels = 28,

        [Localize("Core.Crm.EOrderFieldType.CustomerField"), FieldUsage(EFieldUsage.None)]
        CustomerField = 5,

        [Localize("Core.Crm.EOrderFieldType.Manager"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Order", 200), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        Manager = 19,
        [Localize("Core.Crm.EOrderFieldType.OrderSum"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Order", 210), FieldUsage(EFieldUsage.Filter)]
        OrderSum = 12,
        [Localize("Core.Crm.EOrderFieldType.Products"), FieldType(EFieldType.ProductChooser), FieldTypeValue(EFieldType.ProductChooser, "Core.Crm.EFieldType.GroupName.Order", 220), FieldUsage(EFieldUsage.Filter)]
        Products = 17,
        [Localize("Core.Crm.EOrderFieldType.Categories"), FieldType(EFieldType.CategoryChooser), FieldTypeValue(EFieldType.CategoryChooser, "Core.Crm.EFieldType.GroupName.Order", 230), FieldUsage(EFieldUsage.Filter)]
        Categories = 18,
        [Localize("Core.Crm.EOrderFieldType.HasGiftCertificate"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 235), FieldUsage(EFieldUsage.Filter)]
        HasGiftCertificate = 34,
        [Localize("Core.Crm.EOrderFieldType.OrderSource"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Order", 240), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        OrderSource = 11,
        [Localize("Core.Crm.EOrderFieldType.PaymetMethod"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Order", 250), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        PaymentMethod = 13,
        [Localize("Core.Crm.EOrderFieldType.ShippingMethod"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Order", 260), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        ShippingMethod = 14,
        [Localize("Core.Crm.EOrderFieldType.DeliveryDate"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 265), FieldUsage(EFieldUsage.Filter)]
        DeliveryDate = 35,

        [Localize("Core.Crm.EOrderFieldType.IsPaid"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 270), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter, UseForEdit = new[] {ETriggerEventType.OrderStatusChanged})]
        IsPaid = 27,
        [Localize("Core.Crm.EOrderFieldType.OrderStatus"), FieldType(EFieldType.Select), FieldTypeValue(EFieldType.Select, "Core.Crm.EFieldType.GroupName.Order", 280), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter, UseForEdit = new []{ETriggerEventType.OrderPaied, ETriggerEventType.OrderCreated})]
        OrderStatus = 30,

        [Localize("Core.Crm.EOrderFieldType.IsFromLead"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 290), FieldUsage(EFieldUsage.Filter)]
        IsFromLead = 15,
        [Localize("Core.Crm.EOrderFieldType.IsFromAdminArea"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 300), FieldUsage(EFieldUsage.Filter)]
        IsFromAdminArea = 16,
        [Localize("Core.Crm.ELeadFieldType.UseIn1C"), FieldType(EFieldType.Checkbox), FieldTypeValue(EFieldType.Checkbox, "Core.Crm.EFieldType.GroupName.Order", 310), FieldUsage(EFieldUsage.None)]
        UseIn1C = 32,


        [Localize("Core.Crm.EOrderFieldType.Datetime"), FieldType(EFieldType.Datetime), FieldTypeValue(EFieldType.Datetime, "Core.Crm.EFieldType.GroupName.System", 400), FieldUsage(EFieldUsage.Filter)]
        Datetime = 29,
        [Localize("Core.Crm.EOrderFieldType.Time"), FieldType(EFieldType.Time), FieldTypeValue(EFieldType.Time, "Core.Crm.EFieldType.GroupName.System", 410), FieldUsage(EFieldUsage.Filter)]
        Time = 33,

        [Localize("Core.Crm.ELeadFieldType.BonusAccount"), FieldType(EFieldType.Number), FieldTypeValue(EFieldType.Number, "Core.Crm.EFieldType.GroupName.Client", 420), FieldUsage(EFieldUsage.Edit | EFieldUsage.Filter)]
        BonusAccount = 37

        // next - 38
    }

    public class OrderFieldComparer : IBizObjectFieldComparer<Order>
    {
        public EOrderFieldType FieldType { get; set; }

        public string FieldTypeStr => FieldType.ToString().ToLower();

        public FieldComparer FieldComparer { get; set; }

        public BizObjectFieldCompareType CompareType { get; set; }

        public bool CheckField(Order order)
        {
            var check = Check(order);
            return CompareType == BizObjectFieldCompareType.Equal ? check : !check;
        }

        private bool Check(Order order)
        {
            switch (FieldType)
            {
                case EOrderFieldType.LastName:
                    return FieldComparer.Check(order.OrderCustomer.LastName);

                case EOrderFieldType.FirstName:
                    return FieldComparer.Check(order.OrderCustomer.FirstName);

                case EOrderFieldType.Patronymic:
                    return FieldComparer.Check(order.OrderCustomer.Patronymic);

                case EOrderFieldType.CustomerGroup:
                    var customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
                    return customer != null && FieldComparer.Check(customer.CustomerGroupId);

                case EOrderFieldType.CustomerField:
                    if (!FieldComparer.FieldObjId.HasValue)
                        return false;
                    var customerField = CustomerFieldService.GetCustomerFieldsWithValue(order.OrderCustomer.CustomerID).FirstOrDefault(x => x.Id == FieldComparer.FieldObjId.Value);
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

                case EOrderFieldType.Email:
                    return FieldComparer.Check(order.OrderCustomer.Email);

                case EOrderFieldType.Phone:
                    return FieldComparer.Check(order.OrderCustomer.Phone) ||
                        (order.OrderCustomer.StandardPhone.HasValue && FieldComparer.Check(order.OrderCustomer.StandardPhone.Value.ToString()));

                case EOrderFieldType.Country:
                    return FieldComparer.Check(order.OrderCustomer.Country);

                case EOrderFieldType.Region:
                    return FieldComparer.Check(order.OrderCustomer.Region);

                case EOrderFieldType.City:
                    return FieldComparer.Check(order.OrderCustomer.City);

                case EOrderFieldType.Organization:
                    return FieldComparer.Check(order.OrderCustomer.Organization);

                case EOrderFieldType.CustomerType:
                    return FieldComparer.Check((int)order.OrderCustomer.CustomerType);

                case EOrderFieldType.OrderSource:
                    return FieldComparer.Check(order.OrderSourceId);

                case EOrderFieldType.OrderSum:
                    return FieldComparer.Check(order.Sum);

                case EOrderFieldType.PaymentMethod:
                    return FieldComparer.Check(order.PaymentMethodId != 0 ? order.PaymentMethodId : default(int?));

                case EOrderFieldType.ShippingMethod:
                    return FieldComparer.Check(order.ShippingMethodId != 0 ? order.ShippingMethodId : default(int?));

                case EOrderFieldType.IsFromLead:
                    return FieldComparer.Check(order.LeadId.HasValue);

                case EOrderFieldType.IsFromAdminArea:
                    return FieldComparer.Check(order.IsFromAdminArea);

                case EOrderFieldType.UseIn1C:
                    return FieldComparer.Check(order.UseIn1C);

                case EOrderFieldType.Products:
                {
                    foreach (var orderItem in order.OrderItems)
                        if (orderItem.ProductID != null && FieldComparer.Check(orderItem.ProductID.Value))
                            return true;
                    return false;
                }

                case EOrderFieldType.Categories:
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        if (orderItem.ProductID == null)
                            continue;

                        var categories = ProductService.GetCategoriesByProductId(orderItem.ProductID.Value);

                        foreach (var category in categories)
                            if (FieldComparer.Check(category.CategoryId))
                                return true;
                    }
                    return false;
                }

                case EOrderFieldType.HasGiftCertificate:
                    return FieldComparer.Check(order.OrderCertificates.Any());

                case EOrderFieldType.Manager:
                    return FieldComparer.Check(order.ManagerId);
                    
                case EOrderFieldType.CustomerManager:
                    var c = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
                    return FieldComparer.Check(c != null ? c.ManagerId : null);

                case EOrderFieldType.CustomerSegment:
                    return FieldComparer.Check(order.OrderCustomer.CustomerID);

                case EOrderFieldType.OrdersPaidSum:
                    return FieldComparer.Check(order.OrderCustomer.CustomerID);

                case EOrderFieldType.OrdersCount:
                    return FieldComparer.Check(order.OrderCustomer.CustomerID);

                case EOrderFieldType.OrdersPaidCount:
                    return FieldComparer.Check(order.OrderCustomer.CustomerID);
                    
                case EOrderFieldType.ProductsByCustomer:
                {
                    foreach (var productId in OrderService.GetProductIdsByCustomer(order.OrderCustomer.CustomerID))
                        if (FieldComparer.Check(productId))
                            return true;
                    return false;
                }

                case EOrderFieldType.CategoriesByCustomer:
                {
                    foreach (var productId in OrderService.GetProductIdsByCustomer(order.OrderCustomer.CustomerID))
                    {
                        var categories = ProductService.GetCategoriesByProductId(productId);

                        foreach (var category in categories)
                            if (FieldComparer.Check(category.CategoryId))
                                return true;
                    }
                    return false;
                }
                
                case EOrderFieldType.DeliveryDate:
                    return FieldComparer.Check(order.DeliveryDate != null || !string.IsNullOrEmpty(order.DeliveryTime));

                case EOrderFieldType.IsPaid:
                    return FieldComparer.Check(order.PaymentDate != null);

                case EOrderFieldType.OrderStatus:
                    return FieldComparer.Check(order.OrderStatusId);

                case EOrderFieldType.OpenLeadSalesFunnels:
                    return FieldComparer.Check(order.OrderCustomer.CustomerID);

                case EOrderFieldType.Datetime:
                    return FieldComparer.Check(DateTime.Now);

                case EOrderFieldType.Time:
                    return FieldComparer.Check(DateTime.Now);

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
                    case EOrderFieldType.CustomerField:
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
                    case EOrderFieldType.CustomerGroup:
                        var customerGroup = CustomerGroupService.GetCustomerGroup(fieldValueObjId);
                        _fieldValueObjectName = customerGroup != null ? customerGroup.GroupName : string.Empty;
                        break;
                    case EOrderFieldType.OrderSource:
                        var orderSource = OrderSourceService.GetOrderSource(fieldValueObjId);
                        _fieldValueObjectName = orderSource != null ? orderSource.Name : string.Empty;
                        break;
                    case EOrderFieldType.PaymentMethod:
                        var paymetMethod = PaymentService.GetPaymentMethod(fieldValueObjId);
                        _fieldValueObjectName = paymetMethod != null ? paymetMethod.Name : string.Empty;
                        break;
                    case EOrderFieldType.ShippingMethod:
                        var shippingMethod = ShippingMethodService.GetShippingMethod(fieldValueObjId);
                        _fieldValueObjectName = shippingMethod != null ? shippingMethod.Name : string.Empty;
                        break;
                    case EOrderFieldType.Manager:
                    case EOrderFieldType.CustomerManager:
                        var manager = ManagerService.GetManager(fieldValueObjId);
                        _fieldValueObjectName = manager != null ? manager.FullName : string.Empty;
                        break;
                    case EOrderFieldType.OpenLeadSalesFunnels:
                        var salesFunnel = SalesFunnelService.Get(fieldValueObjId);
                        _fieldValueObjectName = salesFunnel != null ? salesFunnel.Name : string.Empty;
                        break;
                    case EOrderFieldType.CustomerSegment:
                        var segment = CustomerSegmentService.Get(fieldValueObjId);
                        _fieldValueObjectName = segment != null ? segment.Name : string.Empty;
                        break;
                    case EOrderFieldType.OrderStatus:
                        var status = OrderStatusService.GetOrderStatus(fieldValueObjId);
                        _fieldValueObjectName = status != null ? status.StatusName : string.Empty;
                        break;
                    case EOrderFieldType.CustomerType:
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
                    case EOrderFieldType.CustomerField:
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
                case EOrderFieldType.CustomerGroup:
                    return CustomerGroupService.GetCustomerGroup(fieldValueObjId) != null;
                case EOrderFieldType.OrderSource:
                    return OrderSourceService.GetOrderSource(fieldValueObjId) != null;
                case EOrderFieldType.PaymentMethod:
                    return PaymentService.GetPaymentMethod(fieldValueObjId) != null;
                case EOrderFieldType.ShippingMethod:
                    return ShippingMethodService.GetShippingMethod(fieldValueObjId) != null;
                case EOrderFieldType.Manager:
                case EOrderFieldType.CustomerManager:
                    return ManagerService.GetManager(fieldValueObjId) != null;
            }
            return true;
        }
    }
}

