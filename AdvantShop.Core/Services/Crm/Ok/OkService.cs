using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok
{
    public static class OkService
    {
        private const string okCacheKeyCustomer = "_ok_User_";
        private const string okCacheKeyMessage = "_ok_Message_";

        #region User
        public static OkUser GetUser(string userId)
        {
            return CacheManager.Get(CacheNames.Customer + okCacheKeyCustomer + userId, 20,
                () =>
                    SQLDataAccess.Query<OkUser>("Select * From [Customers].[okUser] Where Id=@id", new { id = userId })
                        .FirstOrDefault());
        }
        public static OkUser GetUser(Guid customerId)
        {
            return CacheManager.Get(CacheNames.Customer + okCacheKeyCustomer + "_customerId_" + customerId,
                () =>
                    SQLDataAccess.Query<OkUser>("Select * From [Customers].[okUser] Where CustomerId=@customerId", new { customerId })
                        .FirstOrDefault());
        }
        public static void AddUser(OkUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[okUser] (Id, CustomerId, FirstName, LastName, Photo)" +
                "values (@Id, @CustomerId, @FirstName, @LastName, @Photo)",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@FirstName", user.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", user.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Photo", user.Photo ?? (object)DBNull.Value)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + okCacheKeyCustomer);
        }
        public static void UpdateUser(OkUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[okUser] Set CustomerId=@CustomerId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + okCacheKeyCustomer);
        }
        public static void DeleteUser(string id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[okUser] Where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(CacheNames.Customer + okCacheKeyCustomer);
        }
        
        public static void ChangeCustomer(Guid customerId, Guid newCustomerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[okUser] Set CustomerId=@NewCustomerId Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@NewCustomerId", newCustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + okCacheKeyCustomer);
        }
        
        #endregion

        #region Message
        public static List<OkUserMessage> GetCustomerMessages(Guid customerId)
        {
            var user = GetUser(customerId);
            if (user == null)
                return null;

            var userMessages = SQLDataAccess.Query<OkUserMessage>(
                    "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.Photo From Customers.okMessage m " +
                    "Left Join Customers.okUser u On u.Id = m.FromUser " +
                    "Left Join Customers.Customer cu On cu.CustomerId = u.CustomerId " +
                    "Where m.UserId=@userId OR m.FromUser=@userId " +
                    "Order by CreatedDate desc ",
                    new { userId = user.Id })
                    .ToList();

            foreach (var message in userMessages)
            {
                if (message.FromUser != null && message.FromUser == SettingsOk.GroupId)
                {
                    message.FirstName = SettingsOk.GroupName;
                    message.LastName = null;
                }
            }
            return userMessages;
        }
        public static void AddMessage(OkMessage message)
        {
            message.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "If not Exists(Select 1 From [Customers].[okMessage] Where MessageId=@MessageId) " +
                "begin " +
                    "Insert Into [Customers].[okMessage] (MessageId, ChatId, UserId, FromUser, Text, CreatedDate) " +
                    "Values (@MessageId, @ChatId, @UserId, @FromUser, @Text, @CreatedDate); " +
                    "Select scope_identity(); " +
                "end",
                CommandType.Text,
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@ChatId", message.ChatId),
                new SqlParameter("@UserId", message.UserId),
                new SqlParameter("@FromUser", message.FromUser),
                new SqlParameter("@Text", message.Text),
                new SqlParameter("@CreatedDate", message.CreatedDate)
            ));
        }
        public static OkMessage GetMessage(int id)
        {
            return CacheManager.Get(okCacheKeyMessage + "_id_" + id,
                () =>
                    SQLDataAccess.Query<OkMessage>("Select * From [Customers].[okMessage] Where Id=@id", new { id })
                        .FirstOrDefault());
        }
        public static OkUserMessage GetUserMessage(int id)
        {
            return CacheManager.Get(okCacheKeyMessage + "_id_" + id,
               () =>
                   SQLDataAccess.Query<OkUserMessage>(
                       "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.Photo From [Customers].[okMessage] m " +
                       "Left Join Customers.okUser u On u.Id = m.FromUser " +
                       "Where m.Id=@id"
                       , new { id }).FirstOrDefault());
        }
        public static OkMessage GetMessageByCustomerId(Guid customerId)
        {
            return SQLDataAccess.Query<OkMessage>(
                "Select top(1) m.* From [Customers].[okMessage] m " +
                "Inner Join Customers.okUser u On u.Id = m.FromUser " +
                "Where u.CustomerId=@customerId " +
                "Order by CreatedDate desc",
                new { customerId }).FirstOrDefault();
        }
        #endregion
    }
}