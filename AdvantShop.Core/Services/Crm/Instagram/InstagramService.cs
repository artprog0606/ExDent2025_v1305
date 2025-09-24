using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    public static class InstagramService
    {
        #region Message

        public static void AddMessages(List<InstagramMessage> messages)
        {
            if (messages == null || messages.Count == 0)
                return;

            foreach (var message in messages)
                AddMessage(message);
        }

        public static void AddMessage(InstagramMessage message)
        {
            message.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "If not Exists(Select 1 From [Customers].[InstagramMessage] Where InstagramId=@InstagramId) " +
                "begin " +
                    "Insert Into [Customers].[InstagramMessage] (ThreadId, MediaPk, InstagramId, FromUserPk, ToUserPk, Text, CreatedDate, ItemType, Title) " +
                    "Values (@ThreadId, @MediaPk, @InstagramId, @FromUserPk, @ToUserPk, @Text, @CreatedDate, @ItemType, @Title); " +
                    "Select scope_identity(); " +
                "end",
                CommandType.Text,
                new SqlParameter("@ThreadId", message.ThreadId ?? ""),
                new SqlParameter("@MediaPk", message.MediaPk ?? ""),
                new SqlParameter("@InstagramId", message.InstagramId),
                new SqlParameter("@FromUserPk", message.FromUserPk ?? ""),
                new SqlParameter("@ToUserPk", message.ToUserPk ?? ""),
                new SqlParameter("@Text", message.Text ?? ""),
                new SqlParameter("@CreatedDate", new DateTime(message.CreatedDate.Ticks)),
                new SqlParameter("@ItemType", (int) message.ItemType),
                new SqlParameter("@Title", message.Title ?? "")
            ));
        }

        public static InstagramUserMessage GetMessage(int id)
        {
            return
                SQLDataAccess.Query<InstagramUserMessage>("Select * From Customers.InstagramMessage Where Id=@id",
                    new {id}).FirstOrDefault();
        }

        public static List<InstagramUserMessage> GetCustomerMessages(Guid customerId)
        {
            var user = GetUserByCustomerId(customerId);
            if (user == null)
                return null;

            return SQLDataAccess.Query<InstagramUserMessage>(
                "Select * " +
                "From Customers.InstagramMessage m " +
                "Left Join Customers.InstagramUser u On u.Pk = m.FromUserPk " +
                "Where FromUserPk=@pk Or ToUserPk=@pk " +
                "Order By m.CreatedDate Desc", 
                new {pk = user.Pk}).ToList();
        }


        public static InstagramUserMessage GetCustomerMessage(int id)
        {
            return SQLDataAccess.Query<InstagramUserMessage>(
                "Select * " +
                "From Customers.InstagramMessage m " +
                "Left Join Customers.InstagramUser u On u.Pk = m.FromUserPk " +
                "Where m.Id=@id",
                new {id}).FirstOrDefault();
        }

        public static int GetMessagesCount(string mediaPk)
        {
            return Convert.ToInt32(
                SQLDataAccess.ExecuteScalar(
                    "Select Count(*) From Customers.InstagramMessage Where MediaPk=@mediaPk", CommandType.Text,
                    new SqlParameter("@mediaPk", mediaPk)));
        }

        public static void DeleteAllMessages()
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[InstagramMessage]", CommandType.Text);
        }

        #endregion
        
        #region User

        public static List<InstagramUser> GetUsersByPks(List<string> pks)
        {
            if (pks == null || pks.Count == 0)
                return new List<InstagramUser>();

            return
                SQLDataAccess.Query<InstagramUser>(
                    "Select * From [Customers].[InstagramUser] Where Pk in (" + String.Join(",", pks.Select(x => "'" + x + "'")) + ")").ToList();
        }

        public static InstagramUser GetUser(string pk)
        {
            return SQLDataAccess.Query<InstagramUser>("Select * From [Customers].[InstagramUser] Where Pk=@pk", new {pk}).FirstOrDefault();
        }

        public static InstagramUser GetUserByUserName(string userName)
        {
            return SQLDataAccess.Query<InstagramUser>("Select * From [Customers].[InstagramUser] Where UserName=@userName", new { userName }).FirstOrDefault();
        }

        public static InstagramUser GetUserByCustomerId(Guid customerId)
        {
            return SQLDataAccess.Query<InstagramUser>("Select * From [Customers].[InstagramUser] Where CustomerId=@customerId", new { customerId }).FirstOrDefault();
        }

        public static void AddUser(InstagramUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[InstagramUser] (CustomerId, Pk, UserName, FullName, Email, PhoneNumber, ProfilePicture) " +
                "Values (@CustomerId, @Pk, @UserName, @FullName, @Email, @PhoneNumber, @ProfilePicture) ",
                CommandType.Text,
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@Pk", user.Pk),
                new SqlParameter("@UserName", user.UserName ?? ""),
                new SqlParameter("@FullName", user.FullName ?? ""),
                new SqlParameter("@Email", user.Email ?? ""),
                new SqlParameter("@PhoneNumber", user.PhoneNumber ?? ""),
                new SqlParameter("@ProfilePicture", user.ProfilePicture ?? "")
            );
        }

        public static void UpdateUser(InstagramUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[InstagramUser] Set CustomerId=@CustomerId Where Pk=@Pk",
                CommandType.Text,
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@Pk", user.Pk)
            );
        }

        public static void DeleteUser(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[InstagramUser] Where CustomerId=@customerId",
                CommandType.Text, new SqlParameter("@customerId", customerId));
        }

        public static void DeleteAllUsers()
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Customers].[InstagramUser]", CommandType.Text);
        }

        public static void ChangeCustomer(Guid customerId, Guid newCustomerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[InstagramUser] Set CustomerId=@NewCustomerId Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@NewCustomerId", newCustomerId)
            );
        }

        #endregion
        
        #region Media

        public static void AddMedia(InstagramMessage message)
        {
            SQLDataAccess.ExecuteNonQuery(
                "If not Exists(Select 1 From [Customers].[InstagramMessage] Where InstagramId=@InstagramId) " +
                "begin " +
                "Insert Into [Customers].[InstagramMessage] (ThreadId, MediaPk, InstagramId, FromUserPk, ToUserPk, Text, CreatedDate, ItemType) " +
                "Values (@ThreadId, @MediaPk, @InstagramId, @FromUserPk, @ToUserPk, @Text, @CreatedDate, @ItemType) " +
                "end",
                CommandType.Text,
                new SqlParameter("@ThreadId", message.ThreadId ?? ""),
                new SqlParameter("@MediaPk", message.MediaPk ?? ""),
                new SqlParameter("@InstagramId", message.InstagramId),
                new SqlParameter("@FromUserPk", message.FromUserPk ?? ""),
                new SqlParameter("@ToUserPk", message.ToUserPk ?? ""),
                new SqlParameter("@Text", message.Text ?? ""),
                new SqlParameter("@CreatedDate", message.CreatedDate),
                new SqlParameter("@ItemType", (int)message.ItemType)
            );
        }

        #endregion

    }
}
