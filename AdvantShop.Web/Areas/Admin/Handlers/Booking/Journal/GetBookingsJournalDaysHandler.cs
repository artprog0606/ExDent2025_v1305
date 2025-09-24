using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingsJournalDaysHandler
    {
        private readonly BookingsJournalDaysFilterModel _filter;
        private readonly bool _showByAccess;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public GetBookingsJournalDaysHandler(BookingsJournalDaysFilterModel filter, bool showByAccess = true)
        {
            _filter = filter;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public BookingsJournalDaysModel Execute()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var model = new BookingsJournalDaysModel
            {
                Name = "Journal",
                Weeks = new List<Week>()
            };

            if (_filter.DateFrom.HasValue && _filter.DateTo.HasValue && _filter.DateFrom.Value <= _filter.DateTo.Value)
            {
                var defaultCultureInfo = CultureInfo.CurrentCulture;
                _filter.DateFrom = _filter.DateFrom.Value.Date;
                _filter.DateTo = _filter.DateTo.Value.Date;

                var dateFrom = _filter.DateFrom.Value.GetFirstDayOfWeek();
                var dateTo = _filter.DateTo.Value.GetLastDayOfWeek();
                var today = DateTime.Now.Date;

                //var statisticData =
                //    BookingStatisticsService.GetBookingsCountReservationResource(
                //        "dd",
                //        _filter.DateFrom.Value,
                //        _filter.DateTo.Value.Date.AddDays(1).AddMilliseconds(-1),
                //        _filter.AffiliateFilterId,
                //        _showByAccess && _manager != null ? _manager.ManagerId : (int?) null)
                //    .GroupBy(x => x.Item1)
                //    .ToDictionary(x => x.Key, x => x.ToList());
                var bookingsDictionary =
                    new SortedDictionary<DateTime, List<Core.Services.Booking.Booking>>(
                        BookingService
                            .GetListByDateFromTo(_filter.AffiliateFilterId, _filter.DateFrom.Value,
                                _filter.DateTo.Value.Date.AddDays(1).AddMilliseconds(-1),
                                _showByAccess && !_currentCustomer.IsAdmin && _currentManager != null ? _currentManager.ManagerId : (int?) null)
                            .GroupBy(x => x.BeginDate.Date)
                            .ToDictionary(x => x.Key, x => x.ToList()));

                var reservationResources = _filter.AffiliateFilterId.HasValue
                    ? ReservationResourceService.GetByAffiliate(_filter.AffiliateFilterId.Value)
                    : ReservationResourceService.GetList();

                var listAffiliates =
                    _filter.AffiliateFilterId.HasValue
                        ? new List<Core.Services.Booking.Affiliate> { AffiliateService.Get(_filter.AffiliateFilterId.Value) }
                        : AffiliateService.GetList();

                var dictionaryReservationResourceAccess = new SortedDictionary<AffiliateAndReservationResourceIds, bool>();
                if (_showByAccess)
                {
                    listAffiliates = listAffiliates.Where(x => AffiliateService.CheckAccess(x, _currentManager, true)).ToList();

                    var reservationResourcesAccess = new List<ReservationResource>();
                    foreach (var reservationResource in reservationResources)
                    {
                        foreach (var affiliate in listAffiliates)
                        {
                            var key = new AffiliateAndReservationResourceIds
                            {
                                AffiliateId = affiliate.Id,
                                ReservationResourceId = reservationResource.Id
                            };

                            if (!dictionaryReservationResourceAccess.ContainsKey(key))
                                dictionaryReservationResourceAccess.Add(key, ReservationResourceService.CheckAccess(reservationResource, affiliate, _currentManager));

                            if (dictionaryReservationResourceAccess[key])
                            {
                                reservationResourcesAccess.Add(reservationResource);
                                break;
                            }
                        }
                    }
                    reservationResources = reservationResourcesAccess;
                }


                var dictionaryAffiliateTimesOfBooking = new SortedDictionary<int, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>>();
                // Dictionary<affiliateId, Dictionary<date, List<AffiliateAdditionalTime>>>
                var dictionaryAffiliateAdditionalTimes = new SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>>();
                var dictionaryReservationResourceTimesOfBooking = new SortedDictionary<AffiliateAndReservationResourceIds, SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>>();
                var dictionaryReservationResourceAdditionalTimes = new SortedDictionary<AffiliateAndReservationResourceIds, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>>();

                Week currentWeek = null;
                Day day = null;
                for (var date = dateFrom; date <= dateTo; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == defaultCultureInfo.DateTimeFormat.FirstDayOfWeek)
                    {
                        if (currentWeek != null)
                            model.Weeks.Add(currentWeek);

                        currentWeek = new Week() {Days = new List<Day>()};
                    }

                    day = new Day();

                    if (_filter.DateFrom.Value <= date && date <= _filter.DateTo.Value)
                    {
                        day.Date = date;
                        day.ReservationResources = new List<ReservationResourceStatistic>();

                        foreach (var reservationResource in reservationResources)
                        {
                            ReservationResourceStatistic reservationResourceStatistic = new ReservationResourceStatistic
                            {
                                Id = reservationResource.Id,
                                Name = reservationResource.Name,
                            };

                            foreach (var affiliate in listAffiliates)
                            {
                                var key = new AffiliateAndReservationResourceIds
                                {
                                    AffiliateId = affiliate.Id,
                                    ReservationResourceId = reservationResource.Id
                                };

                                if (_showByAccess)
                                {
                                    if (!dictionaryReservationResourceAccess.ContainsKey(key))
                                        dictionaryReservationResourceAccess.Add(key, ReservationResourceService.CheckAccess(reservationResource, affiliate, _currentManager));

                                    if (!dictionaryReservationResourceAccess[key])
                                        continue;
                                }

                                var reservationResourceBookings = bookingsDictionary.ContainsKey(date)
                                    ? bookingsDictionary[date].Where(x => x.AffiliateId == key.AffiliateId && x.ReservationResourceId == reservationResource.Id).ToList()
                                    : null;

                                LazyLoad(key, dictionaryAffiliateTimesOfBooking, dictionaryAffiliateAdditionalTimes, dictionaryReservationResourceTimesOfBooking, dictionaryReservationResourceAdditionalTimes);

                                var affiliateTimesOfBookingDayOfWeek =
                                    dictionaryAffiliateTimesOfBooking[key.AffiliateId].ContainsKey(date.DayOfWeek)
                                        ? dictionaryAffiliateTimesOfBooking[key.AffiliateId][date.DayOfWeek]
                                        : null;
                                var affiliateAdditionalTime =
                                    dictionaryAffiliateAdditionalTimes[key.AffiliateId].ContainsKey(date)
                                        ? dictionaryAffiliateAdditionalTimes[key.AffiliateId][date]
                                        : null;
                                var reservationResourceTimesOfBookingDayOfWeek =
                                    dictionaryReservationResourceTimesOfBooking[key].ContainsKey(date.DayOfWeek)
                                        ? dictionaryReservationResourceTimesOfBooking[key][date.DayOfWeek]
                                        : null;
                                var reservationResourceAdditionalTime = dictionaryReservationResourceAdditionalTimes[key].ContainsKey(date)
                                    ? dictionaryReservationResourceAdditionalTimes[key][date]
                                    : null;

                                if (reservationResourceBookings != null)
                                {
                                    reservationResourceStatistic.BookingsCount += reservationResourceBookings.Count;

                                    if (!reservationResourceStatistic.CheckErros && today <= date)
                                    {
                                        reservationResourceStatistic.CheckErros =
                                            reservationResourceBookings.Any(x =>
                                                !ReservationResourceService.CheckDateRangeIsWork(x.BeginDate, x.EndDate,
                                                    dictionaryReservationResourceAdditionalTimes[key],
                                                    dictionaryReservationResourceTimesOfBooking[key],
                                                    dictionaryAffiliateAdditionalTimes[key.AffiliateId],
                                                    dictionaryAffiliateTimesOfBooking[key.AffiliateId]));
                                    }
                                }

                                // филиал и ресурс работает в этот день
                                var isWork = false;
                                if ((affiliateAdditionalTime == null || affiliateAdditionalTime[0].IsWork))
                                {
                                    isWork = reservationResourceAdditionalTime != null
                                        ? reservationResourceAdditionalTime[0].IsWork
                                        : reservationResourceTimesOfBookingDayOfWeek != null;
                                }

                                if (isWork)
                                {
                                    reservationResourceStatistic.CountSlots = (reservationResourceStatistic.CountSlots ?? 0) +
                                        (reservationResourceAdditionalTime != null
                                            ? reservationResourceAdditionalTime.Count
                                            : reservationResourceTimesOfBookingDayOfWeek.Count);
                                }
                            }

                            if (reservationResourceStatistic.BookingsCount > 0 || reservationResourceStatistic.CountSlots.HasValue)
                                day.ReservationResources.Add(reservationResourceStatistic);
                        }

                        if (bookingsDictionary.ContainsKey(date) && bookingsDictionary[date].Any(x => !x.ReservationResourceId.HasValue))
                        {
                            ReservationResourceStatistic notReservationResourceStatistic = new ReservationResourceStatistic
                            {
                                Name = "[без ресурса]",
                            };

                            foreach (var affiliate in listAffiliates)
                            {
                                var notReservationResourceBookings =
                                    bookingsDictionary[date].Where(x => x.AffiliateId == affiliate.Id && !x.ReservationResourceId.HasValue).ToList();


                                if (notReservationResourceBookings != null)
                                {
                                    notReservationResourceStatistic.BookingsCount += notReservationResourceBookings.Count;

                                    if (!notReservationResourceStatistic.CheckErros && today <= date)
                                    {
                                        var affiliateTimesOfBookingDayOfWeek =
                                            dictionaryAffiliateTimesOfBooking[affiliate.Id];
                                        var affiliateAdditionalTime =
                                            dictionaryAffiliateAdditionalTimes[affiliate.Id];

                                        notReservationResourceStatistic.CheckErros =
                                            notReservationResourceBookings.Any(x =>
                                                !AffiliateService.CheckDateRangeIsWork(x.BeginDate, x.EndDate,
                                                    affiliateAdditionalTime,
                                                    affiliateTimesOfBookingDayOfWeek));
                                    }
                                }
                            }

                            day.ReservationResources.Add(notReservationResourceStatistic);
                        }
                    }

                    currentWeek.Days.Add(day);
                }
                currentWeek.Days.Add(day);
            }

            ClearEmptyColums(model);

            stopWatch.Stop();
            model.ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;

            return model;
        }

        private void ClearEmptyColums(BookingsJournalDaysModel model)
        {
            if (model.Weeks.Count > 0)
            {
                var colsCounts = new SortedDictionary<int, int>();
                for (int index = 0; index < model.Weeks[0].Days.Count; index++)
                {
                    colsCounts.Add(index,
                        model.Weeks.Sum(
                            w =>
                                w.Days != null && w.Days.Count - 1 >= index
                                    ? w.Days[index].ReservationResources != null
                                        ? w.Days[index].ReservationResources.Count
                                        : 0
                                    : 0));
                }

                foreach (var colsCount in colsCounts.Where(x => x.Value <= 0).Reverse())
                    foreach (var week in model.Weeks)
                        if (week.Days != null && week.Days.Count - 1 >= colsCount.Key)
                            week.Days.RemoveAt(colsCount.Key);
            }
        }

        private void LazyLoad(AffiliateAndReservationResourceIds key,
            SortedDictionary<int, SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>> dictionaryAffiliateTimesOfBooking,
            SortedDictionary<int, SortedDictionary<DateTime, List<AffiliateAdditionalTime>>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<AffiliateAndReservationResourceIds, SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>> dictionaryReservationResourceTimesOfBooking,
            SortedDictionary<AffiliateAndReservationResourceIds, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>> dictionaryReservationResourceAdditionalTimes)
        {

            if (!dictionaryAffiliateTimesOfBooking.ContainsKey(key.AffiliateId))
                dictionaryAffiliateTimesOfBooking.Add(
                    key.AffiliateId,
                    new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                        AffiliateTimeOfBookingService.GetByAffiliate(key.AffiliateId)
                            .GroupBy(x => x.DayOfWeek)
                            .ToDictionary(x => x.Key, x => x.ToList()))
                    );

            if (!dictionaryAffiliateAdditionalTimes.ContainsKey(key.AffiliateId))
                dictionaryAffiliateAdditionalTimes.Add(key.AffiliateId,
                    new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                        AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(key.AffiliateId, _filter.DateFrom.Value.Date,
                            _filter.DateTo.Value.Date.AddDays(1))
                            .GroupBy(x => x.StartTime.Date)
                            .ToDictionary(x => x.Key, x => x.ToList()))
                    );

            if (!dictionaryReservationResourceTimesOfBooking.ContainsKey(key))
                dictionaryReservationResourceTimesOfBooking.Add(
                    key,
                    new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                        ReservationResourceTimeOfBookingService.GetBy(key.AffiliateId, key.ReservationResourceId)
                            .GroupBy(x => x.DayOfWeek)
                            .ToDictionary(x => x.Key, x => x.ToList()))
                    );

            if (!dictionaryReservationResourceAdditionalTimes.ContainsKey(key))
                dictionaryReservationResourceAdditionalTimes.Add(
                    key,
                    new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                        ReservationResourceAdditionalTimeService.GetByDateFromTo(key.AffiliateId, key.ReservationResourceId,
                            _filter.DateFrom.Value.Date, _filter.DateTo.Value.Date.AddDays(1))
                            .GroupBy(x => x.StartTime.Date)
                            .ToDictionary(x => x.Key, x => x.ToList()))
                    );
        }

        public class AffiliateAndReservationResourceIds : IComparable
        {
            public int AffiliateId { get; set; }
            public int ReservationResourceId { get; set; }

            public override int GetHashCode()
            {
                return unchecked (AffiliateId ^ ReservationResourceId);
            }

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                AffiliateAndReservationResourceIds temp = obj as AffiliateAndReservationResourceIds;
                if (temp != null)
                    return this.AffiliateId.CompareTo(temp.AffiliateId) + this.ReservationResourceId.CompareTo(temp.ReservationResourceId);
                else
                    throw new ArgumentException("Object is not a AffiliateAndReservationResourceIds");
            }
        }
    }
}
