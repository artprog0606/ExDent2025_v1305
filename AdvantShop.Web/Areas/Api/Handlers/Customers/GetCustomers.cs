using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class GetCustomers : AbstractCommandHandler<GetCustomersResponse>
    {
        private readonly CustomersFilterModel _filter;
        private SqlPaging _paging;

        public GetCustomers(CustomersFilterModel filter)
        {
            _filter = filter;
        }

        protected override void Validate()
        {
            if (_filter.IsDefaultItemsPerPage)
                _filter.ItemsPerPage = 100;

            if (_filter.ItemsPerPage > 100 || _filter.ItemsPerPage <= 0)
                _filter.ItemsPerPage = 100;

            if (_filter.Page < 0)
                throw new BlException("page can't less than 0");
        }

        protected override GetCustomersResponse Handle()
        {
            GetPaging();

            var model = new GetCustomersResponse()
            {
                Pagination = new ApiPagination()
                {
                    CurrentPage = _paging.CurrentPageIndex,
                    TotalCount = _paging.TotalRowsCount,
                    TotalPageCount = _paging.PageCount()
                },
                Customers = new List<GetCustomersItem>()
            };

            if (model.Pagination.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;

            model.Customers = _paging.PageItemsList<GetCustomersItem>();
            model.Pagination.Count = model.Customers.Count;

            if (_filter.Extended.HasValue && _filter.Extended.Value)
            {
                foreach (var customer in model.Customers)
                {
                    var contacts = CustomerService.GetCustomerContacts(customer.Id);
                    customer.Contact = contacts != null && contacts.Count > 0
                        ? new CustomerContactModel(contacts[0])
                        : new CustomerContactModel();
                    customer.Contacts = contacts != null && contacts.Count > 0
                        ? contacts.Select(x => new CustomerContactModel(x)).ToList()
                        : new List<CustomerContactModel>();

                    customer.Fields = CustomerFieldService.GetCustomerFieldsWithValue(customer.Id)
                        .Where(x => 
                            x.CustomerType == CustomerType.All ||
                            x.CustomerType.ToString().Equals(customer.CustomerType, StringComparison.OrdinalIgnoreCase))
                        .Select(x => new CustomerFieldModel(x))
                        .ToList();
                }
            }

            return model;
        }

        private void GetPaging()
        {
            _paging = 
                new SqlPaging() {ItemsPerPage = _filter.ItemsPerPage, CurrentPageIndex = _filter.Page}
                .Select(
                    "[Customer].CustomerID".AsSqlField("Id"),
                    "[Customer].Email",
                    "[Customer].StandardPhone".AsSqlField("Phone"),
                    "[Customer].FirstName",
                    "[Customer].LastName",
                    "[Customer].Patronymic",
                    "[Customer].Organization",
                    "[Customer].BirthDay",
                    "[Customer].AdminComment",
                    "[Customer].ManagerId",
                    "[Customer].CustomerGroupId".AsSqlField("GroupId"),
                    "[Customer].CustomerType",
                    "[Customer].IsAgreeForPromotionalNewsletter",
                    "(Case when exists(SELECT Id FROM [Customers].[Subscription] WHERE [Email] = [Customer].Email AND [Subscribe] = 1) then 1 else 0 End)"
                        .AsSqlField("SubscribedForNews")
                )
                .From("[Customers].[Customer]")
                .Where("[Customer].[CustomerRole] = {0}", (int) Role.User);

            Filter();
            Sorting();            
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_filter.Search))
            {
                _paging.Where(
                    "([Customer].Email LIKE '%'+{0}+'%' OR [Customer].Firstname + ' ' + [Customer].Lastname LIKE '%'+{0}+'%' OR " +
                    "[Customer].Lastname + ' ' + [Customer].Firstname LIKE '%'+{0}+'%' OR [Customer].Phone LIKE '%'+{0}+'%' OR [Customer].Organization LIKE '%'+{0}+'%')",
                    _filter.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                _paging.Where("([Customer].Lastname + ' ' + [Customer].Firstname LIKE '%'+{0}+'%' OR [Customer].Firstname + ' ' + [Customer].Lastname LIKE '%'+{0}+'%')", _filter.Name);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Email))
            {
                _paging.Where("[Customer].Email LIKE '%'+{0}+'%'", _filter.Email);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Phone))
            {
                long? standartPhone = StringHelper.ConvertToStandardPhone(_filter.Phone, true, true);
                _paging.Where("convert(nvarchar, StandardPhone) LIKE '%'+{0}+'%'", standartPhone != null ? standartPhone.ToString() : "null");
            }

            if (_filter.ManagerId.HasValue)
            {
                if (_filter.ManagerId.Value == -1)
                {
                    _paging.Where("[Customer].ManagerId IS NULL");
                }
                else
                {
                    _paging.Where("[Customer].ManagerId = {0}", _filter.ManagerId.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(_filter.LastOrderNumber))
            {
                _paging.Where(
                    "(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                    "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc) LIKE '%'+{0}+'%'",
                    _filter.LastOrderNumber);
            }

            if (!string.IsNullOrWhiteSpace(_filter.City))
            {
                _paging.Where(
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID]) LIKE '%'+{0}+'%'",
                    _filter.City);
            }

            if (_filter.OrdersCountFrom != null)
            {
                _paging.Where(
                    "(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}", _filter.OrdersCountFrom);
            }

            if (_filter.OrdersCountTo != null)
            {
                _paging.Where(
                    "(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}", _filter.OrdersCountTo);
            }

            if (_filter.OrdersSumFrom != null)
            {
                _paging.Where(
                    "(Select SUM([Sum]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}",
                    _filter.OrdersSumFrom);
            }

            if (_filter.OrdersSumTo != null)
            {
                _paging.Where(
                    "(Select SUM([Sum]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}",
                    _filter.OrdersSumTo);
            }

            DateTime from, to;

            if (!string.IsNullOrWhiteSpace(_filter.RegistrationDateTimeFrom) && DateTime.TryParse(_filter.RegistrationDateTimeFrom, out from))
            {
                _paging.Where("RegistrationDateTime >= {0}", from);
            }

            if (!string.IsNullOrWhiteSpace(_filter.RegistrationDateTimeTo) && DateTime.TryParse(_filter.RegistrationDateTimeTo, out to))
            {
                _paging.Where("RegistrationDateTime <= {0}", to);
            }

            if (_filter.GroupId != 0)
            {
                _paging.Where("[Customer].CustomerGroupId = {0}", _filter.GroupId);
            }

            DateTime fromLastOrder, toLastOrder;

            if (!string.IsNullOrWhiteSpace(_filter.LastOrderDateTimeFrom) && DateTime.TryParse(_filter.LastOrderDateTimeFrom, out fromLastOrder))
            {
                _paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] " +
                    "Order by [OrderDate] Desc) >= {0}",
                    fromLastOrder);
            }
            if (!string.IsNullOrWhiteSpace(_filter.LastOrderDateTimeTo) && DateTime.TryParse(_filter.LastOrderDateTimeTo, out toLastOrder))
            {
                _paging.Where(
                    "(Select top(1) OrderDate " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] " +
                    "Order by [OrderDate] Desc) <= {0}",
                    toLastOrder);
            }

            if (_filter.AverageCheckFrom != null)
            {
                _paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) >= {0}",
                    _filter.AverageCheckFrom);
            }
            if (_filter.AverageCheckTo != null)
            {
                _paging.Where(
                    "(Select SUM([Sum])/Count([Order].OrderId) " +
                    "From [Order].[Order] " +
                    "Left Join [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                    "Where [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null) <= {0}",
                    _filter.AverageCheckTo.Value);
            }

            if (_filter.SocialType != null)
            {
                switch (_filter.SocialType)
                {
                    case "all":
                        _paging.Where(
                            "(Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId]) OR " +
                            "Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId]))");
                        break;
                    case "vk":
                        _paging.Where("Exists(Select 1 From [Customers].[VkUser] Where VkUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "fb":
                        _paging.Where("Exists(Select 1 From [Customers].[FacebookUser] Where FacebookUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "instagram":
                        _paging.Where("Exists(Select 1 From [Customers].[InstagramUser] Where InstagramUser.CustomerId = [Customer].[CustomerId])");
                        break;
                    case "telegram":
                        _paging.Where("Exists(Select 1 From [Customers].[TelegramUser] Where TelegramUser.CustomerId = [Customer].[CustomerId])");
                        break;
                }
            }

            if (_filter.CustomerFields != null)
            {
                foreach (var fieldFilter in _filter.CustomerFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [Customer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }

            if (_filter.Subscription != null)
            {
                _paging.Left_Join("[Customers].[Subscription] On [Customer].Email = [Subscription].[Email]");
                _paging.Where("[Subscription].[Subscribe] = {0}", _filter.Subscription.Value);
            }

            if (_filter.HasBonusCard != null)
            {
                _paging.Left_Join("[Bonus].[Card] ON [Card].[CardId] = [Customer].[CustomerID]");
                _paging.Where("[Card].CardNumber IS " + (_filter.HasBonusCard.Value ? "NOT NULL" : "NULL"));
            }

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.Assigned)
                    {
                        _paging.Where("[Customer].ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersCustomerConstraint == ManagersCustomerConstraint.AssignedAndFree)
                    {
                        _paging.Where("([Customer].ManagerId = {0} or [Customer].ManagerId is null)", manager.ManagerId);
                    }
                }
            }

            if (_filter.Tags != null && _filter.Tags.Count > 0)
            {
                _paging.Where("(SELECT COUNT(*) FROM [Customers].[TagMap] WHERE [TagMap].[CustomerId] = [Customer].CustomerID AND [TagMap].[TagId] in (" +
                    string.Join(",", _filter.Tags.Select(x => x.ToString()).ToArray()) + ")) >= " + _filter.Tags.Count.ToString());
            }

            if (_filter.CustomerSegment.HasValue)
            {
                _paging.Left_Join("[Customers].[CustomerSegment_Customer] On [Customer].CustomerID = [CustomerSegment_Customer].[CustomerId]");
                _paging.Where("[CustomerSegment_Customer].[SegmentId] = {0}", _filter.CustomerSegment.Value);
            }
        }


        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("RegistrationDateTime");
                return;
            }

            var sorting = _filter.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}