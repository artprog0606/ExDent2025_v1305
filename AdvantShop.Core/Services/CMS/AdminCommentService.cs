using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.CMS
{
    public class AdminCommentService
    {
        private static AdminComment GetAdminCommentFromReader(SqlDataReader reader)
        {
            var comment = new AdminComment
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                ParentId = SQLDataHelper.GetNullableInt(reader, "ParentId"),
                ObjId = SQLDataHelper.GetInt(reader, "ObjId"),
                Type = SQLDataHelper.GetString(reader, "Type").TryParseEnum<AdminCommentType>(),
                CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Text = SQLDataHelper.GetString(reader, "Text"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified"),
                Deleted = SQLDataHelper.GetBoolean(reader, "Deleted"),
                Avatar = SQLDataHelper.GetString(reader, "Avatar")
            };
            return comment;
        }

        private static AdminComment GetAdminCommentFromDb(int id)
        {
            return SQLDataAccess.ExecuteReadOne<AdminComment>(
                    "SELECT TOP 1 AdminComment.*, Customer.Avatar FROM CMS.AdminComment " +
                    "LEFT JOIN Customers.Customer ON Customer.CustomerId = AdminComment.CustomerId WHERE Id = @Id",
                    CommandType.Text, GetAdminCommentFromReader,
                    new SqlParameter("@Id", id));
        }

        public static AdminComment GetAdminComment(int id)
        {
            var comment = GetAdminCommentFromDb(id);

            if (comment != null && comment.Type == AdminCommentType.TaskHidden)
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer != null && customer.IsModerator && TaskGroupService.IsPrivateCommentsByTaskId(comment.ObjId))
                {
                    var task = TaskService.GetTask(comment.ObjId);

                    if (task != null && task.AppointedManager != null && task.AppointedManager.CustomerId == customer.Id)
                        return comment;

                    if (comment.CustomerId == customer.Id || (comment.Customer != null && comment.Customer.IsAdmin))
                        return comment;

                    return null;
                }
            }

            return comment;
        }

        public static List<AdminComment> GetAdminComments(int objId, AdminCommentType type)
        {
            var comments = 
                SQLDataAccess.ExecuteReadList<AdminComment>(
                    "SELECT AdminComment.*, Customer.Avatar FROM CMS.AdminComment LEFT JOIN Customers.Customer ON Customer.CustomerId = AdminComment.CustomerId " +
                    "WHERE ObjId = @ObjId AND Type = @Type ORDER BY DateCreated",
                    CommandType.Text, GetAdminCommentFromReader,
                    new SqlParameter("@ObjId", objId),
                    new SqlParameter("@Type", type.ToString()));

            if (type == AdminCommentType.TaskHidden)
            {
                var customer = CustomerContext.CurrentCustomer;
                
                if (customer != null && customer.IsModerator && TaskGroupService.IsPrivateCommentsByTaskId(objId))
                {
                    var task = TaskService.GetTask(objId);

                    if (task != null && task.AppointedManager != null && task.AppointedManager.CustomerId == customer.Id)
                        return comments;


                    var roles = ManagerRoleService.GetManagerRoles(customer.Id);

                    var allComments =
                        comments.Where(
                            x =>
                                x.CustomerId == customer.Id || 
                                (x.Customer != null && x.Customer.IsAdmin && x.ParentId == null) ||
                                (x.Customer != null && roles.Any(y=> ManagerRoleService.GetManagerRoles(x.Customer.Id).Select(role=> role.Id).Contains(y.Id) )) ||
                                (x.Customer != null && x.ParentId != null && GetAdminCommentFromDb(x.ParentId.Value).Customer.Id == customer.Id)
                                ).ToList();

                    var resultComments = new List<AdminComment>();

                    FillCommentsList(resultComments, allComments, null);

                    return resultComments.OrderBy(x => x.DateCreated).ToList();
                }
            }

            return comments;
        }

        private static void FillCommentsList(List<AdminComment> resultList, List<AdminComment> comments, int? parentId)
        {
            foreach (var comment in comments.Where(x => x.ParentId == parentId))
            {
                resultList.Add(comment);
                FillCommentsList(resultList, comments, comment.Id);
            }
        }

        public static int AddAdminComment(AdminComment comment)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO CMS.AdminComment " +
                "(ParentId, ObjId, Type, CustomerId, Name, Email, Text, DateCreated, DateModified) " +
                "VALUES (@ParentId, @ObjId, @Type, @CustomerId, @Name, @Email, @Text, GETDATE(), GETDATE()); SELECT SCOPE_IDENTITY(); ",
                CommandType.Text,
                new SqlParameter("@ParentId", comment.ParentId.HasValue ? comment.ParentId.Value : (object)DBNull.Value),
                new SqlParameter("@ObjId", comment.ObjId),
                new SqlParameter("@Type", comment.Type.ToString()),
                new SqlParameter("@CustomerId", comment.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@Name", comment.Name ?? string.Empty),
                new SqlParameter("@Email", comment.Email ?? string.Empty),
                new SqlParameter("@Text", comment.Text ?? string.Empty));
        }

        public static void UpdateAdminComment(AdminComment comment)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE CMS.AdminComment SET ParentId = @ParentId, ObjId = @ObjId, Type = @Type, CustomerId = @CustomerId, Name = @Name, Email = @Email, " +
                "Text = @Text, DateModified = GETDATE() WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", comment.Id),
                new SqlParameter("@ParentId", comment.ParentId.HasValue ? comment.ParentId.Value : (object)DBNull.Value),
                new SqlParameter("@ObjId", comment.ObjId),
                new SqlParameter("@Type", comment.Type.ToString()),
                new SqlParameter("@CustomerId", comment.CustomerId ?? (object)DBNull.Value),
                new SqlParameter("@Name", comment.Name ?? string.Empty),
                new SqlParameter("@Email", comment.Email ?? string.Empty),
                new SqlParameter("@Text", comment.Text ?? string.Empty));
        }

        public static void DeleteAdminComments(int objId, AdminCommentType type)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM CMS.AdminComment WHERE ObjId = @ObjId AND Type = @Type", CommandType.Text, 
                new SqlParameter("@ObjId", objId), 
                new SqlParameter("@Type", type.ToString()));
        }

        public static void DeleteAdminComment(int id)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE CMS.AdminComment SET Deleted = 1 WHERE Id = @Id", CommandType.Text, 
                new SqlParameter("@Id", id));
        }
    }
}