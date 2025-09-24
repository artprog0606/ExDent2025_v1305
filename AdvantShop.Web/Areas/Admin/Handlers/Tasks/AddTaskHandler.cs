using System;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class AddTaskHandler
    {
        private readonly TaskModel _model;

        public AddTaskHandler(TaskModel model)
        {
            _model = model;
        }

        public Task Execute()
        {
            var taskGroupId = _model.TaskGroupId;
            if (taskGroupId == 0)
            {
                var group = TaskGroupService.GetAllTaskGroups().FirstOrDefault();
                if (group == null)
                    throw new BlException("Выберите проект для задачи");

                taskGroupId = group.Id;
            }

            var task = new Task()
            {
                Name = _model.Name.DefaultOrEmpty(),
                AppointedManagerId = _model.AppointedManagerId,
                DueDate = _model.DueDate,
                Description = _model.Description ?? string.Empty,
                TaskGroupId = taskGroupId,
                Priority = _model.Priority,
                StatusId = ProjectStatusService.GetFirstStatusIdInList(taskGroupId),
                Accepted = false,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                OrderId = _model.OrderId,
                LeadId = _model.LeadId,
                ReviewId = _model.ReviewId,
                CustomerId = _model.ClientCustomerId,
                BindedTaskId = _model.BindedTaskId,
                DateAppointed = _model.DateAppointed,
                IsAutomatic = _model.IsAutomatic,
                IsDeferred = _model.IsDeferred,
                BindedObjectStatus = _model.BindedObjectStatus,
                Reminder = _model.DueDate.HasValue ? (_model.Remind ? _model.Reminder : TaskReminder.NotRemind) : TaskReminder.NotRemind,
                Reminded = false,
                BizProcessRuleId = _model.BizProcessRuleId
            };
            task.SetManagerIds(_model.ManagerIds);

            task.Id = TaskService.AddTask(task);

            if (!task.IsDeferred)
            {
                if (task.LeadId != null)
                    LeadsHistoryService.AddLeadTask(task.LeadId.Value, task, null);
                TaskService.OnTaskCreated(task);

                BizProcessExecuter.TaskAdded(task);
                if (!task.IsAutomatic)
                {
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_TaskCreated);
                }
            }

            return task;
        }
    }
}
