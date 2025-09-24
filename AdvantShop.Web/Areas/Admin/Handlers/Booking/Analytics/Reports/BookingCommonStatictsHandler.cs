using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Booking.Analytics.Reports;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingCommonStatisticHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly int? _affiliateId;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public BookingCommonStatisticHandler(DateTime dateFrom, DateTime dateTo, bool? isPaid, BookingStatus? status, int? affiliateId)
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
        public CommonStatisticData Execute()
        {
            var model = new CommonStatisticData();
            var queryFree = _currentCustomer.IsAdmin || _currentManager == null ||
                            (_affiliate != null && AffiliateService.CheckAccess(_affiliate, _currentManager, false));

            string query;

            if (_status != BookingStatus.Cancel)
            {
                if (queryFree)
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum, avg([Booking].[Sum]*CurrencyValue) as AverageSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "Where Status != @CancelStatus and BeginDate >= @DateFrom and BeginDate < @DateTo" +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                        (_status.HasValue ? " and [Booking].Status = @Status " : "");
                }
                else
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum, avg([Booking].[Sum]*CurrencyValue) as AverageSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                        "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                        "Where Status != @CancelStatus and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                        (_status.HasValue ? " and [Booking].Status = @Status " : "");
                }

                SQLDataAccess.ExecuteForeach(
                    query,
                    CommandType.Text,
                    reader =>
                    {
                        model.CountAllBookings = SQLDataHelper.GetInt(reader, "BookingsCount");
                        model.SumAllBookings = SQLDataHelper.GetFloat(reader, "BookingsSum");
                        model.AverageSumBookings = SQLDataHelper.GetFloat(reader, "AverageSum");
                    },
                    new SqlParameter("@DateFrom", _dateFrom),
                    new SqlParameter("@DateTo", _dateTo),
                    new SqlParameter("@CancelStatus", (int) BookingStatus.Cancel),
                    new SqlParameter("@Status", _status.HasValue ? (int) _status.Value : (object) DBNull.Value),
                    new SqlParameter("@AffiliateId", _affiliateId ?? 0),
                    new SqlParameter("@ManagerId", _currentManager != null ? _currentManager.ManagerId : 0));
            }

            if (_status != BookingStatus.Cancel && (!_isPaid.HasValue || _isPaid == true))
            {
                if (queryFree)
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "Where [PaymentDate] is not null and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                        (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ");
                }
                else
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                        "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                        "Where [PaymentDate] is not null and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                        (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ");
                }

                SQLDataAccess.ExecuteForeach(
                    query,
                    CommandType.Text,
                    reader =>
                    {
                        model.SumPaidBookings = SQLDataHelper.GetFloat(reader, "BookingsSum");
                    },
                    new SqlParameter("@DateFrom", _dateFrom),
                    new SqlParameter("@DateTo", _dateTo),
                    new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                    new SqlParameter("@Status", _status.HasValue ? (int)_status.Value : (object)DBNull.Value),
                    new SqlParameter("@AffiliateId", _affiliateId ?? 0),
                    new SqlParameter("@ManagerId", _currentManager != null ? _currentManager.ManagerId : 0));
            }

            if ((!_status.HasValue || _status == BookingStatus.Cancel) && 
                (!_isPaid.HasValue || _isPaid == true))
            { 
                if (queryFree)
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "Where [Booking].Status = @CancelStatus and [PaymentDate] is not null and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "");
                }
                else
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                        "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                        "Where [Booking].Status = @CancelStatus and [PaymentDate] is not null and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "");
                }

                SQLDataAccess.ExecuteForeach(
                    query,
                    CommandType.Text,
                    reader =>
                    {
                        model.SumCancelledAndPaidBookings = SQLDataHelper.GetFloat(reader, "BookingsSum");
                    },
                    new SqlParameter("@DateFrom", _dateFrom),
                    new SqlParameter("@DateTo", _dateTo),
                    new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                    new SqlParameter("@Status", _status.HasValue ? (int)_status.Value : (object)DBNull.Value),
                    new SqlParameter("@AffiliateId", _affiliateId ?? 0),
                    new SqlParameter("@ManagerId", _currentManager != null ? _currentManager.ManagerId : 0));
            }

            if (!_status.HasValue || _status == BookingStatus.Cancel)
            {
                if (queryFree)
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "Where Status = @CancelStatus and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "");
                }
                else
                {
                    query =
                        "Select count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                        "From [Booking].[Booking] " +
                        "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                        "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                        "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                        "Where Status = @CancelStatus and BeginDate >= @DateFrom and BeginDate < @DateTo " +
                        " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                        (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                        (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "");
                }

                SQLDataAccess.ExecuteForeach(
                    query,
                    CommandType.Text,
                    reader =>
                    {
                        model.CountCancelledBookings = SQLDataHelper.GetInt(reader, "BookingsCount");
                        model.SumCancelledBookings = SQLDataHelper.GetFloat(reader, "BookingsSum");
                    },
                    new SqlParameter("@DateFrom", _dateFrom),
                    new SqlParameter("@DateTo", _dateTo),
                    new SqlParameter("@CancelStatus", (int) BookingStatus.Cancel),
                    new SqlParameter("@Status", _status.HasValue ? (int) _status.Value : (object) DBNull.Value),
                    new SqlParameter("@AffiliateId", _affiliateId ?? 0),
                    new SqlParameter("@ManagerId", _currentManager != null ? _currentManager.ManagerId : 0));
            }

            return model;
        }
    }
}
