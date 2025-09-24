using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.AdminPushNotification;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Customers.AdminInformers
{
    public class AdminInformerService
    {
        private const string CacheName = "AdminInformer";

        public static void Add(AdminInformer informer)
        {
            informer.Id =
                SQLDataAccess.ExecuteScalar<int>(
                    "Insert Into Customers.AdminInformer (Type,ObjId,CustomerId,Title,Body,Link,Count,Seen,CreatedDate,PrivateCustomerId,EntityId) " +
                    "Values (@Type,@ObjId,@CustomerId,@Title,@Body,@Link,@Count,@Seen,getdate(),@PrivateCustomerId,@EntityId); " +
                    "Select scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@Type", (int) informer.Type),
                    new SqlParameter("@ObjId", informer.ObjId),
                    new SqlParameter("@CustomerId", informer.CustomerId ?? (object) DBNull.Value),
                    new SqlParameter("@Title", informer.Title ?? ""),
                    new SqlParameter("@Body", informer.Body ?? ""),
                    new SqlParameter("@Link", informer.Link ?? ""),
                    new SqlParameter("@Count", informer.Count),
                    new SqlParameter("@Seen", informer.Seen),
                    new SqlParameter("@PrivateCustomerId", informer.PrivateCustomerId ?? (object) DBNull.Value),
                    new SqlParameter("@EntityId", informer.EntityId ?? (object)DBNull.Value)
                    );

            CacheManager.RemoveByPattern(CacheName);
            AdminPushNotificationService.SendPushFromAdminInformer(informer);
        }

        public static AdminInformer Get(int id)
        {
            return SQLDataAccess.Query<AdminInformer>("Select * From Customers.AdminInformer Where Id=@id", new {id})
                .FirstOrDefault();
        }

        public static void SetSeen(int notificationId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Customers.AdminInformer Set Seen=1 Where Id=@Id", CommandType.Text,
                new SqlParameter("@Id", notificationId));

            CacheManager.RemoveByPattern(CacheName);
        }

        public static void SetSeen(Guid customerId)
        {
            var c = CustomerContext.CurrentCustomer;
            if (c == null || !HaveAccess(c))
                return;

            SQLDataAccess.ExecuteNonQuery("Update Customers.AdminInformer Set Seen=1 Where CustomerId=@customerId", CommandType.Text,
                new SqlParameter("@customerId", customerId));

            CacheManager.RemoveByPattern(CacheName);
        }

        public static void SetSeen(AdminInformerType type, int entityId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Customers.AdminInformer Set Seen=1 Where type=@type and entityId=@entityId", CommandType.Text,
                new SqlParameter("@type", (int)type),
                new SqlParameter("@entityId", entityId));

            CacheManager.RemoveByPattern(CacheName);
        }

        public static List<AdminInformer> GetLast(int count)
        {
            var customerId = CustomerContext.CustomerId;

            return CacheManager.Get(CacheName + "last" + customerId, 60,
                () => SQLDataAccess.Query<AdminInformer>(
                    "Select top(@count) * " +
                    "From Customers.AdminInformer " +
                    "Where (PrivateCustomerId is null or PrivateCustomerId=@customerId) " +
                    "Order By Seen asc, CreatedDate desc",
                    new { count, customerId })
                    .ToList());
        }

        public static int GetUnseenCount()
        {
            var customerId = CustomerContext.CustomerId;

            return CacheManager.Get(CacheName + "unseencount" + customerId, 60,
                () => SQLDataAccess.ExecuteScalar<int>(
                    "Select Count(id) " +
                    "From Customers.AdminInformer " +
                    "Where Seen = 0 and (PrivateCustomerId is null or PrivateCustomerId=@customerId)",
                    CommandType.Text,
                    new SqlParameter("@customerId", customerId)));
        }

        public static List<AdminInformer> GetListUnSeen()
        {
            var customerId = CustomerContext.CustomerId;

            return SQLDataAccess.Query<AdminInformer>(
                "Select * From Customers.AdminInformer " +
                "Where Seen = 0 and (PrivateCustomerId is null or PrivateCustomerId=@customerId) " +
                "Order By CreatedDate desc",
                new { customerId }).ToList();
        }

        public static List<AdminInformer> GetList()
        {
            var customerId = CustomerContext.CustomerId;

            return CacheManager.Get(CacheName + "list" + customerId, 60,
                () => SQLDataAccess.Query<AdminInformer>(
                    "Select top 100 * From Customers.AdminInformer " +
                    "Where PrivateCustomerId is null or PrivateCustomerId=@customerId " +
                    "Order By CreatedDate desc", 
                    new { customerId }).ToList());
        }

        public static bool HaveAccess(Customer customer)
        {
            return customer != null &&
                   (customer.IsAdmin ||
                    (customer.IsModerator &&
                     (customer.HasRoleAction(RoleAction.Customers) || customer.HasRoleAction(RoleAction.Crm) ||
                      customer.HasRoleAction(RoleAction.Tasks) || customer.HasRoleAction(RoleAction.Orders))));
        }

        public static void ClearOld()
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Customers.AdminInformer Where CreatedDate < @date",
                CommandType.Text, new SqlParameter("@date", DateTime.Now.AddDays(-5)));

            CacheManager.RemoveByPattern(CacheName);
        }

        public static void ClearExpired()
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[AdminInformer] WHERE [CreatedDate] < @date",
                CommandType.Text, 60 * 2, new SqlParameter("@date", DateTime.Now.AddMonths(-6)));

            CacheManager.RemoveByPattern(CacheName);
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Customers.AdminInformer Where id=@id",
                CommandType.Text, new SqlParameter("@id", id));

            CacheManager.RemoveByPattern(CacheName);
        }
    }
}
