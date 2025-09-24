using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Handlers.Shared;
using AdvantShop.Web.Admin.Models.Crm.Leads;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeads
    {
        private LeadsFilterModel _filter;
        private readonly bool _exportToCsv;
        private readonly bool _onlyEmails;
        private SqlPaging _paging;

        public GetLeads(LeadsFilterModel filter)
        {
            _filter = filter;
        }

        public GetLeads(LeadsFilterModel filterModel, bool exportToCsv, bool onlyEmails = false) : this(filterModel)
        {
            _exportToCsv = exportToCsv;
            _onlyEmails = onlyEmails;
        }

        public LeadsFilterResult Execute()
        {
            var model = new LeadsFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            var dealStatuses = DealStatusService.GetListWithCount(_filter.SalesFunnelId);
            foreach (var dealStatus in dealStatuses)
            {
                model.LeadsCount.Add(dealStatus.Id, dealStatus.LeadsCount);
            }

            if (model.TotalPageCount < _filter.Page && _filter.Page > 1)
                return model;
            
            model.DataItems = _paging.PageItemsList<LeadsFilterResultModel>();

            model.TotalString += " " +
                                 LocalizationService.GetResourceFormat("Admin.Leads.Grid.TotalPrice",
                                     PriceFormatService.FormatPrice(
                                         _paging.GetCustomData(
                                                 $"Sum ([Lead].[Sum]/{SettingsCatalog.DefaultCurrency.Rate.ToInvariantString()}*ISNULL([LeadCurrency].[CurrencyValue],1)) as totalPrice",
                                                 "", reader => SQLDataHelper.GetFloat(reader, "totalPrice"),
                                                 true)
                                             .FirstOrDefault(),
                                         SettingsCatalog.DefaultCurrency));
            
            return model;
        }

        public List<int> GetItemsIds()
        {
            GetPaging();

            return _paging.ItemsIds<int>("[Lead].Id");
        }

        public List<Guid?> GetCustomerIds()
        {
            GetPaging();

            return _paging.ItemsIds<Guid?>("Distinct [LeadCustomer].CustomerId").Where(x => x != null).ToList();
        }
        
        public List<RecipientInfo> GetRecipients(List<int> ids, SelectModeCommand selectMode, bool isLetter)
        {
            GetPaging();

            var leads = _paging.PageItemsList<LeadsFilterResultModel>();
            
            if (ids != null && ids.Count > 0)
            {
                leads =
                    selectMode == SelectModeCommand.None
                        ? leads.Where(x => ids.Contains(x.Id)).ToList()
                        : leads.Where(x => !ids.Contains(x.Id)).ToList();
            }

            var recipients = new List<RecipientInfo>();

            foreach (var lead in leads)
            {
                var phone = 0L;
                
                if (isLetter)
                {
                    if (string.IsNullOrEmpty(lead.Email) || 
                        recipients.Find(x => x.Email.Equals(lead.Email, StringComparison.OrdinalIgnoreCase)) != null)
                        continue;
                }
                else
                {
                    if (string.IsNullOrEmpty(lead.Phone))
                        continue;

                    phone = StringHelper.ConvertToStandardPhone(lead.Phone, true, true) ?? 0;
                    
                    if (phone == 0 || recipients.Find(x => x.Phone == phone) != null)
                        continue;
                }

                recipients.Add(new RecipientInfo()
                {
                    Email = lead.Email,
                    Phone = phone,
                    CustomerId = lead.CustomerId,
                    Customer = 
                        new RecipientInfoCustomer(lead.FirstName, lead.LastName, lead.Patronymic, lead.Organization)
                });
            }

            return recipients;
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filter.ItemsPerPage,
                CurrentPageIndex = _filter.Page
            };

            if (_onlyEmails)
            {
                _paging.Select(
                    "[Lead].Id",
                    "[Lead].FirstName",
                    "[Lead].LastName",
                    "[Lead].Patronymic",
                    "[Lead].Phone",
                    "[Lead].Email",
                    "[Lead].CustomerId"
                );
            }
            else
            {
                _paging.Select(
                    "[Lead].Id",

                    "[Lead].FirstName",
                    "[Lead].LastName",
                    "[Lead].Patronymic",
                    "[Lead].Organization",

                    "[Lead].[Description]",

                    "[Lead].Phone",
                    "[Lead].Email",

                    "[Lead].CustomerId",
                    "[LeadCustomer].FirstName as CustomerFirstName",
                    "[LeadCustomer].LastName as CustomerLastName",
                    "[LeadCustomer].Patronymic as CustomerPatronymic",
                    "[LeadCustomer].Organization as CustomerOrganization",

                    "(isNull([Lead].LastName, '') + isNull([Lead].FirstName, '') + isNull([Lead].Patronymic, ''))"
                        .AsSqlField("FullName"),
                    "(isNull([LeadCustomer].LastName, '') + isNull([LeadCustomer].FirstName, '') + isNull([LeadCustomer].Patronymic, ''))"
                        .AsSqlField("CustomerFullName"),

                    "[LeadCustomer].Phone as CustomerPhone",
                    "[LeadCustomer].Email as CustomerEmail",

                    "[Lead].SalesFunnelId",
                    "[Lead].Sum",
                    "isNull((Select Sum(Price*Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)"
                        .AsSqlField("ProductsSum"),
                    "isNull((Select Sum(Amount) From [Order].[LeadItem] as items Where items.LeadId = [Lead].[Id]), 0)"
                        .AsSqlField("ProductsCount"),

                    "[LeadCurrency].CurrencyValue",
                    "[LeadCurrency].CurrencyCode",
                    "[LeadCurrency].CurrencySymbol",
                    "[LeadCurrency].IsCodeBefore",

                    "[Lead].ManagerId",
                    "[ManagerCustomer].FirstName + ' ' + [ManagerCustomer].LastName".AsSqlField("ManagerName"),
                    "[Lead].CreatedDate",
                    "[DealStatus].[Name]".AsSqlField("DealStatusName"),
                    "[DealStatus].[Color]".AsSqlField("DealStatusColor"),
                    "[Order].[OrderSource].[Name]".AsSqlField("OrderSource")
                );
            }

            if (_exportToCsv)
            {
                _paging.Select(
                    "[Lead].[Title]",
                    "(Select Top(1) [Country] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("Country"),
                    "(Select Top(1) [Zone] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("Region"),
                    "(Select Top(1) [City] From [Customers].[Contact] Where [Contact].[CustomerID] = [LeadCustomer].[CustomerID])".AsSqlField("City"),
                    "[LeadCustomer].BirthDay as CustomerBirthDay",
                    "[Lead].[Comment]",
                    "[Lead].[Discount]",
                    "[Lead].[DiscountValue]",
                    "[Lead].[ShippingCost]",
                    "[Lead].[ShippingName]"
                    );
            }

            _paging.From("[Order].[Lead]");
            _paging.Left_Join("[Customers].[Customer] as LeadCustomer on [Lead].[CustomerId] = [LeadCustomer].[CustomerId]");
            _paging.Left_Join("[Customers].[Managers] ON [Lead].[ManagerId] = [Managers].[ManagerID]");
            _paging.Left_Join("[Customers].[Customer] as ManagerCustomer ON [Managers].[CustomerId] = [ManagerCustomer].[CustomerId]");
            _paging.Left_Join("[CRM].[DealStatus] ON [DealStatus].[Id] = [Lead].[DealStatusId]");
            _paging.Left_Join("[Order].[LeadCurrency] ON [Lead].[Id] = [LeadCurrency].[LeadId]");
            _paging.Left_Join("[Order].[OrderSource] ON [Order].[OrderSource].[Id] = [Lead].[OrderSourceId]");

            Filter();
            Sorting();
        }

        private void Filter()
        {
            if (!string.IsNullOrWhiteSpace(_filter.CustomerId))
            {
                _paging.Where("Lead.CustomerId = {0}", _filter.CustomerId);
            }

            if (_filter.DealStatusId != null)
            {
                _paging.Where("DealStatusId = {0}", _filter.DealStatusId.Value);
            }
            else if (_filter.SalesFunnelId > 0)
            {
                _paging.Where("[Lead].SalesFunnelId = {0}", _filter.SalesFunnelId);
            }

            if (_filter.SalesFunnelId == 0)
            {
                _paging.Select("[SalesFunnel].[Name]".AsSqlField("SalesFunnelName"));
                _paging.Left_Join("[CRM].[SalesFunnel] On [SalesFunnel].[Id]=Lead.SalesFunnelId");
            }
            
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
            {
                _paging.Where("([LeadCustomer].Organization LIKE '%'+{0}+'%' OR [Lead].Organization LIKE '%'+{0}+'%')", _filter.Organization);
            }

            DateTime dateFrom, dateTo;
            if (_filter.CreatedDateFrom != null && DateTime.TryParse(_filter.CreatedDateFrom, out dateFrom))
            {
                _paging.Where("Lead.CreatedDate >= {0}", dateFrom);
            }
            if (_filter.CreatedDateTo != null && DateTime.TryParse(_filter.CreatedDateTo, out dateTo))
            {
                _paging.Where("Lead.CreatedDate <= {0}", dateTo);
            }
            
            if (_filter.SumFrom != null)
            {
                _paging.Where("Lead.Sum >= {0}", _filter.SumFrom.Value);
            }
            if (_filter.SumTo != null)
            {
                _paging.Where("Lead.Sum <= {0}", _filter.SumTo.Value);
            }

            if (_filter.ManagerId.HasValue)
            {
                if (_filter.ManagerId.Value == -1)
                {
                    _paging.Where("[Lead].ManagerId IS NULL");
                }
                else
                {
                    _paging.Where("[Lead].ManagerId = {0}", _filter.ManagerId.Value);
                }
            }

            if (_filter.OrderSourceId != null)
            {
                _paging.Where("Lead.OrderSourceId = {0}", _filter.OrderSourceId.Value);
            }

            if (_filter.FunnelId != null)
            {
                _paging.Where("Lead.SalesFunnelId = {0}", _filter.FunnelId.Value);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Description))
            {
                _paging.Where("([Lead].[Description] LIKE '%'+{0}+'%')", _filter.Description);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned)
                    {
                        _paging.Where("Lead.ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree)
                    {
                        _paging.Where("(Lead.ManagerId = {0} or Lead.ManagerId is null)", manager.ManagerId);
                    }

                    _paging.Where(
                        "(Exists ( " +
                        "Select 1 From [CRM].[SalesFunnel_Manager] " +
                        "Where (SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId and SalesFunnel_Manager.ManagerId = {0}) " +
                        ") OR " +
                        "(Select Count(*) From [CRM].[SalesFunnel_Manager] Where SalesFunnel_Manager.SalesFunnelId = Lead.SalesFunnelId) = 0)",
                        manager.ManagerId);
                }
            }

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
            
            if (!string.IsNullOrEmpty(_filter.ProductNameArtNo))
            {
                _paging.Where(
                    "Exists(Select 1 From [Order].[LeadItem] as item " +
                    "Where item.[LeadId] = [Lead].[Id] and (item.ArtNo LIKE '%'+{0}+'%' OR item.Name LIKE '%'+{0}+'%'))",
                    _filter.ProductNameArtNo);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filter.Sorting) || _filter.SortingType == FilterSortingType.None)
            {
                _paging.OrderByDesc("[Lead].Id");
                return;
            }

            string sorting = _filter.Sorting.ToLower().Replace("formatted", "");
            
            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filter.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(field);
                }
                else
                {
                    _paging.OrderByDesc(field);
                }
            }
        }
    }
}
