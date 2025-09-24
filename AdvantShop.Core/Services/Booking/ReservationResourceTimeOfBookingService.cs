using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class ReservationResourceTimeOfBookingService
    {
        public static ReservationResourceTimeOfBooking Get(int id)
        {
            return
                SQLDataAccess.ExecuteReadOne<ReservationResourceTimeOfBooking>(
                    "SELECT TOP 1 * FROM [Booking].[ReservationResourceTimeOfBooking] WHERE Id = @Id", CommandType.Text, GetFromReader,
                    new SqlParameter("@Id", id));
        }

        public static List<ReservationResourceTimeOfBooking> GetBy(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadList<ReservationResourceTimeOfBooking>(
                    "SELECT * FROM [Booking].[ReservationResourceTimeOfBooking] WHERE AffiliateId = @AffiliateId AND ReservationResourceId = @ReservationResourceId ORDER BY [StartTime]",
                    CommandType.Text,
                    GetFromReader,
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static List<ReservationResourceTimeOfBooking> GetByDayOfWeek(int affiliateId, int reservationResourceId, DayOfWeek dayOfWeek)
        {
            return
                SQLDataAccess.ExecuteReadList<ReservationResourceTimeOfBooking>(
                    "SELECT * FROM [Booking].[ReservationResourceTimeOfBooking] WHERE AffiliateId = @AffiliateId AND ReservationResourceId = @ReservationResourceId AND DayOfWeek = @DayOfWeek ORDER BY [StartTime]",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId),
                    new SqlParameter("@DayOfWeek", (byte)dayOfWeek));
        }

        private static ReservationResourceTimeOfBooking GetFromReader(SqlDataReader reader)
        {
            return new ReservationResourceTimeOfBooking
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                AffiliateId = SQLDataHelper.GetInt(reader, "AffiliateId"),
                ReservationResourceId = SQLDataHelper.GetInt(reader, "ReservationResourceId"),
                DayOfWeek = (DayOfWeek)SQLDataHelper.GetInt(reader, "DayOfWeek"),
                StartTime = SQLDataHelper.GetDateTime(reader, "StartTime").TimeOfDay,
                EndTime = SQLDataHelper.GetDateTime(reader, "EndTime").TimeOfDay
            };
        }

        public static int Add(ReservationResourceTimeOfBooking reservationResourceTimeOfBooking)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[ReservationResourceTimeOfBooking] " +
                                                    " ([AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) " +
                                                    " VALUES (@AffiliateId, @ReservationResourceId, @DayOfWeek, @StartTime, @EndTime); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", reservationResourceTimeOfBooking.AffiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceTimeOfBooking.ReservationResourceId),
                new SqlParameter("@DayOfWeek", (byte)reservationResourceTimeOfBooking.DayOfWeek),
                new SqlParameter("@StartTime", new DateTime(2000, 1, 1) + reservationResourceTimeOfBooking.StartTime),
                new SqlParameter("@EndTime", new DateTime(2000, 1, 1) + reservationResourceTimeOfBooking.EndTime)
                );
        }

        public static void Update(ReservationResourceTimeOfBooking reservationResourceTimeOfBooking)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[ReservationResourceTimeOfBooking] SET [AffiliateId] = @AffiliateId, [ReservationResourceId] = @ReservationResourceId, [DayOfWeek] = @DayOfWeek, [StartTime] = @StartTime, [EndTime] = @EndTime " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", reservationResourceTimeOfBooking.Id),
                new SqlParameter("@AffiliateId", reservationResourceTimeOfBooking.AffiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceTimeOfBooking.ReservationResourceId),
                new SqlParameter("@DayOfWeek", (byte)reservationResourceTimeOfBooking.DayOfWeek),
                new SqlParameter("@StartTime", new DateTime(2000, 1, 1) + reservationResourceTimeOfBooking.StartTime),
                new SqlParameter("@EndTime", new DateTime(2000, 1, 1) + reservationResourceTimeOfBooking.EndTime)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResourceTimeOfBooking] WHERE Id = @Id", CommandType.Text, new SqlParameter("@Id", id));
        }

        public static void DeleteBy(int affiliateId, int reservationResourceId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Booking].[ReservationResourceTimeOfBooking] WHERE AffiliateId = @AffiliateId AND ReservationResourceId = @ReservationResourceId",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId));
        }
    }
}
