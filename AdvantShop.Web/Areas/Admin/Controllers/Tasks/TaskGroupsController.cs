using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Tasks
{
    [SaasFeature(ESaasProperty.HaveTasks)]
    [Auth(RoleAction.Tasks)]
    [AccessBySettings(Core.Services.Configuration.EProviderSetting.TasksActive, ETypeRedirect.AdminPanel)]
    public partial class TaskGroupsController : BaseAdminController
    {
        public JsonResult GetTaskGroups(TaskGroupsFilterModel model)
        {
            return Json(new GetTaskGroupsHandler(model).Execute());
        }

        [Auth]
        public JsonResult GetTaskGroup(int id)
        {
            var group = TaskGroupService.GetTaskGroup(id);
            if (group == null || !TaskService.CheckAccessByGroup(group.Id))
                return JsonError();

            return ProcessJsonResult(new GetTaskGroupModel(group));
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult AddTaskGroup(TaskGroupModel model)
        {
            return ProcessJsonResult(new AddEditTaskGroup(model, false));
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult UpdateTaskGroup(TaskGroupModel model)
        {
            return ProcessJsonResult(new AddEditTaskGroup(model, true));
        }

        public JsonResult GetFormData(int? taskGroupId)
        {
            var managers = ManagerService.GetManagers(RoleAction.Tasks);
            var participants = ManagerService.GetManagers(RoleAction.Tasks);

            if (taskGroupId != null)
            {
                var managerIds = TaskGroupService.GetTaskGroupManagerIds(taskGroupId.Value);

                foreach (var managerId in managerIds.Where(x => !managers.Any(m => m.ManagerId == x)))
                {
                    var manager = ManagerService.GetManager(managerId);
                    if (manager != null)
                        managers.Add(manager);
                }

                var participantIds = TaskGroupService.GetTaskGroupParticipantIds(taskGroupId.Value);

                foreach (var managerId in participantIds.Where(x => !participants.Any(m => m.ManagerId == x)))
                {
                    var manager = ManagerService.GetManager(managerId);
                    if (manager != null)
                        participants.Add(manager);
                }
            }

            var managersTaskConstraints = new List<SelectItemModel<int>>()
            {
                new SelectItemModel<int>("Не выбрано", -1)
            };
            
            foreach (ManagersTaskConstraint value in Enum.GetValues(typeof(ManagersTaskConstraint)))
            {
                managersTaskConstraints.Add(new SelectItemModel<int>(value.Localize(), (int)value));
            }
            
            return Json(new
            {
                managers = managers.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                participants = participants.Select(x => new SelectItemModel(x.FullName, x.ManagerId)),
                managerRoles = ManagerRoleService.GetManagerRoles().Select(x => new SelectItemModel(x.Name, x.Id)),
                managersTaskConstraints
            });
        }

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult InplaceTaskGroup(TaskGroupModel model)
        {
            if (model.Id != 0)
            {
                var taskGroup = TaskGroupService.GetTaskGroup(model.Id);
                if (taskGroup == null)
                    return Json(new { result = false });
                taskGroup.SortOrder = model.SortOrder;
                taskGroup.Enabled = model.Enabled;
                TaskGroupService.UpdateTaskGroup(taskGroup);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTaskGroup(int id)
        {
            TaskGroupService.DeleteTaskGroup(id);
            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(TaskGroupsFilterModel command, Func<int, TaskGroupsFilterModel, bool> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                {
                    func(id, command);
                }
            }
            else
            {
                var handler = new GetTaskGroupsHandler(command);
                var ids = handler.GetItemsIds("Id");

                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken, Auth]
        public JsonResult DeleteTaskGroups(TaskGroupsFilterModel command)
        {
            Command(command, (id, c) =>
            {
                TaskGroupService.DeleteTaskGroup(id);
                return true;
            });
            return Json(true);
        }

        #endregion

        #region ProjectStatuses

        public JsonResult GetAllProjectStatuses()
        {
            var projectStatuses = ProjectStatusService.GetList();
            return JsonOk(projectStatuses);
        }

        public JsonResult GetProjectStatuses(int taskGroupId)
        {
            return Json(GetSeparateProjectStatuses(taskGroupId));
        }

        public JsonResult GetDefaultProjectStatuses()
        {
            return Json(GetSeparateProjectStatuses(null));
        }

        private object GetSeparateProjectStatuses(int? taskGroupId)
        {
            var projectStatuses = taskGroupId.HasValue ? ProjectStatusService.GetList(taskGroupId.Value) : ProjectStatusService.GetDefaultProjectStatuses();
            var statusTypeList = Enum.GetValues(typeof(TaskStatusType))
                .Cast<TaskStatusType>()
                .Where(x => x != TaskStatusType.Canceled && x != TaskStatusType.Accepted)
                .Select(x => new
                {
                    label = x.Localize(),
                    value = (int)x
                });
            return new
            {
                items = projectStatuses.Where(x => x.Status == ProjectStatusType.None),
                systemItems = projectStatuses.Where(x => x.Status == ProjectStatusType.FinalSuccess || x.Status == ProjectStatusType.Canceled),
                statusTypeList = statusTypeList
            };
        }

        public JsonResult GetProjectStatus(int id)
        {
            return JsonOk(ProjectStatusService.Get(id));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProjectStatus(ProjectStatus status, int taskGroupId)
        {
            if (string.IsNullOrWhiteSpace(status.Name))
                return JsonError();

            var projectStatusList = ProjectStatusService.GetList(taskGroupId);

            status.SortOrder = 0;
            status.Color = status.Color.DefaultOrEmpty().Replace("#", "");
            var statusId = ProjectStatusService.Add(status);
            TaskGroupService.AddProjectStatus(taskGroupId, status.Id);

            var resultStatuses = new List<int>(projectStatusList.Count + 1);
            //before
            resultStatuses.AddRange(projectStatusList
                .Where(projectStatus => projectStatus.StatusType <= status.StatusType)
                .Select(projectStatus => projectStatus.Id));
            resultStatuses.Add(status.Id);
            //after
            resultStatuses.AddRange(projectStatusList
                .Where(projectStatus => projectStatus.StatusType > status.StatusType)
                .Select(projectStatus => projectStatus.Id));

            ProjectStatusService.UpdateSortOrder(resultStatuses);

            return JsonOk(statusId);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateProjectStatus(ProjectStatus model)
        {
            var projectStatus = ProjectStatusService.Get(model.Id);
            if (projectStatus == null || string.IsNullOrWhiteSpace(model.Name))
                return JsonError();

            projectStatus.Name = model.Name;
            projectStatus.SortOrder = model.SortOrder;
            projectStatus.Color = model.Color.DefaultOrEmpty().Replace("#", "");
            projectStatus.StatusType = model.StatusType;
            ProjectStatusService.Update(projectStatus);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeProjectStatusSorting(List<ProjectStatus> statusList)
        {
            ProjectStatusService.UpdateSortOrder(statusList.Select(projectStatus => projectStatus.Id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProjectStatus(int id)
        {
            ProjectStatusService.Delete(id);

            return JsonOk();
        }

        #endregion

        #region Copy TaskGroup

        [HttpPost]
        public JsonResult CopyTaskGroup(int taskGroupId, string name, bool copyTasks)
        {
            try
            {
                new CopyTaskGroupHandler(taskGroupId, name, copyTasks).Execute();
                return JsonOk();
            }
            catch (BlException ex)
            {
                return JsonError(ex.Message);
            }
        }


        #endregion

        public JsonResult GetTaskGroupsSelectOptions()
        {
            var taskGroups = TaskGroupService.GetAllTaskGroups();
            return Json(taskGroups.Select(x => new SelectItemModel(x.Name, x.Id.ToString())));
        }
    }
}
