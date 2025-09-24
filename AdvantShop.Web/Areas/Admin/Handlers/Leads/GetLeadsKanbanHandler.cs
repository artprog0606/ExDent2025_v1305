using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Handlers.Shared;
using AdvantShop.Web.Admin.Models.Crm.Leads;
using AdvantShop.Web.Admin.Models.Customers;
using AdvantShop.Web.Admin.Models.Shared.Smses;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class GetLeadsKanbanHandler
    {
        private readonly LeadsKanbanFilterModel _filter;
        private readonly bool _onlyCustomerIds;
        private readonly bool _onlyEmails;
        private readonly List<DealStatus> _dealStatuses;

        public GetLeadsKanbanHandler(LeadsKanbanFilterModel filterModel, bool onlyCustomerIds = false, bool onlyEmails = false)
        {
            _filter = filterModel;
            _onlyCustomerIds = onlyCustomerIds;
            _onlyEmails = onlyEmails;
            _dealStatuses = DealStatusService.GetList(_filter.SalesFunnelId);
        }

        public LeadsKanbanModel Execute()
        {
            var model = new LeadsKanbanModel {Name = "Kanban"};

            var funnel = SalesFunnelService.Get(_filter.SalesFunnelId);
            if (funnel != null && !SalesFunnelService.CheckAccess(funnel))
                return model;

            var statuses =
                _filter.StatusId == null
                    ? _dealStatuses.Where(x => x.Status == SalesFunnelStatusType.None).ToList()
                    : _dealStatuses.Where(x => x.Id == _filter.StatusId.Value).ToList();

            var columns = new List<LeadsKanbanColumnFilterModel>();
            foreach (var status in statuses)
            {
                var page = 1;
                var column = _filter.Columns.FirstOrDefault(x => x.Id == status.Id.ToString());
                if (column != null)
                    page = column.Page;

                columns.Add(new LeadsKanbanColumnFilterModel(status.Id.ToString()) { Page = page});
            }

            _filter.Columns = columns;

            foreach (var filterColumn in _filter.Columns)
            {
                var paging = GetPaging(filterColumn);
                if (paging == null)
                    continue;

                var dealStatus = _dealStatuses.FirstOrDefault(x => x.Id.ToString() == filterColumn.Id);
                var column = new LeadsKanbanColumnModel
                {
                    Id = filterColumn.Id,
                    Name = dealStatus != null ? dealStatus.Name : string.Empty,
                    DealStatusId = dealStatus?.Id,
                    Page = filterColumn.Page,
                    CardsPerColumn = filterColumn.CardsPerColumn,
                    TotalCardsCount = paging.TotalRowsCount,
                    TotalPagesCount = paging.PageCount(paging.TotalRowsCount, filterColumn.CardsPerColumn),
                };
                
                if (dealStatus != null && dealStatus.Color.IsNotEmpty())
                {
                    column.HeaderStyle.Add("color", "#" + dealStatus.Color);
                    column.CardStyle.Add("border-top-color", "#" + dealStatus.Color);
                }

                if (column.TotalPagesCount >= filterColumn.Page || filterColumn.Page == 1)
                    column.Cards = paging.PageItemsList<LeadKanbanModel>();

                column.TotalString = PriceFormatService.FormatPrice(column.Cards.Sum(x => x.SumTotal),
                                                                    CurrencyService.CurrentCurrency, true);

                model.Columns.Add(column);
            }

            var canceledDealStatus = _dealStatuses.FirstOrDefault(x => x.Status == SalesFunnelStatusType.Canceled);

            if (!statuses.Any(x => x.Status == SalesFunnelStatusType.Canceled || x.Status == SalesFunnelStatusType.FinalSuccess))
            {
                model.Columns.Add(new LeadsKanbanColumnModel
                {
                    Id = "CompleteLead",
                    DealStatusId = canceledDealStatus != null ? canceledDealStatus.Id : default(int?),
                    Name = "Завершить лид",
                    CardsPerColumn = 0,
                    Class = "leads-kanban-column-complete",
                    HeaderStyle = new Dictionary<string, string>() {{"color", "#676a6c"}}
                });
            }

            return model;
        }

        public List<LeadKanbanModel> GetCards()
        {
            var result = new List<LeadKanbanModel>();
            if (_filter.ColumnId.IsNullOrEmpty() || !_filter.Columns.Any(x => x.Id == _filter.ColumnId))
                return result;

            var paging = GetPaging(_filter.Columns.FirstOrDefault(x => x.Id == _filter.ColumnId), false);

            return paging != null ? paging.PageItemsList<LeadKanbanModel>() : new List<LeadKanbanModel>();
        }

        public List<Guid> GetCustomerIds()
        {
            var customerIds = new List<Guid>();
            var result = Execute();

            if (result != null && result.Columns != null)
            {
                foreach (var column in result.Columns)
                    foreach (var customerId in column.Cards.Select(x => x.CustomerId).Where(x => x != null))
                    {
                        if (!customerIds.Contains(customerId.Value))
                            customerIds.Add(customerId.Value);
                    }
            }

            return customerIds;
        }
        
        public List<RecipientInfo> GetRecipients(bool isLetter)
        {
            var recipients = new List<RecipientInfo>();
            var result = Execute();

            if (result != null && result.Columns != null)
            {
                foreach (var column in result.Columns)
                foreach (var card in column.Cards)
                {
                    var email = card.Email;
                    var phone = 0L;

                    if (isLetter)
                    {
                        if (string.IsNullOrEmpty(email) || 
                            recipients.Find(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) != null)
                            continue;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(card.Phone))
                            continue;
                        
                        phone = StringHelper.ConvertToStandardPhone(card.Phone, true, true) ?? 0;
                        
                        if (phone == 0 || recipients.Find(x => x.Phone == phone) != null)
                            continue;
                    }

                    recipients.Add(new RecipientInfo()
                    {
                        Email = email,
                        Phone = phone,
                        CustomerId = card.CustomerId,
                        Customer = new RecipientInfoCustomer(card.FirstName, card.LastName, card.Patronymic, card.Organization)
                    });
                }
            }

            return recipients;
        }
        
        public List<int> GetLeadIds()
        {
            var ids = new List<int>();

            var result = Execute();

            if(result != null && result.Columns != null)
            {
                foreach (var column in result.Columns)
                    foreach (var id in column.Cards.Select(x => x.Id))
                    {
                        if (!ids.Contains(id))
                            ids.Add(id);
                    }
            }

            return ids;
        }

        private SqlPaging GetPaging(LeadsKanbanColumnFilterModel columnFilter, bool allCards = true)
        {
            var id = columnFilter.Id.TryParseInt(true);
            if (!id.HasValue)
                return null;

            if (_filter.StatusId != null && _filter.StatusId != id)
                return null;

            var paging = new SqlPaging()
            {
                ItemsPerPage =
                    _onlyCustomerIds || _onlyEmails
                        ? 1000000
                        : (allCards ? columnFilter.CardsPerColumn*columnFilter.Page : columnFilter.CardsPerColumn),
                CurrentPageIndex = allCards ? 1 : columnFilter.Page
            };

            if (_onlyCustomerIds)
            {
                paging.Select("[LeadCustomer].CustomerId");
            }
            else if (_onlyEmails)
            {
                paging.Select(
                    "[Lead].Id",
                    "[Lead].FirstName",
                    "[Lead].LastName",
                    "[Lead].Patronymic",
                    "[Lead].Phone",
                    "[Lead].Email",
                    "[Lead].Organization",
                    "[Lead].CustomerId"
                );
            }
            else
            {
                paging.Select(
                    "[Lead].Id",

                    "[Lead].FirstName",
                    "[Lead].LastName",
                    "[Lead].Patronymic",
                    "[Lead].Phone",
                    "[Lead].Email",
                    "[Lead].Organization",

                    "[Lead].CustomerId",
                    "[LeadCustomer].FirstName as CustomerFirstName",
                    "[LeadCustomer].LastName as CustomerLastName",
                    "[LeadCustomer].Patronymic as CustomerPatronymic",
                    "[LeadCustomer].Phone as CustomerPhone",
                    "[LeadCustomer].Email as CustomerEmail",
                    "[LeadCustomer].Organization as CustomerOrganization",

                    "[Lead].SalesFunnelId",
                    "[Lead].Sum",
                    "[Lead].Description",
                    "[CRM].[LeadItemsToString]([Lead].Id, ', ')".AsSqlField("LeadItemsString"),

                    "[Lead].ManagerId",
                    "[ManagerCustomer].FirstName + ' ' + [ManagerCustomer].LastName".AsSqlField("ManagerName"),
                    "ManagerCustomer.Avatar".AsSqlField("ManagerAvatar"),
                    "[Lead].CreatedDate",
                    
                    "[LeadCurrency].[CurrencyValue]",
                    "[LeadCurrency].[CurrencySymbol]",
                    "[LeadCurrency].[IsCodeBefore]",
                    "[LeadCurrency].[RoundNumbers]",
                    "[LeadCurrency].[EnablePriceRounding]",

                    "[OrderSource].[Name]".AsSqlField("OrderSourceName")
                );
            }

            paging
                .From("[Order].[Lead]")
                .Left_Join("[Customers].[Customer] as LeadCustomer on [Lead].[CustomerId] = [LeadCustomer].[CustomerId]")
                .Left_Join("[Customers].[Managers] ON [Lead].[ManagerId] = [Managers].[ManagerID]")
                .Left_Join("[Customers].[Customer] as ManagerCustomer ON [Managers].[CustomerId] = [ManagerCustomer].[CustomerId]")
                .Left_Join("[Order].[OrderSource] ON [OrderSource].[Id] = [Lead].[OrderSourceId]")
                .Left_Join("[Order].[LeadCurrency] ON [LeadCurrency].[LeadId] = [Lead].[Id]");

            Sorting(paging);
            Filter(paging, columnFilter);
            
            return paging;
        }

        private void Filter(SqlPaging paging, LeadsKanbanColumnFilterModel columnFilter)
        {
            paging.Where("DealStatusId = {0}", columnFilter.Id.TryParseInt());

            if (!string.IsNullOrWhiteSpace(_filter.CustomerId))
            {
                paging.Where("Lead.CustomerId = {0}", _filter.CustomerId);
            }

            if (!string.IsNullOrWhiteSpace(_filter.Search))
            {
                paging.Where(
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
                paging.Where(
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
                paging.Where("([LeadCustomer].Organization LIKE '%'+{0}+'%' OR [Lead].Organization LIKE '%'+{0}+'%')", _filter.Organization);
            }

            DateTime dateFrom, dateTo;
            if (_filter.CreatedDateFrom != null && DateTime.TryParse(_filter.CreatedDateFrom, out dateFrom))
            {
                paging.Where("Lead.CreatedDate >= {0}", dateFrom);
            }
            if (_filter.CreatedDateTo != null && DateTime.TryParse(_filter.CreatedDateTo, out dateTo))
            {
                paging.Where("Lead.CreatedDate <= {0}", dateTo);
            }

            if (_filter.SumFrom != null)
            {
                paging.Where("Lead.Sum >= {0}", _filter.SumFrom.Value);
            }
            if (_filter.SumTo != null)
            {
                paging.Where("Lead.Sum <= {0}", _filter.SumTo.Value);
            }

            if (_filter.ManagerId.HasValue)
            {
                paging.Where("[Lead].ManagerId = {0}", _filter.ManagerId.Value);
            }

            if (_filter.OrderSourceId != null)
            {
                paging.Where("Lead.OrderSourceId = {0}", _filter.OrderSourceId.Value);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.Assigned)
                    {
                        paging.Where("Lead.ManagerId = {0}", manager.ManagerId);
                    }
                    else if (SettingsManager.ManagersLeadConstraint == ManagersLeadConstraint.AssignedAndFree)
                    {
                        paging.Where("(Lead.ManagerId = {0} or Lead.ManagerId is null)", manager.ManagerId);
                    }

                    paging.Where(
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
                paging.Where(
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
                        paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                   "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value is not null and CustomerFieldValuesMap.Value <> '' and CustomerFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        paging.Where(
                            "Exists(Select 1 " +
                            "From Customers.CustomerFieldValuesMap " +
                            "Where CustomerFieldValuesMap.CustomerId = [LeadCustomer].[CustomerId] and " +
                                "(CustomerFieldValuesMap.CustomerFieldId = {0} and CustomerFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        paging.Where(
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
                        paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                   "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateFrom.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.DateTo.HasValue)
                    {
                        paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, fieldsFilterModel.DateTo.Value.ToString("yyyy-MM-dd"));
                    }

                    if (fieldsFilterModel.From.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.From.TryParseInt(true);
                        paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value >= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.To.IsNotEmpty())
                    {
                        var value = fieldsFilterModel.To.TryParseInt(true);
                        paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value is not null and LeadFieldValuesMap.Value <> '' and LeadFieldValuesMap.Value <= {1}))",
                            fieldFilter.Key, value ?? Int32.MaxValue);
                    }

                    if (fieldsFilterModel.ValueExact.IsNotEmpty())
                    {
                        paging.Where(
                            "Exists(Select 1 " +
                            "From CRM.LeadFieldValuesMap " +
                            "Where LeadFieldValuesMap.LeadId = [Lead].Id and " +
                                "(LeadFieldValuesMap.LeadFieldId = {0} and LeadFieldValuesMap.Value = {1}))",
                            fieldFilter.Key, fieldsFilterModel.ValueExact);
                    }

                    if (fieldsFilterModel.Value.IsNotEmpty())
                    {
                        paging.Where(
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
                paging.Where(
                    "Exists(Select 1 From [Order].[LeadItem] as item " +
                    "Where item.[LeadId] = [Lead].[Id] and (item.ArtNo LIKE '%'+{0}+'%' OR item.Name LIKE '%'+{0}+'%'))",
                    _filter.ProductNameArtNo);
            }
        }

        private void Sorting(SqlPaging paging)
        {
            paging.OrderBy("[Lead].SortOrder");
        }
    }
}