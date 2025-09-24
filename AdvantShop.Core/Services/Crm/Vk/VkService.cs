using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public static class VkService
    {
        private const string VkCacheKeyCustomer = "_Vk_User_";
        private const string VkCacheKeyMessage = "_Vk_Message_";

        #region User

        public static VkUser GetUser(long userId)
        {
            return CacheManager.Get(CacheNames.Customer + VkCacheKeyCustomer + userId, 20,
                () =>
                    SQLDataAccess.Query<VkUser>("Select * From [Customers].[VkUser] Where Id=@id", new {id = userId})
                        .FirstOrDefault());
        }

        public static VkUser GetUser(Guid customerId)
        {
            return CacheManager.Get(CacheNames.Customer + VkCacheKeyCustomer + "_customerId_" + customerId,
                () =>
                    SQLDataAccess.Query<VkUser>("Select * From [Customers].[VkUser] Where CustomerId=@id", new { id = customerId })
                        .FirstOrDefault());
        }


        public static void AddUser(VkUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[VkUser] (Id, CustomerId, FirstName, LastName, BirthDate, Photo100, MobilePhone, HomePhone, Sex, ScreenName) " +
                "Values (@Id, @CustomerId, @FirstName, @LastName, @BirthDate, @Photo100, @MobilePhone, @HomePhone, @Sex, @ScreenName)",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@FirstName", user.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", user.LastName ?? (object)DBNull.Value),
                new SqlParameter("@BirthDate", user.BirthDate ?? (object)DBNull.Value),
                new SqlParameter("@Photo100", user.Photo100 ?? (object)DBNull.Value),
                new SqlParameter("@MobilePhone", user.MobilePhone ?? (object)DBNull.Value),
                new SqlParameter("@HomePhone", user.HomePhone ?? (object)DBNull.Value),
                new SqlParameter("@Sex", user.Sex ?? (object)DBNull.Value),
                new SqlParameter("@ScreenName", user.ScreenName ?? (object)DBNull.Value)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + VkCacheKeyCustomer);
        }

        public static void UpdateUser(VkUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[VkUser] Set CustomerId=@CustomerId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + VkCacheKeyCustomer);
        }

        public static void DeleteUser(long id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[VkUser] Where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(CacheNames.Customer + VkCacheKeyCustomer);
        }
        
        public static void ChangeCustomer(Guid customerId, Guid newCustomerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[VkUser] Set CustomerId=@NewCustomerId Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@NewCustomerId", newCustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + VkCacheKeyCustomer);
        }

        #endregion

        #region Message

        /// <summary>
        /// Сообщения от/к пользователю из контакта
        /// </summary>
        public static List<VkUserMessage> GetCustomerMessages(Guid customerId)
        {
            var user = GetUser(customerId);
            if (user == null)
                return null;

            var userMessages =
                SQLDataAccess.Query<VkUserMessage>(
                    "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.ScreenName, u.Photo100 From Customers.VkMessage m " +
                    "Left Join Customers.VkUser u On u.Id = m.UserId " +
                    "Where m.UserId=@userId OR m.FromId=@userId " +
                    "Order by Date desc",
                    new { userId = user.Id })
                    .ToList();

            var group = SettingsVk.Group;

            foreach (var message in userMessages)
            {
                // В исходящих сообщениях FromId - автор сообщения
                if (message.FromId != null && group != null && message.FromId.Value == -group.Id)
                {
                    message.FirstName = group.Name;
                    message.LastName = null;
                    message.Photo100 = group.Photo100;
                    message.ScreenName = group.ScreenName;
                }
            }

            return userMessages;
        }

        public static VkUserMessage GetCustomerMessage(int id)
        {
            var message =
                SQLDataAccess.Query<VkUserMessage>(
                    "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.ScreenName, u.Photo100 From Customers.VkMessage m " +
                    "Left Join Customers.VkUser u On u.Id = m.UserId " +
                    "Where m.Id=@id", 
                    new {id}).FirstOrDefault();

            if (message == null)
                return null;

            var group = SettingsVk.Group;
            
            // В исходящих сообщениях FromId - автор сообщения
            if (message.FromId != null && group != null && message.FromId.Value == -group.Id)
            {
                message.FirstName = group.Name;
                message.LastName = null;
                message.Photo100 = group.Photo100;
                message.ScreenName = group.ScreenName;
            }

            return message;
        }


        public static void AddMessages(List<VkMessage> messages)
        {
            if (messages == null || messages.Count == 0)
                return;
            
            foreach (var message in messages.Where(x => x.MessageId != null && x.UserId != null))
            {
                AddMessage(message);
            }
        }

        public static void AddMessage(VkMessage message, bool notify = false)
        {
            message.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "If not Exists(Select 1 From [Customers].[VkMessage] Where MessageId=@MessageId and UserId=@UserId and (PostId=@PostId or @PostId is null)) " +
                "begin " +
                    "Insert Into [Customers].[VkMessage] (MessageId, UserId, Date, Body, ChatId, FromId, Type, PostId) " +
                    "Values (@MessageId, @UserId, @Date, @Body, @ChatId, @FromId, @Type, @PostId); " +
                    "Select scope_identity(); " +
                "end",
                CommandType.Text,
                new SqlParameter("@MessageId", message.MessageId ?? 0),
                new SqlParameter("@UserId", message.UserId ?? 0),
                new SqlParameter("@Date", message.Date ?? DateTime.Now),
                new SqlParameter("@Body", message.Body ?? ""),
                new SqlParameter("@ChatId", message.ChatId ?? (object)DBNull.Value),
                new SqlParameter("@FromId", message.FromId ?? (object)DBNull.Value),
                new SqlParameter("@Type", message.Type.ToString()),
                new SqlParameter("@PostId", message.PostId ?? (object)DBNull.Value)
            ));
            CacheManager.RemoveByPattern(VkCacheKeyMessage);
        }

        public static VkMessage GetMessage(int id)
        {
            return CacheManager.Get(VkCacheKeyMessage + "_id_" + id,
                () =>
                    SQLDataAccess.Query<VkMessage>("Select * From [Customers].[VkMessage] Where Id=@id", new {id})
                        .FirstOrDefault());
        }

        public static VkMessage GetMessage(long messageId)
        {
            return CacheManager.Get(VkCacheKeyMessage + "_messageId_" + messageId,
                () =>
                    SQLDataAccess.Query<VkMessage>("Select * From [Customers].[VkMessage] Where MessageId=@messageId", new {messageId})
                        .FirstOrDefault());
        }


        /// <summary>
        /// Комментарии к посту
        /// </summary>
        public static List<VkMessage> GetPostMessages(long postId)
        {
            return CacheManager.Get(VkCacheKeyMessage + "_PostMessages_" + postId,
                () =>
                    SQLDataAccess.Query<VkMessage>("Select * From [Customers].[VkMessage] Where PostId=@postId",
                        new {postId}).ToList());
        }

        public static List<long> GetUserMessageIds(long userId)
        {
            return CacheManager.Get(VkCacheKeyMessage + "_UserMessageIds_" + userId,
                () =>
                    SQLDataAccess.Query<long>("Select MessageId From [Customers].[VkMessage] Where UserId=@userId",
                        new {userId}).ToList());
        }

        /// <summary>
        /// PostId и Count (кол-во комментариев к посту)
        /// </summary>
        public static List<VkMessagePostCount> GetPostMessagesCount()
        {
            return
                SQLDataAccess.Query<VkMessagePostCount>(
                    "Select PostId, COUNT(MessageId) From[Customers].[VkMessage] Where PostId is not null Group By PostId")
                    .ToList();
        }

        #endregion
    }
}
