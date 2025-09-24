using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.Customers;
using AdvantShop.Core.Services.Triggers;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Triggers;

namespace AdvantShop.Web.Admin.Handlers.Triggers
{
    public class GetTriggerEditFields
    {
        

        private readonly ETriggerEventType _eventType;

        public GetTriggerEditFields(ETriggerEventType eventType)
        {
            _eventType = eventType;
        }

        public List<FieldSelectItem> Execute()
        {
            FieldUsageAttribute attr;
            var fields = new List<FieldSelectItem>();
            switch (_eventType)
            {
                case ETriggerEventType.OrderCreated:
                case ETriggerEventType.OrderStatusChanged:
                case ETriggerEventType.OrderPaied:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(EOrderFieldType)).Cast<EOrderFieldType>()
                        .Where(x => ((attr = x.GetAttribute<FieldUsageAttribute>()).Value.HasFlag(EFieldUsage.Edit) && (attr.UseForEdit == null || attr.UseForEdit.Contains(_eventType))))
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)EOrderFieldType.CustomerField, GetCustomerFieldsList());
                    if(Core.Services.Configuration.Settings.Settings1C.Enabled)
                        fields.Add((FieldSelectItem)EOrderFieldType.UseIn1C);
                    break;

                case ETriggerEventType.LeadCreated:
                case ETriggerEventType.LeadStatusChanged:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ELeadFieldType)).Cast<ELeadFieldType>()
                        .Where(x => ((attr = x.GetAttribute<FieldUsageAttribute>()).Value.HasFlag(EFieldUsage.Edit) && (attr.UseForEdit == null || attr.UseForEdit.Contains(_eventType))))
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)ELeadFieldType.CustomerField, GetCustomerFieldsList());
                    break;
                    
                case ETriggerEventType.CustomerCreated:
                case ETriggerEventType.TimeFromLastOrder:
                case ETriggerEventType.SignificantDate:
                case ETriggerEventType.SignificantCustomerDate:
                case ETriggerEventType.InstallMobileApp:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ECustomerFieldType)).Cast<ECustomerFieldType>()
                        .Where(x => ((attr = x.GetAttribute<FieldUsageAttribute>()).Value.HasFlag(EFieldUsage.Edit) && (attr.UseForEdit == null || attr.UseForEdit.Contains(_eventType))))
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)ECustomerFieldType.CustomerField, GetCustomerFieldsList());
                    break;
                    
                //case ETriggerEventType.TaskCreated:
                //case ETriggerEventType.TaskStatusChanged:
                //    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ETaskFieldType)).Cast<ETaskFieldType>()
                //        .Where(x => x != ETaskFieldType.None)
                //        .Select(x => (FieldSelectItem)x));
                //    break;

                default:
                    throw new NotImplementedException("No implementation for event type " + _eventType);
            }
            return fields.OrderBy(x => x.sortOrder).ToList();
        }

        private List<FieldSelectItem> GetCustomerFieldsList()
        {
            return new List<FieldSelectItem>(CustomerFieldService.GetCustomerFields().Select(x => (FieldSelectItem)x));
        }
    }
}