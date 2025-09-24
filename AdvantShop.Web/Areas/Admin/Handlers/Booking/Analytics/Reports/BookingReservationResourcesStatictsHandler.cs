using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Analytics.Reports;

namespace AdvantShop.Web.Admin.Handlers.Booking.Analytics.Reports
{
    public class BookingReservationResourcesStatictsHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateBy;
        private readonly bool? _isPaid;
        private readonly BookingStatus? _status;
        private readonly int? _affiliateId;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public BookingReservationResourcesStatictsHandler(DateTime dateFrom, DateTime dateBy, bool? isPaid, BookingStatus? status, int? affiliateId)
        {
            _dateFrom = dateFrom;
            _dateBy = dateBy;
            _isPaid = isPaid;
            _status = status;
            _affiliateId = affiliateId;
            _affiliate = affiliateId.HasValue ? AffiliateService.Get(affiliateId.Value) : null;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public List<ReservationResourceStatisticData> Execute()
        {
            var queryFree = _currentCustomer.IsAdmin || _currentManager == null ||
                            (_affiliate != null && AffiliateService.CheckAccess(_affiliate, _currentManager, false));

            string query;
            if (queryFree)
            {
                query =
                    "Select Id, Name, " +

                    "(Select count([Booking].[Id]) from [Booking].[Booking] " +
                    "where [ReservationResourceId] = [ReservationResource].Id and BeginDate >= @DateFrom and BeginDate < @DateTo " + 
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") + 
                    ") as BookingsCount, " +

                    "(Select isnull(Sum([Booking].[Sum]*CurrencyValue),0) from [Booking].[Booking] Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id]  " +
                    "where [ReservationResourceId] = [ReservationResource].Id and BeginDate >= @DateFrom and BeginDate < @DateTo " + 
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId" : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") + 
                    ") as BookingsSum " +

                    "From [Booking].[ReservationResource] " +
                    (_affiliateId != null ? " WHERE EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateReservationResource].[AffiliateId] = @AffiliateId AND [ReservationResourceId] = [ReservationResource].Id) " : "") +
                    "Order By SortOrder, Name";
            }
            else
            {
                query =
                    "Select Id, Name, " +

                    "(Select count([Booking].[Id]) from [Booking].[Booking] " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                    "where [ReservationResourceId] = [ReservationResource].Id and BeginDate >= @DateFrom and BeginDate < @DateTo " + 
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    ") as BookingsCount, " +

                    "(Select isnull(Sum([Booking].[Sum]*CurrencyValue),0) from [Booking].[Booking] Inner Join [Booking].[BookingCurrency] On [BookingCurrency].[BookingId] = [Booking].[Id]  " +
                    "   INNER JOIN Booking.Affiliate ON Affiliate.Id = Booking.AffiliateId " +
                    "   LEFT JOIN [Booking].[ReservationResource] ON [Booking].[ReservationResourceId] = [ReservationResource].[Id] " +
                    "where [ReservationResourceId] = [ReservationResource].Id and BeginDate >= @DateFrom and BeginDate < @DateTo " + 
                    (_affiliateId != null ? " and [AffiliateId] = @AffiliateId " : "") +
                    (_isPaid.HasValue ? " and PaymentDate IS " + (_isPaid.Value ? "NOT " : "") + "NULL " : "") +
                    (_status.HasValue ? " and [Booking].Status = @Status " : " and [Booking].Status != @CancelStatus ") +
                    " AND (Affiliate.AccessForAll = 1 OR EXISTS(SELECT 1 FROM [Booking].[AffiliateManager] as am WHERE am.[AffiliateId] = Affiliate.Id AND am.[ManagerId] = @ManagerId) OR ReservationResource.ManagerId = @ManagerId) " +
                    ") as BookingsSum " +

                    "From [Booking].[ReservationResource] " +
                    (_affiliateId != null ? " WHERE EXISTS(SELECT 1 FROM [Booking].[AffiliateReservationResource] WHERE [AffiliateReservationResource].[AffiliateId] = @AffiliateId AND [ReservationResourceId] = [ReservationResource].Id) " : "") +
                    "Order By SortOrder, Name";
            }

            var model = SQLDataAccess.Query<ReservationResourceStatisticData>(
                query,
                new
                {
                    DateFrom = _dateFrom,
                    DateTo = _dateBy,
                    CancelStatus = (int) BookingStatus.Cancel,
                    Status = _status.HasValue ? (int)_status.Value : (object)DBNull.Value,
                    AffiliateId = _affiliateId ?? 0,
                    ManagerId = _currentManager != null ? _currentManager.ManagerId : 0
                }).ToList();

            if (_isPaid == null && _status == null)
                LoadFillingPlace(model);

            return model;
        }

        private void LoadFillingPlace(List<ReservationResourceStatisticData> model)
        {
            if (model.Count > 0)
            {
                var dayOfWeekCount = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToDictionary(x => x, x => 0);

                var date = _dateFrom;
                while (date < _dateBy)
                {
                    dayOfWeekCount[date.DayOfWeek] += 1;

                    date = date.AddDays(1);
                }


                var dictionaryAffiliateAdditionalTimes = new SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>>();
                List<Core.Services.Booking.Affiliate> listAffiliates =
                    _affiliateId.HasValue
                        ? new List<Core.Services.Booking.Affiliate> {AffiliateService.Get(_affiliateId.Value)}
                        : AffiliateService.GetList();
                var dayOfWeekCountGt0 = dayOfWeekCount.Keys.Where(x => dayOfWeekCount[x] > 0).ToList();

                foreach (var statisticData in model)
                {
                    foreach (var affiliate in listAffiliates)
                    {
                        var dictionaryReservationResourceTimesOfBooking =
                            ReservationResourceTimeOfBookingService.GetBy(affiliate.Id, statisticData.Id)
                                .GroupBy(x => x.DayOfWeek)
                                .ToDictionary(x => x.Key, x => x.ToList());

                        var dictionaryReservationResourceAdditionalTimes =
                            ReservationResourceAdditionalTimeService.GetByDateFromTo(affiliate.Id, statisticData.Id,
                                _dateFrom.Date, _dateBy.Date.AddDays(1)).GroupBy(x => x.StartTime.Date)
                                .ToDictionary(x => x.Key, x => x.ToList());

                        // если рабочие дни недели ресурса пересекаются с запрошеными днями
                        // или в запрошенных днях для ресурса есть юсключение рабочего дня (т.е. расписание ресурса может состоять из одних исключений, а постоянного не иметь)
                        if (dictionaryReservationResourceTimesOfBooking.Keys.Intersect(dayOfWeekCountGt0).Any() 
                            || dictionaryReservationResourceAdditionalTimes.Any(d => d.Value.Any(at => at.IsWork)))
                        {
                            statisticData.CountSlots = 0;

                            foreach (var dayOfWeekTimesOfBooking in dictionaryReservationResourceTimesOfBooking)
                            {
                                statisticData.CountSlots += dayOfWeekTimesOfBooking.Value.Count * dayOfWeekCount[dayOfWeekTimesOfBooking.Key];
                            }

                            if (!dictionaryAffiliateAdditionalTimes.ContainsKey(affiliate.Id))
                                dictionaryAffiliateAdditionalTimes.Add(affiliate.Id,
                                    new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                                        AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(affiliate.Id, _dateFrom.Date, _dateBy.Date.AddDays(1))
                                            .GroupBy(x => x.StartTime.Date)
                                            .ToDictionary(x => x.Key, x => x.ToList()))
                                    );

                            var datesAdditionalTimes =
                                dictionaryReservationResourceAdditionalTimes.Keys
                                    .Union(dictionaryAffiliateAdditionalTimes[affiliate.Id].Keys)
                                    .ToList();

                            foreach (var dateAdditionalTimes in datesAdditionalTimes)
                            {
                                if (dictionaryReservationResourceAdditionalTimes.ContainsKey(dateAdditionalTimes) 
                                    || dictionaryAffiliateAdditionalTimes[affiliate.Id].ContainsKey(dateAdditionalTimes))
                                {
                                    // этот день для филиала не рабочий
                                    var additionalTimeAffiliateIsNotWork =
                                        dictionaryAffiliateAdditionalTimes[affiliate.Id].ContainsKey(dateAdditionalTimes) &&
                                        !dictionaryAffiliateAdditionalTimes[affiliate.Id][dateAdditionalTimes][0].IsWork;

                                    // если этот день нерабочий для филиала
                                    // или если этот день имеет исключение для ресурса
                                    if (additionalTimeAffiliateIsNotWork
                                        || dictionaryReservationResourceAdditionalTimes.ContainsKey(dateAdditionalTimes))
                                    {
                                        if (dictionaryReservationResourceTimesOfBooking.ContainsKey(dateAdditionalTimes.DayOfWeek))
                                            statisticData.CountSlots -= dictionaryReservationResourceTimesOfBooking[dateAdditionalTimes.DayOfWeek].Count;
                                    }

                                    // если этот день рабочий для филиала и этот день имеет исключение для ресурса
                                    if (!additionalTimeAffiliateIsNotWork && dictionaryReservationResourceAdditionalTimes.ContainsKey(dateAdditionalTimes))
                                    {
                                        // если у этого дня для ресурса особое рабочее расписание
                                        if (dictionaryReservationResourceAdditionalTimes[dateAdditionalTimes][0].IsWork)
                                            statisticData.CountSlots += dictionaryReservationResourceAdditionalTimes[dateAdditionalTimes].Count;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            
        }
    }
}
