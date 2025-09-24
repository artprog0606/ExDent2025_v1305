using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Booking
{
    public class BookingStatisticsService
    {
        public static List<Tuple<DateTime, BookingStatus, float>> GetBookingsSum(string group, DateTime minDate, DateTime maxDate,
            bool? isPaid, BookingStatus? status,
            int? affiliateId = null, int? managerId = null)
        {
            var affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            var manager = managerId.HasValue ? ManagerService.GetManager(managerId.Value) : null;

            if (manager == null || (affiliate != null && AffiliateService.CheckAccess(affiliate, manager, false)))
                return SQLDataAccess.ExecuteReadList(
                    "Select DATEADD(" + group + ", DATEDIFF(" + group +
                    ", 0, [BeginDate]), 0) as 'Date', [Status], SUM([Sum]*CurrencyValue) as 'Sum' " +
                    "FROM [Booking].[Booking] " +
                    "Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                    (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (isPaid.HasValue ? " and PaymentDate IS " + (isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (status.HasValue ? " and [Booking].Status = @Status " : "") +
                    "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [Status] " +
                    "Order By Date",
                    CommandType.Text,
                    reader =>
                        new Tuple<DateTime, BookingStatus, float>(
                            SQLDataHelper.GetDateTime(reader, "Date"),
                            (BookingStatus) SQLDataHelper.GetInt(reader, "Status"), 
                            SQLDataHelper.GetFloat(reader, "Sum")),
                    new SqlParameter("@MinDate", minDate),
                    new SqlParameter("@MaxDate", maxDate),
                    new SqlParameter("@CancelStatus", (int) BookingStatus.Cancel),
                    new SqlParameter("@Status", status.HasValue ? (int)status.Value : (object)DBNull.Value),
                    new SqlParameter("@AffiliateId", affiliateId ?? 0));

            return SQLDataAccess.ExecuteReadList(
                "Select DATEADD(" + group + ", DATEDIFF(" + group +
                ", 0, [BeginDate]), 0) as 'Date', [Status], SUM(Booking.[Sum]*CurrencyValue) as 'Sum' " +
                "FROM [Booking].[Booking] " +
                "Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                "INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                "LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]" +
                "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                (isPaid.HasValue ? " and PaymentDate IS " + (isPaid.Value ? "NOT " : "") + "NULL " : "") +
                (status.HasValue ? " and [Booking].Status = @Status " : "") +
                " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [Status] " +
                "Order By Date",
                CommandType.Text,
                reader =>
                    new Tuple<DateTime, BookingStatus, float>(
                        SQLDataHelper.GetDateTime(reader, "Date"),
                        (BookingStatus) SQLDataHelper.GetInt(reader, "Status"), 
                        SQLDataHelper.GetFloat(reader, "Sum")),
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate),
                new SqlParameter("@CancelStatus", (int) BookingStatus.Cancel),
                new SqlParameter("@Status", status.HasValue ? (int)status.Value : (object)DBNull.Value),
                new SqlParameter("@ManagerId", managerId.Value),
                new SqlParameter("@AffiliateId", affiliateId ?? 0));
        }

        public static List<Tuple<DateTime, BookingStatus, float>> GetBookingsCount(string group, DateTime minDate, DateTime maxDate,
            bool? isPaid, BookingStatus? status,
            int? affiliateId = null, int? managerId = null)
        {
            var affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            var manager = managerId.HasValue ? ManagerService.GetManager(managerId.Value) : null;

            if (manager == null || (affiliate != null && AffiliateService.CheckAccess(affiliate, manager, false)))
                return SQLDataAccess.ExecuteReadList(
                    "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0) as 'Date', [Status], Count([Id]) as Count " +
                    "FROM [Booking].[Booking] " +
                    "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                    (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (isPaid.HasValue ? " and PaymentDate IS " + (isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (status.HasValue ? " and [Booking].Status = @Status " : "") +
                    "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [Status] " +
                    "Order By Date",
                    CommandType.Text,
                    reader =>
                        new Tuple<DateTime, BookingStatus, float>(
                            SQLDataHelper.GetDateTime(reader, "Date"),
                            (BookingStatus)SQLDataHelper.GetInt(reader, "Status"), 
                            SQLDataHelper.GetFloat(reader, "Count")),
                    new SqlParameter("@MinDate", minDate),
                    new SqlParameter("@MaxDate", maxDate),
                    new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                    new SqlParameter("@Status", status.HasValue ? (int)status.Value : (object)DBNull.Value),
                    new SqlParameter("@AffiliateId", affiliateId ?? 0));

            return SQLDataAccess.ExecuteReadList(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0) as 'Date', [Status], Count(Booking.[Id]) as Count " +
                "FROM [Booking].[Booking] " +
                "INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                "LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]" +
                "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                (isPaid.HasValue ? " and PaymentDate IS " + (isPaid.Value ? "NOT " : "") + "NULL " : "") +
                (status.HasValue ? " and [Booking].Status = @Status " : "") +
                " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [Status] " +
                "Order By Date",
                CommandType.Text,
                reader =>
                    new Tuple<DateTime, BookingStatus, float>(
                        SQLDataHelper.GetDateTime(reader, "Date"),
                        (BookingStatus)SQLDataHelper.GetInt(reader, "Status"), 
                        SQLDataHelper.GetFloat(reader, "Count")),
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate),
                new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                new SqlParameter("@Status", status.HasValue ? (int)status.Value : (object)DBNull.Value),
                new SqlParameter("@ManagerId", managerId.Value),
                new SqlParameter("@AffiliateId", affiliateId ?? 0));
        }

        public static List<Tuple<DateTime, int?, int>> GetBookingsCountReservationResource(string group, DateTime minDate, DateTime maxDate,
            int? affiliateId = null, int? managerId = null, bool excledeCancelStatus = true)
        {
            var affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            var manager = managerId.HasValue ? ManagerService.GetManager(managerId.Value) : null;

            if (manager == null || (affiliate != null && AffiliateService.CheckAccess(affiliate, manager, false)))
                return SQLDataAccess.ExecuteReadList(
                    "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0) as 'Date', [ReservationResourceId], Count([Id]) as Count " +
                    "FROM [Booking].[Booking] " +
                    "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                    (excledeCancelStatus ? " and [Status] != @CancelStatus " : "") +
                    (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [ReservationResourceId] " +
                    "Order By Date",
                    CommandType.Text,
                    reader =>
                        new Tuple<DateTime, int?, int>(
                            SQLDataHelper.GetDateTime(reader, "Date"),
                            SQLDataHelper.GetNullableInt(reader, "ReservationResourceId"), 
                            SQLDataHelper.GetInt(reader, "Count")),
                    new SqlParameter("@MinDate", minDate),
                    new SqlParameter("@MaxDate", maxDate),
                    new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                    new SqlParameter("@AffiliateId", affiliateId ?? 0));

            return SQLDataAccess.ExecuteReadList(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0) as 'Date', [ReservationResourceId], Count(Booking.[Id]) as Count " +
                "FROM [Booking].[Booking] " +
                "INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                "LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id]" +
                "WHERE [BeginDate] >= @MinDate and [BeginDate] < @MaxDate " +
                (excledeCancelStatus ? " and [Status] != @CancelStatus " : "") +
                (affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [BeginDate]), 0), [ReservationResourceId] " +
                "Order By Date",
                CommandType.Text,
                reader =>
                    new Tuple<DateTime, int?, int>(
                        SQLDataHelper.GetDateTime(reader, "Date"),
                        SQLDataHelper.GetNullableInt(reader, "ReservationResourceId"),
                        SQLDataHelper.GetInt(reader, "Count")),
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate),
                new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                new SqlParameter("@ManagerId", managerId.Value),
                new SqlParameter("@AffiliateId", affiliateId ?? 0));

        }
    }
}
