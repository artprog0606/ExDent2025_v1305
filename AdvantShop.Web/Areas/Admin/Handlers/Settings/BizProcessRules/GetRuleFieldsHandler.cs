using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.BusinessProcesses.MessageReplies;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Settings.BizProcessRules
{
    public class GetRuleFieldsHandler
    {
        public class FieldSelectItem
        {
            public int type { get; set; }
            public string typeStr { get; set; }
            public string name { get; set; }
            public int? objId { get; set; }
            public string fieldType { get; set; }
            public string groupName { get; set; }
            public int sortOrder { get; set; }

            public static explicit operator FieldSelectItem(Enum fieldType)
            {
                var fieldTypeValue = fieldType.FieldTypeValue();
                
                return new FieldSelectItem
                {
                    type = Convert.ToInt32(fieldType),
                    typeStr = fieldType.ToString().ToLower(),
                    name = fieldType.Localize(),
                    fieldType = fieldType.FieldType().ToString().ToLower(),
                    groupName = fieldTypeValue?.GroupName ?? "",
                    sortOrder = fieldTypeValue?.SortOrder ?? 0
                };
            }

            public static explicit operator FieldSelectItem(CustomerField customerField)
            {
                return new FieldSelectItem
                {
                    type = (int)EOrderFieldType.CustomerField,
                    typeStr = EOrderFieldType.CustomerField.ToString().ToLower(),
                    name = customerField.Name,
                    objId = customerField.Id,
                    fieldType = customerField.FieldType.FieldType().ToString().ToLower(),
                    groupName = "Пользовательские поля",
                    sortOrder = ((int) EOrderFieldType.CustomerField)*10,
                };
            }
            
            public static explicit operator FieldSelectItem(LeadField leadField)
            {
                var salesFunnel = SalesFunnelService.Get(leadField.SalesFunnelId);
                var item = new FieldSelectItem
                {
                    type = (int)ELeadFieldType.LeadField,
                    typeStr = ELeadFieldType.LeadField.ToString().ToLower(),
                    name = leadField.Name,
                    objId = leadField.Id,
                    groupName = (salesFunnel != null ? salesFunnel.Name + ". ": null) + "Доп. поля",
                    sortOrder = ((int)ELeadFieldType.LeadField) * 10,
                    fieldType = leadField.FieldType.FieldType().ToString().ToLower()
                };
                return item;
            }
        }

        private readonly EBizProcessEventType _eventType;

        public GetRuleFieldsHandler(EBizProcessEventType eventType)
        {
            _eventType = eventType;
        }

        public List<FieldSelectItem> Execute()
        {
            var fields = new List<FieldSelectItem>();
            switch (_eventType)
            {
                case EBizProcessEventType.OrderCreated:
                case EBizProcessEventType.OrderStatusChanged:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(EOrderFieldType)).Cast<EOrderFieldType>()
                        .Where(x => x != EOrderFieldType.None && x != EOrderFieldType.CustomerField)
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)EOrderFieldType.CustomerField, GetCustomerFieldsList());
                    break;
                
                case EBizProcessEventType.LeadCreated:
                case EBizProcessEventType.LeadStatusChanged:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ELeadFieldType)).Cast<ELeadFieldType>()
                        .Where(x => x != ELeadFieldType.None && x != ELeadFieldType.CustomerField && x != ELeadFieldType.LeadField)
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)ELeadFieldType.CustomerField, GetCustomerFieldsList());
                    fields.AddRange(GetLeadFieldsList());
                    break;
                
                case EBizProcessEventType.CallMissed:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ECallFieldType)).Cast<ECallFieldType>()
                        .Where(x => x != ECallFieldType.None && x != ECallFieldType.CustomerField)
                        .Select(x => (FieldSelectItem)x));
                    fields.InsertRange((int)ECallFieldType.CustomerField, GetCustomerFieldsList());
                    break;
                
                case EBizProcessEventType.ReviewAdded:
                    fields = new List<FieldSelectItem>();
                    break;
                
                case EBizProcessEventType.MessageReply:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(EMessageReplyFieldType))
                            .Cast<EMessageReplyFieldType>()
                            .Where(x => x != EMessageReplyFieldType.Facebook) // temporary not show
                            .Select(x => (FieldSelectItem) x))
                        .ToList();
                    break;
                
                case EBizProcessEventType.TaskCreated:
                case EBizProcessEventType.TaskStatusChanged:
                    fields = new List<FieldSelectItem>(Enum.GetValues(typeof(ETaskFieldType)).Cast<ETaskFieldType>()
                        .Where(x => x != ETaskFieldType.None)
                        .Select(x => (FieldSelectItem)x));
                    break;
                
                default:
                    throw new NotImplementedException("No implementation for event type " + _eventType);
            }
            return fields.OrderBy(x => x.sortOrder).ToList();
        }

        private List<FieldSelectItem> GetCustomerFieldsList()
        {
            return new List<FieldSelectItem>(CustomerFieldService.GetCustomerFields().Select(x => (FieldSelectItem)x));
        }
        
        private List<FieldSelectItem> GetLeadFieldsList()
        {
            return new List<FieldSelectItem>(LeadFieldService.GetLeadFields().Select(x => (FieldSelectItem)x));
        }
    }
}