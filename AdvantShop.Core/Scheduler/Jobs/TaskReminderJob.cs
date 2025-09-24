//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class TaskReminderJob: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            foreach(var task in TaskService.GetTasksWithReminder())
            {
                if (!task.DueDate.HasValue)
                    continue;

                var delta = task.DueDate.Value - DateTime.Now;
                switch (task.Reminder)
                {
                    case TaskReminder.AtTerm:
                        if (delta.TotalMinutes <= 0 && delta.TotalHours >= -24)
                            SendNotifications(task);
                        break;
                    case TaskReminder.TenMinutesBefore:
                        if (delta.TotalMinutes <= 10 && (delta - TimeSpan.FromMinutes(10)).TotalHours >= -24)
                            SendNotifications(task);
                        break;
                    case TaskReminder.HourBefore:
                        if (delta.TotalHours <= 1 && (delta - TimeSpan.FromHours(1)).TotalHours >= -24)
                            SendNotifications(task);
                        break;
                    case TaskReminder.ThreeHoursBefore:
                        if (delta.TotalHours <= 3 && (delta - TimeSpan.FromHours(3)).TotalHours >= -24)
                            SendNotifications(task);
                        break;
                    case TaskReminder.DayBefore:
                        if (delta.TotalDays <= 1 && (delta - TimeSpan.FromDays(1)).TotalHours >= -24)
                            SendNotifications(task);
                        break;
                    case TaskReminder.ThreeDaysBefore:
                        if (delta.TotalDays <= 3 && (delta - TimeSpan.FromDays(3)).TotalHours >= -24)
                            SendNotifications(task);
                        break;
                }
            }
        }

        private void SendNotifications(Task task)
        {
            TaskService.OnTaskReminder(task);

            foreach (var manager in task.Managers)
            {
                AdminInformerService.Add(new AdminInformer(AdminInformerType.TaskReminder, task.Id, null)
                {
                    Title = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskReminderNotification.Title", task.Id),
                    Body = LocalizationService.GetResourceFormat("Core.Services.CMS.OnTaskReminderNotification.Body", task.TaskGroup.Name, task.Name, task.DueDate.HasValue ? task.DueDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"),
                    Link = UrlService.GetAdminUrl("tasks/#?modalShow=true&modal=" + task.Id),
                    PrivateCustomerId = manager.CustomerId,
                });
            }

            task.Reminded = true;
            TaskService.UpdateTask(task, trackChanges: false);
        }
    }
}
