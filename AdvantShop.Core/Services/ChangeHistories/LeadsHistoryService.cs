using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Crm.LeadFields;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class LeadsHistoryService
    {
        public static void NewLead(Lead lead, ChangedBy changedBy)
        {
            var leadSource = OrderSourceService.GetOrderSource(lead.OrderSourceId);

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = lead.Id,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName =
                    LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.LeadCreated", lead.Id) +
                    (leadSource != null && leadSource.Type != OrderType.None
                        ? string.Format("<div class=\"lead-source\">{0}: {1}</div>",
                            LocalizationService.GetResource("Admin.Handlers.Leads.GetLeadEvents.LeadSource"), leadSource.Name)
                        : null)
            });
        }

        public static void DeleteLead(int leadId, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.LeadDeleted", leadId)
            });
        }

        public static void TrackLeadChanges(Lead lead, ChangedBy changedBy)
        {
            var oldLead = LeadService.GetLead(lead.Id);
            if (oldLead == null)
                return;

            var history = ChangeHistoryService.GetChanges(lead.Id, ChangeHistoryObjType.Lead, oldLead, lead, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackLeadCustomerChanges(Lead lead, ChangedBy changedBy)
        {
            var newValue = lead.Customer;

            if (newValue == null || newValue.Id == Guid.Empty)
                return;

            var oldCustomer = CustomerService.GetCustomerFromDb(newValue.Id);
            if (oldCustomer == null)
                return;

            var history = ChangeHistoryService.GetChanges(lead.Id, ChangeHistoryObjType.Lead, oldCustomer, newValue, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackLeadItemChanges(int leadId, LeadItem oldLeadItem, LeadItem newLeadItem, ChangedBy changedBy)
        {
            var history = new List<ChangeHistory>();
                
            if (oldLeadItem != null && newLeadItem != null)
                history = ChangeHistoryService.GetChanges(leadId, ChangeHistoryObjType.Lead, oldLeadItem, newLeadItem, changedBy, oldLeadItem.LeadItemId);

            else if (oldLeadItem == null && newLeadItem != null)
            {
                history = new List<ChangeHistory>()
                {
                    new ChangeHistory(changedBy)
                    {
                        ObjId = leadId,
                        ObjType = ChangeHistoryObjType.Lead,
                        ParameterName =
                            "Добавлен товар " + newLeadItem.Name +
                            (!string.IsNullOrEmpty(newLeadItem.ArtNo) ? " (" + newLeadItem.ArtNo + ")" : ""),
                        ParameterId = newLeadItem.ProductId,
                    }
                };
            }
            else if (oldLeadItem != null && newLeadItem == null)
            {
                history = new List<ChangeHistory>()
                {
                    new ChangeHistory(changedBy)
                    {
                        ObjId = leadId,
                        ObjType = ChangeHistoryObjType.Lead,
                        ParameterName =
                            "Удален товар " + oldLeadItem.Name +
                            (!string.IsNullOrEmpty(oldLeadItem.ArtNo) ? " (" + oldLeadItem.ArtNo + ")" : ""),
                        ParameterId = oldLeadItem.ProductId,
                    }
                };
            }

            ChangeHistoryService.Add(history);
        }

        public static void TrackLeadFieldChanges(int leadId, int leadFieldId, string newValue, ChangedBy changedBy)
        {
            var oldField = LeadFieldService.GetLeadFieldWithValue(leadId, leadFieldId);
            if (oldField == null)
                return;

            var oldValue = oldField.Value ?? "";
            newValue = newValue ?? "";

            if (oldValue == newValue)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName = oldField.Name,
                OldValue = oldValue,
                NewValue = newValue ?? ""
            });
        }

        public static void TrackLeadCustomerFieldChanges(int leadId, Guid customerId, int id, string newValue, ChangedBy changedBy)
        {
            var oldField = CustomerFieldService.GetCustomerFieldsWithValue(customerId, id);
            if (oldField == null)
                return;

            var oldValue = oldField.Value ?? "";
            newValue = newValue ?? "";

            if (oldValue == newValue)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName = oldField.Name,
                OldValue = oldValue,
                NewValue = newValue ?? ""
            });
        }

        public static void AddLeadTask(int leadId, Task task, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterId = task.Id,
                ParameterType = ChangeHistoryParameterType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.AddingLeadTask", task.Id, task.Name)
            });
        }

        public static void DeleteLeadTask(int leadId, Task task, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterId = task.Id,
                ParameterType = ChangeHistoryParameterType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.DeleteLeadTask", task.Id, task.Name)
            });
        }

        public static void TaskCompleted(int leadId, Task task, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterId = task.Id,
                ParameterType = ChangeHistoryParameterType.Task,
                ParameterName = 
                !string.IsNullOrEmpty(task.ResultFull) 
                    ? LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.TaskCompleted", task.Name, task.ResultFull)
                    : LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.TaskCompletedNoresult", task.Name)
            });
        }

        public static void AddOrder(int leadId, Order order, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterId = order.OrderID,
                ParameterType = ChangeHistoryParameterType.Order,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.AddOrder", order.OrderID, order.Number)
            });
        }

        public static void AddAttachment(int leadId, string attachmentName, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.AttachmentAdded", leadId) + ": " + attachmentName
            });
        }

        public static void RemoveAttachment(int leadId, string attachmentName, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = leadId,
                ObjType = ChangeHistoryObjType.Lead,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.LeadsHistory.AttachmentRemoved", leadId) + ": " + attachmentName
            });
        }
    }
}
