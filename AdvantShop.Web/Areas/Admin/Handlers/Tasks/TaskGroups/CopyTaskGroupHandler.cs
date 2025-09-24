using AdvantShop.CMS;
using AdvantShop.Core;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using System;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class CopyTaskGroupHandler
    {
        private int _id;
        private string _name;
        private bool _copyTasks;

        private Dictionary<int, int> ProjectStatusReferences;

        public CopyTaskGroupHandler(int id, string name, bool copyTasks)
        {
            _id = id;
            _name = name;
            _copyTasks = copyTasks;
            ProjectStatusReferences = new Dictionary<int, int>();
        }

        public void Execute()
        {
            var taskGroup = TaskGroupService.GetTaskGroup(_id);
            if (taskGroup == null)
                throw new BlException("Проект не найден");

            var newTaskGroup = new TaskGroup
            {
                Name = _name,
                SortOrder = taskGroup.SortOrder,
                DateCreated = DateTime.Now,
                Enabled = taskGroup.Enabled,
                IsNotBeCompleted = taskGroup.IsNotBeCompleted,
                IsPrivateComments = taskGroup.IsPrivateComments,
                ManagersTaskGroupConstraint = taskGroup.ManagersTaskGroupConstraint,
            };

            int newId = TaskGroupService.AddTaskGroup(newTaskGroup);
            if (newId == 0)
                throw new BlException("Не удалось добавить проект");

            AddProjectStatuses(_id, newId);
            AddManagerRoles(_id, newId);
            AddManagers(_id, newId);
            AddParticipants(_id, newId);

            if (_copyTasks)
                AddTasks(_id, newId);
        }

        private void AddProjectStatuses(int oldTaskGroupId, int newTaskGroupId)
        {
            var projectStatuses = ProjectStatusService.GetList(oldTaskGroupId);
            foreach (var status in projectStatuses)
            {
                int oldStatusId = status.Id;
                int newStatusId = ProjectStatusService.Add(status);
                TaskGroupService.AddProjectStatus(newTaskGroupId, newStatusId);

                if (_copyTasks)
                    ProjectStatusReferences.Add(oldStatusId, newStatusId);
            }
        }

        private void AddManagers(int oldTaskGroupId, int newTaskGroupId)
        {
            var managerIds = TaskGroupService.GetTaskGroupManagerIds(oldTaskGroupId);
            foreach (var managerId in managerIds)
            {
                TaskGroupService.AddTaskGroupManager(newTaskGroupId, managerId);
            }
        }

        private void AddManagerRoles(int oldTaskGroupId, int newTaskGroupId)
        {
            var managerIds = TaskGroupService.GetTaskGroupManagerRoleIds(oldTaskGroupId);
            foreach (var managerId in managerIds)
            {
                TaskGroupService.AddTaskGroupManagerRole(newTaskGroupId, managerId);
            }
        }

        private void AddParticipants(int oldTaskGroupId, int newTaskGroupId)
        {
            var participantIds = TaskGroupService.GetTaskGroupParticipantIds(oldTaskGroupId);
            foreach (var participantId in participantIds)
            {
                TaskGroupService.AddTaskGroupParticipant(newTaskGroupId, participantId);
            }
        }

        private void AddTasks(int oldTaskGroupId, int newTaskGroupId)
        {
            var tasks = TaskService.GetTasksByTaskGroupId(oldTaskGroupId);

            foreach (var item in tasks)
            {
                var task = item.DeepClone() as Task;
                task.TaskGroupId = newTaskGroupId;
                task.SetManagerIds(item.ManagerIds);

                if (ProjectStatusReferences.ContainsKey(item.StatusId))
                    task.StatusId = ProjectStatusReferences[item.StatusId];
                else
                    continue;

                int newTaskId = TaskService.AddTask(task);

                foreach (var attachment in task.Attachments)
                {
                    attachment.ObjId = newTaskId;
                    AttachmentService.AddAttachment(attachment);
                }

                foreach (var observerId in task.ObserverIds)
                    TaskService.AddTaskObserver(newTaskId, observerId);

                var comments = AdminCommentService.GetAdminComments(item.Id, AdminCommentType.Task);
                foreach (var comment in comments)
                {
                    comment.ObjId = newTaskId;
                    AdminCommentService.AddAdminComment(comment);
                }
            }
        }
    }
}
