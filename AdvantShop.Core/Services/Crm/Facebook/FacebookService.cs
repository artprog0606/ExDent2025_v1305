using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    public class FacebookService
    {
        private const string FbCacheKeyCustomerId = "_Fb_User_";

        #region Message 

        public static void AddMessages(List<FacebookMessage> messages)
        {
            if (messages == null || messages.Count == 0)
                return;

            foreach (var message in messages)
                AddMessage(message);
        }

        public static void AddMessage(FacebookMessage message)
        {
            message.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "If not Exists(Select 1 From [Customers].[FacebookMessage] Where MessageId=@MessageId) " +
                "begin " +
                    "Insert Into [Customers].[FacebookMessage] (MessageId, FromId, ToId, Message, CreatedTime, Type, PostId, ConversationId) " +
                    "Values (@MessageId, @FromId, @ToId, @Message, @CreatedTime, @Type, @PostId, @ConversationId); " +
                    "Select scope_identity(); " +
                "end",
                CommandType.Text,
                new SqlParameter("@MessageId", message.MessageId ?? ""),
                new SqlParameter("@FromId", message.FromId ?? (object) DBNull.Value),
                new SqlParameter("@ToId", message.ToId ?? (object) DBNull.Value),
                new SqlParameter("@Message", message.Message ?? ""),
                new SqlParameter("@CreatedTime", message.CreatedTime),
                new SqlParameter("@Type", (int) message.Type),
                new SqlParameter("@PostId", message.PostId ?? (object) DBNull.Value),
                new SqlParameter("@ConversationId", message.ConversationId ?? (object) DBNull.Value)
            ));
        }

        public static FacebookMessage Get(int id)
        {
            return
                SQLDataAccess.Query<FacebookMessage>(
                    "Select * From[Customers].[FacebookMessage] Where Id=@id", new { id })
                    .FirstOrDefault();
        }

        public static FacebookMessage Get(string messageId)
        {
            return
                SQLDataAccess.Query<FacebookMessage>(
                    "Select * From[Customers].[FacebookMessage] Where MessageId=@messageId", new {messageId})
                    .FirstOrDefault();
        }

        public static List<FacebookMessage> GetPostMessages(string postId)
        {
            return
                SQLDataAccess.Query<FacebookMessage>(
                    "Select * From[Customers].[FacebookMessage] Where PostId=@postId", new {postId})
                    .ToList();
        }

        public static List<FacebookMessage> GetConversationMessages(string conversationtId)
        {
            return
                SQLDataAccess.Query<FacebookMessage>(
                    "Select * From[Customers].[FacebookMessage] Where ConversationId=@conversationtId", new { conversationtId })
                    .ToList();
        }

        public static List<FacebookUserMessage> GetCustomerMessages(Guid customerId)
        {
            var user = GetUser(customerId);
            if (user == null)
                return null;

            return SQLDataAccess.Query<FacebookUserMessage>(
                "Select m.*, u.Id as UserId, u.FirstName, u.LastName, u.CustomerId, u.PhotoPicByPsyId " +
                "From Customers.FacebookMessage m " +
                "Left Join Customers.FacebookUser u On u.Id = m.FromId " +
                "Where FromId=@pk Or ToId=@pk " +
                "Order By m.CreatedTime Desc",
                new { pk = user.Id }).ToList();
        }

        public static FacebookUserMessage GetCustomerMessage(int id)
        {
            return SQLDataAccess.Query<FacebookUserMessage>(
                "Select m.*, u.Id as UserId, u.FirstName, u.LastName, u.CustomerId " +
                "From Customers.FacebookMessage m " +
                "Left Join Customers.FacebookUser u On u.Id = m.FromId " +
                "Where m.Id = @id ",
                new { id }).FirstOrDefault();
        }

        #endregion

        #region User 

        public static FacebookUser GetUser(string id)
        {
            return CacheManager.Get(CacheNames.Customer + FbCacheKeyCustomerId + id, 20,
                () =>
                    SQLDataAccess.Query<FacebookUser>("Select * From [Customers].[FacebookUser] Where Id=@id", new {id})
                        .FirstOrDefault());
        }

        public static FacebookUser GetUser(Guid customerId)
        {
            return CacheManager.Get(CacheNames.Customer + FbCacheKeyCustomerId + "_customerId_" + customerId,
                () =>
                    SQLDataAccess.Query<FacebookUser>("Select * From [Customers].[FacebookUser] Where CustomerId=@customerId", new {customerId})
                        .FirstOrDefault());
        }

        public static FacebookUser GetUserByPsyId(string psyId)
        {
            if (psyId == null)
                return null;

            return CacheManager.Get(CacheNames.Customer + FbCacheKeyCustomerId + "psyid" + psyId, 20,
                () =>
                    SQLDataAccess.Query<FacebookUser>("Select * From [Customers].[FacebookUser] Where PsyId=@psyId", new { psyId })
                        .FirstOrDefault());
        }


        public static void AddUser(FacebookUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[FacebookUser] (Id, CustomerId, FirstName, LastName, Gender, PsyId, PhotoPicByPsyId) " +
                "Values (@Id, @CustomerId, @FirstName, @LastName, @Gender, @PsyId, @PhotoPicByPsyId)",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@FirstName", user.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", user.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Gender", user.Gender ?? (object)DBNull.Value),
                new SqlParameter("@PsyId", user.PsyId ?? (object)DBNull.Value),
                new SqlParameter("@PhotoPicByPsyId", user.PhotoPicByPsyId ?? (object)DBNull.Value)
            );
        }

        public static void UpdateUser(FacebookUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[FacebookUser] Set CustomerId=@CustomerId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId)
            );
        }
        
        public static void DeleteUser(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[FacebookUser] Where CustomerId=@customerId",
                CommandType.Text, new SqlParameter("@customerId", customerId));

            CacheManager.RemoveByPattern(CacheNames.Customer + FbCacheKeyCustomerId);
        }

        #endregion

    }
}
