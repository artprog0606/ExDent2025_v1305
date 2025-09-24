using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class AddEditTaskGroup : AbstractCommandHandler<int>
    {
        private readonly TaskGroupModel _model;
        private bool _editMode;

        public AddEditTaskGroup(TaskGroupModel model, bool editMode)
        {
            _model = model;
            _editMode = editMode;
        }

        protected override void Validate()
        {
            if (_model.Name.IsNullOrEmpty())
                throw new BlException("Укажите название");

            if (_editMode && !TaskService.CheckAccessByGroup(_model.Id))
                throw new BlException("Нет доступа");

            if (!_editMode)
            {
                _model.ProjectStatuses = (_model.ProjectStatuses ?? new List<ProjectStatus>()).Where(x => x.Name.IsNotEmpty() && x.Color.IsNotEmpty()).ToList();
                if (!_model.ProjectStatuses.Any(x => x.Status == ProjectStatusType.None) ||
                    !_model.ProjectStatuses.Any(x => x.Status == ProjectStatusType.Canceled) ||
                    !_model.ProjectStatuses.Any(x => x.Status == ProjectStatusType.FinalSuccess))
                    throw new BlException("Укажите статусы проекта");

            }
        }

        protected override int Handle()
        {
            var taskGroup = new TaskGroup
            {
                Name = _model.Name,
                SortOrder = _model.SortOrder,
                Enabled = _model.Enabled,
                IsPrivateComments = _model.IsPrivateComments,
                ManagersTaskGroupConstraint = 
                    _model.ManagersTaskGroupConstraint != -1
                    ? (ManagersTaskConstraint) _model.ManagersTaskGroupConstraint
                    : default(ManagersTaskConstraint?),
                IsNotBeCompleted = _model.IsNotBeCompleted
            };

            if (_editMode)
            {
                taskGroup.Id = _model.Id;
                TaskGroupService.UpdateTaskGroup(taskGroup);

                TaskGroupService.ClearTaskGroupManagers(taskGroup.Id);
                TaskGroupService.ClearTaskGroupManagerRoles(taskGroup.Id);
                TaskGroupService.ClearTaskGroupParticipants(taskGroup.Id);
            }
            else
            {
                taskGroup.Id = TaskGroupService.AddTaskGroup(taskGroup);

                if (_model.ProjectStatuses.Any())
                {
                    for (int i = 0; i < _model.ProjectStatuses.Count; i++)
                    {
                        var statusModel = _model.ProjectStatuses[i];
                        statusModel.SortOrder = i * 10;
                        //var projectStatus = new ProjectStatus
                        //{
                        //    Name = statusModel.Name,
                        //    Color = statusModel.Color,
                        //    Status = statusModel.Status,
                        //    SortOrder = i * 10,
                        //    StatusType = statusModel.StatusType
                        //};
                        ProjectStatusService.Add(statusModel);
                        TaskGroupService.AddProjectStatus(taskGroup.Id, statusModel.Id);
                    }
                }

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Tasks_TaskProjectCreated);
            }

            if (_model.ManagerIds != null)
            {
                foreach (var managerId in _model.ManagerIds)
                    TaskGroupService.AddTaskGroupManager(taskGroup.Id, managerId);
            }

            if (_model.ManagerRoleIds != null)
            {
                foreach (var managerRoleId in _model.ManagerRoleIds)
                    TaskGroupService.AddTaskGroupManagerRole(taskGroup.Id, managerRoleId);
            }

            if (_model.ParticipantIds != null)
            {
                foreach (var managerId in _model.ParticipantIds)
                    TaskGroupService.AddTaskGroupParticipant(taskGroup.Id, managerId);
            }

            return taskGroup.Id;
        }
    }
}
