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
    public class AffiliateAdditionalTimeService
    {
        public static AffiliateAdditionalTime Get(int id)
        {
            return
                SQLDataAccess.Query<AffiliateAdditionalTime>(
                    "SELECT * FROM [Booking].[AffiliateAdditionalTime] WHERE Id = @Id", new {Id = id})
                    .FirstOrDefault();
        }

        public static List<AffiliateAdditionalTime> GetByAffiliate(int affiliateId)
        {
            return
                SQLDataAccess.Query<AffiliateAdditionalTime>(
                    "SELECT * FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId ORDER BY [StartTime]",
                    new {AffiliateId = affiliateId})
                    .ToList();
        }

        public static List<AffiliateAdditionalTime> GetByAffiliateAndDate(int affiliateId, DateTime date)
        {
            return GetByAffiliateAndDateFromTo(affiliateId, date.Date, date.Date.AddDays(1));
        }

        public static List<AffiliateAdditionalTime> GetByAffiliateAndDateFrom(int affiliateId, DateTime dateFrom)
        {
            return
                SQLDataAccess.Query<AffiliateAdditionalTime>(
                    "SELECT * FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId AND [StartTime] >= @StartDate ORDER BY [StartTime]",
                    new
                    {
                        AffiliateId = affiliateId,
                        StartDate = dateFrom
                    })
                    .ToList();
        }

        public static List<AffiliateAdditionalTime> GetByAffiliateAndDateFromTo(int affiliateId, DateTime dateFrom, DateTime dateTo)
        {
            return
                SQLDataAccess.Query<AffiliateAdditionalTime>(
                    "SELECT * FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndTime] AND [StartTime] < @EndDate ORDER BY [StartTime]",
                    new
                    {
                        AffiliateId = affiliateId,
                        StartDate = dateFrom,
                        EndDate = dateTo
                    })
                    .ToList();
        }

        public static bool Exists(int affiliateId, DateTime date)
        {
            return Exists(affiliateId, date.Date, date.Date.AddDays(1));
        }

        public static bool Exists(int affiliateId, DateTime dateFrom, DateTime dateTo)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                @"IF EXISTS(SELECT 1 FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndTime] AND [StartTime] < @EndDate)
                begin 
                    SELECT 1
                end
                else
                begin 
                    SELECT 0
                end",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@StartDate", dateFrom),
                new SqlParameter("@EndDate", dateTo)
                );
        }

        public static List<int> GetListIds()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [Booking].[AffiliateAdditionalTime]",
                CommandType.Text, "Id");
        }

        public static List<int> GetListIdsByAffiliate(int affiliateId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<int>(
                    "SELECT [Id] FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId",
                    CommandType.Text, "Id", new SqlParameter("@AffiliateId", affiliateId));
        }

        public static int Add(AffiliateAdditionalTime affiliateAdditionalTime)
        {
            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [Booking].[AffiliateAdditionalTime] " +
                                                    " ([AffiliateId], [StartTime], [EndTime], [IsWork]) " +
                                                    " VALUES (@AffiliateId, @StartTime, @EndTime, @IsWork); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateAdditionalTime.AffiliateId),
                new SqlParameter("@StartTime", affiliateAdditionalTime.StartTime),
                new SqlParameter("@EndTime", affiliateAdditionalTime.EndTime),
                new SqlParameter("@IsWork", affiliateAdditionalTime.IsWork)
                );
        }

        public static void Update(AffiliateAdditionalTime affiliateAdditionalTime)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [Booking].[AffiliateAdditionalTime] SET [AffiliateId] = @AffiliateId, [StartTime] = @StartTime, [EndTime] = @EndTime, [IsWork] = @IsWork " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", affiliateAdditionalTime.Id),
                new SqlParameter("@AffiliateId", affiliateAdditionalTime.AffiliateId),
                new SqlParameter("@StartTime", affiliateAdditionalTime.StartTime),
                new SqlParameter("@EndTime", affiliateAdditionalTime.EndTime),
                new SqlParameter("@IsWork", affiliateAdditionalTime.IsWork)
                );
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateAdditionalTime] WHERE Id = @Id",
                CommandType.Text,
                new SqlParameter("@Id", id));
        }

        public static void DeleteByAffiliate(int affiliateId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId));
        }

        public static void DeleteByAffiliateAndDate(int affiliateId, DateTime date)
        {
            DeleteByAffiliateAndDateFromTo(affiliateId, date.Date, date.Date.AddDays(1));
        }

        public static void DeleteByAffiliateAndDateFromTo(int affiliateId, DateTime dateFrom, DateTime dateTo)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Booking].[AffiliateAdditionalTime] WHERE [AffiliateId] = @AffiliateId AND @StartDate < [EndTime] AND [StartTime] < @EndDate",
                CommandType.Text,
                new SqlParameter("@AffiliateId", affiliateId),
                new SqlParameter("@StartDate", dateFrom),
                new SqlParameter("@EndDate", dateTo));
        }
    }
}
