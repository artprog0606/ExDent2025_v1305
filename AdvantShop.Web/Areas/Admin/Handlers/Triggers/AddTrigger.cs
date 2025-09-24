using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.Services.Triggers.Customers;
using AdvantShop.Core.Services.Triggers.Leads;
using AdvantShop.Core.Services.Triggers.Orders;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Triggers;
using AdvantShop.Web.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class AddTrigger : ICommandHandler<SaveTriggerResult>
    {
        private readonly TriggerModel _model;

        public AddTrigger(TriggerModel model)
        {
            _model = model;
        }

        public SaveTriggerResult Execute()
        {
            TriggerRule trigger;
            var subname = "";

            var orderFieldComparers = new List<OrderFieldComparer>();
            var leadFieldComparers = new List<LeadFieldComparer>();
            if (_model.OrderSourceId.HasValue)
            {
                var orderSource = OrderSourceService.GetOrderSource(_model.OrderSourceId.Value);
                if (orderSource != null)
                {
                    orderFieldComparers.Add(new OrderFieldComparer
                    {
                        FieldType = EOrderFieldType.OrderSource,
                        FieldComparer = new FieldEqualityComparer
                        {
                            Value = orderSource.Id.ToString(),
                            ValueObjId = orderSource.Id
                        }
                    });
                    leadFieldComparers.Add(new LeadFieldComparer
                    {
                        FieldType = ELeadFieldType.Source,
                        FieldComparer = new FieldEqualityComparer
                        {
                            Value = orderSource.Id.ToString(),
                            ValueObjId = orderSource.Id
                        }
                    });
                }
            }

            switch (_model.EventType)
            {
                case ETriggerEventType.OrderCreated:
                    trigger = new OrderCreatedTriggerRule();
                    if (orderFieldComparers.Any())
                        trigger.Filter = new OrderFilter { Comparers = orderFieldComparers };
                    break;

                case ETriggerEventType.OrderStatusChanged:
                    trigger = new OrderStatusChangedTriggerRule();

                    if (_model.EventObjId.HasValue)
                    {
                        var status = OrderStatusService.GetOrderStatus(_model.EventObjId.Value);
                        if (status == null)
                            throw new BlException(LocalizationService.GetResource("Admin.BizProcessRules.Validate.EventObjectNotSet"));

                        subname = " на " + status.StatusName;

                        orderFieldComparers.Add(new OrderFieldComparer()
                        {
                            CompareType = BizObjectFieldCompareType.Equal,
                            FieldType = EOrderFieldType.OrderStatus,
                            FieldComparer = new FieldEqualityComparer()
                            {
                                Value = status.StatusID.ToString(),
                                ValueObjId = status.StatusID
                            },
                        });
                        _model.EventObjId = null;
                    }
                    if (orderFieldComparers.Any())
                        trigger.Filter = new OrderFilter { Comparers = orderFieldComparers };
                    break;

                case ETriggerEventType.OrderPaied:
                    trigger = new OrderPayTriggerRule();
                    if (orderFieldComparers.Any())
                        trigger.Filter = new OrderFilter { Comparers = orderFieldComparers };
                    break;

                case ETriggerEventType.LeadCreated:
                    trigger = new LeadCreatedTriggerRule();
                    if (leadFieldComparers.Any())
                        trigger.Filter = new LeadFilter { Comparers = leadFieldComparers };
                    break;

                case ETriggerEventType.LeadStatusChanged:
                    trigger = new LeadStatusChangedTriggerRule();

                    if (_model.EventObjId.HasValue)
                    {
                        var status = DealStatusService.Get(_model.EventObjId.Value);
                        if (status == null)
                            throw new BlException(LocalizationService.GetResource("Admin.BizProcessRules.Validate.EventObjectNotSet"));

                        var salesFunnel = SalesFunnelService.GetByDealStatus(status.Id);

                        subname = " на " + status.Name;

                        leadFieldComparers.Add(new LeadFieldComparer()
                        {
                            CompareType = BizObjectFieldCompareType.Equal,
                            FieldType = ELeadFieldType.DealStatus,
                            FieldComparer = new FieldsDealStatusComparer
                            {
                                SalesFunnelId = salesFunnel != null ? salesFunnel.Id : 0,
                                DealStatusId = status.Id
                            }
                        });
                        _model.EventObjId = null;
                    }
                    if (leadFieldComparers.Any())
                        trigger.Filter = new LeadFilter { Comparers = leadFieldComparers };
                    break;

                case ETriggerEventType.CustomerCreated:
                    trigger = new CustomerCreatedTriggerRule();
                    break;

                case ETriggerEventType.TimeFromLastOrder:
                    trigger = new TimeFromLastOrderTriggerRule();

                    if (_model.EventObjValue != null)
                        subname = " " + _model.EventObjValue + " дн.";
                    break;

                case ETriggerEventType.SignificantDate:
                    trigger = new SignificantDateTriggerRule();
                    
                    var triggerParamsDate = !string.IsNullOrEmpty(_model.TriggerParamsSerialized)
                        ? JsonConvert.DeserializeObject<TriggerParamsDate>(_model.TriggerParamsSerialized)
                        : null;

                    if (triggerParamsDate == null || triggerParamsDate.DateTime == DateTime.MinValue)
                        throw new BlException("Укажите дату срабатывания триггера");

                    trigger.TriggerParams = triggerParamsDate;
                    subname = " " + triggerParamsDate.DateTime.ToString(!triggerParamsDate.IgnoreYear ? "dd MMMM yyyy" : "dd MMMM");
                    break;

                case ETriggerEventType.SignificantCustomerDate:
                    trigger = new SignificantCustomerDateTriggerRule();
                    
                    var triggerParamsCustomerDate = !string.IsNullOrEmpty(_model.TriggerParamsSerialized)
                        ? JsonConvert.DeserializeObject<TriggerParamsDaysBeforeDate>(_model.TriggerParamsSerialized)
                        : null;

                    if (triggerParamsCustomerDate == null)
                        throw new BlException("Укажите данные");

                    triggerParamsCustomerDate.IsCustomField = !string.IsNullOrEmpty(triggerParamsCustomerDate.CustomFieldId) && triggerParamsCustomerDate.CustomFieldId != "0";
                    trigger.TriggerParams = triggerParamsCustomerDate;
                    break;

                case ETriggerEventType.InstallMobileApp:
                    trigger = new InstallMobileAppTriggerRule();
                    break;

                default:
                    throw new BlException("Wrong type " + _model.EventType);
            }

            trigger.EventObjId = _model.EventObjId;
            trigger.EventObjValue = _model.EventObjValue;
            trigger.WorksOnlyOnce = _model.WorksOnlyOnce;
            trigger.PreferredHour = _model.PreferredHour;

            trigger.CategoryId = _model.CategoryId.HasValue && _model.CategoryId.Value > 0 ? _model.CategoryId : null;
            
            trigger.Name = !string.IsNullOrWhiteSpace(_model.Name) ? _model.Name : trigger.EventType.Localize() + subname;

            trigger.Enabled = true;

            TriggerRuleService.Add(trigger);
            Track.TrackService.TrackEvent(Track.ETrackEvent.Triggers_TriggerCreated, trigger.EventType.ToString());

            var action = new TriggerAction() {TriggerRuleId = trigger.Id, ActionType = ETriggerActionType.Email};
            TriggerActionService.Add(action);

            return new SaveTriggerResult() { Id = trigger.Id};
        }
        
    }
}
