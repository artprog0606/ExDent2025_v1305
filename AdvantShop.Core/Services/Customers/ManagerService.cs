//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Crm.ProjectStatuses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class ManagerService
    {
        public static Manager GetManager(int managerId)
        {
            return SQLDataAccess.ExecuteReadOne<Manager>(
                "SELECT TOP 1 * FROM [Customers].[Managers] WHERE ManagerId = @ManagerId", CommandType.Text,
                GetManagerFromReader, new SqlParameter("@ManagerId", managerId));
        }

        public static Manager GetManager(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadOne<Manager>(
                "SELECT TOP 1 * FROM [Customers].[Managers] WHERE CustomerId = @CustomerId", CommandType.Text,
                GetManagerFromReader, new SqlParameter("@CustomerId", customerId));
        }

        public static List<Manager> GetManagersList(bool onlyActive = true)
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT Managers.* FROM [Customers].[Managers] INNER JOIN Customers.Customer ON Customer.CustomerId = Managers.CustomerId " +
                    (onlyActive ? "WHERE [Enabled] = 1 " : string.Empty) +
                    "Order By Customer.SortOrder, Customer.FirstName + ' ' + Customer.LastName",
                    CommandType.Text, GetManagerFromReader);
        }

        public static List<Manager> GetManagers(List<int> ids)
        {
            return ids == null || !ids.Any()
                ? new List<Manager>()
                : SQLDataAccess.ExecuteReadList(
                    "SELECT m.* FROM [Customers].[Managers] m INNER JOIN Customers.Customer c ON c.CustomerId = m.CustomerId " +
                    string.Format("WHERE m.ManagerId IN ({0}) ", ids.AggregateString(",")) +
                    "ORDER BY c.SortOrder, c.FirstName + ' ' + c.LastName",
                    CommandType.Text, GetManagerFromReader);
        }

        public static List<Manager> GetManagers(params RoleAction[] roleActions)
        {
            return GetManagersList()
                .Where(x => roleActions == null || !roleActions.Any() || roleActions.All(roleAction => x.HasRoleAction(roleAction)))
                .ToList();
        }

        public static List<Manager> GetManagers(EBizProcessEventType bizProcessEventType)
        {
            return GetManagers(RoleActionService.GetBizProcessRoleActions(bizProcessEventType).ToArray());
        }

        /// <summary>
        /// Список всех сотрудников с учетом прав доступа и участников проекта задач
        /// </summary>
        /// <param name="taskGroupId">ID проекта</param>
        /// <param name="taskId">ID задачи</param>
        /// <param name="assigned"><para>Включая сотрудников без нужных прав или неактивных, но с назначенными ему задачами.</para>
        /// <para>Если указан taskGroupId - учитывая задачи в проекте, если указан taskId - учитывая конкретную задачу</para></param>
        /// <param name="appointed"><para>Включая сотрудников без нужных прав или неактивных, но с поставленными им задачами.</para>
        /// <para>Если указан taskGroupId - учитывая задачи в проекте, если указан taskId - учитывая конкретную задачу</para></param>
        /// <returns></returns>
        public static List<Manager> GetAllTaskManagers(int? taskGroupId = null, int? taskId = null, bool assigned = false, bool appointed = false, bool accepted = false, bool observed = false)
        {
            var sql =
                "SELECT m.* FROM Customers.Managers m INNER JOIN Customers.Customer c ON m.CustomerId = c.CustomerID " +
                "WHERE " +
                    "(c.Enabled = 1 AND " +
                        // админ
                        "(c.CustomerRole = @AdminRole " +
                        "OR (" +
                            // модератор с правами доступа
                            "(c.CustomerRole = @ModeratorRole AND EXISTS(SELECT 1 FROM Customers.CustomerRoleAction cra WHERE cra.CustomerID = m.CustomerId AND cra.RoleActionKey = @RoleActionKey AND cra.Enabled = 1)) " +
                            (taskGroupId.HasValue ? 
                            "AND (" +
                                // в проекте не заданы участники
                                "(SELECT COUNT(*) FROM [Customers].[TaskGroupManagerRole] WHERE TaskGroupManagerRole.TaskGroupId = @TaskGroupId) = 0 " +
                                // или сотруднику назначена роль участника проекта
                                "OR EXISTS(SELECT 1 FROM [Customers].[TaskGroupManagerRole] WHERE TaskGroupManagerRole.TaskGroupId = @TaskGroupId " +
                                    "AND TaskGroupManagerRole.ManagerRoleId IN (SELECT ManagerRoleId FROM Customers.ManagerRolesMap WHERE ManagerRolesMap.CustomerId = m.CustomerId)) " +
                                // или сотрудник добавлен в проект отдельно
                                "OR EXISTS(SELECT 1 FROM [Customers].[TaskGroupParticipant] WHERE TaskGroupParticipant.TaskGroupId = @TaskGroupId AND TaskGroupParticipant.ManagerId = m.ManagerId) " +
                                ") " : String.Empty)+
                            ")" +
                        ") " +
                    ") ";
            // неактивные или без нужных прав, но с назначенными или поставленными задачами
            if (assigned)
            {
                sql += taskId.HasValue
                    ? "OR EXISTS (SELECT 1 FROM Customers.TaskManager tm WHERE tm.TaskId = @TaskId AND tm.ManagerId = m.ManagerId) "
                    : "OR EXISTS (SELECT 1 FROM Customers.TaskManager tm INNER JOIN Customers.Task t ON t.Id = tm.TaskId AND tm.ManagerId = m.ManagerId AND t.Accepted = " + (accepted ? 1 : 0) +
                        (taskGroupId.HasValue ? " AND t.TaskGroupId = @TaskGroupId" : string.Empty) + ") ";
            }
            if (appointed)
            {
                sql += taskId.HasValue
                    ? "OR EXISTS (SELECT 1 FROM Customers.Task t WHERE t.AppointedManagerId = m.ManagerId AND t.Id = @TaskId) "
                    : "OR EXISTS (SELECT 1 FROM Customers.Task t WHERE t.AppointedManagerId = m.ManagerId AND t.Accepted = " + (accepted ? 1 : 0) +
                        (taskGroupId.HasValue ? " AND t.TaskGroupId = @TaskGroupId" : string.Empty) + ") ";
            }
            // отслеживающий задачи 
            if (observed)
            {
                sql += taskId.HasValue 
                    ? "OR EXISTS (SELECT 1 FROM Customers.TaskObserver tob WHERE tob.TaskId = @TaskId AND tob.ManagerId = m.ManagerId) "
                    : "OR EXISTS (SELECT 1 FROM Customers.TaskObserver tob INNER JOIN Customers.Task t ON t.Id = tob.TaskId AND tob.ManagerID = m.ManagerId AND t.Accepted = " + (accepted ? 1 : 0) +
                      (taskGroupId.HasValue ? " AND t.TaskGroupId = @TaskGroupId" : string.Empty) + ") ";
            }


            sql += "ORDER BY c.SortOrder, c.FirstName + ' ' + c.LastName";

            return SQLDataAccess.ExecuteReadList(sql,
                    CommandType.Text, GetManagerFromReader, 
                    new SqlParameter("@TaskGroupId", taskGroupId ?? (object)DBNull.Value),
                    new SqlParameter("@TaskId", taskId ?? (object)DBNull.Value),
                    new SqlParameter("@RoleActionKey", RoleAction.Tasks.ToString()),
                    new SqlParameter("@AdminRole", (int)Role.Administrator),
                    new SqlParameter("@ModeratorRole", (int)Role.Moderator));
        }

        public static Customer GetMostFreeCustomer(int? managerRoleId, string city = null, params RoleAction[] roleActions)
        {
            var customers = SQLDataAccess.ExecuteReadList<Customer>(
                "SELECT TOP(1) c.*, " +
                "(SELECT COUNT(t.Id) FROM Customers.Task t INNER JOIN Customers.TaskManager tm ON tm.TaskId = t.Id " +
                "INNER JOIN Customers.ProjectStatus ps ON t.StatusId = ps.Id " +
                "WHERE tm.ManagerId = m.ManagerId AND ps.Status <> @StatusFInalSuccess AND ps.Status <> @StatusCanceled) AS OpenTasksCount " +
                "FROM Customers.Customer AS c " +
                "INNER JOIN Customers.Managers AS m ON m.CustomerID = c.CustomerId " +
                "WHERE c.[Enabled] = 1 " +
                (managerRoleId.HasValue ? "AND EXISTS(SELECT CustomerId FROM Customers.ManagerRolesMap WHERE ManagerRoleId = @ManagerRoleId AND CustomerId = m.CustomerId) " : string.Empty) +
                (city.IsNotEmpty() ? "AND City = @City " : string.Empty) +
                "ORDER BY OpenTasksCount",
                CommandType.Text, CustomerService.GetFromSqlDataReader, 
                new SqlParameter("@StatusFInalSuccess", (int)ProjectStatusType.FinalSuccess),
                new SqlParameter("@StatusCanceled", (int)ProjectStatusType.Canceled),
                managerRoleId.HasValue ? new SqlParameter("@ManagerRoleId", managerRoleId.Value) : null,
                city.IsNotEmpty() ? new SqlParameter("@City", city) : null);

            return customers.FirstOrDefault(x => roleActions == null || !roleActions.Any() || roleActions.All(roleAction => x.HasRoleAction(roleAction)));
        }

        public static bool CustomerIsManager(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Customers].[Managers] WHERE CustomerId = @CustomerId", CommandType.Text,
                new SqlParameter("@CustomerId", customerId)) > 0;
        }

        private static Manager GetManagerFromReader(SqlDataReader reader)
        {
            return new Manager
            {
                ManagerId = SQLDataHelper.GetInt(reader, "ManagerId"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                DepartmentId = SQLDataHelper.GetNullableInt(reader, "DepartmentId"),
                Position = SQLDataHelper.GetString(reader, "Position"),
                Sign = SQLDataHelper.GetString(reader, "Sign"),
            };
        }

        public static int GetManagersCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT Count(ManagerId) FROM [Customers].[Managers]", CommandType.Text);
        }

        public static void AddOrUpdateManager(Manager manager)
        {
            manager.ManagerId = SQLDataAccess.ExecuteScalar<int>(
                "IF ((SELECT COUNT(*) FROM [Customers].[Managers] WHERE CustomerId = @CustomerId) = 0) BEGIN" +
                " INSERT INTO [Customers].[Managers] ([CustomerId],[DepartmentId],[Position],[Sign]) VALUES (@CustomerId,@DepartmentId,@Position,@Sign); SELECT SCOPE_IDENTITY(); " +
                "END ELSE BEGIN " +
                " UPDATE [Customers].[Managers] SET [DepartmentId] = @DepartmentId, [Position] = @Position, [Sign] = @Sign " +
                " WHERE CustomerId = @CustomerId; " +
                " SELECT ManagerId FROM [Customers].[Managers] WHERE CustomerId = @CustomerId " +
                "END",
                CommandType.Text,
                new SqlParameter("@CustomerId", manager.CustomerId),
                new SqlParameter("@Position", manager.Position ?? string.Empty),
                new SqlParameter("@Sign", manager.Sign ?? string.Empty),
                new SqlParameter("@DepartmentId", manager.DepartmentId ?? (object)DBNull.Value));

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void DeleteManagerPhoto(int managerId)
        {
            PhotoService.DeletePhotos(managerId, PhotoType.Manager);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void DeleteManager(int managerId)
        {
            DeleteManagerPhoto(managerId);

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[Managers] WHERE ManagerId = @ManagerId",
                CommandType.Text, new SqlParameter("@ManagerId", managerId));

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void ForcedDeleteManager(Manager manager)
        {
            try
            {
                var managerId = manager.ManagerId;

                SQLDataAccess.ExecuteNonQuery("Update [Order].[Order] Set ManagerId = null Where ManagerId=@ManagerId",
                    CommandType.Text, new SqlParameter("@ManagerId", managerId));

                SQLDataAccess.ExecuteNonQuery("Update [Order].[Lead] Set ManagerId = null Where ManagerId=@ManagerId",
                    CommandType.Text, new SqlParameter("@ManagerId", managerId));

                SQLDataAccess.ExecuteNonQuery("Update Customers.Customer Set ManagerId = null Where ManagerId=@ManagerId",
                    CommandType.Text, new SqlParameter("@ManagerId", managerId));

                SQLDataAccess.ExecuteNonQuery(
                    "Update Customers.ManagerTask Set AssignedManagerId = null Where AssignedManagerId=@ManagerId",
                    CommandType.Text, new SqlParameter("@ManagerId", managerId));

                TaskService.UnassignTaskManager(managerId);

                SQLDataAccess.ExecuteNonQuery(
                    "Update Customers.ManagerTask Set AppointedManagerId = null Where AppointedManagerId=@ManagerId",
                    CommandType.Text, new SqlParameter("@ManagerId", managerId));

                TaskService.ClearTaskAppointedManager(managerId);

                DeleteManager(managerId);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        private static bool HasAssignedTasks(int managerId)
        {
            var oldTasksCount = SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from Customers.ManagerTask Where AssignedManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId));
            var tasksCount = SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from Customers.TaskManager Where ManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId));
            return oldTasksCount + tasksCount > 0;
        }

        private static bool HasAppointedTasks(int managerId)
        {
            var oldTasksCount = SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from Customers.ManagerTask Where AppointedManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId));
            var tasksCount = SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from Customers.Task Where AppointedManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId));
            return oldTasksCount + tasksCount > 0;
        }

        private static bool HasAssignedOrders(int managerId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from [order].[order] Where ManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId)) > 0;
        }

        private static bool HasAssignedLeads(int managerId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from [order].[lead] Where ManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId)) > 0;
        }

        private static bool HasAssignedCustomers(int managerId)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(*) from Customers.Customer Where ManagerId=@ManagerId", CommandType.Text, new SqlParameter("@ManagerId", managerId)) > 0;
        }

        public static bool CanDelete(int managerId)
        {
            string message;
            return CanDelete(managerId, out message);
        }

        public static bool CanDelete(int managerId, out string message)
        {
            var manager = GetManager(managerId);
            var reasons = new List<string>();
            if (HasAssignedTasks(managerId))
                reasons.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager.Tasks"));
            if (HasAssignedOrders(managerId))
                reasons.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager.Orders"));
            if (HasAssignedLeads(managerId))
                reasons.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager.Leads"));
            if (HasAssignedCustomers(managerId))
                reasons.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager.Customers"));
            if (HasAppointedTasks(managerId))
                reasons.Add(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager.AppointedTasks"));
            message = reasons.Any() 
                ? string.Format(LocalizationService.GetResource("Core.Customers.ErrorDeleteManager"), manager.FullName, reasons.AggregateString(", ")) 
                : string.Empty;
            return !reasons.Any();
        }
    }
}
