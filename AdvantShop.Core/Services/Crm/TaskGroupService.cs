using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Caching;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Crm
{
    public class TaskGroupService
    {
        private const string TaskGroupManagerRoleIdsCacheKey = "TaskGroupManagerRoleIds_";

        private static TaskGroup GetTaskGroupFromReader(SqlDataReader reader)
        {
            var managersTaskGroupConstraint = SQLDataHelper.GetNullableInt(reader, "ManagersTaskGroupConstraint");

            return new TaskGroup
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                IsPrivateComments = SQLDataHelper.GetBoolean(reader, "IsPrivateComments"),
                ManagersTaskGroupConstraint =
                    managersTaskGroupConstraint != null
                        ? (ManagersTaskConstraint) managersTaskGroupConstraint.Value
                        : default(ManagersTaskConstraint?),
                IsNotBeCompleted = SQLDataHelper.GetBoolean(reader, "IsNotBeCompleted")
            };
        }

        public static TaskGroup GetTaskGroup(int TaskGroupId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM Customers.TaskGroup WHERE Id = @Id", CommandType.Text,
                GetTaskGroupFromReader, new SqlParameter("@Id", TaskGroupId));
        }

        public static List<TaskGroup> GetAllTaskGroups()
        {
            return SQLDataAccess
                .ExecuteReadList("SELECT * FROM Customers.TaskGroup where Enabled=1 ORDER BY SortOrder", CommandType.Text, GetTaskGroupFromReader)
                .Where(x => TaskService.CheckAccessByGroup(x.Id))
                .ToList();
        }

        public static TaskGroup GetByProjectStatus(int projectStatusId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select TOP 1 tg.* From Customers.TaskGroup tg INNER JOIN Customers.TaskGroup_ProjectStatus tgps ON tgps.TaskGroupId = tg.Id Where ProjectStatusId = @projectStatusId",
                CommandType.Text, GetTaskGroupFromReader,
                new SqlParameter("@projectStatusId", projectStatusId));
        }

        public static List<TaskGroup> GetRecentTaskGroups(int count, int? managerId = null)
        {
            return SQLDataAccess.ExecuteReadList<TaskGroup>(
                    "SELECT TOP(@count) tg.*, " +
                        "(select CASE WHEN Date1 > Date2 THEN Date1 ELSE Date2 END " +
                        "FROM (SELECT MAX(vt.ViewDate) as Date1, MAX(ch.ModificationTime) as Date2 " +
                            "FROM Customers.Task t  " +
                            "LEFT JOIN Customers.ViewedTask vt ON vt.TaskId = t.Id AND vt.ManagerId = @ManagerId AND vt.ViewDate > @DateFrom " +
                            "LEFT JOIN CMS.ChangeHistory ch ON ch.ObjId = t.Id AND ch.ObjType = @ObjType AND ch.ChangedById = @CustomerId AND ch.ModificationTime > @DateFrom " +
                            "WHERE t.TaskGroupId = tg.Id AND t.IsDeferred = 0 " +
                        ") as Date0) as LastDate " +
                    "FROM Customers.TaskGroup tg Where tg.Enabled = 1 ORDER BY LastDate DESC, tg.SortOrder",
                    CommandType.Text, GetTaskGroupFromReader,
                    new SqlParameter("@count", count),
                    new SqlParameter("@ManagerId", managerId ?? (object) DBNull.Value),
                    new SqlParameter("@DateFrom", DateTime.Now.AddMonths(-1)),
                    new SqlParameter("@ObjType", ChangeHistoryObjType.Task),
                    new SqlParameter("@CustomerId", CustomerContext.CustomerId))
                .Where(x => TaskService.CheckAccessByGroup(x.Id))
                .ToList();
        }

        public static int AddTaskGroup(TaskGroup group)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Customers.TaskGroup (Name, SortOrder, DateCreated, DateModified, Enabled, IsPrivateComments, ManagersTaskGroupConstraint, IsNotBeCompleted) " +
                "VALUES (@Name, @SortOrder, GETDATE(), GETDATE(), @Enabled, @IsPrivateComments, @ManagersTaskGroupConstraint, @IsNotBeCompleted); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", group.Name),
                new SqlParameter("@SortOrder", group.SortOrder),
                new SqlParameter("@Enabled", group.Enabled),
                new SqlParameter("@IsPrivateComments", group.IsPrivateComments),
                new SqlParameter("@ManagersTaskGroupConstraint",
                    group.ManagersTaskGroupConstraint != null
                        ? (int) group.ManagersTaskGroupConstraint
                        : (object) DBNull.Value),
                new SqlParameter("@IsNotBeCompleted", group.IsNotBeCompleted)
            );
        }

        public static void UpdateTaskGroup(TaskGroup group)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE Customers.TaskGroup "+
                "SET Name=@Name, SortOrder=@SortOrder, Enabled=@Enabled, DateModified = GETDATE(), IsPrivateComments=@IsPrivateComments, " +
                "ManagersTaskGroupConstraint=@ManagersTaskGroupConstraint, IsNotBeCompleted=@IsNotBeCompleted " +
                "WHERE Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", group.Id),
                new SqlParameter("@Name", group.Name),
                new SqlParameter("@SortOrder", group.SortOrder),
                new SqlParameter("@Enabled", group.Enabled),
                new SqlParameter("@IsPrivateComments", group.IsPrivateComments),
                new SqlParameter("@ManagersTaskGroupConstraint",
                    group.ManagersTaskGroupConstraint != null
                        ? (int) group.ManagersTaskGroupConstraint
                        : (object) DBNull.Value),
                new SqlParameter("@IsNotBeCompleted", group.IsNotBeCompleted)
            );
        }

        public static void DeleteTaskGroup(int id)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT(Id) FROM Customers.Task WHERE TaskGroupId = @Id) = 0 DELETE FROM Customers.TaskGroup WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        #region Task group Manager

        public static List<Manager> GetTaskGroupManagers(int taskGroupId, bool enabled = false)
        {
            return SQLDataAccess.Query<Manager>(
                "SELECT Managers.* FROM Customers.Managers " +
                "INNER JOIN Customers.TaskGroupManager ON TaskGroupManager.ManagerId = Managers.ManagerId " +
                "WHERE TaskGroupId = @TaskGroupId " + (enabled ? "and Active=1" : ""),
                new {TaskGroupId = taskGroupId}).ToList();
        }

        public static List<int> GetTaskGroupManagerIds(int taskGroupId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT ManagerId FROM Customers.TaskGroupManager WHERE TaskGroupId = @TaskGroupId",
                new { TaskGroupId = taskGroupId }).ToList();
        }

        public static List<int> GetTaskGroupIdsByManagerId(int managerId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT TaskGroupId FROM Customers.TaskGroupManager WHERE ManagerId = @managerId",
                new { managerId }).ToList();
        }

        public static List<int> GetTaskGroupIdsByManagerTasks(int managerId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT distinct TaskGroupId FROM Customers.Task " +
                "WHERE AppointedManagerId = @managerId OR Exists(Select 1 From Customers.TaskManager Where TaskManager.TaskId = Task.Id and TaskManager.ManagerId = @managerId)",
                new { managerId }).ToList();
        }

        public static void ClearTaskGroupManagers(int taskGroupId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskGroupManager WHERE TaskGroupId = @TaskGroupId", CommandType.Text,
                new SqlParameter("@TaskGroupId", taskGroupId));
        }

        public static void AddTaskGroupManager(int taskGroupId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO Customers.TaskGroupManager (TaskGroupId, ManagerId) VALUES (@TaskGroupId, @ManagerId)", 
                CommandType.Text,
                new SqlParameter("@TaskGroupId", taskGroupId),
                new SqlParameter("@ManagerId", managerId));
        }

        public static List<TaskGroup> GetTaskGroupsCanBeCompleted()
        {
            return SQLDataAccess
                .ExecuteReadList("SELECT * FROM Customers.TaskGroup where Enabled=1 AND IsNotBeCompleted=0 ORDER BY SortOrder", CommandType.Text, GetTaskGroupFromReader)
                .Where(x => TaskService.CheckAccessByGroup(x.Id))
                .ToList();
        }
        #endregion

        #region Task group ManagerRole

        public static List<ManagerRole> GetTaskGroupManagerRoles(int taskGroupId)
        {
            return SQLDataAccess.Query<ManagerRole>(
                "SELECT ManagerRole.* FROM Customers.ManagerRole INNER JOIN Customers.TaskGroupManagerRole ON TaskGroupManagerRole.ManagerRoleId = ManagerRole.Id WHERE TaskGroupId = @TaskGroupId",
                new { TaskGroupId = taskGroupId }).ToList();
        }

        public static List<int> GetTaskGroupManagerRoleIds(int taskGroupId)
        {
            return CacheManager.Get(TaskGroupManagerRoleIdsCacheKey + taskGroupId, () =>
                SQLDataAccess.Query<int>(
                    "SELECT ManagerRoleId FROM Customers.TaskGroupManagerRole WHERE TaskGroupId = @TaskGroupId",
                    new {TaskGroupId = taskGroupId}).ToList());
        }

        public static void ClearTaskGroupManagerRoles(int taskGroupId)
        {
           SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskGroupManagerRole WHERE TaskGroupId = @TaskGroupId", CommandType.Text,
                new SqlParameter("@TaskGroupId", taskGroupId));

            CacheManager.RemoveByPattern(TaskGroupManagerRoleIdsCacheKey);
        }

        public static void AddTaskGroupManagerRole(int taskGroupId, int managerRoleId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO Customers.TaskGroupManagerRole (TaskGroupId, ManagerRoleId) VALUES (@TaskGroupId, @ManagerRoleId)",
                CommandType.Text,
                new SqlParameter("@TaskGroupId", taskGroupId),
                new SqlParameter("@ManagerRoleId", managerRoleId));
        }

        #endregion

        #region Task group participant

        public static void ClearTaskGroupParticipants(int groupId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.TaskGroupParticipant WHERE TaskGroupId = @TaskGroupId", CommandType.Text,
                new SqlParameter("@TaskGroupId", groupId));
        }

        public static void AddTaskGroupParticipant(int groupId, int managerId)
        {
            SQLDataAccess.ExecuteNonQuery("INSERT INTO Customers.TaskGroupParticipant (TaskGroupId, ManagerId) VALUES (@TaskGroupId, @ManagerId)",
                CommandType.Text,
                new SqlParameter("@TaskGroupId", groupId),
                new SqlParameter("@ManagerId", managerId));
        }

        public static List<int> GetTaskGroupParticipantIds(int groupId)
        {
            return SQLDataAccess.Query<int>(
                "SELECT ManagerId FROM Customers.TaskGroupParticipant WHERE TaskGroupId = @TaskGroupId",
                new { TaskGroupId = groupId }).ToList();
        }

        #endregion


        public static bool IsPrivateCommentsByTaskId(int taskId)
        {
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(
                "SELECT IsPrivateComments FROM Customers.TaskGroup WHERE Id = (Select TaskGroupId From Customers.Task Where Id = @Id)",
                CommandType.Text, new SqlParameter("@Id", taskId)));
        }

        public static bool IsNotBeCompletedByTaskId(int taskId)
        {
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(
                "SELECT IsNotBeCompleted FROM Customers.TaskGroup WHERE Id = (Select TaskGroupId From Customers.Task Where Id = @Id)",
                CommandType.Text, new SqlParameter("@Id", taskId)));
        }

        public static bool IsNotBeCompletedByTaskGroupId(int taskGroupId)
        {
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(
                "SELECT IsNotBeCompleted FROM Customers.TaskGroup WHERE Id = @Id",
                CommandType.Text, new SqlParameter("@Id", taskGroupId)));
        }

        public static void AddProjectStatus(int taskGroupId, int projectStatusId)
        {
            SQLDataAccess.ExecuteNonQuery(
                    "Insert Into Customers.TaskGroup_ProjectStatus (TaskGroupId, ProjectStatusId) Values (@TaskGroupId, @ProjectStatusId)",
                    CommandType.Text,
                    new SqlParameter("@TaskGroupId", taskGroupId),
                    new SqlParameter("@ProjectStatusId", projectStatusId));
        }
    }
}