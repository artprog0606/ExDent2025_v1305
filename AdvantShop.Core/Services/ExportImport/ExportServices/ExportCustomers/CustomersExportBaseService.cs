using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.ExportImport;

namespace AdvantShop.Core.Services.ExportImport.ExportServices.ExportCustomers
{
    public class CustomersExportBaseService
    {
        protected readonly CustomersExportSettings _settings;
        protected readonly List<ECustomerFields> _fields;
        protected readonly List<string> _additionalFields;
        
        protected const int BatchSize = 100;

        public CustomersExportBaseService(CustomersExportSettings settings)
        {
            _settings = settings;
            
            _fields = new List<ECustomerFields>();
            _additionalFields = new List<string>();

            foreach (var field in _settings.SelectedExportFields)
            {
                if (Enum.TryParse(field, true, out ECustomerFields customerField))
                {
                    if (customerField != ECustomerFields.None)
                        _fields.Add(customerField);
                }
                else
                    _additionalFields.Add(field);
            }
        }
        
        protected IEnumerable<CustomerExportDto> GetCustomers()
        {
            var currentPage = 1;
            do
            {
                var customers = GetPaging(currentPage).PageItemsList<CustomerExportDto>();
                if (customers.Count == 0)
                    break;
                
                foreach (var customer in customers)
                    yield return customer;

                if (customers.Count < BatchSize)
                    break;
                
                currentPage++;

            } while (true);
        }
        
        protected int GetCustomerRowsCount()
        {
            return GetPaging().GetRowsCount("Customer.CustomerId");
        }
        
        private SqlPaging GetPaging(int? currentPage = null)
        {
            var paging = new SqlPaging()
                {
                    ItemsPerPage = BatchSize,
                    CurrentPageIndex = currentPage ?? 1
                }
                .Select(
                    "Customer.CustomerId".AsSqlField("CustomerId"),
                    "Customer.FirstName",
                    "Customer.LastName",
                    "Customer.Patronymic",
                    "Customer.Phone",
                    "Customer.Email",
                    "Customer.BirthDay",
                    "Customer.Organization",
                    "Customer.CustomerType",
                    "Customer.Enabled",
                    "Customer.RegistrationDateTime",
                    "Customer.RegisteredFrom",
                    "Customer.RegisteredFromIp",
                    "Customer.RegistrationDateTime",
                    "Customer.AdminComment",
                    "Customer.ManagerId",
                    "[Subscription].Subscribe".AsSqlField("IsAgreeForPromotionalNewsletter"),
                    "[Subscription].SubscribeDate".AsSqlField("AgreeForPromotionalNewsletterDateTime"),
                    "[Subscription].SubscribeFromPage".AsSqlField("AgreeForPromotionalNewsletterFrom"),
                    "[Subscription].SubscribeFromIp".AsSqlField("AgreeForPromotionalNewsletterFromIp")
                )
                .From("Customers.Customer")
                .Left_Join("[Customers].[Subscription] ON [Customer].[Email] = [Subscription].[Email]")
                .Where("Customer.CustomerRole = {0}", (int) Role.User);

            SetFilter(paging);

            return paging;
        }
        
        private void SetFilter(SqlPaging query)
        {
            if (_fields.Contains(ECustomerFields.CustomerGroup) || _settings.GroupId != null)
            {
                query
                    .Select("g.GroupName".AsSqlField("CustomerGroupName"))
                    .Inner_Join("Customers.CustomerGroup g on g.CustomerGroupId = Customer.CustomerGroupId");
                
                if (_settings.GroupId != null && _settings.GroupId > 0)
                    query.Where("Customer.CustomerGroupId = {0}", _settings.GroupId.Value);
            }
            
            if (_fields.Contains(ECustomerFields.ManagerName))
            {
                query
                    .Select(
                        "(Select [FirstName] + ' ' + [LastName] From [Customers].[Customer] WHERE [CustomerId] = [Managers].[CustomerId])"
                            .AsSqlField("ManagerName")
                    )
                    .Left_Join("[Customers].[Managers] ON [Managers].[ManagerId] = Customer.[ManagerId]");
            }

            if (_fields.Contains(ECustomerFields.PaidOrdersSum) || 
                _fields.Contains(ECustomerFields.PaidOrdersCount))
            {
                query.Select(
                    (
                        "(Select ISNULL(SUM([Sum]),0) From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                        "WHERE [OrderCustomer].[CustomerId] = Customer.CustomerId and [Order].[PaymentDate] is not null)"

                    ).AsSqlField("PaidOrdersSum"),
                    (
                        "(Select COUNT([Order].[OrderId]) From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                        "WHERE [OrderCustomer].[CustomerId] = Customer.CustomerId and [Order].[PaymentDate] is not null)"

                    ).AsSqlField("PaidOrdersCount")
                );
            }

            if (_fields.Contains(ECustomerFields.Zip) || 
                _fields.Contains(ECustomerFields.Country) ||
                _fields.Contains(ECustomerFields.Region) || 
                _fields.Contains(ECustomerFields.City) ||
                _fields.Contains(ECustomerFields.District) || 
                _fields.Contains(ECustomerFields.Street) ||
                _fields.Contains(ECustomerFields.House) || 
                _fields.Contains(ECustomerFields.Apartment) ||
                _fields.Contains(ECustomerFields.Structure) || 
                _fields.Contains(ECustomerFields.Entrance) ||
                _fields.Contains(ECustomerFields.Floor))
            {
                query
                    .Select(
                        "fc.Zip",
                        "fc.CountryId",
                        "fc.RegionId",
                        "fc.City",
                        "fc.District",
                        "fc.Street",
                        "fc.House",
                        "fc.Apartment",
                        "fc.Structure",
                        "fc.Entrance",
                        "fc.Floor"
                    )
                    .Outer_Apply("(Select Top(1) [Contact].* From [Customers].[Contact] " +
                                 "Where [Contact].[CustomerID] = Customer.CustomerId " +
                                 "Order by IsMain Desc) as fc");
            }
            
            if (_settings.RegistrationDateFrom != null)
                query.Where("RegistrationDateTime >= {0}", _settings.RegistrationDateFrom);
            
            if (_settings.RegistrationDateTo != null)
                query.Where("RegistrationDateTime <= {0}", _settings.RegistrationDateTo);
            
            if (_settings.ManagerId != null)
            {
                if (_settings.ManagerId.Value == -1)
                    query.Where("с.ManagerId IS NULL");
                else
                    query.Where("с.ManagerId = {0}", _settings.ManagerId.Value);
            }
            
            if (_fields.Contains(ECustomerFields.BonusCard) || _settings.HasBonusCard != null)
            {
                query
                    .Select("[Card].CardNumber".AsSqlField("BonusCardNumber"))
                    .Left_Join("[Bonus].[Card] ON [Card].[CardId] = Customer.CustomerId");
                
                if (_settings.HasBonusCard != null)
                    query.Where("[Card].CardNumber is " + (_settings.HasBonusCard.Value ? "NOT NULL" : "NULL"));
            }
            
            if (_settings.Subscription != null)
            {
                query.Left_Join("[Customers].[Subscription] On Customer.Email = [Subscription].[Email]");
                query.Where(
                    _settings.Subscription.Value
                        ? "[Subscription].[Subscribe] = {0}"
                        : "[Subscription].[Subscribe] is null or [Subscription].[Subscribe] = {0}",
                    _settings.Subscription.Value);
            }

            if (_settings.OrdersCountFrom != null)
            {
                query.Where(
                    "(Select COUNT(o.[OrderId]) From [Order].[Order] o LEFT JOIN [Order].[OrderCustomer] oc ON o.[OrderID] = oc.[OrderId] " +
                    "WHERE oc.CustomerId = Customer.CustomerId and o.[PaymentDate] is not null) >= {0}", _settings.OrdersCountFrom);
            }

            if (_settings.OrdersCountTo != null)
            {
                query.Where(
                    "(Select COUNT(o.[OrderId]) From [Order].[Order] o LEFT JOIN [Order].[OrderCustomer] oc ON o.[OrderID] = oc.[OrderId] " +
                    "WHERE oc.CustomerId = Customer.CustomerId and o.[PaymentDate] is not null) <= {0}", _settings.OrdersCountTo);
            }

            if (_settings.OrderSumFrom != null)
            {
                query.Where(
                    "(Select IsNull(SUM([Sum]), 0) From [Order].[Order] o LEFT JOIN [Order].[OrderCustomer] oc ON o.[OrderID] = oc.[OrderId] " +
                    "WHERE oc.CustomerId = Customer.CustomerId and o.[PaymentDate] is not null) >= {0}",
                    _settings.OrderSumFrom);
            }

            if (_settings.OrderSumTo != null)
            {
                query.Where(
                    "(Select IsNull(SUM([Sum]), 0) From [Order].[Order] o LEFT JOIN [Order].[OrderCustomer] oc ON o.[OrderID] = oc.[OrderId] " +
                    "WHERE oc.CustomerId = Customer.CustomerId and o.[PaymentDate] is not null) <= {0}",
                    _settings.OrderSumTo);
            }


            if (_fields.Contains(ECustomerFields.LastOrder) || 
                _settings.LastOrderFrom != null ||
                _settings.LastOrderTo != null ||
                _settings.LastOrderDateTimeFrom != null || 
                _settings.LastOrderDateTimeTo != null)
            {
                query
                    .Select(
                        "LastOrder.Number".AsSqlField("LastOrderNumber"),
                        "LastOrder.OrderId".AsSqlField("LastOrderId")
                    )
                    .Outer_Apply("(Select Top(1) [Order].Number, [Order].OrderId, [Order].OrderDate " +
                                 "From [Order].[Order] " +
                                 "LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                                 "WHERE [CustomerId] = Customer.CustomerId Order by [OrderDate] Desc) as LastOrder");

                if (_settings.LastOrderFrom != null)
                    query.Where("LastOrder.OrderId >= {0}", _settings.LastOrderFrom);

                if (_settings.LastOrderTo != null)
                    query.Where("LastOrder.OrderId <= {0}", _settings.LastOrderTo);

                if (_settings.LastOrderDateTimeFrom != null)
                    query.Where("LastOrder.OrderDate >= {0}", _settings.LastOrderDateTimeFrom);

                if (_settings.LastOrderDateTimeTo != null)
                    query.Where("LastOrder.OrderDate <= {0}", _settings.LastOrderDateTimeTo);
            }

            if (_settings.AverageCheckFrom != null)
            {
                query.Where(
                    "(Select IsNull(SUM([Sum])/Count([Order].OrderId), 0) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = Customer.CustomerId and [Order].[PaymentDate] is not null) >= {0}",
                    _settings.AverageCheckFrom);
            }
            
            if (_settings.AverageCheckTo != null)
            {
                query.Where(
                    "(Select IsNull(SUM([Sum])/Count([Order].OrderId), 0) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = Customer.CustomerId and [Order].[PaymentDate] is not null) <= {0}",
                    _settings.AverageCheckTo.Value);
            }

            if (_settings.SocialType != null)
            {
                switch (_settings.SocialType)
                {
                    case "all":
                        query.Where(
                            "(Exists (Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = Customer.CustomerId) OR " +
                            "Exists (Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = Customer.CustomerId) OR " +
                            "Exists (Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = Customer.CustomerId) OR " +
                            "Exists (Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = Customer.CustomerId))");
                        break;
                    case "vk":
                        query.Where("Exists (Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = Customer.CustomerId)");
                        break;
                    case "fb":
                        query.Where("Exists (Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = Customer.CustomerId)");
                        break;
                    case "instagram":
                        query.Where("Exists (Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = Customer.CustomerId)");
                        break;
                    case "telegram":
                        query.Where("Exists (Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = Customer.CustomerId)");
                        break;
                }
            }

            if (_settings.CustomerType != null && _settings.CustomerType != CustomerType.All)
            {
                query.Where("Customer.CustomerType = {0}", (int) _settings.CustomerType);
            }

            if (_settings.CustomerFields != null)
            {
                foreach (var fieldFilter in _settings.CustomerFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        query.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = с.[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }
        }
    }
}