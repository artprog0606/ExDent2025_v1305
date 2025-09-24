using System.Collections.Generic;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Core.Services.Crm.ProjectStatuses;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class TasksHistoryService
    {
        public static void NewTask(Task task, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = task.Id,
                ObjType = ChangeHistoryObjType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.TasksHistory.TaskCreated", task.Id)
            });
        }

        public static void DeleteTask(Task task, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = task.Id,
                ObjType = ChangeHistoryObjType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.ChangeHistories.TasksHistory.TaskDeleted", task.Id)
            });
        }

        public static void TrackTaskChanges(Task task, ChangedBy changedBy, Task prevTaskState = null)
        {
            var oldTask = prevTaskState ?? TaskService.GetTask(task.Id);
            if (oldTask == null)
                return;

            var history = ChangeHistoryService.GetChanges(task.Id, ChangeHistoryObjType.Task, oldTask, task, changedBy);

            ChangeHistoryService.Add(history);
        }

        public static void TrackTaskStatusChanges(int taskId, ProjectStatus status, ChangedBy changedBy)
        {
            var oldTask = TaskService.GetTask(taskId);
            if (oldTask == null)
                return;

            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = taskId,
                ObjType = ChangeHistoryObjType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.Crm.Task.Status"),
                OldValue = oldTask.StatusName,
                NewValue = status.Name
            });
        }

        public static void TrackTaskAddAttachment(int taskid, string attachmentName, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = taskid,
                ObjType = ChangeHistoryObjType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.Crm.Task.Attachment"),
                NewValue = attachmentName
            });
        }

        public static void TrackTaskRemoveAttachment(int taskid, string attachmentName, ChangedBy changedBy)
        {
            ChangeHistoryService.Add(new ChangeHistory(changedBy)
            {
                ObjId = taskid,
                ObjType = ChangeHistoryObjType.Task,
                ParameterName = LocalizationService.GetResourceFormat("Core.Crm.Task.Attachment"),
                OldValue = attachmentName
            });
        }
    }
}
