using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Crm.ProjectStatuses
{
    public class ProjectStatusService
    {
        private const string ProjectStatusCacheKey = "ProjectStatus_";

        public static List<ProjectStatus> GetDefaultProjectStatuses()
        {
            return new List<ProjectStatus>()
            {
                new ProjectStatus() { Name = "Новая", Color ="8bc34a",  SortOrder = 10, StatusType = TaskStatusType.Open},
                new ProjectStatus() { Name = "В работе", Color ="ffc73e",  SortOrder = 20, StatusType = TaskStatusType.InProgress},
                new ProjectStatus() { Name = "Завершена", Color ="1ec5b8",  SortOrder = 30, StatusType = TaskStatusType.Completed},
                new ProjectStatus() { Name = "Задача принята", Color ="000000",  SortOrder = 50, Status = ProjectStatusType.FinalSuccess,StatusType = TaskStatusType.Accepted},
                new ProjectStatus() { Name = "Задача отклонена", Color ="b0bec5",  SortOrder = 60, Status = ProjectStatusType.Canceled, StatusType = TaskStatusType.Canceled},
            };
        }

        public static List<ProjectStatus> GetList()
        {
            return CacheManager.Get(ProjectStatusCacheKey + "alllist",
                () => SQLDataAccess.Query<ProjectStatus>("Select * From Customers.ProjectStatus Order by SortOrder").ToList());
        }

        public static List<ProjectStatus> GetList(int taskGroupId)
        {
            return CacheManager.Get(ProjectStatusCacheKey + "list_" + taskGroupId,
                () => SQLDataAccess.Query<ProjectStatus>(
                    "Select * From Customers.ProjectStatus ps " +
                    "Inner Join Customers.TaskGroup_ProjectStatus tp On tp.ProjectStatusId=ps.Id " +
                    "Where tp.TaskGroupId=@taskGroupId " +
                    "Order by SortOrder",
                    new { taskGroupId }).ToList());
        }

        public static List<ProjectStatus> GetList(int taskGroupId, int statusType)
        {
            return CacheManager.Get($"{ProjectStatusCacheKey}list_{taskGroupId}_{statusType}",
                () => SQLDataAccess.Query<ProjectStatus>(
                    "Select * From Customers.ProjectStatus ps " +
                    "Inner Join Customers.TaskGroup_ProjectStatus tp On tp.ProjectStatusId=ps.Id " +
                    "Where tp.TaskGroupId=@taskGroupId " +
                    "And ps.StatusType=@statusType " +
                    "Order by SortOrder",
                    new { taskGroupId, statusType }).ToList());
        }

        public static List<ProjectStatus> GetListByTaskId(int taskId)
        {
            return CacheManager.Get(ProjectStatusCacheKey + "list_byTaskId_" + taskId,
                () => SQLDataAccess.Query<ProjectStatus>(
                    "Select ps.Id, ps.Name, ps.Color, ps.SortOrder, ps.Status, ps.StatusType " +
                    "From Customers.ProjectStatus ps " +
                    "Inner Join Customers.TaskGroup_ProjectStatus tg_ps " +
                    "On tg_ps.ProjectStatusId = ps.Id " +
                    "Inner Join Customers.Task t " +
                    "on t.TaskGroupId = tg_ps.TaskGroupId " +
                    "Where t.Id = @taskId order by ps.SortOrder",
                    new { taskId }).ToList());
        }

        public static ProjectStatus Get(int id)
        {
            return CacheManager.Get(ProjectStatusCacheKey + id,
                () =>
                    SQLDataAccess.Query<ProjectStatus>("Select * From Customers.ProjectStatus Where Id = @id", new { id })
                        .FirstOrDefault());
        }

        public static ProjectStatus Get(int taskGroupId, int statusType)
        {
            return CacheManager.Get(ProjectStatusCacheKey + taskGroupId + "_" + statusType,
                () => SQLDataAccess.Query<ProjectStatus>(
                    "Select * From Customers.ProjectStatus ps " +
                    "Inner Join Customers.TaskGroup_ProjectStatus tp On tp.ProjectStatusId = ps.Id " +
                    "Where tp.TaskGroupId = @taskGroupId And ps.StatusType = @statusType",
                    new { taskGroupId, statusType }).FirstOrDefault());
        }

        public static int GetFirstStatusIdInList(int taskGroupId)
        {
            var status =  CacheManager.Get(ProjectStatusCacheKey + "firstStatus_" + taskGroupId.ToString(),
                () => SQLDataAccess.Query<ProjectStatus>(
                    "Select * From Customers.ProjectStatus ps " +
                    "Inner Join Customers.TaskGroup_ProjectStatus tp On tp.ProjectStatusId=ps.Id " +
                    "Where tp.TaskGroupId=@taskGroupId " +
                    "Order by SortOrder",
                    new { taskGroupId }).FirstOrDefault());

            return status != null ? status.Id : 0;
        }

        public static int Add(ProjectStatus status)
        {
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);

            status.Id = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Customers.ProjectStatus (Name, SortOrder, Color, Status, StatusType) VALUES (@Name, @SortOrder, @Color, @Status, @StatusType); SELECT SCOPE_IDENTITY()",
                CommandType.Text,
                new SqlParameter("@Name", status.Name),
                new SqlParameter("@SortOrder", status.SortOrder),
                new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value),
                new SqlParameter("@Status", (int)status.Status),
                new SqlParameter("@StatusType", (int)status.StatusType));

            return status.Id;
        }

        public static int Update(ProjectStatus status)
        {
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);

            return SQLDataAccess.ExecuteScalar<int>(
                "UPDATE Customers.ProjectStatus SET Name = @Name, SortOrder = @SortOrder, Color = @Color, StatusType = @StatusType WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", status.Id),
                new SqlParameter("@Name", status.Name),
                new SqlParameter("@SortOrder", status.SortOrder),
                new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value),
                new SqlParameter("@StatusType", status.StatusType));
        }

        public static int UpdateSortOrder(int id, int sortOrder)
        {
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);

            return SQLDataAccess.ExecuteScalar<int>(
                "UPDATE Customers.ProjectStatus SET SortOrder = @SortOrder WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@SortOrder", sortOrder));
        }

        public static void UpdateSortOrder(IEnumerable<int> statusList)
        {
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);

            var sortOrder = 0;
            foreach (var statusId in statusList)
            {
                UpdateSortOrder(statusId, sortOrder);
                sortOrder++;
            }
        }

        public static void Delete(int id)
        {
            var taskGroup = TaskGroupService.GetByProjectStatus(id);
            if (taskGroup != null)
            {
                var statuses = GetList(taskGroup.Id);

                ProjectStatus newStatus = null;

                for (int i = 0; i < statuses.Count; i++)
                {
                    if (statuses[i].Id == id)
                        break;
                    newStatus = statuses[i];
                }

                if (newStatus == null)
                    newStatus = statuses.FirstOrDefault(x => x.Id != id);

                if (newStatus != null)
                    SQLDataAccess.ExecuteNonQuery(
                        "Update Customers.Task Set StatusId=@NewProjectlStatusId WHERE StatusId = @OldProjectStatusId",
                        CommandType.Text,
                        new SqlParameter("@NewProjectlStatusId", newStatus.Id),
                        new SqlParameter("@OldProjectStatusId", id));
            }

            SQLDataAccess.ExecuteNonQuery("DELETE FROM Customers.ProjectStatus WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);
        }

        public static void ClearCache()
        {
            CacheManager.RemoveByPattern(ProjectStatusCacheKey);
        }
    }
}
