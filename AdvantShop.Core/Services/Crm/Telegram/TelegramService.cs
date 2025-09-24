using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Crm.Telegram
{
    public class TelegramService
    {
        private const string TelegramCacheKeyCustomer = "_Telegram_User_";
        private const string TelegramCacheKeyMessage = "_Telegram_Message_";

        #region User

        public TelegramUser GetUser(long userId)
        {
            return CacheManager.Get(CacheNames.Customer + TelegramCacheKeyCustomer + userId, 20,
                () =>
                    SQLDataAccess.Query<TelegramUser>("Select * From [Customers].[TelegramUser] Where Id=@id", new { id = userId })
                        .FirstOrDefault());
        }

        public TelegramUser GetUser(Guid customerId)
        {
            return CacheManager.Get(CacheNames.Customer + TelegramCacheKeyCustomer + "_customerId_" + customerId,
                () =>
                    SQLDataAccess.Query<TelegramUser>("Select * From [Customers].[TelegramUser] Where CustomerId=@customerId", new { customerId })
                        .FirstOrDefault());
        }


        public void AddUser(TelegramUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[TelegramUser] (Id, CustomerId, FirstName, LastName, Username, IsBot, PhotoUrl) " +
                "Values (@Id, @CustomerId, @FirstName, @LastName, @Username, @IsBot, @PhotoUrl)",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId),
                new SqlParameter("@FirstName", user.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", user.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Username", user.Username ?? (object)DBNull.Value),
                new SqlParameter("@PhotoUrl", user.PhotoUrl ?? (object)DBNull.Value),
                new SqlParameter("@IsBot", user.IsBot)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + TelegramCacheKeyCustomer);
        }

        public void UpdateUser(TelegramUser user)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[TelegramUser] Set CustomerId=@CustomerId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@CustomerId", user.CustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + TelegramCacheKeyCustomer);
        }
        
        public void ChangeCustomer(Guid customerId, Guid newCustomerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[TelegramUser] Set CustomerId=@NewCustomerId Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@NewCustomerId", newCustomerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + TelegramCacheKeyCustomer);
        }
        
        public void DeleteUser(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete from [Customers].[TelegramUser] Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId)
            );

            CacheManager.RemoveByPattern(CacheNames.Customer + TelegramCacheKeyCustomer);
        }

        #endregion

        #region Message

        /// <summary>
        /// Сообщения от/к пользователю из контакта
        /// </summary>
        public List<TelegramUserMessage> GetCustomerMessages(Guid customerId)
        {
            var user = GetUser(customerId);
            if (user == null)
                return null;

            var userMessages =
                SQLDataAccess.Query<TelegramUserMessage>(
                    "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.Username, cu.Avatar as PhotoUrl " +
                    "From Customers.TelegramMessage m " +
                    "Left Join Customers.TelegramUser u On u.Id = m.FromId " +
                    "Left Join Customers.Customer cu On cu.CustomerId = u.CustomerId " +
                    "Where m.FromId=@userId or m.ToId=@userId  " +
                    "Order by Date desc",
                    new { userId = user.Id })
                    .ToList();

            var bot = SettingsTelegram.BotUser;
            if (bot != null)
            {
                foreach (var message in userMessages)
                {
                    if (message.FromId == bot.Id)
                    {
                        message.LastName = bot.LastName;
                        message.FirstName = bot.FirstName;
                        message.UserName = bot.Username;
                        message.PhotoUrl = bot.PhotoUrl;
                    }
                }
            }

            return userMessages;
        }

        public TelegramUserMessage GetCustomerMessage(int messageId)
        {
            var message =
                SQLDataAccess.Query<TelegramUserMessage>(
                    "Select m.*, u.CustomerId, u.FirstName, u.LastName, u.Username, cu.Avatar as PhotoUrl " +
                    "From Customers.TelegramMessage m " +
                    "Left Join Customers.TelegramUser u On u.Id = m.FromId " +
                    "Left Join Customers.Customer cu On cu.CustomerId = u.CustomerId " +
                    "Where m.MessageId=@messageId",
                    new { messageId }).FirstOrDefault();

            if (message == null)
                return null;

            var bot = SettingsTelegram.BotUser;

            if (bot != null && message.FromId == bot.Id)
            {
                message.LastName = bot.LastName;
                message.FirstName = bot.FirstName;
                message.UserName = bot.Username;
                message.PhotoUrl = bot.PhotoUrl;
            }

            return message;
        }


        public void AddMessage(TelegramMessage message)
        {
            message.Id = Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "If not Exists(Select 1 From [Customers].[TelegramMessage] Where MessageId=@MessageId) " +
                "begin " +
                    "Insert Into [Customers].[TelegramMessage] (MessageId, FromId, ToId, Date, Text, ChatId, Type) " +
                    "Values (@MessageId, @FromId, @ToId, @Date, @Text, @ChatId, @Type); " +
                    "Select scope_identity(); " +
                "end",
                CommandType.Text,
                new SqlParameter("@MessageId", message.MessageId),
                new SqlParameter("@FromId", message.FromId),
                new SqlParameter("@ToId", message.ToId ?? (object)DBNull.Value),
                new SqlParameter("@Date", message.Date),
                new SqlParameter("@Text", message.Text ?? ""),
                new SqlParameter("@ChatId", message.ChatId),
                new SqlParameter("@Type", message.Type)
            ));
            CacheManager.RemoveByPattern(TelegramCacheKeyMessage);
        }

        public TelegramMessage GetMessage(int id)
        {
            return CacheManager.Get(TelegramCacheKeyMessage + "_id_" + id,
                () =>
                    SQLDataAccess.Query<TelegramMessage>("Select * From [Customers].[TelegramMessage] Where Id=@id", new { id })
                        .FirstOrDefault());
        }

        public TelegramMessage GetMessageByCustomerId(Guid customerId)
        {
            return SQLDataAccess.Query<TelegramMessage>(
                "Select top(1) m.* From [Customers].[TelegramMessage] m " +
                "Inner Join Customers.TelegramUser u On u.Id = m.FromId " +
                "Where u.CustomerId=@customerId " +
                "Order by Date desc", 
                new { customerId }).FirstOrDefault();
        }

        public TelegramMessage GetMessageByMessageId(int messageId)
        {
            return CacheManager.Get(TelegramCacheKeyMessage + "_messageId_" + messageId,
                () =>
                    SQLDataAccess.Query<TelegramMessage>("Select * From [Customers].[TelegramMessage] Where MessageId=@messageId", new { messageId })
                        .FirstOrDefault());
        }
        
        #endregion
    }
}
