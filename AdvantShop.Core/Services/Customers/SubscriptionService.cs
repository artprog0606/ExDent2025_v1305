//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.UrlRewriter;


namespace AdvantShop.Customers
{
    public class SubscriptionService
    {
        public static void AddSubscription(Subscription subscription)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"INSERT INTO [Customers].[Subscription] 
                                (Email,Subscribe,SubscribeDate,UnsubscribeDate) 
                                VALUES (@Email,@Subscribe,GETDATE(),NULL)",
                CommandType.Text,
                new SqlParameter("@Email", subscription.Email),
                new SqlParameter("@Subscribe", subscription.Subscribe));

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void UpdateSubscription(Subscription subscription)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Customers].[Subscription] 
                                SET Email=@Email, Subscribe=@Subscribe 
                                WHERE [Id]=@Id",
                CommandType.Text,
                new SqlParameter("@Email", subscription.Email),
                new SqlParameter("@Subscribe", subscription.Subscribe),
                new SqlParameter("@Id", subscription.Id));

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static bool IsExistsSubscription(string email)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "IF(SELECT Count(Id) FROM [Customers].[Subscription] WHERE [Email] = @Email) > 0 BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END",
                CommandType.Text,
                new SqlParameter("@Email", email));
        }

        public static bool IsSubscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var id = SQLDataAccess.ExecuteScalar(
                "SELECT Id FROM [Customers].[Subscription] WHERE [Email] = @Email AND [Subscribe] = 1",
                CommandType.Text, new SqlParameter("@Email", email));

            return id != null && !(id is DBNull);
        }

        public static void Subscribe(string email)
        {
            if (email.IsNullOrEmpty())
                return;

            var fromUri = HttpContext.Current != null && HttpContext.Current.TryGetRequest(out var request) &&
                          CustomerContext.CurrentCustomer != null && !CustomerContext.CurrentCustomer.IsAdmin &&
                          !CustomerContext.CurrentCustomer.IsModerator
                ? request.GetUrlReferrer() ?? request.Url
                : null;
            var fromIp = HttpContext.Current != null && CustomerContext.CurrentCustomer != null &&
                         !CustomerContext.CurrentCustomer.IsAdmin && !CustomerContext.CurrentCustomer.IsModerator
                ? HttpContext.Current.TryGetIp()
                : null;

            SQLDataAccess.ExecuteNonQuery(
                @"IF (SELECT COUNT(*)
                                FROM [Customers].[Subscription]
                                WHERE [Email] = @Email) > 0
                                BEGIN
                                    UPDATE [Customers].[Subscription]
                                    SET [Subscribe]       = 1
                                      , [SubscribeDate]   = GETDATE()
                                      , [SubscribeFromPage]   = @SubscribeFromPage
                                      , [SubscribeFromIp] = @SubscribeFromIp
                                    WHERE [Email] = @Email
                                END
                            ELSE
                                BEGIN
                                    INSERT INTO [Customers].[Subscription] ([Email], [Subscribe], [SubscribeDate], [UnsubscribeDate],
                                                                            [SubscribeFromPage], [SubscribeFromIp])
                                    VALUES (@Email, 1, GETDATE(), NULL, @SubscribeFromPage, @SubscribeFromIp)
                                END",
                CommandType.Text,
                new SqlParameter("@Email", email),
                new SqlParameter("@SubscribeFromPage", fromUri != null
                    ? fromUri.GetLeftPart(UriPartial.Path).Reduce(500)
                    : (object)DBNull.Value),
                new SqlParameter("@SubscribeFromIp", fromIp ?? (object)DBNull.Value));

            var customer = CustomerService.GetCustomerByEmail(email);
            var subscription = new Subscription { Email = email, CustomerType = EMailRecipientType.Subscriber };
            if (customer != null)
            {
                subscription.FirstName = customer.FirstName;
                subscription.LastName = customer.LastName;
                subscription.Phone = customer.StandardPhone.ToString();
                subscription.CustomerType = EMailRecipientType.Subscriber;
            }

            ModulesExecuter.Subscribe(subscription);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void Subscribe(int id)
        {
            var fromUri = HttpContext.Current != null && HttpContext.Current.TryGetRequest(out var request) &&
                          CustomerContext.CurrentCustomer != null && !CustomerContext.CurrentCustomer.IsAdmin &&
                          !CustomerContext.CurrentCustomer.IsModerator
                ? request.GetUrlReferrer() ?? request.Url
                : null;
            var fromIp = HttpContext.Current != null && CustomerContext.CurrentCustomer != null &&
                         !CustomerContext.CurrentCustomer.IsAdmin && !CustomerContext.CurrentCustomer.IsModerator
                ? HttpContext.Current.TryGetIp()
                : null;

            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Customers].[Subscription] 
                            SET [Subscribe] = 1
                              ,[SubscribeDate] = GETDATE()
                              ,[SubscribeFromPage] = @SubscribeFromPage 
                              ,[SubscribeFromIp] = @SubscribeFromIp 
                            WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id),
                new SqlParameter("@SubscribeFromPage", fromUri != null
                    ? fromUri.GetLeftPart(UriPartial.Path).Reduce(500)
                    : (object)DBNull.Value),
                new SqlParameter("@SubscribeFromIp", fromIp ?? (object)DBNull.Value));

            //todo: лишние запросы получения кастомера и подписки <Sckeef>
            var subscription = GetSubscription(id);
            var customer = CustomerService.GetCustomerByEmail(subscription.Email);
            subscription = new Subscription
            {
                Email = subscription.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.StandardPhone.ToString(),
                CustomerType = EMailRecipientType.Subscriber
            };

            ModulesExecuter.Subscribe(subscription);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void Unsubscribe(string email)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Customers].[Subscription] SET [Subscribe] = 0, [UnsubscribeDate] = GETDATE() WHERE [Email] = @Email",
                CommandType.Text,
                new SqlParameter("@Email", email));

            ModulesExecuter.UnSubscribe(email);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static List<Subscription> GetSubscriptions()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Customers].[Subscription]",
                CommandType.Text, GetSubscriptionFromReader);
        }

        public static Subscription GetSubscription(int id)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT TOP 1 * FROM [Customers].[Subscription] WHERE Id = @Id",
                CommandType.Text, GetSubscriptionFromReader, new SqlParameter("@Id", id));
        }

        public static Subscription GetSubscriptionExt(int id)
        {
            return SQLDataAccess.ExecuteReadOne(
                @"SELECT TOP 1 s.*, c.FirstName, c.LastName, c.Phone, c.CustomerId, c.StandardPhone 
                                FROM Customers.Subscription s  
                                LEFT JOIN Customers.Customer c ON s.Email = c.Email  
                                WHERE s.Id = @Id and c.IsAgreeForPromotionalNewsletter = 1",
                CommandType.Text,
                reader => new Subscription
                {
                    Id = SQLDataHelper.GetInt(reader, "Id"),
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    Subscribe = SQLDataHelper.GetBoolean(reader, "Subscribe"),
                    SubscribeDate = SQLDataHelper.GetDateTime(reader, "SubscribeDate"),
                    SubscribeFromPage = SQLDataHelper.GetString(reader, "SubscribeFromPage"),
                    SubscribeFromIp = SQLDataHelper.GetString(reader, "SubscribeFromIp"),
                    UnsubscribeDate = SQLDataHelper.GetDateTime(reader, "UnsubscribeDate"),
                    FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                    LastName = SQLDataHelper.GetString(reader, "LastName"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                    StandardPhone = SQLDataHelper.GetNullableLong(reader, "StandardPhone"),
                    CustomerId = SQLDataHelper.GetNullableGuid(reader, "CustomerId")
                },
                new SqlParameter("@Id", id));
        }

        public static Subscription GetSubscription(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            
            return SQLDataAccess.ExecuteReadOne(
                "SELECT TOP 1 * FROM [Customers].[Subscription] WHERE [Email] = @Email",
                CommandType.Text, GetSubscriptionFromReader,
                new SqlParameter("@Email", email));
        }

        public static List<ISubscriber> GetSubscribedEmails()
        {
            return SQLDataAccess.ExecuteReadList<ISubscriber>(
                @"SELECT [Subscription].[Email], [Customer].[FirstName], [Customer].[LastName], [Customer].[Phone] 
                                FROM [Customers].[Subscription] 
                                LEFT JOIN [Customers].[Customer] ON [Subscription].[Email] = [Customer].[Email] 
                                WHERE [Subscribe] = 1",
                CommandType.Text,
                reader => new Subscription
                {
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                    LastName = SQLDataHelper.GetString(reader, "LastName"),
                    Phone = SQLDataHelper.GetString(reader, "Phone"),
                });
        }

        public static void DeleteSubscription(int id)
        {
            var email = SQLDataAccess.ExecuteScalar<string>(
                "Select email from  [Customers].[Subscription] WHERE [Id] = @Id; DELETE FROM [Customers].[Subscription] WHERE [Id] = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));

            ModulesExecuter.UnSubscribe(email);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static void DeleteSubscription(string email)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Customers].[Subscription] WHERE [Email] = @Email",
                CommandType.Text,
                new SqlParameter("@Email", email));

            ModulesExecuter.UnSubscribe(email);

            CacheManager.RemoveByPattern(CacheNames.Customer);
        }

        public static Subscription GetSubscriptionFromReader(SqlDataReader reader)
        {
            return new Subscription
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Subscribe = SQLDataHelper.GetBoolean(reader, "Subscribe"),
                SubscribeDate = SQLDataHelper.GetDateTime(reader, "SubscribeDate"),
                SubscribeFromPage = SQLDataHelper.GetString(reader, "SubscribeFromPage"),
                SubscribeFromIp = SQLDataHelper.GetString(reader, "SubscribeFromIp"),
                UnsubscribeDate = SQLDataHelper.GetDateTime(reader, "UnsubscribeDate")
            };
        }
    }
}