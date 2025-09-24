using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Analytics.Reports;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingSourcesStatictsHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly int? _affiliateId;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public BookingSourcesStatictsHandler(DateTime dateFrom, DateTime dateTo, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _isPaid = isPaid;
            _status = status;
            _affiliateId = affiliateId;
            _affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public List<BookinSourceStatisticData> Execute()
        {
            var queryFree = _currentCustomer.IsAdmin || _currentManager == null ||
                            (_affiliate != null && AffiliateService.CheckAccess(_affiliate, _currentManager, false));

            string query;
            if (queryFree)
            {
                query =
                    "SELECT Id,Name,Type, " +
                    "(Select Count([Booking].Id) From [Booking].[Booking] " +
                    "Where [Booking].[OrderSourceId] = [OrderSource].[Id] and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") + 
                    ") as BookinsCount " +
                    "FROM [Order].[OrderSource] order by SortOrder";
            }
            else
            {
                query =
                    "SELECT Id,Name,Type, " +
                    "(Select Count([Booking].Id) From [Booking].[Booking] " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                    "Where [Booking].[OrderSourceId] = [OrderSource].[Id] and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    ") as BookinsCount " +
                    "FROM [Order].[OrderSource] order by SortOrder";
            }

            var model = SQLDataAccess.Query<BookinSourceStatisticData>(
                    query,
                    new
                    {
                        DateFrom = _dateFrom,
                        DateTo = _dateTo,
                        CancelStatus = (int) BookingStatus.Cancel,
                        Status = _status.HasValue ? (int)_status.Value : (object)DBNull.Value,
                        AffiliateId = _affiliateId ?? 0,
                        ManagerId = _currentManager != null ? _currentManager.ManagerId : 0
                    }).ToList();

            var totalCount = model.Sum(x => x.BookinsCount);

            model = model.OrderByDescending(x => x.BookinsCount).ToList();

            foreach (var source in model)
            {
                source.Percent = totalCount == 0 ? 0 : (int)Math.Round((decimal)source.BookinsCount * 100 / totalCount);
            }

            return model;
        }
    }
}
