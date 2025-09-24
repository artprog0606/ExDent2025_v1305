using System;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Handlers.Shared.AdminNotifications;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class ChangeTaskGroupHandler
    {
        private readonly bool _forced;
        private readonly int _taskId;
        private readonly int _targetTaskGroupId;
        private readonly int? _taskGroupStatusId;

        public ChangeTaskGroupHandler(int taskId, int targetTaskGroupId, int? taskGroupStatusId, bool forced = false)
        {
            _forced = forced;
            _taskId = taskId;
            _targetTaskGroupId = targetTaskGroupId;
            _taskGroupStatusId = taskGroupStatusId;
        }

        public (bool result, string error) Execute()
        {
            var task = TaskService.GetTask(_taskId);
            
            if (task is null || !TaskService.CheckAccess(task))
                return (false, "Задача не найдена или нет доступа");

            var taskStatus = ProjectStatusService.Get(task.StatusId);
            if ((taskStatus.StatusType == TaskStatusType.Accepted || taskStatus.StatusType == TaskStatusType.Canceled || taskStatus.StatusType == TaskStatusType.Completed)
                && TaskGroupService.IsNotBeCompletedByTaskGroupId(_targetTaskGroupId))
                return (false, "Нельзя перенести задачу в проект с текущим статусом");
            
            if (task.TaskGroupId == _targetTaskGroupId)
                return (false, "Не удалось перенести задачу");
            
            var prevState = (Task)task.Clone();

            if (_taskGroupStatusId.HasValue)
            {
                var projectStatus = ProjectStatusService.Get(_taskGroupStatusId.Value);

                if (projectStatus is null)
                    return (false, "Не удалось найти выбранный статус");

                task.StatusId = projectStatus.Id;
                task.TaskGroupId = _targetTaskGroupId;
                
                TaskService.UpdateTask(task);
                ProcessNotifications(prevState, task);

                return (true, null);
            }
            
            var filteredTargetProjectStatuses = ProjectStatusService.GetList(_targetTaskGroupId)
                .Where(x => x.Status != ProjectStatusType.Canceled && x.Status != ProjectStatusType.FinalSuccess)
                .ToList();
            
            if (filteredTargetProjectStatuses.Count == 0)
                return (false, "В выбранном проекте отсутствуют статусы");
            
            if (filteredTargetProjectStatuses.Count == 1)
            {
                var status = filteredTargetProjectStatuses.Single();
                
                task.StatusId = status.Id;
                task.TaskGroupId = _targetTaskGroupId;
                
                TaskService.UpdateTask(task);
                ProcessNotifications(prevState, task);

                return (true, null);
            }
            
            var currentTaskStatus = taskStatus;

            var idealNewStatus = filteredTargetProjectStatuses.FirstOrDefault(x =>
                x.StatusType == currentTaskStatus.StatusType &&
                string.Equals(x.Name, currentTaskStatus.Name, StringComparison.OrdinalIgnoreCase));

            var onlyOnePossibleStatus =
                filteredTargetProjectStatuses.Count(x => x.StatusType == currentTaskStatus.StatusType) == 1;

            ProjectStatus newTaskStatus = null;
            if (idealNewStatus is null)
            {
                if (_forced || onlyOnePossibleStatus)
                {
                    newTaskStatus = filteredTargetProjectStatuses.FirstOrDefault(x => x.StatusType == currentTaskStatus.StatusType);
                }
            }
            else
            {
                newTaskStatus = idealNewStatus;
            }

            if (newTaskStatus is null)
                return (false, "Не удалось найти подходящий статус");
            
            
            task.StatusId = newTaskStatus.Id;
            task.TaskGroupId = _targetTaskGroupId;
            TaskService.UpdateTask(task);
            ProcessNotifications(prevState, task);
            return (true, null);
        }

        private void ProcessNotifications(Task prevState, Task newState)
        {
            var modifier = CustomerContext.CurrentCustomer;
            var assignedCustomers = newState.Managers.Select(x => x.Customer).ToList();
            var appointedCustomer = newState.AppointedManager != null ? newState.AppointedManager.Customer : null;

            var customerIds = new List<Guid>();
            foreach (var customer in assignedCustomers.Where(x => x.Id != modifier.Id && x.HasRoleAction(RoleAction.Tasks)))
                customerIds.Add(customer.Id);
            if (appointedCustomer != null && appointedCustomer.Id != modifier.Id && appointedCustomer.HasRoleAction(RoleAction.Tasks))
                customerIds.Add(appointedCustomer.Id);
            if (!customerIds.Any())
                return;

            var isNotifyRestricted =
                prevState.TaskGroup.IsPrivateComments &&
                (prevState.ManagerIds != newState.ManagerIds || prevState.TaskGroupId != newState.TaskGroupId);

            TaskService.OnTaskChanged(modifier, prevState, newState, isNotifyRestricted);

            if (isNotifyRestricted)
                return;

            var notificationsHandler = new AdminNotificationsHandler();
            var onChangeNotifications = new List<AdminNotification>();

            if (prevState.TaskGroupId != newState.TaskGroupId)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.TaskGroup"),
                    prevState.TaskGroup.Name, newState.TaskGroup.Name));
            if (prevState.StatusId != newState.StatusId)
                onChangeNotifications.Add(new OnTaskChangeNotification(prevState, modifier, LocalizationService.GetResource("Admin.Tasks.TaskChanges.Status"),
                    prevState.StatusName, newState.StatusName));

            notificationsHandler.NotifyCustomers(onChangeNotifications.ToArray(), customerIds.ToArray());
        }
    }
}
