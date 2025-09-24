using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Core.Services.Attachments;
using AdvantShop.CMS;
using AdvantShop.Customers;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Localization;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.Modules;

namespace AdvantShop.Core.Services.Crm
{
    public class TaskService
    {
        private static Task GetTaskFromReader(SqlDataReader reader)
        {
            return new Task
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                TaskGroupId = SQLDataHelper.GetInt(reader, "TaskGroupId"),
                AppointedManagerId = SQLDataHelper.GetNullableInt(reader, "AppointedManagerId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                StatusId = SQLDataHelper.GetInt(reader, "StatusId"),
                Accepted = SQLDataHelper.GetBoolean(reader, "Accepted"),
                Priority = (TaskPriority)SQLDataHelper.GetInt(reader, "Priority"),
                DueDate = SQLDataHelper.GetNullableDateTime(reader, "DueDate"),
                LeadId = SQLDataHelper.GetNullableInt(reader, "LeadId"),
                OrderId = SQLDataHelper.GetNullableInt(reader, "OrderId"),
                ReviewId = SQLDataHelper.GetNullableInt(reader, "ReviewId"),
                CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId"),
                BindedTaskId = SQLDataHelper.GetNullableInt(reader, "BindedTaskId"),
                ResultShort = SQLDataHelper.GetString(reader, "ResultShort"),
                ResultFull = SQLDataHelper.GetString(reader, "ResultFull"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
                DateAppointed = SQLDataHelper.GetDateTime(reader, "DateAppointed"),
                IsAutomatic = SQLDataHelper.GetBoolean(reader, "IsAutomatic"),
                IsDeferred = SQLDataHelper.GetBoolean(reader, "IsDeferred"),
                BindedObjectStatus = SQLDataHelper.GetNullableInt(reader, "BindedObjectStatus"),
                Reminder = (TaskReminder)SQLDataHelper.GetInt(reader, "Reminder"),
                Reminded = SQLDataHelper.GetBoolean(reader, "Reminded"),
                BizProcessRuleId = SQLDataHelper.GetNullableInt(reader, "BizProcessRuleId")
            };
        }

        public static Task GetTask(int taskId, int? managerId = null)
        {
            var task = SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM Customers.Task WHERE Id = @Id", CommandType.Text,
                GetTaskFromReader, new SqlParameter("@Id", taskId));

            if (task != null && managerId.HasValue)
                SetTaskViewed(taskId, managerId.Value);
            return task;
        }

        public static IEnumerable<Task> GetAllTasks()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Task>(
                "SELECT * FROM Customers.Task", CommandType.Text, GetTaskFromReader);
        }

        public static IEnumerable<Task> GetTasksByTaskGroupId(int taskGroupId)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Task>(
                "SELECT * FROM Customers.Task WHERE TaskGroupId = @taskGroupId",
                CommandType.Text,
                GetTaskFromReader,
                new SqlParameter("@taskGroupId", taskGroupId));
        }

        public static List<Task> GetDeferredTasks()
        {
            return SQLDataAccess.Query<Task>(
                "SELECT * FROM Customers.Task WHERE IsDeferred = 1 AND DateAppointed <= @dateTo",
                new { dateTo = DateTime.Now }).ToList();
        }

        public static List<Task> DeleteOldDeferredTasks()
        {
            return SQLDataAccess.Query<Task>(
                "DELETE FROM Customers.Task WHERE IsDeferred = 1 AND DateAppointed <= @dateTo",
                new { dateTo = DateTime.Now.AddMonths(-1) }).ToList();
        }
        
        public static void DeleteDeferredTasksByBizProcessRuleId(int bizProcessRuleId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Customers.Task WHERE BizProcessRuleId = @BizProcessRuleId and DateAppointed > @Date",
                CommandType.Text,
                new SqlParameter("@BizProcessRuleId", bizProcessRuleId),
                new SqlParameter("@Date", DateTime.Now));
        }

        public static void ProcessDeferredTasks()
        {
            if (!Saas.SaasDataService.IsEnabledFeature(Saas.ESaasProperty.BizProcess))
                return;

            var tasks = GetDeferredTasks();
            foreach (var task in tasks)
            {
                // не ставить задачу, если у связанной задачи уже сменился статус
                if (task.BindedTaskId.HasValue && task.BindedObjectStatus.HasValue)
                {
                    var bindedTask = GetTask(task.BindedTaskId.Value);
                    if (bindedTask != null &&
                        !CheckBPTaskStatus(bindedTask, (BPTaskStatus)task.BindedObjectStatus.Value))
                    {
                        DeleteTask(task.Id);
                        continue;
                    }
                }

                if (task.DueDate.HasValue)
                    task.DueDate = DateTime.Now.Add(task.DueDate.Value - task.DateAppointed);

                task.DateAppointed = DateTime.Now;
                task.IsDeferred = false;

                UpdateTask(task);

                if (task.LeadId != null)
                    LeadsHistoryService.AddLeadTask(task.LeadId.Value, task, null);
                OnTaskCreated(task);

                BizProcessExecuter.DeferredTaskAdded(task);
            }

            DeleteOldDeferredTasks();
        }

        public static List<Task> GetTasksForAutocomplete(string query)
        {
            if (query.IsDecimal())
            {
                return SQLDataAccess.Query<Task>(
                    "SELECT * FROM [Customers].[Task] " +
                    "WHERE convert(nvarchar,[id]) LIKE '%' + @q + '%' AND IsDeferred = 0 " +
                    "Order by [Task].Id desc",
                    new { q = query }).ToList();
            }

            var translitKeyboard = StringHelper.TranslitToRusKeyboard(query);

            return SQLDataAccess.Query<Task>(
                "SELECT * FROM [Customers].[Task] " +
                "WHERE (convert(nvarchar,[id]) = @q " +
                "OR [Description] LIKE '%' + @q + '%' OR [Name] LIKE '%' + @q + '%' OR [Name] like '%' + @qtr + '%') AND IsDeferred = 0 " +
                "Order by [Task].Id desc",
                new { q = query, qtr = translitKeyboard }).ToList();
        }

        public static int AddTask(Task task, ChangedBy changedBy = null, bool trackChanges = true)
        {
            task.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Customers.Task " +
                "(TaskGroupId, AppointedManagerId, Name, Description, StatusId, Accepted, Priority, DueDate, LeadId, OrderId, CustomerId, ResultShort, ResultFull, " +
                "DateCreated, DateModified, DateAppointed, IsAutomatic, IsDeferred, ReviewId, BindedTaskId, BindedObjectStatus, Reminder, Reminded, BizProcessRuleId) " +
                "VALUES (@TaskGroupId, @AppointedManagerId, @Name, @Description, @StatusId, @Accepted, @Priority, @DueDate, @LeadId, @OrderId, @CustomerId, @ResultShort, @ResultFull, " +
                "@DateNow, @DateNow, @DateAppointed, @IsAutomatic, @IsDeferred, @ReviewId, @BindedTaskId, @BindedObjectStatus, @Reminder, @Reminded, @BizProcessRuleId " +
                "); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@TaskGroupId", task.TaskGroupId),
                new SqlParameter("@AppointedManagerId", task.AppointedManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Name", task.Name),
                new SqlParameter("@Description", task.Description ?? string.Empty),
                new SqlParameter("@StatusId", task.StatusId),
                new SqlParameter("@Accepted", task.Accepted),
                new SqlParameter("@Priority", task.Priority),
                new SqlParameter("@DueDate", task.DueDate ?? (object)DBNull.Value),
                new SqlParameter("@LeadId", task.LeadId ?? (object)DBNull.Value),
                new SqlParameter("@OrderId", task.OrderId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", task.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@ResultShort", task.ResultShort ?? string.Empty),
                new SqlParameter("@ResultFull", task.ResultFull ?? string.Empty),
                new SqlParameter("@DateAppointed", task.DateAppointed),
                new SqlParameter("@IsAutomatic", task.IsAutomatic),
                new SqlParameter("@IsDeferred", task.IsDeferred),
                new SqlParameter("@ReviewId", task.ReviewId ?? (object)DBNull.Value),
                new SqlParameter("@BindedTaskId", task.BindedTaskId ?? (object)DBNull.Value),
                new SqlParameter("@BindedObjectStatus", task.BindedObjectStatus ?? (object)DBNull.Value),
                new SqlParameter("@DateNow", DateTime.Now),
                new SqlParameter("@Reminder", task.Reminder),
                new SqlParameter("@Reminded", task.Reminded),
                new SqlParameter("@BizProcessRuleId", task.BizProcessRuleId ?? (object)DBNull.Value)
            );

            foreach (var managerId in task.ManagerIds)
                AddTaskManager(task.Id, managerId);

            if (task.AppointedManagerId.HasValue)
                SetTaskViewed(task.Id, task.AppointedManagerId.Value);

            if (trackChanges)
                TasksHistoryService.NewTask(task, changedBy);

            return task.Id;
        }

        public static void UpdateTask(Task task, ChangedBy changedBy = null, bool trackChanges = true,
            Task prevTaskState = null)
        {
            if (trackChanges)
                TasksHistoryService.TrackTaskChanges(task, changedBy);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET TaskGroupId=@TaskGroupId, AppointedManagerId=@AppointedManagerId, Name=@Name, Description=@Description, " +
                "StatusId=@StatusId, Accepted=@Accepted, Priority=@Priority, DueDate=@DueDate, LeadId=@LeadId, OrderId=@OrderId, CustomerId=@CustomerId, ResultShort=@ResultShort, ResultFull=@ResultFull, " +
                "DateModified=@DateModified, DateAppointed=@DateAppointed, IsAutomatic=@IsAutomatic, IsDeferred=@IsDeferred, ReviewId=@ReviewId, BindedTaskId=@BindedTaskId, BindedObjectStatus=@BindedObjectStatus, " +
                "Reminder=@Reminder, Reminded=@Reminded WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", task.Id),
                new SqlParameter("@TaskGroupId", task.TaskGroupId),
                new SqlParameter("@AppointedManagerId", task.AppointedManagerId ?? (object)DBNull.Value),
                new SqlParameter("@Name", task.Name),
                new SqlParameter("@Description", task.Description ?? string.Empty),
                new SqlParameter("@StatusId", task.StatusId),
                new SqlParameter("@Accepted", task.Accepted),
                new SqlParameter("@Priority", task.Priority),
                new SqlParameter("@DueDate", task.DueDate ?? (object)DBNull.Value),
                new SqlParameter("@LeadId", task.LeadId ?? (object)DBNull.Value),
                new SqlParameter("@OrderId", task.OrderId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", task.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@ResultShort", task.ResultShort ?? string.Empty),
                new SqlParameter("@ResultFull", task.ResultFull ?? string.Empty),
                new SqlParameter("@DateAppointed", task.DateAppointed),
                new SqlParameter("@IsAutomatic", task.IsAutomatic),
                new SqlParameter("@IsDeferred", task.IsDeferred),
                new SqlParameter("@ReviewId", task.ReviewId ?? (object)DBNull.Value),
                new SqlParameter("@BindedTaskId", task.BindedTaskId ?? (object)DBNull.Value),
                new SqlParameter("@BindedObjectStatus", task.BindedObjectStatus ?? (object)DBNull.Value),
                new SqlParameter("@DateModified", DateTime.Now),
                new SqlParameter("@Reminder", task.Reminder),
                new SqlParameter("@Reminded", task.Reminded)
            );
        }

        public static void DeleteTask(int id, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var task = GetTask(id);
            if (task == null || !CheckAccess(task))
                return;

            OnTaskDeleted(CustomerContext.CurrentCustomer, task);
            AttachmentService.DeleteAttachments<TaskAttachment>(id);
            AdminCommentService.DeleteAdminComments(id, AdminCommentType.Task);

            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Customers.Task WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));

            if (trackChanges)
            {
                TasksHistoryService.DeleteTask(task, changedBy);

                if (task.LeadId != null)
                    LeadsHistoryService.DeleteLeadTask(task.LeadId.Value, task, changedBy);
            }
        }

        public static void ChangeTaskStatus(int id, ProjectStatus status, ChangedBy changedBy = null,
            bool trackChanges = true)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET StatusId = @StatusId WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@StatusId", status.Id), new SqlParameter("@Id", id));

            if (trackChanges)
                TasksHistoryService.TrackTaskStatusChanges(id, status, changedBy);
        }

        public static bool CheckAccessByGroup(int taskGroupId)
        {
            return CheckAccessByGroup(CustomerContext.CurrentCustomer, taskGroupId);
        }

        public static bool CheckAccessByGroup(Customer customer, int taskGroupId)
        {
            return CheckAccessByGroup(customer, taskGroupId, null, null);
        }

        public static bool CheckAccessByGroup(Customer customer, List<int> groupManagerRoleIds,
            List<int> groupParticipantIds)
        {
            return CheckAccessByGroup(customer, 0, null, groupManagerRoleIds, groupParticipantIds);
        }

        public static bool CheckAccessByGroup(Customer customer, int taskGroupId, List<int> managerRoleIds,
            List<int> groupManagerRoleIds, List<int> groupParticipantIds = null)
        {
            if (customer == null || customer.IsAdmin || customer.IsVirtual)
                return true;

            if (taskGroupId != 0)
            {
                groupManagerRoleIds = TaskGroupService.GetTaskGroupManagerRoleIds(taskGroupId);
                groupParticipantIds = TaskGroupService.GetTaskGroupParticipantIds(taskGroupId);
            }

            if ((groupManagerRoleIds == null || groupManagerRoleIds.Count == 0) &&
                (groupParticipantIds == null || groupParticipantIds.Count == 0))
                return true;

            if (!customer.IsModerator)
                return false;

            if (managerRoleIds == null)
                managerRoleIds = ManagerRoleService.GetManagerRoles(customer.Id).Select(x => x.Id).ToList();

            if (groupManagerRoleIds != null && groupManagerRoleIds.Any(x => managerRoleIds.Contains(x)))
                return true;

            var manager = ManagerService.GetManager(customer.Id);
            return manager != null && groupParticipantIds != null &&
                   groupParticipantIds.Any(x => x == manager.ManagerId);
        }

        public static bool CheckAccess(Task task)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsAdmin || customer.IsVirtual)
                return true;

            if (customer.IsModerator)
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && manager.Enabled)
                {
                    var managersTaskConstraint = task.TaskGroup.ManagersTaskGroupConstraint ??
                                                 SettingsManager.ManagersTaskConstraint;

                    if (managersTaskConstraint == ManagersTaskConstraint.All)
                        return CheckAccessByGroup(task.TaskGroupId);

                    if (managersTaskConstraint == ManagersTaskConstraint.Assigned &&
                        (task.ManagerIds.Contains(manager.ManagerId) || task.AppointedManagerId == manager.ManagerId))
                        return CheckAccessByGroup(task.TaskGroupId);

                    if (managersTaskConstraint == ManagersTaskConstraint.AssignedAndFree &&
                        (task.ManagerIds.Contains(manager.ManagerId) || task.ManagerIds.Count == 0 ||
                         task.AppointedManagerId == manager.ManagerId))
                        return CheckAccessByGroup(task.TaskGroupId);
                }
            }

            return false;
        }

        public static bool IsReadonlyTask(Task task)
        {
            var customer = CustomerContext.CurrentCustomer;

            if (customer.IsAdmin || customer.IsVirtual)
                return false;

            if (customer.IsModerator && TaskGroupService.IsPrivateCommentsByTaskId(task.Id))
            {
                var manager = ManagerService.GetManager(customer.Id);
                if (manager != null && (!task.ManagerIds.Contains(manager.ManagerId) ||
                                        task.AppointedManagerId == manager.ManagerId))
                    return false;
                return true;
            }

            return false;
        }

        #region Tasks Count

        /// <summary>
        /// Количество назначенных менеджеру задач
        /// </summary>
        public static int GetAssignedTasksCount(int managerId, int? taskGroupId)
        {
            var sql = "SELECT COUNT(*) FROM [Customers].[Task] t " +
                      "INNER JOIN Customers.TaskManager tm ON tm.TaskId = t.Id " +
                      "INNER JOIN Customers.ProjectStatus ps ON t.[StatusId] = ps.[Id] " +
                      "WHERE tm.ManagerId = @ManagerId AND t.IsDeferred = 0 AND t.Accepted = 0 AND ps.Status <> @FinalSuccessStatus AND ps.Status <> @CanceledStatus " +
                      (taskGroupId.HasValue ? "AND t.TaskGroupId = @TaskGroupId" : string.Empty);

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
                sql += GetSqlScriptForRoleAccess(customer, taskGroupId);

            return SQLDataAccess.ExecuteScalar<int>(sql,
                CommandType.Text,
                new SqlParameter("@ManagerId", managerId),
                new SqlParameter("@TaskGroupId", taskGroupId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", customer.Id),
                new SqlParameter("@FinalSuccessStatus", (int)ProjectStatusType.FinalSuccess),
                new SqlParameter("@CanceledStatus", (int)ProjectStatusType.Canceled)
            );
        }

        /// <summary>
        /// Количество назначенных менеджером задач
        /// </summary>
        public static int GetAppointedTasksCount(int managerId, int? taskGroupId)
        {
            var sql =
                "SELECT COUNT(*) FROM [Customers].[Task] t " +
                "INNER JOIN Customers.ProjectStatus ps ON t.[StatusId] = ps.[Id] " +
                "WHERE AppointedManagerId = @ManagerId AND IsDeferred = 0 AND Accepted = 0 AND ps.Status <> @FinalSuccessStatus AND ps.Status <> @CanceledStatus " +
                (taskGroupId.HasValue ? "AND TaskGroupId = @TaskGroupId" : string.Empty);

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
                sql += GetSqlScriptForRoleAccess(customer, taskGroupId);

            return SQLDataAccess.ExecuteScalar<int>(sql,
                CommandType.Text,
                new SqlParameter("@ManagerId", managerId),
                new SqlParameter("@TaskGroupId", taskGroupId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", customer.Id),
                new SqlParameter("@FinalSuccessStatus", (int)ProjectStatusType.FinalSuccess),
                new SqlParameter("@CanceledStatus", (int)ProjectStatusType.Canceled)
            );
        }

        public static int GetOpenTasksCount(int managerId)
        {
            var sql =
                "SELECT COUNT(t.Id) FROM Customers.Task t " +
                "INNER JOIN Customers.TaskManager tm ON tm.TaskId = t.Id AND tm.ManagerId = @ManagerId " +
                "INNER JOIN Customers.ProjectStatus ps ON t.StatusId = ps.Id " +
                "WHERE ps.Status <> @FinalSuccessStatus AND ps.Status <> @CanceledStatus AND t.IsDeferred = 0";

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
                sql += GetSqlScriptForRoleAccess(customer, null);

            return SQLDataAccess.ExecuteScalar<int>(sql,
                CommandType.Text,
                new SqlParameter("@ManagerId", managerId),
                new SqlParameter("@FinalSuccessStatus", (int)ProjectStatusType.FinalSuccess),
                new SqlParameter("@CanceledStatus", (int)ProjectStatusType.Canceled),
                new SqlParameter("@CustomerId", customer.Id)
            );
        }

        private static string GetSqlScriptForRoleAccess(Customer customer, int? taskGroupId)
        {
            var sql = "";

            var manager = ManagerService.GetManager(customer.Id);
            if (manager != null && manager.Enabled)
            {
                var managersTaskConstraint = SettingsManager.ManagersTaskConstraint;

                if (taskGroupId != null)
                {
                    var group = TaskGroupService.GetTaskGroup(taskGroupId.Value);
                    if (group != null && group.ManagersTaskGroupConstraint != null)
                    {
                        managersTaskConstraint = group.ManagersTaskGroupConstraint.Value;
                    }
                }

                if (managersTaskConstraint == ManagersTaskConstraint.Assigned)
                {
                    sql +=
                        " and EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = t.Id AND (ManagerId = @ManagerId OR t.AppointedManagerId = @ManagerId))";
                }
                else if (managersTaskConstraint == ManagersTaskConstraint.AssignedAndFree)
                {
                    sql +=
                        " and (EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = t.Id AND (ManagerId = @ManagerId OR t.AppointedManagerId = @ManagerId)) OR " +
                        "NOT EXISTS(SELECT * FROM Customers.TaskManager WHERE TaskId = t.Id))";
                }

                // если у группы нет ролей и участников
                // или роль участника и пользователя пересекаются
                // или участник = пользователю
                sql +=
                    " and (" +
                    "(" +
                    "(Select Count(*) From [Customers].[TaskGroupManagerRole] Where TaskGroupManagerRole.TaskGroupId = t.TaskGroupId) = 0 and " +
                    "(Select Count(*) From [Customers].[TaskGroupParticipant] Where TaskGroupParticipant.TaskGroupId = t.TaskGroupId) = 0) " +
                    " OR Exists ( " +
                    "Select 1 From [Customers].[TaskGroupManagerRole] " +
                    "Where TaskGroupManagerRole.TaskGroupId = t.TaskGroupId and TaskGroupManagerRole.ManagerRoleId in (Select ManagerRoleId From Customers.ManagerRolesMap Where ManagerRolesMap.[CustomerId] = @CustomerId) " +
                    ")" +
                    " OR Exists ( " +
                    "Select 1 From [Customers].[TaskGroupParticipant] Where TaskGroupParticipant.TaskGroupId = t.TaskGroupId and TaskGroupParticipant.ManagerId = @ManagerId " +
                    ")" +
                    ")";
            }

            return sql;
        }

        /// <summary>
        /// Количество наблюдаемых менеджером задач
        /// </summary>
        public static int GetObservedTasksCount(int managerId, int? taskGroupId)
        {
            var sql = "SELECT COUNT(*) FROM [Customers].[Task] t " +
                      "INNER JOIN Customers.TaskObserver tobs ON tobs.TaskId = t.Id " +
                      "WHERE tobs.ManagerId = @ManagerId AND t.IsDeferred = 0 AND t.Accepted = 0 " +
                      (taskGroupId.HasValue ? "AND t.TaskGroupId = @TaskGroupId" : string.Empty);

            var customer = CustomerContext.CurrentCustomer;
            if (customer.IsModerator)
                sql += GetSqlScriptForRoleAccess(customer, taskGroupId);

            return SQLDataAccess.ExecuteScalar<int>(sql,
                CommandType.Text,
                new SqlParameter("@ManagerId", managerId),
                new SqlParameter("@TaskGroupId", taskGroupId ?? (object)DBNull.Value),
                new SqlParameter("@CustomerId", customer.Id)
            );
        }

        #endregion

        public static void SetTaskViewed(int taskId, int managerId, bool viewed = true)
        {
            if (!viewed)
            {
                SQLDataAccess.ExecuteNonQuery(
                    "DELETE FROM Customers.ViewedTask WHERE TaskId = @TaskId AND ManagerId = @ManagerId",
                    CommandType.Text, new SqlParameter("@TaskId", taskId), new SqlParameter("@ManagerId", managerId));
            }
            else
            {
                SQLDataAccess.ExecuteNonQuery(
                    @"IF(SELECT COUNT(TaskId) FROM Customers.ViewedTask WHERE TaskId = @TaskId AND ManagerId = @ManagerId) > 0 " +
                    "UPDATE Customers.ViewedTask SET ViewDate = @ViewDate WHERE TaskId = @TaskId AND ManagerId = ManagerId " +
                    "ELSE " +
                    "INSERT INTO Customers.ViewedTask (TaskId, ManagerId, ViewDate) VALUES (@TaskId, @ManagerId, @ViewDate)",
                    CommandType.Text,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@ManagerId", managerId),
                    new SqlParameter("@ViewDate", DateTime.Now));
            }
        }

        public static void SetTaskAccepted(int id, ChangedBy changedBy = null, bool trackChanges = true)
        {
            var list = ProjectStatusService.GetListByTaskId(id);
            var status = list.FirstOrDefault(x => x.Status == ProjectStatusType.FinalSuccess);

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET StatusId = @StatusId, Accepted = 1 WHERE Id = @Id",
                CommandType.Text, new SqlParameter("@Id", id), new SqlParameter("@StatusId", status.Id));

            if (trackChanges)
                TasksHistoryService.TrackTaskStatusChanges(id, status, changedBy);
        }

        public static void UnassignTaskManager(int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskManager Where ManagerId = @ManagerId",
                CommandType.Text, new SqlParameter("@ManagerId", managerId));
        }

        public static void ClearTaskAppointedManager(int managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET AppointedManagerId = NULL WHERE AppointedManagerId = @ManagerId",
                CommandType.Text, new SqlParameter("@ManagerId", managerId));
        }

        public static List<Task> GetTaskByCustomerId(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList<Task>(
                "SELECT * FROM Customers.Task where CustomerId=@CustomerId", CommandType.Text, GetTaskFromReader,
                new SqlParameter("@CustomerId", customerId));
        }

        public static void ChangeTaskSorting(int id, int? prevId, int? nextId)
        {
            SQLDataAccess.ExecuteNonQuery("CRM.ChangeTaskSorting", CommandType.StoredProcedure,
                new SqlParameter("@Id", id),
                new SqlParameter("@prevId", prevId ?? (object)DBNull.Value),
                new SqlParameter("@nextId", nextId ?? (object)DBNull.Value));
        }

        public static bool CheckBPTaskStatus(Task task, BPTaskStatus status)
        {
            return (status == BPTaskStatus.Accepted && task.Accepted) || (status == (BPTaskStatus)task.StatusId);
        }

        public static BPTaskStatus GetBPTaskStatus(Task task)
        {
            return task.Accepted ? BPTaskStatus.Accepted : (BPTaskStatus)task.StatusId;
        }

        public static IEnumerable<Task> GetTasksWithReminder()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<Task>(
                "SELECT * FROM Customers.Task WHERE Reminder != 0 AND Reminded = 0 AND Accepted = 0 AND StatusId NOT IN (@StatusCompleted, @StatusAccepted)",
                CommandType.Text, GetTaskFromReader,
                new SqlParameter("@StatusCompleted", BPTaskStatus.Completed),
                new SqlParameter("@StatusAccepted", BPTaskStatus.Accepted));
        }

        #region TaskManager

        public static List<int> GetTaskManagerIds(int taskId)
        {
            return SQLDataAccess.Query<int>("SELECT ManagerId FROM Customers.TaskManager WHERE TaskId = @TaskId",
                new { taskId }).ToList();
        }

        public static void ClearTaskManager(int taskId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskManager WHERE TaskId = @TaskId", CommandType.Text,
                new SqlParameter("@TaskId", taskId));
        }

        public static void AddTaskManager(int taskId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Customers.TaskManager (TaskId, ManagerId) VALUES (@TaskId, @ManagerId)",
                CommandType.Text,
                new SqlParameter("@TaskId", taskId),
                new SqlParameter("@ManagerId", managerId));
        }

        public static void SetTaskManagers(int taskId, List<int> managerIds)
        {
            ClearTaskManager(taskId);

            if (managerIds is null)
                return;

            foreach (var managerId in managerIds)
                AddTaskManager(taskId, managerId);
        }

        #endregion

        public static void SetAppointedManager(int taskId, int? appointedManagerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET AppointedManagerId=@AppointedManagerId WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", taskId),
                new SqlParameter("@AppointedManagerId", appointedManagerId ?? (object)DBNull.Value));
        }

        public static void SetDueDate(int taskId, DateTime? date)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET DueDate=@DueDate WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", taskId),
                new SqlParameter("@DueDate", date ?? (object)DBNull.Value));
        }        
        
        public static void SetReminder(int taskId, TaskReminder reminder)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET Reminder=@Reminder WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", taskId),
                new SqlParameter("@Reminder", reminder));
        }

        public static void SetPriority(int taskId, TaskPriority priority)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.Task SET Priority=@Priority WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", taskId),
                new SqlParameter("@Priority", priority));
        }
        
        #region TaskObserver

        public static List<int> GetTaskObserverIds(int taskId)
        {
            return SQLDataAccess.Query<int>("SELECT ManagerId FROM Customers.TaskObserver WHERE TaskId = @TaskId",
                new { taskId }).ToList();
        }

        public static void AddTaskObserver(int taskId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO Customers.TaskObserver (TaskId, ManagerId) VALUES (@TaskId, @ManagerId)",
                CommandType.Text,
                new SqlParameter("@TaskId", taskId),
                new SqlParameter("@ManagerId", managerId));
        }

        public static void ClearTaskObserver(int taskId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskObserver WHERE TaskId = @TaskId", CommandType.Text,
                new SqlParameter("@TaskId", taskId));
        }

        public static void SetTaskObservers(int taskId, List<int> managerIds)
        {
            ClearTaskObserver(taskId);

            if (managerIds is null)
                return;

            foreach (var managerId in managerIds)
                AddTaskObserver(taskId, managerId);
        }

        #endregion

        #region Task Events

        public static void OnTaskCreated(Task task)
        {
            if (task.IsDeferred)
                return;
            // уведомление исполнителю
            if (task.Managers.Any())
            {
                var mailTemplate = new TaskAssignedMailTemplate(task);
                SendMails(mailTemplate, task.Managers.ToDictionary(x => x.CustomerId, x => x.Customer));

                foreach (var manager in task.Managers)
                {
                    ModulesExecuter.DoTaskAdded(task, manager.Customer);
                }
            }

            var mailTplTaskCreated = new TaskCreatedMailTemplate(task);

            NotifyWatchers(task.TaskGroupId, mailTplTaskCreated, task.Managers.Select(x => x.CustomerId).ToList());
        }

        public static void OnTaskDeleted(Customer modifier, Task task)
        {
            if (task.IsDeferred)
                return;

            var mailTemplate = new TaskDeletedMailTemplate(task,
                modifier: modifier.FirstName + " " + modifier.LastName);
            mailTemplate.BuildMail();

            var customersToNotify = task.Managers.ToDictionary(x => x.CustomerId, x => x.Customer);
            if (task.AppointedManager != null)
                customersToNotify.TryAddValue(task.AppointedManager.CustomerId, task.AppointedManager.Customer);

            foreach (var item in task.Observers.ToDictionary(x => x.CustomerId, x => x.Customer))
            {
                customersToNotify.TryAddValue(item.Key, item.Value);
            }

            SendMails(mailTemplate, customersToNotify);
        }

        public static void OnTaskCommentAdded(AdminComment comment, Task task)
        {
            if (task.IsDeferred)
                return;

            var customersToNotify = task.Managers.ToDictionary(x => x.CustomerId, x => x.Customer);
            if (task.AppointedManager != null)
                customersToNotify.TryAddValue(task.AppointedManager.CustomerId, task.AppointedManager.Customer);

            AdminComment parentComment;
            if (comment.ParentId.HasValue &&
                (parentComment = AdminCommentService.GetAdminComment(comment.ParentId.Value)) != null &&
                parentComment.Customer != null)
                customersToNotify.TryAddValue(parentComment.Customer.Id, parentComment.Customer);

            foreach (var item in task.Observers.ToDictionary(x => x.CustomerId, x => x.Customer))
            {
                customersToNotify.TryAddValue(item.Key, item.Value);
            }

            DelayHelper.Wait(1, DelayType.Minutes, () =>
            {
                var retrievedComment = AdminCommentService.GetAdminComment(comment.Id);

                if (retrievedComment == null || retrievedComment.Deleted) 
                    return;
                
                var mailTemplate = new TaskCommentAddedMailTemplate(task,
                    author: retrievedComment.Name,
                    comment: HttpUtility.HtmlEncode(retrievedComment.Text).Replace("\n", "<br/>"));
                mailTemplate.BuildMail();
                    
                SendMails(mailTemplate, customersToNotify);
                    
                foreach (var manager in customersToNotify.Values)
                    ModulesExecuter.DoTaskCommentAdded(task, retrievedComment, manager);

                NotifyWatchers(task.TaskGroupId, mailTemplate, customersToNotify.Keys.ToList());
            });
        }

        public static void OnTaskChanged(Customer modifier, Task old, Task modified, bool isNotifyRestricted = false)
        {
            if (modified.IsDeferred)
                return;
            if (old.StatusId != modified.StatusId || old.Accepted != modified.Accepted)
                BizProcessExecuter.TaskStatusChanged(modified);

            if (modified.ObserverIds.Any())
            {
                var newObserver = modified.Observers.Where(x => !old.ObserverIds.Contains(x.ManagerId))
                    .FirstOrDefault();
                if (newObserver != null)
                {
                    var managersToNotify = new Dictionary<Guid, Customer>();
                    // оповестить постановщика задачи, если он не является тем, кто добавил наблюдателя
                    if (modified.AppointedManager.CustomerId != modifier.Id)
                        managersToNotify.Add(modified.AppointedManager.CustomerId, modified.AppointedManager.Customer);
                    // оповестить наблюдателя, если он не является тем, кто добавил наблюдателя или не является постановщиком задачи
                    if (newObserver.CustomerId != modifier.Id &&
                        newObserver.CustomerId != modified.AppointedManager.CustomerId)
                        managersToNotify.Add(newObserver.CustomerId, newObserver.Customer);

                    if (managersToNotify.Any())
                    {
                        var mailTpl = new TaskObserverAddedMailTemplate(modified, newObserver.FullName);
                        mailTpl.BuildMail();
                        SendMails(mailTpl, managersToNotify);
                    }
                }
            }

            var changesTable = BuildTaskChangesTable(modifier, old, modified);
            if (changesTable.IsNullOrEmpty())
                return;
            var mailTemplate = new TaskChangedMailTemplate(modified,
                changesTable: changesTable,
                modifier: modifier.FirstName + " " + modifier.LastName,
                taskPrev: old);
            mailTemplate.BuildMail();

            var customersToNotify = new Dictionary<Guid, Customer>();
            var notifiedCustomerIds = new List<Guid>();

            if (modified.Managers.Any())
            {
                // новые исполнители
                var newManagers = modified.Managers.Where(x => !old.ManagerIds.Contains(x.ManagerId)).ToList();
                // сменился исполнитель - новому исполнителю письмо о назначенной задаче
                if (newManagers.Any())
                {
                    notifiedCustomerIds.AddRange(newManagers.Select(x => x.CustomerId));

                    var mailTpl = new TaskAssignedMailTemplate(modified);
                    mailTpl.BuildMail();
                    SendMails(mailTpl, newManagers.ToDictionary(x => x.CustomerId, x => x.Customer));

                    foreach (var manager in newManagers)
                    {
                        ModulesExecuter.DoTaskAdded(modified, manager.Customer);
                    }
                }
                else
                    customersToNotify = modified.Managers.ToDictionary(x => x.CustomerId, x => x.Customer);
            }

            if (modified.AppointedManager != null)
                customersToNotify.TryAddValue(modified.AppointedManager.CustomerId, modified.AppointedManager.Customer);

            // предыдущие исполнители
            var oldManagers = old.Managers.Where(x => !modified.ManagerIds.Contains(x.ManagerId)).ToList();
            // при смене постановщика или исполнителя оповестить предыдущего
            if (oldManagers.Any())
                oldManagers.ForEach((manager) => customersToNotify.TryAddValue(manager.CustomerId, manager.Customer));
            if (old.AppointedManager != null && old.AppointedManagerId != modified.AppointedManagerId)
                customersToNotify.TryAddValue(old.AppointedManager.CustomerId, old.AppointedManager.Customer);

            foreach (var item in modified.Observers.ToDictionary(x => x.CustomerId, x => x.Customer))
            {
                customersToNotify.TryAddValue(item.Key, item.Value);
            }

            SendMails(mailTemplate, customersToNotify, notifiedCustomerIds);

            foreach (var customer in customersToNotify)
            {
                ModulesExecuter.DoTaskChanged(old, modified, customer.Value);
            }

            notifiedCustomerIds.AddRange(customersToNotify.Keys);

            // наблюдателям только при изменении описания (task 7.0.4-10687)
            if (old.Description != modified.Description)
            {
                if (old.TaskGroupId != modified.TaskGroupId)
                    notifiedCustomerIds.AddRange(NotifyWatchers(old.TaskGroupId, mailTemplate, notifiedCustomerIds,
                        isNotifyRestricted));
                NotifyWatchers(modified.TaskGroupId, mailTemplate, notifiedCustomerIds, isNotifyRestricted);
            }
        }

        public static void OnTaskReminder(Task task)
        {
            var mailTemplate = new TaskReminderMailTemplate(task);

            SendMails(mailTemplate, task.Managers.ToDictionary(x => x.CustomerId, x => x.Customer));
        }

        private static void SendMails(MailTemplate mailTemplate, Dictionary<Guid, Customer> customersToNotify, List<Guid> excludingCustomerIds = null)
        {
            var temp =
                customersToNotify.Where(
                        x => 
                            (CustomerContext.CurrentCustomer == null || x.Key != CustomerContext.CustomerId) 
                             && x.Value != null 
                             && x.Value.Enabled 
                             && x.Value.HasRoleAction(RoleAction.Tasks) 
                             && (excludingCustomerIds == null || !excludingCustomerIds.Contains(x.Key)))
                    .ToList();
            
            foreach (var item in temp)
            {
                MailService.SendMailNow(item.Key, item.Value.EMail, mailTemplate, lettercount: temp.Count, logging: false);
            }
        }

        private static List<Guid> NotifyWatchers(int taskGroupId, MailTemplate mailTemplate,
            List<Guid> excludingCustomerIds, bool isNotifyRestricted = false)
        {
            var watchers = TaskGroupService.GetTaskGroupManagers(taskGroupId, true)
                .Where(x => !excludingCustomerIds.Contains(x.CustomerId) && (!isNotifyRestricted || x.Customer.IsAdmin))
                .ToList();
            if (watchers.Count == 0)
                return new List<Guid>();

            var customersToNotify = new Dictionary<Guid, Customer>();

            foreach (var manager in watchers)
                customersToNotify.TryAddValue(manager.CustomerId, manager.Customer);

            SendMails(mailTemplate, customersToNotify);

            return new List<Guid>(customersToNotify.Keys);
        }

        #region TaskChangesTable

        private static string BuildTaskChangesTable(Customer modifier, Task old, Task modified)
        {
            var sbRows = new StringBuilder();
            if (old.Name.DefaultOrEmpty() != modified.Name.DefaultOrEmpty())
                sbRows.Append(GetHtmlRow(LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Name"),
                    StringHelper.GenerateDiffHtml(old.Name, modified.Name)));

            var oldManagerIds = old.ManagerIds.Except(modified.ManagerIds).ToList();
            var newManagerIds = modified.ManagerIds.Except(old.ManagerIds).ToList();
            if (oldManagerIds.Any() || newManagerIds.Any())
            {
                var allManagers = old.Managers.Union(modified.Managers).OrderBy(x => x.FullName).ToList();
                var managersNames = new List<string>();
                foreach (var manager in allManagers)
                {
                    if (oldManagerIds.Contains(manager.ManagerId))
                        managersNames.Add(GetHtmlOldValue(manager.FullName));
                    else if (newManagerIds.Contains(manager.ManagerId))
                        managersNames.Add(GetHtmlNewValue(manager.FullName));
                    else
                        managersNames.Add(string.Format("<span>{0}</span>", manager.FullName));
                }

                sbRows.Append(GetHtmlRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.AssignedManager"),
                    managersNames.AggregateString(", ")));
            }

            if (old.AppointedManagerId != modified.AppointedManagerId)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.AppointedManager"),
                    old.AppointedManager != null ? old.AppointedManager.FullName : string.Empty,
                    modified.AppointedManager != null ? modified.AppointedManager.FullName : string.Empty));
            if (old.TaskGroupId != modified.TaskGroupId)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.TaskGroup"),
                    old.TaskGroup.Name, modified.TaskGroup.Name));
            if (old.StatusId != modified.StatusId)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Status"),
                    old.StatusName, modified.StatusName));
            else if (!old.Accepted && modified.Accepted)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Status"),
                    old.StatusName, LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Accepted")));
            if (old.ResultFull.DefaultOrEmpty() != modified.ResultFull.DefaultOrEmpty())
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Result"),
                    old.ResultFull, modified.ResultFull));
            if (old.DueDate != modified.DueDate)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.DueDate"),
                    old.DueDate.HasValue ? Culture.ConvertShortDate(old.DueDate.Value) : null,
                    modified.DueDate.HasValue ? Culture.ConvertShortDate(modified.DueDate.Value) : null));
            if (old.Priority != modified.Priority)
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Priority"),
                    old.Priority.Localize(), modified.Priority.Localize()));

            var deletedAttachments = old.Attachments.Select(x => x.FileName)
                .Where(x => !modified.Attachments.Select(y => y.FileName).Contains(x)).ToList();
            var newAttachments = modified.Attachments
                .Where(x => !old.Attachments.Select(y => y.FileName).Contains(x.FileName))
                .Select(x => GetLinkHTML(x.Path, x.FileName)).ToList();
            if (deletedAttachments.Any() || newAttachments.Any())
                sbRows.Append(GetHtmlChangesRow(
                    LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.Attachments"),
                    deletedAttachments.AggregateString(", "), newAttachments.AggregateString(", ")));

            if (old.Description.DefaultOrEmpty() != modified.Description.DefaultOrEmpty())
                sbRows.AppendFormat("<tr><td colspan='2' style='padding: 10px 0;'>{0}</td></tr>",
                    StringHelper.GenerateDiffHtml(old.Description, modified.Description));

            var rowsHtml = sbRows.ToString();
            if (rowsHtml.IsNullOrEmpty())
                return string.Empty;

            return string.Format("<table><tr>{0}</tr>{1}</table>",
                GetHtmlRow(LocalizationService.GetResource("Core.Services.Crm.Task.ChangesTable.ModifiedBy"),
                    modifier.FirstName + " " + modifier.LastName),
                rowsHtml);
        }

        private static string GetHtmlRow(string fieldName, string value)
        {
            return string.Format("<tr><td style='color: #acacac; padding: 5px 15px 5px 0;'>{0}:</td><td>{1}</td></tr>",
                fieldName, value);
        }

        private static string GetHtmlChangesRow(string fieldName, string oldValue, string newValue)
        {
            return GetHtmlRow(fieldName,
                string.Format("{0} {1}", GetHtmlOldValue(oldValue), GetHtmlNewValue(newValue)));
        }

        private static string GetHtmlNewValue(string value)
        {
            return value.IsNotEmpty()
                ? string.Format("<span style='background-color:#ddfade;'>{0}</span>", value)
                : string.Empty;
        }

        private static string GetHtmlOldValue(string value)
        {
            return value.IsNotEmpty()
                ? string.Format("<span style='background-color:#ffe7e7;text-decoration:line-through;'>{0}</span>",
                    value)
                : string.Empty;
        }

        private static string GetLinkHTML(string url, string name)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, name);
        }

        #endregion

        #endregion
    }
}