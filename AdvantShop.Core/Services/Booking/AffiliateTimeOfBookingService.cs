using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class AffiliateTimeOfBookingService
    {
        public static AffiliateTimeOfBooking Get(int id)
        {
            return
                SQLDataAccess.ExecuteReadOne<AffiliateTimeOfBooking>(
                    "SELECT TOP 1 * FROM [Booking].[AffiliateTimeOfBooking] WHERE Id = @Id", CommandType.Text, GetFromReader,
                    new SqlParameter("@Id", id));
        }

        public static List<AffiliateTimeOfBooking> GetByAffiliate(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadList<AffiliateTimeOfBooking>(
                    "SELECT * FROM [Booking].[AffiliateTimeOfBooking] WHERE AffiliateId = @AffiliateId ORDER BY [StartTime]",
                    CommandType.Text, GetFromReader, new SqlParameter("@AffiliateId", affiliateId));
        }

        public static List<AffiliateTimeOfBooking> GetByAffiliateAndDayOfWeek(int affiliateId, DayOfWeek dayOfWeek)
        {
            return
                SQLDataAccess.ExecuteReadList<AffiliateTimeOfBooking>(
                    "SELECT * FROM [Booking].[AffiliateTimeOfBooking] WHERE AffiliateId = @AffiliateId AND DayOfWeek = @DayOfWeek ORDER BY [StartTime]",
                    CommandType.Text, GetFromReader, 
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@DayOfWeek", (byte) dayOfWeek));
        }

        private static AffiliateTimeOfBooking GetFromReader(SqlDataReader reader)
        {
            return new AffiliateTimeOfBooking
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                AffiliateId = SQLDataHelper.GetInt(reader, "AffiliateId"),
                DayOfWeek = (DayOfWeek)SQLDataHelper.GetInt(reader, "DayOfWeek"),
                StartTime = SQLDataHelper.GetDateTime(reader, "StartTime").TimeOfDay,
                EndTime = SQLDataHelper.GetDateTime(reader, "EndTime").TimeOfDay
            };
        }

        public static int Add(AffiliateTimeOfBooking affiliatetimeofbooking)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[AffiliateTimeOfBooking] " +
                                                    " ([AffiliateId], [DayOfWeek], [StartTime], [EndTime]) " +
                                                    " VALUES (@AffiliateId, @DayOfWeek, @StartTime, @EndTime); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliatetimeofbooking.AffiliateId),
                new SqlParameter("@DayOfWeek", (byte) affiliatetimeofbooking.DayOfWeek),
                new SqlParameter("@StartTime", new DateTime(2000, 1, 1) + affiliatetimeofbooking.StartTime),
                new SqlParameter("@EndTime", new DateTime(2000, 1, 1) + affiliatetimeofbooking.EndTime)
                );
        }

        public static void Update(AffiliateTimeOfBooking affiliatetimeofbooking)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[AffiliateTimeOfBooking] SET [AffiliateId] = @AffiliateId, [DayOfWeek] = @DayOfWeek, [StartTime] = @StartTime, [EndTime] = @EndTime " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", affiliatetimeofbooking.Id),
                new SqlParameter("@AffiliateId", affiliatetimeofbooking.AffiliateId),
                new SqlParameter("@DayOfWeek", (byte) affiliatetimeofbooking.DayOfWeek),
                new SqlParameter("@StartTime", new DateTime(2000, 1, 1) + affiliatetimeofbooking.StartTime),
                new SqlParameter("@EndTime", new DateTime(2000, 1, 1) + affiliatetimeofbooking.EndTime)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateTimeOfBooking] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

    }
}
