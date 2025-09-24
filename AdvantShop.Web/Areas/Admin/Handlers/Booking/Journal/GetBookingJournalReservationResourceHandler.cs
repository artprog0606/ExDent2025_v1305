using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingJournalReservationResourceHandler
    {
        private readonly BookingsReservationResourceJournalFilterModel _filter;
        private readonly bool _showByAccess;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;
        private static readonly List<AffiliateTimeOfBooking> _emptyAffiliateTimesOfBooking = new List<AffiliateTimeOfBooking>();
        private static readonly List<AffiliateAdditionalTime> _emptyAffiliateAdditionalTimes = new List<AffiliateAdditionalTime>();
        private static readonly List<ReservationResourceTimeOfBooking> _emptyReservationResourceTimesOfBooking = new List<ReservationResourceTimeOfBooking>();
        private static readonly List<ReservationResourceAdditionalTime> _emptyReservationResourceAdditionalTimes = new List<ReservationResourceAdditionalTime>();

        public List<string> Errors { get; set; }

        public GetBookingJournalReservationResourceHandler(BookingsReservationResourceJournalFilterModel filter, bool showByAccess = true)
        {
            _filter = filter;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
            Errors = new List<string>();
        }

        public BookingsReservationResourceJournalModel Execute()
        {
            var model = new BookingsReservationResourceJournalModel();

            _filter.StartDate = _filter.StartDate.Value.Date;// не менять, важно значение

            //fullcalendar шлет дату окончания до начала следующего за необходимым диапозоном
            _filter.EndDate = _filter.EndDate.Value.AddDays(-1).Date.AddDays(1).AddMilliseconds(-1); // ковертируем до конца дня окончания диапозона

            var affiliate = AffiliateService.Get(_filter.AffiliateId.Value);
            var reservationResource = ReservationResourceService.Get(_filter.ReservationResourceId.Value);

            if (_showByAccess && !ReservationResourceService.CheckAccess(reservationResource, affiliate, _currentManager))
            {
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                return null;
            }

            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking;
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes;
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking;
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes;
            LoadTimeDictionaries(reservationResource, out dictionaryAffiliateTimesOfBooking, out dictionaryAffiliateAdditionalTimes, out dictionaryReservationResourceTimesOfBooking, out dictionaryReservationResourceAdditionalTimes);

            var bookings = LoadBookings();

            var listTimes =
                BookingService.GetListTimes(
                    ReservationResourceService.GetBookingIntervalMinutesForAffiliate(_filter.AffiliateId.Value,
                        _filter.ReservationResourceId.Value) ?? affiliate.BookingIntervalMinutes);

            model.MinTime = listTimes[0].Item1;
            model.MaxTime = listTimes[0].Item2;


            model.Events = new List<BookingJournalModel>();

            var eventsBooking = bookings.Select(BookingJournalModel.CreateFromBooking).ToList();

            eventsBooking.ForEach(x =>
                x.IsWork = x.Start >= DateTime.Today
                    ? ReservationResourceService.CheckDateRangeIsWork(x.Start, x.End,
                        dictionaryReservationResourceAdditionalTimes,
                        dictionaryReservationResourceTimesOfBooking,
                        dictionaryAffiliateAdditionalTimes,
                        dictionaryAffiliateTimesOfBooking)
                    : (bool?) null);

            eventsBooking.Where(x => x.IsWork == false).ForEach(x => x.ClassName += " booking-nowork");


            model.Events.AddRange(eventsBooking);

            var date = _filter.StartDate.Value;
            while (date < _filter.EndDate.Value)
            {
                model.Events.AddRange(
                    GetNoWorkTimes(date, reservationResource, affiliate, listTimes, dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking,
                        dictionaryReservationResourceAdditionalTimes, dictionaryReservationResourceTimesOfBooking));

                date = date.AddDays(1);
            }


            SetMinMaxTime(model, bookings, dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking, dictionaryReservationResourceAdditionalTimes, dictionaryReservationResourceTimesOfBooking);

            return model;
        }

        private List<BookingJournalModel> GetNoWorkTimes(DateTime date, ReservationResource reservationResource, Core.Services.Booking.Affiliate affiliate, 
            List<Tuple<TimeSpan, TimeSpan>> listTimes,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking)
        {
            var events = new List<BookingJournalModel>();

            Tuple<TimeSpan, TimeSpan> prevTime = null;
            BookingJournalModel @event = null;

            foreach (var listTime in listTimes)
            {
                var isWork = dictionaryReservationResourceAdditionalTimes != null && dictionaryReservationResourceTimesOfBooking != null
                    ? ReservationResourceService.CheckDateRangeIsWork(date + listTime.Item1,//ExistDateRangeInTimeOfBooking
                        date + listTime.Item2, dictionaryReservationResourceAdditionalTimes, dictionaryReservationResourceTimesOfBooking,
                        dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking)
                    : AffiliateService.CheckDateRangeIsWork(date + listTime.Item1,//ExistDateRangeInTimeOfBooking
                        date + listTime.Item2, dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking);

                if (isWork)
                {
                    if (@event != null)
                    {
                        @event.End = date + prevTime.Item2;
                        events.Add(@event);
                        @event = null;
                    }

                    prevTime = null;
                }
                else
                {
                    if (prevTime == null)
                    {
                        @event = new BookingJournalModel()
                        {
                            AffiliateId = affiliate.Id,
                            ReservationResourceId = reservationResource != null ? (int?)reservationResource.Id : null,
                            Start = date + listTime.Item1,
                            End = date + listTime.Item2,
                            Title = "No Work",
                            EnRendering = EnRendering.Background,
                            ClassName = "no-work",
                            IsWork = false
                        };
                    }
                    else if (prevTime.Item2 != listTime.Item1)
                    {
                        @event.End = date + prevTime.Item2;
                        events.Add(@event);

                        @event = new BookingJournalModel()
                        {
                            AffiliateId = affiliate.Id,
                            ReservationResourceId = reservationResource != null ? (int?)reservationResource.Id : null,
                            Start = date + listTime.Item1,
                            End = date + listTime.Item2,
                            Title = "No Work",
                            EnRendering = EnRendering.Background,
                            ClassName = "no-work",
                            IsWork = false
                        };
                    }

                    prevTime = listTime;
                }
            }

            if (@event != null)
            {
                @event.End = date + prevTime.Item2;
                events.Add(@event);
            }

            return events;
        }

        private void SetMinMaxTime(BookingsReservationResourceJournalModel model, List<Core.Services.Booking.Booking> bookings,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking)
        {
            TimeSpan? minTime;
            TimeSpan? maxTime;
            GetMinMaxTime(bookings, dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking, dictionaryReservationResourceAdditionalTimes, dictionaryReservationResourceTimesOfBooking, out minTime, out maxTime);

            if (minTime.HasValue)
                model.MinTime = minTime.Value;

            if (maxTime.HasValue)
                model.MaxTime = maxTime.Value;
        }

        private void GetMinMaxTime(List<Core.Services.Booking.Booking> bookings, SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking, SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking, out TimeSpan? minTime, out TimeSpan? maxTime)
        {
            TimeSpan? minTimeByReservationResourceTime;
            TimeSpan? maxTimeByReservationResourceTime;
            GetMinMaxTimeByReservationResourceTime(dictionaryReservationResourceAdditionalTimes, dictionaryReservationResourceTimesOfBooking,
                dictionaryAffiliateAdditionalTimes,
                out minTimeByReservationResourceTime, out maxTimeByReservationResourceTime);

            TimeSpan? minTimeByAffiliateWorkTimes = null;
            TimeSpan? maxTimeByAffiliateWorkTimes = null;

            if (minTimeByReservationResourceTime.HasValue || maxTimeByReservationResourceTime.HasValue)
                GetMinMaxTimeByAffiliateWorkTimes(dictionaryAffiliateAdditionalTimes, dictionaryAffiliateTimesOfBooking,
                    out minTimeByAffiliateWorkTimes, out maxTimeByAffiliateWorkTimes);

            TimeSpan? minTimeByBooking;
            TimeSpan? maxTimeByBooking;
            GetMinMaxTimeByBooking(bookings, out minTimeByBooking, out maxTimeByBooking);


            minTime = null;
            maxTime = null;

            if (minTimeByAffiliateWorkTimes.HasValue || minTimeByBooking.HasValue || minTimeByReservationResourceTime.HasValue)
            {
                if (minTimeByAffiliateWorkTimes.HasValue || minTimeByReservationResourceTime.HasValue)
                {
                    minTime =
                        !minTimeByReservationResourceTime.HasValue || minTimeByAffiliateWorkTimes > minTimeByReservationResourceTime
                            ? minTimeByAffiliateWorkTimes.Value
                            : minTimeByReservationResourceTime.Value;

                    if (minTimeByBooking.HasValue)
                        minTime = minTime < minTimeByBooking.Value
                            ? minTime
                            : minTimeByBooking.Value;
                }
                else
                    minTime = minTimeByBooking.Value;
            }

            if (maxTimeByAffiliateWorkTimes.HasValue || maxTimeByBooking.HasValue || maxTimeByReservationResourceTime.HasValue)
            {
                if (maxTimeByAffiliateWorkTimes.HasValue || maxTimeByReservationResourceTime.HasValue)
                {
                    maxTime =
                        !maxTimeByReservationResourceTime.HasValue || maxTimeByAffiliateWorkTimes < maxTimeByReservationResourceTime
                            ? maxTimeByAffiliateWorkTimes.Value
                            : maxTimeByReservationResourceTime.Value;

                    if (maxTimeByBooking.HasValue)
                        maxTime = maxTime > maxTimeByBooking.Value
                            ? maxTime
                            : maxTimeByBooking.Value;
                }
                else
                    maxTime = maxTimeByBooking.Value;
            }
        }

        private void GetMinMaxTimeByReservationResourceTime(SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes, 
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            out TimeSpan? minTime, out TimeSpan? maxTime)
        {
            minTime = null;
            maxTime = null;
            var nullTime = new TimeSpan(0, 0, 0);

            var date = _filter.StartDate.Value;
            while (date < _filter.EndDate.Value)
            {
                var affiliateAdditionalTimes = dictionaryAffiliateAdditionalTimes.ContainsKey(date) ? dictionaryAffiliateAdditionalTimes[date] : _emptyAffiliateAdditionalTimes;
                var reservationResourceAdditionalTimes = dictionaryReservationResourceAdditionalTimes.ContainsKey(date) ? dictionaryReservationResourceAdditionalTimes[date] : _emptyReservationResourceAdditionalTimes;
                var reservationResourceTimesOfBooking =
                    dictionaryReservationResourceTimesOfBooking.ContainsKey(date.DayOfWeek) ? dictionaryReservationResourceTimesOfBooking[date.DayOfWeek] : _emptyReservationResourceTimesOfBooking;

                var reservationResourceWorkDay = reservationResourceAdditionalTimes.Count > 0
                    ? reservationResourceAdditionalTimes[0].IsWork
                : reservationResourceTimesOfBooking.Count > 0;

                if (affiliateAdditionalTimes.Count > 0 && !affiliateAdditionalTimes[0].IsWork)
                    reservationResourceWorkDay = false;

                if (reservationResourceWorkDay)
                {
                    var tempMinTime =
                        reservationResourceAdditionalTimes.Count > 0
                            ? reservationResourceAdditionalTimes[0].IsWork
                                ? (TimeSpan?)reservationResourceAdditionalTimes.Min(x => x.StartTime).TimeOfDay
                                : minTime
                            : reservationResourceTimesOfBooking.Min(x => x.StartTime);

                    var tempMaxTime =
                        reservationResourceAdditionalTimes.Count > 0
                            ? reservationResourceAdditionalTimes[0].IsWork
                                ? (TimeSpan?)reservationResourceAdditionalTimes.Max(x => x.EndTime).TimeOfDay
                                : maxTime
                            : reservationResourceTimesOfBooking.Max(x => x.EndTime);

                    if (tempMaxTime == nullTime)
                        tempMaxTime = new TimeSpan(1, 0, 0, 0);

                    minTime = !minTime.HasValue || minTime > tempMinTime ? tempMinTime : minTime;
                    maxTime = !maxTime.HasValue || maxTime < tempMaxTime ? tempMaxTime : maxTime;
                }

                date = date.AddDays(1);
            }
        }

        private void GetMinMaxTimeByAffiliateWorkTimes(
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking, out TimeSpan? minTime,
            out TimeSpan? maxTime)
        {
            minTime = null;
            maxTime = null;
            var nullTime = new TimeSpan(0, 0, 0);

            var date = _filter.StartDate.Value;
            while (date < _filter.EndDate.Value)
            {
                var affiliateAdditionalTimes = dictionaryAffiliateAdditionalTimes.ContainsKey(date) ? dictionaryAffiliateAdditionalTimes[date] : _emptyAffiliateAdditionalTimes;
                var affiliateTimesOfBooking =
                    dictionaryAffiliateTimesOfBooking.ContainsKey(date.DayOfWeek) ? dictionaryAffiliateTimesOfBooking[date.DayOfWeek] : _emptyAffiliateTimesOfBooking;

                var affiliateWorkDay = affiliateAdditionalTimes.Count > 0
                    ? affiliateAdditionalTimes[0].IsWork
                : affiliateTimesOfBooking.Count > 0;

                if (affiliateWorkDay)
                {
                    var tempMinTime =
                        affiliateAdditionalTimes.Count > 0
                            ? affiliateAdditionalTimes[0].IsWork
                                ? (TimeSpan?)affiliateAdditionalTimes.Min(x => x.StartTime).TimeOfDay
                                : minTime
                            : affiliateTimesOfBooking.Min(x => x.StartTime);

                    var tempMaxTime =
                        affiliateAdditionalTimes.Count > 0
                            ? affiliateAdditionalTimes[0].IsWork
                                ? (TimeSpan?)affiliateAdditionalTimes.Max(x => x.EndTime).TimeOfDay
                                : maxTime
                            : affiliateTimesOfBooking.Max(x => x.EndTime);

                    if (tempMaxTime == nullTime)
                        tempMaxTime = new TimeSpan(1, 0, 0, 0);

                    minTime = !minTime.HasValue || minTime > tempMinTime ? tempMinTime : minTime;
                    maxTime = !maxTime.HasValue || maxTime < tempMaxTime ? tempMaxTime : maxTime;
                }

                date = date.AddDays(1);
            }
        }

        private void GetMinMaxTimeByBooking(List<Core.Services.Booking.Booking> bookings, out TimeSpan? minTime, out TimeSpan? maxTime)
        {
            minTime = null;
            maxTime = null;

            if (bookings.Count > 0)
            {
                var nullTime = new TimeSpan(0, 0, 0);
                var oneDayTime = new TimeSpan(1, 0, 0, 0);

                minTime =
                    bookings.Aggregate((x1, x2) => x1.BeginDate.TimeOfDay < x2.BeginDate.TimeOfDay ? x1 : x2).BeginDate.TimeOfDay;

                maxTime = bookings.Aggregate((x1, x2) =>
                {
                    return (x1.EndDate.TimeOfDay == nullTime ? oneDayTime : x1.EndDate.TimeOfDay) >
                           (x2.EndDate.TimeOfDay == nullTime ? oneDayTime : x2.EndDate.TimeOfDay)
                        ? x1
                        : x2;
                }).EndDate.TimeOfDay;
            }
        }

        private List<Core.Services.Booking.Booking> LoadBookings()
        {
            return BookingService.GetListByDateFromToAndReservationResource(_filter.AffiliateId.Value,
                _filter.StartDate.Value, _filter.EndDate.Value, _filter.ReservationResourceId.Value);
            /*List<Core.Services.Booking.Booking> bookings = new List<Core.Services.Booking.Booking>();

            var date = _filter.StartDate.Value;
            while (date < _filter.EndDate.Value)
            {
                bookings.AddRange(BookingService.GetListByDateAndReservationResource(_filter.AffiliateId.Value, date,
                    _filter.ReservationResourceId.Value));
                date = date.AddDays(1);
            }
            return bookings;*/
        }

        private void LoadTimeDictionaries(ReservationResource reservationResource, out SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> dictionaryAffiliateTimesOfBooking,
            out SortedDictionary<DateTime, List<AffiliateAdditionalTime>> dictionaryAffiliateAdditionalTimes, out SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> dictionaryReservationResourceTimesOfBooking,
            out SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> dictionaryReservationResourceAdditionalTimes)
        {
            dictionaryAffiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_filter.AffiliateId.Value)
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList()));

            dictionaryAffiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(
                _filter.AffiliateId.Value,
                _filter.StartDate.Value.Date, _filter.EndDate.Value.Date.AddDays(1))
                .GroupBy(x => x.StartTime.Date)
                .ToDictionary(x => x.Key, x => x.ToList()));


            dictionaryReservationResourceTimesOfBooking = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                ReservationResourceTimeOfBookingService.GetBy(_filter.AffiliateId.Value, reservationResource.Id)
                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList()));

            dictionaryReservationResourceAdditionalTimes = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(_filter.AffiliateId.Value,
                    reservationResource.Id, _filter.StartDate.Value.Date, _filter.EndDate.Value.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList()));
        }
    }
}
