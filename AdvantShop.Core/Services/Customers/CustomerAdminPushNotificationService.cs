using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Customers
{
    public static class CustomerAdminPushNotificationService
    {
        public static List<(Guid CustomerId, string FcmToken)> GetFcmTokensToNotify()
        {
            return SQLDataAccess.ExecuteReadList(
                "select CustomerId, FcmToken From [Customers].[CustomerAdminPushNotification] Where FcmToken is not null and NotificationsEnabled=1",
                CommandType.Text,
                reader => (SQLDataHelper.GetGuid(reader, "CustomerId"), SQLDataHelper.GetString(reader, "FcmToken")));
        }

        public static void AddNew(Guid customerId, string fcmToken = null)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [Customers].[CustomerAdminPushNotification] " +
                "(CustomerId, FcmToken, NotificationsEnabled) " +
                "Values (@CustomerId, @FcmToken, 1)",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@FcmToken", fcmToken ?? (object)DBNull.Value));
        }

        public static bool Exists(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select 1 from [Customers].[CustomerAdminPushNotification] where CustomerId = @CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId));
        }

        public static void UpdateFcmToken(Guid customerId, string fcmToken)
        {
            if (!string.IsNullOrWhiteSpace(fcmToken))
            {
                SQLDataAccess.ExecuteNonQuery(
                    "Update [Customers].[CustomerAdminPushNotification] Set FcmToken = null Where FcmToken=@FcmToken",
                    CommandType.Text,
                    new SqlParameter("@FcmToken", fcmToken));
            }

            if (!Exists(customerId))
            {
                AddNew(customerId, fcmToken);
                return;
            }

            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[CustomerAdminPushNotification] Set FcmToken=@FcmToken Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@FcmToken", fcmToken ?? (object)DBNull.Value));
        }

        public static (string fcmToken, bool notificationEnabled) GetNotificationSettings(Guid customerId) =>
            SQLDataAccess.ExecuteReadOne(
                "select FcmToken, NotificationsEnabled from [Customers].[CustomerAdminPushNotification] Where CustomerId=@CustomerId",
                CommandType.Text,
                reader => (fcmToken: SQLDataHelper.GetString(reader, "FcmToken"),
                    notificationEnabled: SQLDataHelper.GetBoolean(reader, "NotificationsEnabled")),
                new SqlParameter("@CustomerId", customerId));

        public static void SetNotificationsEnabled(Guid customerId, bool enabled)
        {
            if (!Exists(customerId))
            {
                AddNew(customerId);
            }

            SQLDataAccess.ExecuteNonQuery(
                "Update [Customers].[CustomerAdminPushNotification] Set NotificationsEnabled=@NotificationsEnabled " +
                "Where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@NotificationsEnabled", enabled));
        }
    }
}