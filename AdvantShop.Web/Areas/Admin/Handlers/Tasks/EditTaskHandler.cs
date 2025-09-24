using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class EditTaskHandler
    {
        private readonly TaskModel _model;
        
        public EditTaskHandler() { }

        public EditTaskHandler(TaskModel model)
        {
            _model = model;
        }

        public bool Execute()
        {
            var task = TaskService.GetTask(_model.Id);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();

            task.Reminded = task.Reminder != _model.Reminder || _model.Remind == false || task.DueDate != _model.DueDate ? false : task.Reminded;

            task.Name = _model.Name;
            task.AppointedManagerId = _model.AppointedManagerId;
            task.DueDate = _model.DueDate;
            task.Description = _model.Description;
            task.TaskGroupId = _model.TaskGroupId;
            task.Priority = _model.Priority;
            task.DateModified = DateTime.Now;
            task.StatusId = _model.StatusId;
            task.Accepted = _model.Accepted;
            task.ResultShort = _model.ResultShort;
            task.ResultFull = _model.ResultFull;
            task.Reminder = task.DueDate.HasValue ? (_model.Remind ? _model.Reminder : TaskReminder.NotRemind) : TaskReminder.NotRemind;

            TaskService.UpdateTask(task, prevTaskState: prevState);

            ProcessNotifications(prevState, task);

            AddLeadEvent(prevState, task);

            TrackEvents(prevState, task);

            return true;
        }

        public bool ChangeAssignedManagers(int taskId, List<int> managerIds)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.SetManagerIds(managerIds);
            
            TaskService.SetTaskManagers(task.Id, managerIds);
            TasksHistoryService.TrackTaskChanges(task, null, prevState);
            
            ProcessNotifications(prevState, task);
            
            return true;
        }
        
        public bool ChangeObservers(int taskId, List<int> observerIds)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.ObserverIds = observerIds;
            
            TaskService.SetTaskObservers(task.Id, observerIds);
            TasksHistoryService.TrackTaskChanges(task, null, prevState);

            ProcessNotifications(prevState, task);
            
            return true;
        }

        private void ProcessNotifications(Task prevState, Task newState)
        {
            var taskGroup = TaskGroupService.GetTaskGroup(prevState.TaskGroupId);

            var isNotifyRestricted =
                taskGroup.IsPrivateComments &&
                (prevState.ManagerIds != newState.ManagerIds || prevState.TaskGroupId != newState.TaskGroupId);

            var modifier = CustomerContext.CurrentCustomer;
            var notificationsHandler = new AdminNotificationsHandler();

            var assignedCustomers = newState.Managers.Select(x => x.Customer).ToList();
            var appointedCustomer = newState.AppointedManager != null ? newState.AppointedManager.Customer : null;

            var oldManagers = prevState.Managers.Except(newState.Managers, (prevManager, newManager) => prevManager.ManagerId == newManager.ManagerId).ToList();
            var newManagers = newState.Managers.Except(prevState.Managers, (newManager, prevManager) => newManager.ManagerId == prevManager.ManagerId).ToList();
            // сменился исполнитель
            foreach (var assignedCustomer in newManagers.Where(x => x.CustomerId != modifier.Id && x.HasRoleAction(RoleAction.Tasks)))
            {
                notificationsHandler.NotifyCustomers(new OnSetTaskNotification(prevState, modifier), assignedCustomer.CustomerId);
            }

            TaskService.OnTaskChanged(modifier, prevState, newState, isNotifyRestricted);

            if (isNotifyRestricted)
                return;

            var customerIds = new List<Guid>();
            foreach (var customer in assignedCustomers.Where(x => x.Id != modifier.Id && x.HasRoleAction(RoleAction.Tasks)))
                customerIds.Add(customer.Id);
            if (appointedCustomer != null && appointedCustomer.Id != modifier.Id && appointedCustomer.HasRoleAction(RoleAction.Tasks))
                customerIds.Add(appointedCustomer.Id);

            if (!customerIds.Any())
                return;

            var onChangeNotifications = new List<AdminNotification>();
            if (prevState.TaskGroupId != newState.TaskGroupId)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.TaskGroup"),
                    prevState.TaskGroup.Name, newState.TaskGroup.Name));
            if (prevState.StatusId != newState.StatusId)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.Status"),
                    prevState.StatusName, newState.StatusName));
            if (prevState.DueDate != newState.DueDate)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.DueDate"),
                    prevState.DueDate.HasValue ? Culture.ConvertShortDate(prevState.DueDate.Value) : LocalizationService.GetResource("Admin.Tasks.TaskChanges.DueDateNotSet"),
                    newState.DueDate.HasValue ? Culture.ConvertShortDate(newState.DueDate.Value) : LocalizationService.GetResource("Admin.Tasks.TaskChanges.DueDateNotSet")));
            if (prevState.Priority != newState.Priority)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.Priority"),
                    prevState.Priority.Localize(), newState.Priority.Localize()));
            if (prevState.Description != newState.Description)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.Description"), null, null));
            if (prevState.Name != newState.Name)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.Name"), null, newState.Name));
            if (!prevState.Accepted && newState.Accepted)
                onChangeNotifications.Add(new OnTaskAcceptedNotification(prevState, modifier));

            notificationsHandler.NotifyCustomers(onChangeNotifications.ToArray(), customerIds.ToArray());
        }
        
        private void AddLeadEvent(Task prevState, Task task)
        {
            if (task.LeadId == null)
                return;

            if (prevState.StatusType != TaskStatusType.Completed.ToString() && task.StatusType == TaskStatusType.Completed.ToString())
                LeadsHistoryService.TaskCompleted(task.LeadId.Value, task, null);
        }

        private void TrackEvents(Task prevState, Task newState)
        {
            if (prevState.StatusId != newState.StatusId || prevState.Accepted != newState.Accepted)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_TaskStatusChanged);
            else
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_EditTask);
        }

        public bool ChangeAppointedManager(int taskId, int? appointedManagerId)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.ManagerId = appointedManagerId;

            TaskService.SetAppointedManager(taskId, appointedManagerId);
            
            TasksHistoryService.TrackTaskChanges(task, null, prevState);
            
            ProcessNotifications(prevState, task);
            
            return true;
        }

        public bool ChangeDueDate(int taskId, DateTime? date)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.DueDate = date;

            TaskService.SetDueDate(taskId, date);

            if (date.HasValue == false)
            {
                TaskService.SetReminder(taskId, TaskReminder.NotRemind);
            }
            
            TasksHistoryService.TrackTaskChanges(task, null, prevState);
            
            ProcessNotifications(prevState, task);
            
            return true;
        }

        public bool ChangeReminder(int taskId, TaskReminder reminder)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.Reminder = reminder;

            TaskService.SetReminder(taskId, reminder);
            
            TasksHistoryService.TrackTaskChanges(task, null, prevState);
            
            ProcessNotifications(prevState, task);
            
            return true;
        }
        
        public bool ChangePriority(int taskId, TaskPriority priority)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return false;

            if (!TaskService.CheckAccess(task))
                return false;

            var prevState = task.DeepClone();
            
            task.Priority = priority;

            TaskService.SetPriority(taskId, priority);
            
            TasksHistoryService.TrackTaskChanges(task, null, prevState);
            
            ProcessNotifications(prevState, task);
            
            return true;
        }
    }
}
