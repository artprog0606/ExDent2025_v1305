using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Track.EventLog
{
    public static class TrackEventLogService
    {
        public static bool IsEventLimitedPerHour(string eventKey) =>
            CountEventLogsPerLastHour(eventKey) >= SettingsTracking.MaxEventsPerHour;

        public static int CountEventLogsPerLastHour(string eventKey)
        {
            var count = SQLDataAccess.ExecuteScalar<int>(
                "select count(*) from [dbo].TrackEventLogs where EventKey = @EventKey and TrackedAtUTC > @GapTime",
                CommandType.Text,
                new SqlParameter("@EventKey", eventKey),
                new SqlParameter("@GapTime", DateTime.UtcNow.AddHours(-1))
            );
            return count;
        }

        public static void AddEventLog(string eventKey, HttpContext httpContext)
        {
            var userAgent = httpContext?.Request.UserAgent;
            var ip = httpContext?.TryGetIp();
            SQLDataAccess.ExecuteNonQuery(
                "insert into [dbo].TrackEventLogs (EventKey, UserAgent, Ip) values (@EventKey, @UserAgent, @Ip)",
                CommandType.Text,
                new SqlParameter("@EventKey", eventKey),
                new SqlParameter("@UserAgent", userAgent.IsNullOrEmpty() ? (object)DBNull.Value : userAgent),
                new SqlParameter("@Ip", ip.IsNullOrEmpty() ? (object)DBNull.Value : ip)
            );
        }

        public static void ClearExpiredEventLogs()
        {
            SQLDataAccess.ExecuteNonQuery(
                "delete from [dbo].TrackEventLogs where TrackedAtUTC > @GapTime",
                CommandType.Text,
                new SqlParameter("@GapTime", DateTime.UtcNow.AddDays(-14)));
        }

        public static bool IsEventWasToday(string eventKey) => 
            SQLDataAccess.ExecuteReadOne(
                @"IF EXISTS (SELECT 1
                           FROM [dbo].[TrackEventLogs]
                           WHERE [EventKey] = @EventKey
                             AND CONVERT(date, [TrackedAtUTC]) = CONVERT(date, GETDATE()))
                    BEGIN
                        SELECT 1 AS WasToday
                    END
                ELSE
                    BEGIN
                        SELECT 0 AS WasToday
                    END",
                CommandType.Text,
                reader => SQLDataHelper.GetBoolean(reader, "WasToday"),
                new SqlParameter("@EventKey", eventKey));
    }
}