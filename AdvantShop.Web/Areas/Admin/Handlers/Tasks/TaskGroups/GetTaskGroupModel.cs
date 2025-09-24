using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class GetTaskGroupModel : AbstractCommandHandler<TaskGroupModel>
    {
        private readonly TaskGroup _taskGroup;

        public GetTaskGroupModel(TaskGroup taskGroup)
        {
            _taskGroup = taskGroup;
        }

        protected override TaskGroupModel Handle()
        {
            var model = new TaskGroupModel
            {
                Id = _taskGroup.Id,
                Name = _taskGroup.Name,
                SortOrder = _taskGroup.SortOrder,
                Enabled = _taskGroup.Enabled,
                IsPrivateComments = _taskGroup.IsPrivateComments,
                ManagersTaskGroupConstraint = _taskGroup.ManagersTaskGroupConstraint != null ? (int)_taskGroup.ManagersTaskGroupConstraint : -1,
                ManagerIds = TaskGroupService.GetTaskGroupManagerIds(_taskGroup.Id),
                ManagerRoleIds = TaskGroupService.GetTaskGroupManagerRoleIds(_taskGroup.Id),
                ParticipantIds = TaskGroupService.GetTaskGroupParticipantIds(_taskGroup.Id),
                ProjectStatuses = ProjectStatusService.GetList(_taskGroup.Id),
                IsNotBeCompleted = _taskGroup.IsNotBeCompleted
            };

            return model;
        }
    }
}
