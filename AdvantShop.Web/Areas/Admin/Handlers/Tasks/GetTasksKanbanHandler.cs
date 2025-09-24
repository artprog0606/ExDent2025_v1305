using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Tasks;
using AdvantShop.Web.Infrastructure.Extensions;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class GetTasksKanbanHandler
    {
        private readonly TasksKanbanFilterModel _filter;
        private readonly int _currentManagerId;
        private readonly Customer _currentCustomer;
        private readonly List<ProjectStatus> _projectStatuses;
        private readonly UrlHelper _urlHelper;

        public GetTasksKanbanHandler(TasksKanbanFilterModel filterModel)
        {
            _filter = filterModel;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = ManagerService.GetManager(_currentCustomer.Id);
            if (currentManager != null)
                _currentManagerId = currentManager.ManagerId;
            _projectStatuses = _filter.TaskGroupId.HasValue
                ? ProjectStatusService.GetList(_filter.TaskGroupId.Value)
                : ProjectStatusService.GetDefaultProjectStatuses();
        }

        public TasksKanbanModel Execute()
        {
            var model = new TasksKanbanModel
            {
                Name = "Kanban"
            };

            if (_filter.TaskGroupId != null && !TaskService.CheckAccessByGroup(_filter.TaskGroupId.Value))
                return model;

            model.AssignedToMeTasksCount = TaskService.GetAssignedTasksCount(_currentManagerId, _filter.TaskGroupId);
            model.AppointedByMeTasksCount = TaskService.GetAppointedTasksCount(_currentManagerId, _filter.TaskGroupId);
            model.ObservedByMeTasksCount = TaskService.GetObservedTasksCount(_currentManagerId, _filter.TaskGroupId);
            var statuses = _filter.Status == 0
                ? _projectStatuses.Where(x => x.Status == ProjectStatusType.None).OrderBy(x => x.SortOrder).ToList()
                : _projectStatuses.Where(x => x.Id == _filter.Status).OrderBy(x => x.SortOrder).ToList();

            
            
            if (!_filter.Columns.Any())
            {
                var columns = new List<TasksKanbanColumnFilterModel>();
                if (_filter.TaskGroupId.HasValue)
                {
                    foreach (var status in statuses)
                    {
                        var page = 1;
                        var column = _filter.Columns.FirstOrDefault(x => x.Id == status.Id.ToString());
                        if (column != null)
                            page = column.Page;

                        columns.Add(new TasksKanbanColumnFilterModel(status.Id.ToString()) { Page = page });
                    }
                }
                else
                {
                    columns = new List<TasksKanbanColumnFilterModel>
                    {
                        new TasksKanbanColumnFilterModel(TaskStatusType.Open.ToString()),
                        new TasksKanbanColumnFilterModel(TaskStatusType.InProgress.ToString()),
                        new TasksKanbanColumnFilterModel(TaskStatusType.Completed.ToString())
                    };
                }

                _filter.Columns = columns;
            }
            _filter.Columns.RemoveAll(x => x.Id == TaskStatusType.Accepted.ToString() || x.Id == TaskStatusType.Canceled.ToString());

            foreach (var filterColumn in _filter.Columns)
            {
                var paging = GetPaging(filterColumn);
                if (paging == null)
                    continue;

                var projectStatus = filterColumn.ProjectStatus;
                string name = projectStatus != null
                    ? projectStatus.Name
                    : ((TaskStatusType)Enum.Parse(typeof(TaskStatusType), filterColumn.Id)).Localize();
                var column = new TasksKanbanColumnModel
                {
                    Id = filterColumn.Id,
                    Name = name,
                    ProjectStatusId = projectStatus?.Id,
                    Page = filterColumn.Page,
                    CardsPerColumn = filterColumn.CardsPerColumn,
                    TotalCardsCount = paging.TotalRowsCount,
                    TotalPagesCount = paging.PageCount(paging.TotalRowsCount, filterColumn.CardsPerColumn),
                    StatusType = filterColumn.StatusType
                };
                if (projectStatus != null && projectStatus.Color.IsNotEmpty())
                {
                    column.HeaderStyle.Add("color", "#" + projectStatus.Color);
                    column.CardStyle.Add("border-top-color", "#" + projectStatus.Color);
                }

                if (column.TotalPagesCount >= filterColumn.Page || filterColumn.Page == 1)
                {
                    column.Cards = paging.PageItemsList<TaskKanbanModel>();
                    
                    foreach (var card in column.Cards)
                    {
                        card.TaskGroupUrl = _urlHelper.AbsoluteRouteUrl("Admin_v3_project", new { taskGroupId = card.TaskGroupId });
                    }
                }
                    

                model.Columns.Add(column);
            }

            List<TasksKanbanFinishColumnModel> tasksKanbanFinishColumnModels = new List<TasksKanbanFinishColumnModel>
            {
                new TasksKanbanFinishColumnModel
                {
                    Id = TaskStatusType.Accepted.ToString(),
                    Name = _filter.ShowAcceptedTasks
                        ? "Принятые задачи"
                        : "Принять задачу",
                    CardsPerColumn = 0,
                    Class = "tasks-kanban-column-complete",
                    HeaderStyle = new Dictionary<string, string>() { { "color", "#676a6c" } },
                    StatusType = TaskStatusType.Accepted,
                    Status = ProjectStatusType.FinalSuccess
                },
                new TasksKanbanFinishColumnModel
                {
                    Id = TaskStatusType.Canceled.ToString(),
                    Name = "Отклонённые задачи",
                    CardsPerColumn = 0,
                    Class = "tasks-kanban-column-complete",
                    HeaderStyle = new Dictionary<string, string>() { { "color", "#676a6c" } },
                    StatusType = TaskStatusType.Canceled,
                    Status = ProjectStatusType.Canceled
                },
            };

            foreach (var columnModel in tasksKanbanFinishColumnModels)
            {
                var projectStatus = _projectStatuses.FirstOrDefault(x => x.Status == columnModel.Status);
                int? projectStatusId = projectStatus?.Id;
                columnModel.ProjectStatusId = projectStatusId;

                if (_filter.ShowAcceptedTasks)
                {
                    var columnFilterModel = new TasksKanbanColumnFilterModel(_filter.TaskGroupId.HasValue
                        ? projectStatus?.StatusType.ToString()
                        : columnModel.StatusType.ToString());

                    var acceptedTaskPaging = GetPaging(columnFilterModel);

                    if (acceptedTaskPaging != null)
                    {
                        columnModel.CardsPerColumn = columnFilterModel.CardsPerColumn;
                        columnModel.TotalCardsCount = acceptedTaskPaging.TotalRowsCount;
                        columnModel.TotalPagesCount = acceptedTaskPaging.PageCount(acceptedTaskPaging.TotalRowsCount, columnFilterModel.CardsPerColumn);
                        columnModel.Cards = acceptedTaskPaging.PageItemsList<TaskKanbanModel>();
                        if (projectStatus != null)
                            columnModel.CardStyle.Add("border-top-color", "#" + projectStatus.Color);
                    }

                    model.Columns.Add(columnModel);
                }
                else if (columnModel.Id == TaskStatusType.Accepted.ToString())
                    model.Columns.Add(columnModel);
            }

            return model;
        }

        public List<TaskKanbanModel> GetCards()
        {
            var result = new List<TaskKanbanModel>();
            if (_filter.ColumnId.IsNullOrEmpty() || _filter.Columns.All(x => x.Id != _filter.ColumnId))
                return result;

            var paging = GetPaging(_filter.Columns.FirstOrDefault(x => x.Id == _filter.ColumnId), false);

            return paging != null ? paging.PageItemsList<TaskKanbanModel>() : new List<TaskKanbanModel>();
        }

        private SqlPaging GetPaging(TasksKanbanColumnFilterModel columnFilter, bool allCards = true)
        {
            //var type = columnFilter.Id.TryParseEnum<ETasksKanbanColumn>();
            //if (type == ETasksKanbanColumn.None)
            //    return null;

            var paging = new SqlPaging()
            {
                ItemsPerPage = allCards ? columnFilter.CardsPerColumn * columnFilter.Page : columnFilter.CardsPerColumn,
                CurrentPageIndex = allCards ? 1 : columnFilter.Page
            };

            paging.Select(
                "Task.Id",
                "Task.TaskGroupId",
                "Task.Name",
                "Task.StatusId",
                "Task.Accepted",
                "Task.Priority",
                "Task.DueDate",
                "Task.DateAppointed",
                "Task.LeadId",
                "Task.OrderId",
                "TaskGroup.IsNotBeCompleted",
                "AppointedCustomer.CustomerID".AsSqlField("AppointedCustomerId"),
                "AppointedCustomer.FirstName + ' ' + AppointedCustomer.LastName".AsSqlField("AppointedName"),
                (!_currentCustomer.IsAdmin ? "(case when IsPrivateComments = 0 then CRM.GetTaskManagersJson(Task.Id) else '' end )" : "CRM.GetTaskManagersJson(Task.Id)").AsSqlField("ManagersJson"),
                "TaskGroup.Name".AsSqlField("TaskGroupName"),
                "ViewedTask.ViewDate",
                ("(case when Task.Priority = " + (int)TaskPriority.Critical + " then 0 when Task.Priority = " + (int)TaskPriority.High + " then 1 else 2 end)").AsSqlField("PrioritySort"),
                ("(case when ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL then 1 else 0 end)").AsSqlField("Viewed"),

                ("(select count(AdminComment.Id) FROM CMS.AdminComment WHERE ObjId = Task.Id AND (Type = 'Task' " + (CustomerContext.CurrentCustomer.IsAdmin ? " OR Type = 'TaskHidden'" : "") + ") " +
                "AND Deleted = 0 AND AdminComment.CustomerId <> '" + _currentCustomer.Id.ToString() + "' " +
                "AND (ViewedTask.ViewDate is null OR AdminComment.DateCreated > ViewedTask.ViewDate))").AsSqlField("NewCommentsCount"),

                "AppointedCustomer.Avatar".AsSqlField("AppointedCustomerAvatar")
                );

            paging.From("[Customers].[Task]");
            paging.Left_Join("Customers.Managers as AppointedManager ON Task.AppointedManagerId = AppointedManager.ManagerId");
            paging.Left_Join("Customers.Customer as AppointedCustomer ON AppointedCustomer.CustomerID = AppointedManager.CustomerId");
            paging.Inner_Join("Customers.TaskGroup ON Task.TaskGroupId = TaskGroup.Id");
            paging.Inner_Join("Customers.ProjectStatus as ProjectStatus ON Task.StatusId = ProjectStatus.Id");
            paging.Left_Join("Customers.ViewedTask ON Task.Id = ViewedTask.TaskId AND ViewedTask.ManagerId = " + _currentManagerId.ToString());
            paging.Left_Join("Customers.TaskManager ON Task.Id = TaskManager.TaskId AND TaskManager.ManagerId = " + _currentManagerId.ToString());

            paging.Where("Task.IsDeferred = 0");
            Sorting(paging);
            Filter(paging, columnFilter);

            return paging;
        }

        private void Filter(SqlPaging paging, TasksKanbanColumnFilterModel columnFilter)
        {
            switch (_filter.SelectTasks)
            {
                case ETasksKanbanViewTasks.All:
                    _filter.AppointedManagerId = _filter.AppointedManagerId;
                    _filter.AssignedManagerId = _filter.AssignedManagerId;
                    break;
                case ETasksKanbanViewTasks.AssignedToMe:
                    _filter.AssignedManagerId = _currentManagerId;
                    break;
                case ETasksKanbanViewTasks.AppointedByMe:
                    _filter.AppointedManagerId = _currentManagerId;
                    break;
                case ETasksKanbanViewTasks.ObservedByMe:
                    _filter.ObserverId = _currentManagerId;
                    break;
                default:
                    return;
            }

            if (!string.IsNullOrWhiteSpace(_filter.Search))
            {
                paging.Where(_filter.Search.IsInt()
                    ? "(Task.Id = {0} OR Task.Name LIKE '%' + {0} + '%' OR Task.Description LIKE '%' + {0} + '%')"
                    : "(Task.Name LIKE '%' + {0} + '%' OR Task.Description LIKE '%' + {0} + '%')", _filter.Search);
            }
            if (!string.IsNullOrWhiteSpace(_filter.Name))
            {
                paging.Where("Task.Name LIKE '%' + {0} + '%'", _filter.Search);
            }
            if (columnFilter.ProjectStatus != null)
            {
                _filter.Status = columnFilter.ProjectStatus.Id;
                paging.Where("Task.StatusId = {0}", _filter.Status);
            }
            else
            {
                paging.Where("ProjectStatus.StatusType = {0}", columnFilter.StatusType);
            }
            if (_filter.TaskGroupId.HasValue)
            {
                paging.Where("Task.TaskGroupId = {0}", _filter.TaskGroupId.Value);
            }
            if (_filter.Accepted.HasValue)
            {
                paging.Where("Task.Accepted = {0}", _filter.Accepted.Value);
            }
            if (_filter.Viewed.HasValue)
            {
                paging.Where(_filter.Viewed.Value
                    ? "(ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL)"
                    : "ViewedTask.ViewDate is null AND TaskManager.ManagerId IS NOT NULL");
            }
            if (_filter.Priority.HasValue)
            {
                paging.Where("Task.Priority = {0}", _filter.Priority.Value);
            }
            if (_filter.AppointedManagerId.HasValue)
            {
                paging.Where("Task.AppointedManagerId = {0}", _filter.AppointedManagerId.Value);
            }
            if (_filter.AssignedManagerId.HasValue)
            {
                if (_filter.AssignedManagerId.Value == -1)
                    paging.Where("NOT EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id)");
                else
                    paging.Where("EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id AND ManagerId = {0})", _filter.AssignedManagerId.Value);
            }
            if (_filter.DueDateFrom.HasValue)
            {
                paging.Where("Task.DueDate >= {0}", _filter.DueDateFrom.Value);
            }
            if (_filter.DueDateTo.HasValue)
            {
                paging.Where("Task.DueDate <= {0}", _filter.DueDateTo.Value);
            }
            if (_filter.DateCreatedFrom.HasValue)
            {
                paging.Where("Task.DateAppointed >= {0}", _filter.DateCreatedFrom.Value);
            }
            if (_filter.DateCreatedTo.HasValue)
            {
                paging.Where("Task.DateAppointed <= {0}", _filter.DateCreatedTo.Value);
            }

            if (_filter.ObserverId.HasValue)
            {
                paging.Where("EXISTS(SELECT * FROM Customers.TaskObserver WHERE TaskId = Task.Id AND ManagerId = {0})", _filter.ObserverId.Value);
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    var managersTaskConstraint = SettingsManager.ManagersTaskConstraint;

                    if (_filter.TaskGroupId.HasValue)
                    {
                        var group = TaskGroupService.GetTaskGroup(_filter.TaskGroupId.Value);
                        if (group != null && group.ManagersTaskGroupConstraint != null)
                        {
                            managersTaskConstraint = group.ManagersTaskGroupConstraint.Value;
                        }
                    }

                    if (managersTaskConstraint == ManagersTaskConstraint.Assigned)
                    {
                        paging.Where("EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id AND (ManagerId = {0} OR Task.AppointedManagerId = {0}))", manager.ManagerId);
                    }
                    else if (managersTaskConstraint == ManagersTaskConstraint.AssignedAndFree)
                    {
                        paging.Where("(EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id AND (ManagerId = {0} OR Task.AppointedManagerId = {0})) OR " +
                                     "NOT EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = Task.Id))", manager.ManagerId);
                    }

                    // если у группы нет ролей и участников
                    // или роль участника и пользователя пересекаются
                    // или участник = пользователю
                    paging.Where(
                        "(" +
                            "(" +
                                "(Select Count(*) From [Customers].[TaskGroupManagerRole] Where TaskGroupManagerRole.TaskGroupId = Task.TaskGroupId) = 0 and " +
                                "(Select Count(*) From [Customers].[TaskGroupParticipant] Where TaskGroupParticipant.TaskGroupId = Task.TaskGroupId) = 0) " +
                            " OR Exists ( " +
                                "Select 1 From [Customers].[TaskGroupManagerRole] " +
                                "Where TaskGroupManagerRole.TaskGroupId = Task.TaskGroupId and TaskGroupManagerRole.ManagerRoleId in (Select ManagerRoleId From Customers.ManagerRolesMap Where ManagerRolesMap.[CustomerId] = {0}) " +
                            ")" +
                            " OR Exists ( " +
                                "Select 1 From [Customers].[TaskGroupParticipant] Where TaskGroupParticipant.TaskGroupId = Task.TaskGroupId and TaskGroupParticipant.ManagerId = {1} " +                                
                            ")" +
                        ")",
                        customer.Id,
                        manager.ManagerId);
                }
            }
        }

        private void Sorting(SqlPaging paging)
        {
            paging.OrderBy("PrioritySort");
            paging.OrderBy("Task.SortOrder");
        }
    }
}