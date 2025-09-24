using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.CustomerSegments;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Customers.CustomerSegments;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Customers.CustomerSegments
{
    public class GetCustomersBySegment
    {
        private readonly bool? _onlyCustomerId;
        private readonly CustomersBySegmentFilterModel _filterModel;
        private readonly bool _exportToCsv;

        private SqlPaging _paging;

        public GetCustomersBySegment(CustomersBySegmentFilterModel filterModel)
        {
            _filterModel = filterModel;
            _exportToCsv = filterModel.OutputDataType == FilterOutputDataType.Csv;
        }

        public GetCustomersBySegment(CustomersBySegmentFilterModel filterModel, bool onlyCustomerId) :this(filterModel)
        {
            _onlyCustomerId = onlyCustomerId;
        }

        public FilterResult<CustomerBySegmentViewModel> Execute()
        {
            var model = new FilterResult<CustomerBySegmentViewModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", model.TotalItemsCount);

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<CustomerBySegmentViewModel>();
            
            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            if (_onlyCustomerId.HasValue)
            {
                _paging.Select("[Customer].CustomerID");
            }
            else
            {
                _paging.Select(
                    "[Customer].CustomerID",
                    "[Customer].Email".AsSqlField("Email"),
                    "[Customer].Phone".AsSqlField("Phone"),
                    "[Customer].FirstName",
                    "[Customer].LastName",
                    "[Customer].Patronymic",
                    "[Customer].Organization",
                    "(Select [Customer].Lastname + ' ' + [Customer].Firstname + ' ' + ISNULL([Customer].Patronymic,''))".AsSqlField("Name"),

                    ("(Select COUNT([Order].[OrderId]) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                     "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                        .AsSqlField("OrdersCount"),

                    "[Customer].RegistrationDateTime".AsSqlField("RegistrationDateTime"),
                    "[Customer].BirthDay".AsSqlField("BirthDay"),
                    "[Customer].CustomerType"
                    );
                if (SettingsCustomers.IsRegistrationAsLegalEntity)
                {
                    var field = CustomerFieldService.GetCustomerFieldByFieldAssignment(CustomerFieldAssignment.CompanyName);
                    if (field != null)
                    {
                        _paging.Select("[CustomerFieldValuesMap].Value".AsSqlField("CompanyName"));
                        _paging.Left_Join("[Customers].[CustomerFieldValuesMap] ON [CustomerFieldValuesMap].[CustomerId] = [Customer].CustomerID " +
                            "AND [Customer].[CustomerType] = 1 AND CustomerFieldId = " + field.Id);
                    }
                }

                if (_exportToCsv)
                {
                    _paging.Select(
                        ("(Select Top(1) [Order].OrderId From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                         "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField("LastOrderId"),

                        ("(Select Top(1) [Order].Number From [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderId] = [OrderCustomer].[OrderId] " +
                         "WHERE [CustomerId] = [Customer].[CustomerId] Order by [OrderDate] Desc)").AsSqlField(
                             "LastOrderNumber"),

                        ("(Select ISNULL(SUM([Sum]),0) From  [Order].[Order] LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                         "WHERE [OrderCustomer].[CustomerId] = [Customer].[CustomerId] and [Order].[PaymentDate] is not null)")
                            .AsSqlField("OrdersSum"),

                        "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [Customer].[CustomerID])"
                            .AsSqlField("Location"),

                        "(Select [FirstName] + ' ' + [LastName] From  [Customers].[Customer]  WHERE [CustomerId] = [Managers].[CustomerId])"
                            .AsSqlField("ManagerName")
                        );

                    _paging.Left_Join("[Customers].[Managers] ON [Customer].[ManagerId] = [Managers].[ManagerId]");
                }
            }

            CustomerSegmentService.GetCustomersBySegmentFilter(_paging, _filterModel.Id, _filterModel.Search);

            Sorting();
        }

        

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("RegistrationDateTime");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower().Replace("formatted", "");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
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