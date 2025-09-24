using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Analytics.Reports;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingServicesStatisticHandler
    {
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateTo;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly BookingStatus? _noStatus;
        private readonly int? _affiliateId;
        private readonly int? _reservationResourceId;
        private readonly bool? _groupByReservationResource;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly ReservationResource _reservationResource;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public BookingServicesStatisticHandler(DateTime? dateFrom, DateTime? dateTo, bool? isPaid, BookingStatus? status,
            BookingStatus? noStatus, int? affiliateId, int? reservationResourceId, bool? groupByReservationResource)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _isPaid = isPaid;
            _status = status;
            _noStatus = noStatus;
            _affiliateId = affiliateId;
            _reservationResourceId = reservationResourceId;
            _groupByReservationResource = groupByReservationResource;
            _affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            _reservationResource = reservationResourceId.HasValue
                ? ReservationResourceService.Get(reservationResourceId.Value)
                : null;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public List<ServiceStatisticData> Execute()
        {
            var queryFree = _currentCustomer.IsAdmin || _currentManager == null ||
                            (_reservationResource == null && _affiliate != null && AffiliateService.CheckAccess(_affiliate, _currentManager, false)) ||
                            (_reservationResource != null && _affiliate != null && ReservationResourceService.CheckAccess(_reservationResource, _affiliate, _currentManager));

            string query;
            if (queryFree)
            {
                var where = (_affiliateId != null ? " AND [Booking].[AffiliateId] = @AffiliateId " : "") +
                            (_reservationResourceId != null ? " AND [Booking].[ReservationResourceId] = @ReservationResourceId " : "") +
                            (_dateFrom.HasValue ? " AND [Booking].BeginDate >= @DateFrom " : "") +
                            (_dateTo.HasValue ? " AND [Booking].BeginDate < @DateTo " : "") +
                            (_isPaid.HasValue ? " AND [Booking].PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                            (_status.HasValue ? " AND [Booking].Status = @Status " : "") +
                            (_noStatus.HasValue ? " AND [Booking].Status != @NoStatus " : "");

                var additionalFields = new List<string>();
                var additionalGroups = new List<string>();

                if (_groupByReservationResource == true)
                {
                    additionalFields.Add("[ReservationResource].Id AS ReservationResourceId");
                    additionalFields.Add("ISNULL([ReservationResource].[Name],'-') AS ReservationResourceName");

                    additionalGroups.Add("[ReservationResource].Id");
                    additionalGroups.Add("[ReservationResource].[Name]");
                }

                query =
                    "SELECT [Service].[Id] AS [Id], [Service].[ArtNo] AS [ArtNo], [Service].[Name] AS [Name], SUM([BookingItems].[Amount]) AS [Count], SUM([BookingItems].[Price]*CurrencyValue*[BookingItems].[Amount]) AS [Sum] " +
                    (additionalFields.Count > 0 ? "," + string.Join(",", additionalFields) + " " : "") +
                    "FROM [Booking].[BookingItems] " +
                    "   INNER JOIN [Booking].[Booking] ON [Booking].[Id] = [BookingItems].[BookingId] " +
                    "   INNER JOIN [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "   INNER JOIN [Booking].[Service] ON [Service].[Id] = [BookingItems].[ServiceId] " +
                    (_groupByReservationResource == true ? "   LEFT JOIN [Booking].[ReservationResource] ON [ReservationResource].[Id] = [Booking].[ReservationResourceId] " : "") +
                    "WHERE [BookingItems].[ServiceId] IS NOT NULL " +
                    where +
                    "GROUP BY [Service].[Id], [Service].[ArtNo], [Service].[Name] " +
                    (additionalGroups.Count > 0 ? "," + string.Join(",", additionalGroups) + " " : "") +

                    "UNION ALL " +

                    "SELECT [BookingItems].[ServiceId] AS [Id], [BookingItems].[ArtNo] AS [ArtNo], [BookingItems].[Name] AS [Name], SUM([BookingItems].[Amount]) AS [Count], SUM([BookingItems].[Price]*CurrencyValue*[BookingItems].[Amount]) AS [Sum] " +
                    (additionalFields.Count > 0 ? "," + string.Join(",", additionalFields) + " " : "") +
                    "FROM [Booking].[BookingItems] " +
                    "   INNER JOIN [Booking].[Booking] ON [Booking].[Id] = [BookingItems].[BookingId] " +
                    "   INNER JOIN [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    (_groupByReservationResource == true ? "   LEFT JOIN [Booking].[ReservationResource] ON [ReservationResource].[Id] = [Booking].[ReservationResourceId] " : "") +
                    "WHERE [BookingItems].[ServiceId] IS NULL " +
                    where +
                    "GROUP BY [BookingItems].[ServiceId], [BookingItems].[ArtNo], [BookingItems].[Name]" +
                    (additionalGroups.Count > 0 ? "," + string.Join(",", additionalGroups) + " " : "") +

                    (_groupByReservationResource == true ? "ORDER BY [ReservationResourceName] " : "");
            }
            else
            {
                var where = (_affiliateId != null ? " AND [Booking].[AffiliateId] = @AffiliateId " : "") +
                            (_reservationResourceId != null ? " AND [Booking].[ReservationResourceId] = @ReservationResourceId " : "") +
                            (_dateFrom.HasValue ? " AND [Booking].BeginDate >= @DateFrom " : "") +
                            (_dateTo.HasValue ? " AND [Booking].BeginDate < @DateTo " : "") +
                            (_isPaid.HasValue ? " AND [Booking].PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                            (_status.HasValue ? " AND [Booking].Status = @Status " : "") +
                            (_noStatus.HasValue ? " AND [Booking].Status != @NoStatus " : "");

                var additionalFields = new List<string>();
                var additionalGroups = new List<string>();

                if (_groupByReservationResource == true)
                {
                    additionalFields.Add("[ReservationResource].Id AS ReservationResourceId");
                    additionalFields.Add("ISNULL([ReservationResource].[Name],'-') AS ReservationResourceName");

                    additionalGroups.Add("[ReservationResource].Id");
                    additionalGroups.Add("[ReservationResource].[Name]");
                }

                query =
                    "SELECT [Service].[Id] AS [Id], [Service].[ArtNo] AS [ArtNo], [Service].[Name] AS [Name], SUM([BookingItems].[Amount]) AS [Count], SUM([BookingItems].[Price]*CurrencyValue*[BookingItems].[Amount]) AS [Sum] " +
                    (additionalFields.Count > 0 ? "," + string.Join(",", additionalFields) + " " : "") +
                    "FROM [Booking].[BookingItems] " +
                    "   INNER JOIN [Booking].[Booking] ON [Booking].[Id] = [BookingItems].[BookingId] " +
                    "   INNER JOIN [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "   INNER JOIN [Booking].[Service] ON [Service].[Id] = [BookingItems].[ServiceId] " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [ReservationResource].[Id] = [Booking].[ReservationResourceId] " +
                    "WHERE [BookingItems].[ServiceId] IS NOT NULL " +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    " AND EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateReservationResource].[AffiliateId] = Affiliate.Id AND [ReservationResourceId] = [ReservationResource].[Id]) " +
                    where +
                    "GROUP BY [Service].[Id], [Service].[ArtNo], [Service].[Name] " +
                    (additionalGroups.Count > 0 ? "," + string.Join(",", additionalGroups) + " " : "") +

                    "UNION ALL " +

                    "SELECT [BookingItems].[ServiceId] AS [Id], [BookingItems].[ArtNo] AS [ArtNo], [BookingItems].[Name] AS [Name], SUM([BookingItems].[Amount]) AS [Count], SUM([BookingItems].[Price]*CurrencyValue*[BookingItems].[Amount]) AS [Sum] " +
                    (additionalFields.Count > 0 ? "," + string.Join(",", additionalFields) + " " : "") +
                    "FROM [Booking].[BookingItems] " +
                    "   INNER JOIN [Booking].[Booking] ON [Booking].[Id] = [BookingItems].[BookingId] " +
                    "   INNER JOIN [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [ReservationResource].[Id] = [Booking].[ReservationResourceId] " +
                    "WHERE [BookingItems].[ServiceId] IS NULL " +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    " AND EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateReservationResource].[AffiliateId] = Affiliate.Id AND [ReservationResourceId] = [ReservationResource].[Id]) " +
                    where +
                    "GROUP BY [BookingItems].[ServiceId], [BookingItems].[ArtNo], [BookingItems].[Name]" +
                    (additionalGroups.Count > 0 ? "," + string.Join(",", additionalGroups) + " " : "") +

                    (_groupByReservationResource == true ? "ORDER BY [ReservationResourceName] " : "");
            }

            var model = SQLDataAccess.Query<ServiceStatisticData>(
                query,
                new
                {
                    AffiliateId = _affiliateId ?? 0,
                    ReservationResourceId = _reservationResourceId ?? 0,
                    DateFrom = _dateFrom ?? (object)DBNull.Value,
                    DateTo = _dateTo ?? (object)DBNull.Value,
                    Status = _status.HasValue ? (int)_status.Value : (object)DBNull.Value,
                    NoStatus = _noStatus.HasValue ? (int)_noStatus.Value : (object)DBNull.Value,
                    ManagerId = _currentManager != null ? _currentManager.ManagerId : 0
                }).ToList();

            return model;
        }
    }
}
