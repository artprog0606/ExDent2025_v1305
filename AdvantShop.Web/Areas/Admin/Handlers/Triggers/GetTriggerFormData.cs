using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Core.Services.Triggers.Customers;
using AdvantShop.Core.Services.Triggers.Leads;
using AdvantShop.Core.Services.Triggers.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Triggers;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class GetTriggerFormData
    {
        private readonly ETriggerEventType? _eventType;
        private readonly ETriggerObjectType[] _objectTypes;

        public GetTriggerFormData(ETriggerEventType? eventType, ETriggerObjectType[] objectTypes)
        {
            _eventType = eventType;
            _objectTypes = objectTypes;
        }

        public TriggerDataModel Execute()
        {
            var model = new TriggerDataModel(_objectTypes);
            var eventType = !_eventType.HasValue || _eventType.Value == ETriggerEventType.None
                ? model.EventTypes[0].EventType
                : _eventType.Value;

            TriggerRule trigger = null;

            switch (eventType)
            {
                case ETriggerEventType.OrderCreated:
                    trigger = new OrderCreatedTriggerRule();
                    break;

                case ETriggerEventType.OrderStatusChanged:
                    trigger = new OrderStatusChangedTriggerRule();
                    model.EventObjects = OrderStatusService.GetOrderStatuses().Select(x => new SelectItemModel(x.StatusName, x.StatusID)).ToList();
                    break;

                case ETriggerEventType.OrderPaied:
                    trigger = new OrderPayTriggerRule();
                    break;

                case ETriggerEventType.LeadCreated:
                    trigger = new LeadCreatedTriggerRule();
                    break;

                case ETriggerEventType.LeadStatusChanged:
                    trigger = new LeadStatusChangedTriggerRule();
                    var salesFunnels = SalesFunnelService.GetList();
                    model.EventObjectGroups = salesFunnels.Select(x => new SelectItemModel(x.Name, x.Id.ToString())).ToList();
                    model.EventObjectsFetchUrl = "salesFunnels/getDealStatuses?salesFunnelId=";
                    if (salesFunnels.Any())
                        model.EventObjects = DealStatusService.GetList(salesFunnels[0].Id).Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
                    break;

                case ETriggerEventType.CustomerCreated:
                    trigger = new CustomerCreatedTriggerRule();
                    break;

                case ETriggerEventType.TimeFromLastOrder:
                    trigger = new TimeFromLastOrderTriggerRule();
                    break;

                case ETriggerEventType.SignificantDate:
                    trigger = new SignificantDateTriggerRule();
                    break;

                case ETriggerEventType.SignificantCustomerDate:
                    trigger = new SignificantCustomerDateTriggerRule();
                    model.EventObjectGroups = GetEventObjectGroupsForSignificantCustomerDate();
                    break;

                case ETriggerEventType.InstallMobileApp:
                    trigger = new InstallMobileAppTriggerRule();
                    break;

                default:
                    throw new NotImplementedException("No implementation for event type " + eventType);
            }

            model.IntervalTypes =
                Enum.GetValues(typeof(TimeIntervalType))
                    .Cast<TimeIntervalType>()
                    .Select(x => new SelectItemModel(x.Localize(), x.ConvertIntString()))
                    .ToList();

            model.AvailableVariables = trigger.AvailableVariables;
            model.DefaultMailTemplate = trigger.GetDefaultMailTemplate();
            model.ProcessType = trigger.ProcessType.ToString().ToLower();
            model.SendRequestParameters = trigger.SendRequestParameters;


            model.Categories = TriggerCategoryService.GetList().Select(x => new SelectItemModel(x.Name, x.Id)).ToList();
            model.Categories.Insert(0, new SelectItemModel("Общая", 0));

            model.IsTriggerForOrder = trigger.ObjectType == ETriggerObjectType.Order;
            model.SendToShippingServiceVariables = trigger.SendToShippingServiceVariables;
            if (model.IsTriggerForOrder)
            {
                var availableShippings = new List<string>();
                var listShippings = AdvantshopConfigService.GetDropdownShippings();
                var listModules = Core.Modules.ModulesExecuter.GetDropdownShippings();
                foreach (var shippingType in ShippingMethodService.ShippingMethodTypesUseUnloadOrder)
                {
                    var type = listShippings.FirstOrDefault(x => x.Value.Equals(shippingType, StringComparison.OrdinalIgnoreCase));
                    if (type == null)
                        type = listModules.FirstOrDefault(x => x.Value.Equals(shippingType, StringComparison.OrdinalIgnoreCase));
                    if (type != null)
                        availableShippings.Add(type != null ? type.Text : shippingType);
                }
                model.ShippingMethodsUseUnloadOrder = string.Join(", ", availableShippings);
            }
            return model;
        }


        public List<SelectItemModel> GetEventObjectGroupsForSignificantCustomerDate()
        {
            var list =
                CustomerFieldService.GetCustomerFields()
                    .Where(x => x.FieldType == CustomerFieldType.Date)
                    .Select(x => new SelectItemModel(x.Name, x.Id))
                    .ToList();

            list.Insert(0, new SelectItemModel("День рождения", 0));

            return list;
        }
    }
}
