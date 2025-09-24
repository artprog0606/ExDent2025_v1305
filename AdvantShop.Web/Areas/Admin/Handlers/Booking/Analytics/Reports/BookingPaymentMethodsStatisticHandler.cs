using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Payment;
using AdvantShop.Web.Admin.Models.Booking.Analytics.Reports;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingPaymentMethodsStatisticHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly int? _affiliateId;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public BookingPaymentMethodsStatisticHandler(DateTime dateFrom, DateTime dateTo, bool? isPaid, BookingStatus? status, int? affiliateId)
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

        public List<PaymentMethodStatisticData> Execute()
        {
            var model = PaymentService.GetAllPaymentMethods(false).Select(x => new PaymentMethodStatisticData{Id = x.PaymentMethodId, Name = x.Name}).ToList();
            var queryFree = _currentCustomer.IsAdmin || _currentManager == null ||
                            (_affiliate != null && AffiliateService.CheckAccess(_affiliate, _currentManager, false));

            string query;
            if (queryFree)
            {
                query =
                    "Select [PaymentMethod].[PaymentMethodID], count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                    "From [Booking].[Booking] " +
                    "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "   Left join [Order].[PaymentMethod] ON [Booking].[PaymentMethodID] = [Order].[PaymentMethod].[PaymentMethodID] " +
                    "Where BeginDate >= @DateFrom and BeginDate < @DateTo " + 
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") + " " +
                    "Group By [PaymentMethod].[PaymentMethodID] ";
            }
            else
            {
                query =
                    "Select [PaymentMethod].[PaymentMethodID], count([Booking].[Id]) as BookingsCount, sum([Booking].[Sum]*CurrencyValue) as BookingsSum " +
                    "From [Booking].[Booking] " +
                    "   Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id] " +
                    "   Left join [Order].[PaymentMethod] ON [Booking].[PaymentMethodID] = [Order].[PaymentMethod].[PaymentMethodID] " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                    "Where BeginDate >= @DateFrom and BeginDate < @DateTo " +
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") + " " +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    "Group By [PaymentMethod].[PaymentMethodID] ";
            }

            SQLDataAccess.ExecuteForeach(
                query,
                CommandType.Text,
                reader =>
                {
                    if (!SQLDataHelper.IsDbNull(reader, "PaymentMethodID"))
                    {
                        var statisticData = model.First(x => x.Id == SQLDataHelper.GetInt(reader, "PaymentMethodID"));
                        statisticData.BookingsCount = SQLDataHelper.GetInt(reader, "BookingsCount");
                        statisticData.BookingsSum = SQLDataHelper.GetFloat(reader, "BookingsSum");
                    }
                    else
                    {
                        model.Add(new PaymentMethodStatisticData
                        {
                            Id = 0,
                            Name = "Без метода оплаты",
                            BookingsCount = SQLDataHelper.GetInt(reader, "BookingsCount"),
                            BookingsSum = SQLDataHelper.GetFloat(reader, "BookingsSum"),
                        });
                    }
                },
                new SqlParameter("@DateFrom", _dateFrom),
                new SqlParameter("@DateTo", _dateTo),
                new SqlParameter("@CancelStatus", (int)BookingStatus.Cancel),
                new SqlParameter("@Status", _status.HasValue ? (int)_status.Value : (object)DBNull.Value),
                new SqlParameter("@AffiliateId", _affiliateId ?? 0),
                new SqlParameter("@ManagerId", _currentManager != null ? _currentManager.ManagerId : 0));


            var totalSum = model.Sum(x => x.BookingsSum);

            model = model.OrderByDescending(x => x.BookingsSum).ToList();

            foreach (var source in model)
                source.Percent = totalSum == 0 ? 0f : (float)Math.Round(source.BookingsSum * 100 / totalSum);


            return model;
        }
    }
}
