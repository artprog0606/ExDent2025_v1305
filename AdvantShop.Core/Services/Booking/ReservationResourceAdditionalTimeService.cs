//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Booking
{
    public class ReservationResourceAdditionalTimeService
    {
        public static ReservationResourceAdditionalTime Get(int id)
        {
            return
                SQLDataAccess.Query<ReservationResourceAdditionalTime>(
                    "SELECT * FROM [Booking].[ReservationResourceAdditionalTime] WHERE Id = @Id", new { Id = id })
                    .FirstOrDefault();
        }

        public static List<ReservationResourceAdditionalTime> GetBy(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.Query<ReservationResourceAdditionalTime>(
                    "SELECT * FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId ORDER BY [StartTime]",
                    new
                    {
                        AffiliateId = affiliateId,
                        ReservationResourceId = reservationResourceId
                    })
                    .ToList();
        }

        public static List<ReservationResourceAdditionalTime> GetByDate(int affiliateId, int reservationResourceId, DateTime date)
        {
            return GetByDateFromTo(affiliateId, reservationResourceId, date.Date,
                date.Date.AddDays(1));
        }

        public static List<ReservationResourceAdditionalTime> GetByDateFrom(int affiliateId, int reservationResourceId, DateTime dateFrom)
        {
            return
                SQLDataAccess.Query<ReservationResourceAdditionalTime>(
                    "SELECT * FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND [StartTime] >= @StartDate ORDER BY [StartTime]",
                    new
                    {
                        AffiliateId = affiliateId,
                        ReservationResourceId = reservationResourceId,
                        StartDate = dateFrom
                    })
                    .ToList();
        }

        public static List<ReservationResourceAdditionalTime> GetByDateFromTo(int affiliateId, int reservationResourceId, DateTime dateFrom, DateTime dateTo)
        {
            return
                SQLDataAccess.Query<ReservationResourceAdditionalTime>(
                    "SELECT * FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND @StartDate < [EndTime] AND [StartTime] < @EndDate ORDER BY [StartTime]",
                    new
                    {
                        AffiliateId = affiliateId,
                        ReservationResourceId = reservationResourceId,
                        StartDate = dateFrom,
                        EndDate = dateTo
                    })
                    .ToList();
        }

        public static bool Exists(int affiliateId, int reservationResourceId, DateTime date)
        {
            return Exists(affiliateId, reservationResourceId, date.Date, date.Date.AddDays(1));
        }

        public static bool Exists(int affiliateId, int reservationResourceId, DateTime dateFrom, DateTime dateTo)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS(SELECT 1 FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND @StartDate < [EndTime] AND [StartTime] < @EndDate)
                begin 
                    SELECT 1
                end
                else
                begin 
                    SELECT 0
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@StartDate", dateFrom),
                new SqlParameter("@EndDate", dateTo)
                );
        }

        public static List<int> GetListIds()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[ReservationResourceAdditionalTime]",
                CommandType.Text, "Id");
        }

        public static List<int> GetListIds(int affiliateId, int reservationResourceId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT [Id] FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId",
                    CommandType.Text,
                    "Id",
                    new SqlParameter("@AffiliateId", affiliateId),
                    new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static int Add(ReservationResourceAdditionalTime reservationResourceAdditionalTime)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[ReservationResourceAdditionalTime] " +
                                                    " ([AffiliateId], [ReservationResourceId], [StartTime], [EndTime], [IsWork]) " +
                                                    " VALUES (@AffiliateId, @ReservationResourceId, @StartTime, @EndTime, @IsWork); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", reservationResourceAdditionalTime.AffiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceAdditionalTime.ReservationResourceId),
                new SqlParameter("@StartTime", reservationResourceAdditionalTime.StartTime),
                new SqlParameter("@EndTime", reservationResourceAdditionalTime.EndTime),
                new SqlParameter("@IsWork", reservationResourceAdditionalTime.IsWork)
                );
        }

        public static void Update(ReservationResourceAdditionalTime reservationResourceAdditionalTime)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[ReservationResourceAdditionalTime] SET [AffiliateId] = @AffiliateId, [ReservationResourceId] = @ReservationResourceId, [StartTime] = @StartTime, [EndTime] = @EndTime, [IsWork] = @IsWork " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", reservationResourceAdditionalTime.Id),
                new SqlParameter("@AffiliateId", reservationResourceAdditionalTime.AffiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceAdditionalTime.ReservationResourceId),
                new SqlParameter("@StartTime", reservationResourceAdditionalTime.StartTime),
                new SqlParameter("@EndTime", reservationResourceAdditionalTime.EndTime),
                new SqlParameter("@IsWork", reservationResourceAdditionalTime.IsWork)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResourceAdditionalTime] WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void DeleteBy(int affiliateId, int reservationResourceId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId));
        }

        public static void DeleteByDate(int affiliateId, int reservationResourceId, DateTime date)
        {
            DeleteByDateFromTo(affiliateId, reservationResourceId, date.Date, date.Date.AddDays(1));
        }

        public static void DeleteByDateFromTo(int affiliateId, int reservationResourceId, DateTime dateFrom, DateTime dateTo)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[ReservationResourceAdditionalTime] WHERE AffiliateId = @AffiliateId AND [ReservationResourceId] = @ReservationResourceId AND @StartDate < [EndTime] AND [StartTime] < @EndDate",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@ReservationResourceId", reservationResourceId),
                new SqlParameter("@StartDate", dateFrom),
                new SqlParameter("@EndDate", dateTo));
        }
    }
}
