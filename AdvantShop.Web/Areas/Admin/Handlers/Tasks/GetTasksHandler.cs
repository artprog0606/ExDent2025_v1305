using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Tasks;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class GetTasksHandler
    {
        private readonly TasksFilterModel _filterModel;
        private SqlPaging _paging;
        private int _currentManagerId;
        private Customer _currentCustomer;

        public GetTasksHandler(TasksFilterModel filterModel)
        {
            _filterModel = filterModel;
            _currentCustomer = CustomerContext.CurrentCustomer;
            var currentManager = ManagerService.GetManager(_currentCustomer.Id);
            if (currentManager != null)
                _currentManagerId = currentManager.ManagerId;
        }

        public TasksFilterResult Execute()
        {
            var model = new TasksFilterResult();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();

            foreach (TasksPreFilterType preFilter in Enum.GetValues(typeof(TasksPreFilterType)))
            {
                if (preFilter.Ignore())
                    continue;
                var countPaging = new SqlPaging();
                countPaging.From("Customers.Task");
                Filter(countPaging, new TasksFilterModel
                {
                    FilterBy = preFilter,
                    TaskGroupId = _filterModel.TaskGroupId
                });

                int customData = countPaging.GetCustomData("COUNT(Task.Id) as tasksCount", "and IsDeferred = 0", 
                    reader => SQLDataHelper.GetInt(reader, "tasksCount"), true,
                    "Inner Join Customers.ProjectStatus as ProjectStatus On Task.StatusId=ProjectStatus.Id").FirstOrDefault();
                model.TasksCount.Add(preFilter, customData);
            }

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<TaskModel>();

            model.TotalString = LocalizationService.GetResourceFormat("Admin.Tasks.Grid.TotalString", model.TotalItemsCount);

            return model;
        }

        public List<int> GetItemsIds(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<int>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging()
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Task.Id",
                "Task.TaskGroupId",
                "Task.Name",
                "Task.StatusId",
                "Task.Accepted",
                "Task.Priority",
                "Task.DueDate",
                "Task.DateAppointed",
                "Task.ResultFull",
                "AppointedCustomer.CustomerID".AsSqlField("AppointedCustomerId"),
                "AppointedCustomer.FirstName + ' ' + AppointedCustomer.LastName".AsSqlField("AppointedName"),

                (!_currentCustomer.IsAdmin ? "(case when IsPrivateComments = 0 then CRM.GetTaskManagersJson(Task.Id) else '' end )" : "CRM.GetTaskManagersJson(Task.Id)").AsSqlField("ManagersJson"),
                ("(case when ProjectStatus.StatusType = " + (int) TaskStatusType.Completed + " then 1 else 0 end)").AsSqlField("StatusSort"),

                "TaskGroup.Name".AsSqlField("TaskGroupName"),
                "ViewedTask.ViewDate",
                ("(case when ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL then 1 else 0 end)").AsSqlField("Viewed"),
                
                ("(select count(AdminComment.Id) FROM CMS.AdminComment WHERE ObjId = Task.Id AND (Type = 'Task' " + (CustomerContext.CurrentCustomer.IsAdmin ? " OR Type = 'TaskHidden'" : "") + ") " +
                "AND Deleted = 0 AND AdminComment.CustomerId <> '" + _currentCustomer.Id.ToString() + "' " +
                "AND (ViewedTask.ViewDate is null OR AdminComment.DateCreated > ViewedTask.ViewDate))").AsSqlField("NewCommentsCount"),

                //("(case when [AppointedManagerId] = " + _currentManagerId.ToString() + " then 1 else 0 end)").AsSqlField("CanDelete"),
                "AppointedCustomer.Avatar".AsSqlField("AppointedCustomerAvatar")
                );

            _paging.From("[Customers].[Task]");
            _paging.Left_Join("Customers.Managers as AppointedManager ON Task.AppointedManagerId = AppointedManager.ManagerId");
            _paging.Left_Join("Customers.Customer as AppointedCustomer ON AppointedCustomer.CustomerID = AppointedManager.CustomerId");
            _paging.Inner_Join("Customers.TaskGroup ON Task.TaskGroupId = TaskGroup.Id");
            _paging.Left_Join("Customers.ViewedTask ON Task.Id = ViewedTask.TaskId AND ViewedTask.ManagerId = " + _currentManagerId.ToString());
            _paging.Left_Join("Customers.TaskManager ON Task.Id = TaskManager.TaskId AND TaskManager.ManagerId = " + _currentManagerId.ToString());
            _paging.Inner_Join("Customers.ProjectStatus as ProjectStatus ON Task.StatusId = ProjectStatus.Id");
            if (_filterModel.ObjId.IsNotEmpty())
            {
                _paging.Left_Join("[Order].OrderCustomer ON OrderCustomer.OrderId = Task.OrderId");
                _paging.Left_Join("[Order].[Order] ON [Order].OrderId = Task.OrderId");
                _paging.Left_Join("[Order].[Lead] ON [Lead].Id = Task.LeadId");
            }

            // for grouping in grid
            _paging.OrderBy("TaskGroup.SortOrder", "TaskGroupId");
            _paging.Where("Task.IsDeferred = 0");
            
            Filter();
            Sorting();
        }

        private void Filter()
        {
            Filter(_paging, _filterModel);
        }

        private void Filter(SqlPaging paging, TasksFilterModel filterModel)
        {
            switch (filterModel.FilterBy)
            {
                case TasksPreFilterType.AssignedToMe:
                    filterModel.AssignedManagerId = _currentManagerId;
                    filterModel.Accepted = false;
                    filterModel.Canceled = false;
                    break;
                case TasksPreFilterType.AppointedByMe:
                    filterModel.AppointedManagerId = _currentManagerId;
                    filterModel.Accepted = false;
                    filterModel.Canceled = false;
                    break;
                case TasksPreFilterType.Open:
                    filterModel.StatusType = (int)TaskStatusType.Open;
                    break;
                case TasksPreFilterType.InProgress:
                    filterModel.StatusType = (int)TaskStatusType.InProgress;
                    break;
                case TasksPreFilterType.Completed:
                    filterModel.StatusType = (int)TaskStatusType.Completed;
                    break;
                case TasksPreFilterType.Accepted:
                    filterModel.StatusType = (int)TaskStatusType.Accepted;
                    filterModel.Accepted = true;
                    break;
                case TasksPreFilterType.ObservedByMe:
                    filterModel.ObserverId = _currentManagerId;
                    filterModel.Accepted = false;
                    filterModel.Canceled = false;
                    break;
                case TasksPreFilterType.Canceled:
                    filterModel.StatusType = (int)TaskStatusType.Canceled;
                    filterModel.Canceled = true;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(filterModel.Search))
            {
                if (filterModel.Search.IsInt())
                    paging.Where("(Task.Id = {0} OR Task.Name LIKE '%' + {0} + '%' OR Task.Description LIKE '%' + {0} + '%')", filterModel.Search);
                else
                    paging.Where("(Task.Name LIKE '%' + {0} + '%' OR Task.Description LIKE '%' + {0} + '%')", filterModel.Search);
            }

            if (filterModel.FilterBy == TasksPreFilterType.AssignedToMe)
            {
                //TasksPreFilterType.AssignedToMe - все, кроме завершенных, если не выбран статус в фильтре
                if (filterModel.Status != null && filterModel.Status.Status != ProjectStatusType.FinalSuccess && filterModel.Status.Status != ProjectStatusType.Canceled)
                    paging.Where("Task.StatusId = {0}", filterModel.Status.Id);
                else
                    paging.Where("(ProjectStatus.Status != {0} AND ProjectStatus.Status != {1})", (int)ProjectStatusType.FinalSuccess, (int)ProjectStatusType.Canceled);

            }
            else if (filterModel.Status != null)
                paging.Where("Task.StatusId = {0}", filterModel.Status.Id);

            if (filterModel.StatusType.HasValue)
                paging.Where("ProjectStatus.StatusType = {0}", filterModel.StatusType.Value);

            if (filterModel.TaskGroupId.HasValue)
                paging.Where("Task.TaskGroupId = {0}", filterModel.TaskGroupId.Value);

            if (filterModel.Accepted.HasValue)
                paging.Where("Task.Accepted = {0}", filterModel.Accepted.Value);

            if (filterModel.Canceled.HasValue)
            {
                if (filterModel.Canceled.Value)
                    paging.Where("ProjectStatus.Status = {0}", (int)ProjectStatusType.Canceled);
                else
                    paging.Where("ProjectStatus.Status != {0}", (int)ProjectStatusType.Canceled);
            }
                

            if (filterModel.Viewed.HasValue)
            {
                if (filterModel.Viewed.Value)
                    paging.Where("(ViewedTask.ViewDate is not null OR TaskManager.ManagerId IS NULL)");
                else
                    paging.Where("ViewedTask.ViewDate is null AND TaskManager.ManagerId IS NOT NULL");
            }

            if (filterModel.Priority.HasValue)
                paging.Where("Task.Priority = {0}", filterModel.Priority.Value);


            if (filterModel.AppointedManagerId.HasValue)
                paging.Where("Task.AppointedManagerId = {0}", filterModel.AppointedManagerId.Value);

            if (filterModel.AssignedManagerId.HasValue)
            {
                if(filterModel.AssignedManagerId.Value == -1)
                    paging.Where("NOT EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id)");
                else
                    paging.Where("EXISTS(SELECT * FROM Customers.Taskmanager WHERE TaskId = Task.Id AND ManagerId = {0})", filterModel.AssignedManagerId.Value);
            }

            if (filterModel.DueDateFrom.HasValue)
                paging.Where("Task.DueDate >= {0}", filterModel.DueDateFrom.Value);
            if (filterModel.DueDateTo.HasValue)
                paging.Where("Task.DueDate <= {0}", filterModel.DueDateTo.Value);

            if (filterModel.DateCreatedFrom.HasValue)
                paging.Where("Task.DateAppointed >= {0}", filterModel.DateCreatedFrom.Value);
            if (filterModel.DateCreatedTo.HasValue)
                paging.Where("Task.DateAppointed <= {0}", filterModel.DateCreatedTo.Value);

            if (filterModel.ObserverId.HasValue)
                paging.Where("EXISTS(SELECT * FROM Customers.TaskObserver WHERE TaskId = Task.Id AND ManagerId = {0})", filterModel.ObserverId.Value);

            if (filterModel.ObjId.IsNotEmpty())
            {
                switch (filterModel.FilterBy)
                {
                    case TasksPreFilterType.Order:
                        var orderCustomer = filterModel.IntObjId.HasValue ? OrderService.GetOrderCustomer(filterModel.IntObjId.Value) : null;
                        if (orderCustomer != null)
                            paging.Where("(Task.OrderId = {0} OR [Lead].CustomerId = {1} OR Task.CustomerId = {1})", orderCustomer.OrderID, orderCustomer.CustomerID);
                        break;
                    case TasksPreFilterType.Lead:
                        var lead = filterModel.IntObjId.HasValue ? LeadService.GetLead(filterModel.IntObjId.Value) : null;
                        if (lead != null && lead.CustomerId.HasValue)
                            paging.Where("(Task.LeadId = {0} OR Task.CustomerId = {1} OR OrderCustomer.CustomerId = {1})", lead.Id, lead.CustomerId);
                        else if (lead != null)
                            paging.Where("Task.LeadId = {0}", lead.Id);
                        break;
                    case TasksPreFilterType.Customer:
                        if (filterModel.GuidObjId.HasValue)
                            paging.Where("(Task.CustomerId = {0} OR OrderCustomer.CustomerId = {0} OR [Lead].CustomerId = {0})", filterModel.GuidObjId.Value);
                        break;
                }
            }

            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    var managersTaskConstraint = SettingsManager.ManagersTaskConstraint;

                    if (filterModel.TaskGroupId.HasValue)
                    {
                        var group = TaskGroupService.GetTaskGroup(filterModel.TaskGroupId.Value);
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

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                switch (_filterModel.FilterBy)
                {
                    case TasksPreFilterType.AssignedToMe:
                    case TasksPreFilterType.AppointedByMe:
                    case TasksPreFilterType.Order:
                    case TasksPreFilterType.Lead:
                        // задачи в работе выше завершенных
                        _paging.OrderBy("StatusSort");
                        break;
                    default:
                        break;
                }
                _paging.OrderByDesc("Task.DateAppointed");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower()
                .Replace("formatted", string.Empty)
                .Replace("managers", "managersjson");

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sorting);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sorting);
                }
                else
                {
                    _paging.OrderByDesc(sorting);
                }
            }
        }
    }
}