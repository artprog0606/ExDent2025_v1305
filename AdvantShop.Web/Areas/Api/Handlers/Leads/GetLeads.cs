using System;
using System.Linq;
using AdvantShop.Areas.Api.Models.Leads;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Api;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Leads
{
    public sealed class GetLeads : ICommandHandler<GetLeadsListResponse>
    {
        private readonly LeadsFilter _filter;
        private SqlPaging _paging;

        public GetLeads(LeadsFilter filter)
        {
            _filter = filter;
        }
        
        public GetLeadsListResponse Execute()
        {
            var data = GetDataItems();
            return data;
        }

        private GetLeadsListResponse GetDataItems()
        {
            var model = new GetLeadsListResponse();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.PageIndex = _paging.CurrentPageIndex;
            model.ItemsPerPage = _paging.ItemsPerPage;

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;
            
            model.DataItems = _paging.PageItemsList<LeadApi>();

            if (_filter.LoadItems && model.DataItems != null)
            {
                foreach (var item in model.DataItems)
                    item.LeadItems = LeadService.GetLeadItems(item.Id).Select(x => new LeadItemApi(x)).ToList();
            }

            if (_filter.LoadCustomerFields && model.DataItems != null)
            {
                foreach (var item in model.DataItems)
                {
                    if (item.CustomerId == null) 
                        continue;
                    
                    item.CustomerFields =
                        CustomerFieldService.GetMappedCustomerFieldsWithValue(item.CustomerId.Value)
                            .Select(x => new CustomerFieldWithValueApi(x))
                            .ToList();
                }
            }

            return model;
        } 
        
        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };

            _paging.Select(
                "[Lead].Id",
                "[Lead].Title",
                "[Lead].Description",
                
                "[Lead].Comment".AsSqlField("CustomerComment"),
                "[Lead].AdminComment",
                "[Lead].SalesFunnelId",
                "[SalesFunnel].[Name]".AsSqlField("SalesFunnelName"),
                "[DealStatus].[Id]".AsSqlField("DealStatusId"),
                "[DealStatus].[Name]".AsSqlField("DealStatusName"),
                "[Lead].OrderSourceId".AsSqlField("SourceId"),
                "[Lead].Sum",
                "[Lead].CreatedDate".AsSqlField("Date"),

                "[Lead].CustomerId",
                "[Lead].FirstName",
                "[Lead].LastName",
                "[Lead].Patronymic",
                "[Lead].Phone",
                "[Lead].Email",
                "LeadCustomer.CustomerType",
                
                "[Lead].Country",
                "[Lead].Region",
                "[Lead].District",
                "[Lead].City",
                "[Lead].Zip",
                
                "isNull((Select Sum(Price*Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)".AsSqlField("ProductsSum"),
                "isNull((Select Sum(Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)".AsSqlField("ProductsCount"),

                "[LeadCurrency].CurrencyValue",
                "[LeadCurrency].CurrencyCode",
                "[LeadCurrency].CurrencySymbol",
                "[LeadCurrency].IsCodeBefore",

                "[Lead].ManagerId",
                "[ManagerCustomer].FirstName + ' ' + [ManagerCustomer].LastName".AsSqlField("ManagerName")
            );

            _paging.From("[Order].[Lead]");
            _paging.Left_Join("[Customers].[Customer] as LeadCustomer on [Lead].[CustomerId] = [LeadCustomer].[CustomerId]");
            _paging.Left_Join("[Customers].[Managers] ON [Lead].[ManagerId] = [Managers].[ManagerID]");
            _paging.Left_Join("[Customers].[Customer] as ManagerCustomer ON [Managers].[CustomerId] = [ManagerCustomer].[CustomerId]");
            _paging.Left_Join("[CRM].[DealStatus] ON [DealStatus].[Id] = [Lead].[DealStatusId]");
            _paging.Left_Join("[CRM].[SalesFunnel] On [SalesFunnel].[Id]=Lead.SalesFunnelId");
            _paging.Left_Join("[Order].[LeadCurrency] ON [Lead].[Id] = [LeadCurrency].[LeadId]");

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (_filter.CustomerId != null && _filter.CustomerId.Value != Guid.Empty)
                _paging.Where("Lead.CustomerId = {0}", _filter.CustomerId);

            if (_filter.DealStatusId != null)
                _paging.Where("DealStatusId = {0}", _filter.DealStatusId.Value);
            
            if (_filter.SalesFunnelId != null)
                _paging.Where("[Lead].SalesFunnelId = {0}", _filter.SalesFunnelId);
            
            if (!string.IsNullOrWhiteSpace(_filter.Search))
            {
                _paging.Where(
                    "([Lead].Id LIKE '%'+{0}+'%' OR " +
                    "[Lead].FirstName LIKE '%'+{0}+'%' OR " +
                    "[Lead].LastName LIKE '%'+{0}+'%' OR " +
                    "[Lead].Patronymic LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].LastName, '') + ' ' + isNull([Lead].FirstName, '') + ' ' + isNull([Lead].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].FirstName, '') + ' ' + isNull([Lead].LastName, '')) LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].FirstName LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].LastName LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Patronymic LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].LastName, '') + ' ' + isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].LastName, '')) LIKE '%'+{0}+'%' OR " +
                    "[Lead].Phone LIKE '%'+{0}+'%' OR " +
                    "[Lead].Email LIKE '%'+{0}+'%' OR " +
                    "[Lead].Organization LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Phone LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Email LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Organization LIKE '%'+{0}+'%' OR " +
                    "[Lead].Description LIKE '%'+{0}+'%')",
                    _filter.Search);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                _paging.Where(
                    "([Lead].FirstName LIKE '%'+{0}+'%' OR " +
                    "[Lead].LastName LIKE '%'+{0}+'%' OR " +
                    "[Lead].Patronymic LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].LastName, '') + ' ' + isNull([Lead].FirstName, '') + ' ' + isNull([Lead].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([Lead].FirstName, '') + ' ' + isNull([Lead].LastName, '')) LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].FirstName LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].LastName LIKE '%'+{0}+'%' OR " +
                    "[LeadCustomer].Patronymic LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].LastName, '') + ' ' + isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].Patronymic, '')) LIKE '%'+{0}+'%' OR " +
                    "(isNull([LeadCustomer].FirstName, '') + ' ' + isNull([LeadCustomer].LastName, '')) LIKE '%'+{0}+'%')",
                    _filter.Name);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Organization))
                _paging.Where("([LeadCustomer].Organization LIKE '%'+{0}+'%' OR [Lead].Organization LIKE '%'+{0}+'%')", _filter.Organization);

            if (_filter.DateFrom != null && DateTime.TryParse(_filter.DateFrom, out var dateFrom))
                _paging.Where("Lead.CreatedDate >= {0}", dateFrom);
            
            if (_filter.DateTo != null && DateTime.TryParse(_filter.DateTo, out var dateTo))
                _paging.Where("Lead.CreatedDate <= {0}", dateTo);
            
            if (_filter.SumFrom != null)
                _paging.Where("Lead.Sum >= {0}", _filter.SumFrom.Value);
            
            if (_filter.SumTo != null)
                _paging.Where("Lead.Sum <= {0}", _filter.SumTo.Value);
            
            if (_filter.ManagerId != null)
            {
                if (_filter.ManagerId.Value == -1)
                    _paging.Where("[Lead].ManagerId IS NULL");
                else
                    _paging.Where("[Lead].ManagerId = {0}", _filter.ManagerId.Value);
            }

            if (_filter.SourceId != null)
                _paging.Where("Lead.OrderSourceId = {0}", _filter.SourceId.Value);
            
            if (!string.IsNullOrWhiteSpace(_filter.Description))
                _paging.Where("([Lead].[Description] LIKE '%'+{0}+'%')", _filter.Description);

            if (!string.IsNullOrWhiteSpace(_filter.City))
            {
                _paging.Where(
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID]) LIKE '%'+{0}+'%'",
                    _filter.City);
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
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }

            if (_filter.LeadFields != null)
            {
                foreach (var fieldFilter in _filter.LeadFields.Where(x => x.Value != null))
                {
                    var fieldsFilterModel = fieldFilter.Value;
                    if (fieldsFilterModel.DateFrom.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                   "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        _paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value like '%' + {1} + '%'))",
                            fieldFilter.Key, fieldsFilterModel.Value);
                    }
                }
            }
        }
        
        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("[Lead].Id");
                return;
            }
        }
    }
}