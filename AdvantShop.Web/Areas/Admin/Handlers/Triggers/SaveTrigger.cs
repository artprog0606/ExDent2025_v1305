using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Web.Admin.Models.Triggers;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Triggers.Customers;
using AdvantShop.Core.Services.Triggers.Leads;
using AdvantShop.Core.Services.Triggers.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class SaveTrigger : ICommandHandler<SaveTriggerResult>
    {
        private readonly TriggerModel _model;

        public SaveTrigger(TriggerModel model)
        {
            _model = model;
        }

        public SaveTriggerResult Execute()
        {
            TriggerRule trigger;
            var subname = "";

            switch (_model.EventType)
            {
                case ETriggerEventType.OrderCreated:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<OrderCreatedTriggerRule>(_model.Id) : new OrderCreatedTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<OrderCreatedTriggerRule>(_model.FilterSerialized);

                    ValidateOrderFilter(trigger.Filter as OrderFilter);
                    break;

                case ETriggerEventType.OrderStatusChanged:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<OrderStatusChangedTriggerRule>(_model.Id) : new OrderStatusChangedTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<OrderStatusChangedTriggerRule>(_model.FilterSerialized);

                    ValidateOrderFilter((OrderFilter)trigger.Filter);

                    if (_model.EventObjId.HasValue)
                    {
                        var status = OrderStatusService.GetOrderStatus(_model.EventObjId.Value);
                        if (status == null)
                            throw new BlException(LocalizationService.GetResource("Admin.BizProcessRules.Validate.EventObjectNotSet"));

                        subname = " на " + status.StatusName;
                    }
                    break;

                case ETriggerEventType.OrderPaied:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<OrderPayTriggerRule>(_model.Id) : new OrderPayTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<OrderPayTriggerRule>(_model.FilterSerialized);

                    ValidateOrderFilter((OrderFilter)trigger.Filter);
                    break;

                case ETriggerEventType.LeadCreated:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<LeadCreatedTriggerRule>(_model.Id) : new LeadCreatedTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<LeadCreatedTriggerRule>(_model.FilterSerialized);

                    ValidateLeadFilter(trigger.Filter as LeadFilter);
                    break;

                case ETriggerEventType.LeadStatusChanged:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<LeadStatusChangedTriggerRule>(_model.Id) : new LeadStatusChangedTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<LeadStatusChangedTriggerRule>(_model.FilterSerialized);

                    ValidateLeadFilter((LeadFilter)trigger.Filter);

                    if (_model.EventObjId.HasValue)
                    {
                        var status = DealStatusService.Get(_model.EventObjId.Value);
                        if (status == null)
                            throw new BlException(LocalizationService.GetResource("Admin.BizProcessRules.Validate.EventObjectNotSet"));

                        subname = " на " + status.Name;
                    }
                    break;

                case ETriggerEventType.CustomerCreated:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<CustomerCreatedTriggerRule>(_model.Id) : new CustomerCreatedTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<CustomerCreatedTriggerRule>(_model.FilterSerialized);
                    break;

                case ETriggerEventType.TimeFromLastOrder:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<TimeFromLastOrderTriggerRule>(_model.Id) : new TimeFromLastOrderTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<TimeFromLastOrderTriggerRule>(_model.FilterSerialized);

                    if (_model.EventObjValue != null)
                        subname = " " + _model.EventObjValue + " дн.";
                    break;

                case ETriggerEventType.SignificantDate:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<SignificantDateTriggerRule>(_model.Id) : new SignificantDateTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<SignificantDateTriggerRule>(_model.FilterSerialized);
                    
                    var triggerParamsDate = !string.IsNullOrEmpty(_model.TriggerParamsSerialized)
                        ? JsonConvert.DeserializeObject<TriggerParamsDate>(_model.TriggerParamsSerialized)
                        : null;

                    if (triggerParamsDate == null || triggerParamsDate.DateTime == DateTime.MinValue)
                        throw new BlException("Укажите дату срабатывания триггера");

                    trigger.TriggerParams = triggerParamsDate;
                    subname = " " + triggerParamsDate.DateTime.ToString(!triggerParamsDate.IgnoreYear ? "dd MMMM yyyy" : "dd MMMM");
                    break;

                case ETriggerEventType.SignificantCustomerDate:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<SignificantCustomerDateTriggerRule>(_model.Id) : new SignificantCustomerDateTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<SignificantCustomerDateTriggerRule>(_model.FilterSerialized);
                    
                    var triggerParamsCustomerDate = !string.IsNullOrEmpty(_model.TriggerParamsSerialized)
                        ? JsonConvert.DeserializeObject<TriggerParamsDaysBeforeDate>(_model.TriggerParamsSerialized)
                        : null;

                    if (triggerParamsCustomerDate == null)
                        throw new BlException("Укажите данные");

                    triggerParamsCustomerDate.IsCustomField = !string.IsNullOrEmpty(triggerParamsCustomerDate.CustomFieldId) && triggerParamsCustomerDate.CustomFieldId != "0";
                    trigger.TriggerParams = triggerParamsCustomerDate;
                    break;

                case ETriggerEventType.InstallMobileApp:
                    trigger = _model.Id != 0 ? TriggerRuleService.Get<InstallMobileAppTriggerRule>(_model.Id) : new InstallMobileAppTriggerRule();
                    trigger.Filter = TriggerRuleService.GetTriggerFilterFromJson<InstallMobileAppTriggerRule>(_model.FilterSerialized);
                    break;
                default:
                    throw new BlException("Wrong type " + _model.EventType);
            }

            trigger.EventObjId = _model.EventObjId;
            trigger.EventObjValue = _model.EventObjValue;
            trigger.WorksOnlyOnce = _model.WorksOnlyOnce;
            trigger.PreferredHour = _model.PreferredHour;

            trigger.CategoryId = _model.CategoryId.HasValue && _model.CategoryId.Value > 0 ? _model.CategoryId : null;
            
            trigger.Name = !string.IsNullOrWhiteSpace(_model.Name)
                ? _model.Name
                : trigger.EventType.Localize() + subname;

            if (_model.Actions == null)
                _model.Actions = new List<TriggerAction>();


            if (trigger.Id == 0)
            {
                trigger.Enabled = true;

                TriggerRuleService.Add(trigger);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_TriggerCreated, trigger.EventType.ToString());

                var i = 0;
                foreach (var action in _model.Actions)
                {
                    action.TriggerRuleId = trigger.Id;
                    action.SortOrder = i * 10;

                    TriggerActionService.Add(action);

                    i++;
                }
            }
            else
            {
                TriggerRuleService.Update(trigger);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_EditTrigger);

                var prevActions = trigger.Actions;

                for (var i = 0; i < _model.Actions.Count; i++)
                {
                    _model.Actions[i].SortOrder = i * 10;
                }

                foreach (var action in _model.Actions)
                {
                    action.TriggerRuleId = trigger.Id;

                    if (prevActions.Find(x => x.Id == action.Id) == null)
                    {
                        TriggerActionService.Add(action);
                    }
                    else
                    {
                        TriggerActionService.Update(action);
                    }
                }

                foreach (var action in prevActions.Where(x => !_model.Actions.Any(a => a.Id == x.Id)))
                {
                    TriggerActionService.Delete(action.Id);
                }
            }

            return new SaveTriggerResult() { Id = trigger.Id};
        }
        

        private bool ValidateOrderFilter(OrderFilter filter)
        {
            var errors = new List<string>();
            foreach (var comparer in filter.Comparers)
            {
                string error;
                if (!ValidateFieldComparer(comparer.FieldComparer, comparer.FieldType, comparer.FieldType == EOrderFieldType.CustomerField, comparer.FieldName, out error))
                    errors.Add(error);
            }
            if (errors.Any())
                throw new BlException(LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.OrderFilterErrors", errors.AggregateString(", ")));

            return !errors.Any();
        }

        private bool ValidateLeadFilter(LeadFilter filter)
        {
            var errors = new List<string>();
            foreach (var comparer in filter.Comparers)
            {
                string error;
                if (!ValidateFieldComparer(comparer.FieldComparer, comparer.FieldType, comparer.FieldType == ELeadFieldType.CustomerField, comparer.FieldName, out error))
                    errors.Add(error);
            }
            if (errors.Any())
                throw new BlException(LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.LeadFilterErrors", errors.AggregateString(", ")));

            return !errors.Any();
        }

        private bool ValidateFieldComparer(FieldComparer fieldComparer, Enum bizObjectFieldType, bool isCustomerField, string fieldName, out string error)
        {
            var fieldType = BizProcessRuleService.GetFieldType(bizObjectFieldType, isCustomerField, fieldComparer.FieldObjId);
            error = null;
            switch (fieldComparer.Type)
            {
                case EFieldComparerType.Equal:
                    var equalityComparer = fieldComparer as FieldEqualityComparer;
                    
                    // нужно разрешать пустые значения
                    //if (equalityComparer.Value.IsNullOrEmpty()) 
                    //    error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);

                    //if ((fieldType == EFieldType.Date || fieldType == EFieldType.Datetime) && !equalityComparer.Value.TryParseDateTime(true).HasValue)
                    //    error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.WrongDate", fieldName);
                    //else if (fieldType == EFieldType.Number && !equalityComparer.Value.TryParseFloat(true).HasValue)
                    //    error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.WrongNumber", fieldName);
                    break;

                case EFieldComparerType.Range:
                    var rangeComparer = fieldComparer as FieldRangeComparer;
                    if ((fieldType == EFieldType.Date || fieldType == EFieldType.Datetime || fieldType == EFieldType.Time) && !rangeComparer.DateFrom.HasValue && !rangeComparer.DateTo.HasValue)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoDateRange", fieldName);
                    else if ((fieldType != EFieldType.Date && fieldType != EFieldType.Datetime && fieldType != EFieldType.Time) && !rangeComparer.From.HasValue && !rangeComparer.To.HasValue)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoRange", fieldName);
                    break;

                case EFieldComparerType.Flag:
                    break;

                case EFieldComparerType.Contains:
                    var containsComparer = fieldComparer as FieldContainsComparer;
                    if (containsComparer.Value.IsNullOrEmpty())
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.Products:
                    var productsComparer = fieldComparer as FieldsProductsComparer;
                    if (productsComparer == null || productsComparer.Products == null || productsComparer.Products.Count == 0)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.Categories:
                    var categoriesComparer = fieldComparer as FieldsCategoriesComparer;
                    if (categoriesComparer == null || categoriesComparer.Categories == null || categoriesComparer.Categories.Count == 0)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.CustomerSegment:
                    var segmentComparer = fieldComparer as FieldsCustomerSegmentComparer;
                    if (segmentComparer == null || segmentComparer.Value == 0)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.OrdersPaidSum:
                case EFieldComparerType.OrdersPaidCount:
                case EFieldComparerType.OrdersCount:
                    var ordersPaiedSumComparer = fieldComparer as FieldsOrdersCustomerSumCountComparer;
                    if (ordersPaiedSumComparer == null || (!ordersPaiedSumComparer.From.HasValue && !ordersPaiedSumComparer.To.HasValue))
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.OpenLeadsInFunnel:
                    var openLeadsComparer = fieldComparer as FieldsOpenLeadsInFunnelComparer;
                    if (openLeadsComparer == null || openLeadsComparer.SalesFunnelId == 0)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                case EFieldComparerType.DealStatus:
                    var dealStatusComparer = fieldComparer as FieldsDealStatusComparer;
                    if (dealStatusComparer == null || dealStatusComparer.SalesFunnelId == 0)
                        error = LocalizationService.GetResourceFormat("Admin.BizProcessRules.Validate.FieldComparer.NoValue", fieldName);
                    break;

                    //default:
                    //    throw new NotImplementedException("No implementation for FieldComparerType " + fieldComparer.Type);
            }
            return error.IsNullOrEmpty();
        }
    }

    public class SaveTriggerResult
    {
        public int Id { get; set; }
    }

}
