using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Journal;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingsJournalHandler
    {
        private readonly BookingsJournalFilterModel _filter;
        private readonly bool _showByAccess;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public GetBookingsJournalHandler(BookingsJournalFilterModel filter, bool showByAccess = true)
        {
            _filter = filter;
            _showByAccess = showByAccess;
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public BookingsJournalModel Execute()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var model = new BookingsJournalModel
            {
                Name = "Journal"
            };

            Core.Services.Booking.Affiliate affiliate;
            List<AffiliateTimeOfBooking> affiliateTimesOfBooking;
            List<AffiliateAdditionalTime> affiliateAdditionalTimes;
            List<Tuple<TimeSpan, TimeSpan>> affiliateListTimes;
            LoadAffiliateData(out affiliate, out affiliateListTimes, out affiliateAdditionalTimes, out affiliateTimesOfBooking);

            if (_showByAccess && !AffiliateService.CheckAccess(affiliate))
                return null;

            model.DefaultBookingDuration = new TimeSpan(0, affiliate.BookingIntervalMinutes, 0);

            var bookings = BookingService.GetListByDate(_filter.AffiliateFilterId, _filter.Date);

            var reservationResources = GetReservationResources(affiliate);


            var columnDate = _filter.Date.Date;
            var affiliateIsWork = (affiliateAdditionalTimes.Count > 0 && affiliateAdditionalTimes[0].IsWork) ||
                                  (affiliateAdditionalTimes.Count == 0 && affiliateTimesOfBooking.Count > 0);

            for (int index = 0; index < reservationResources.Count; index++)
            {
                var reservationResource = reservationResources[index];
                var reservationResourceBookings =
                    bookings.Where(x => x.ReservationResourceId == reservationResource.Id).ToList();
                var reservationResourceTimesOfBooking =
                    ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliate.Id, reservationResource.Id, _filter.Date.DayOfWeek);
                var reservationResourceAdditionalTimes =
                    ReservationResourceAdditionalTimeService.GetByDate(affiliate.Id, reservationResource.Id, _filter.Date);

                var reservationResourceBookingIntervalMinutes =
                    ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id,
                        reservationResource.Id);

                var column = new BookingsJournalColumnModel(reservationResource)
                {
                    Date = columnDate,
                    TotalEventsCount = reservationResourceBookings.Count,
                    BookingDuration = new TimeSpan(0, reservationResourceBookingIntervalMinutes ?? affiliate.BookingIntervalMinutes, 0),
                    ListTimes =
                        reservationResourceBookingIntervalMinutes == null ||
                        reservationResourceBookingIntervalMinutes.Value == affiliate.BookingIntervalMinutes
                            ? affiliateListTimes
                            : BookingService.GetListTimes(reservationResourceBookingIntervalMinutes.Value),
                    TimesOfBooking = reservationResourceTimesOfBooking,
                    AdditionalTimes = reservationResourceAdditionalTimes
                };

                var reservationResourceIsWork = (reservationResourceAdditionalTimes.Count > 0 && reservationResourceAdditionalTimes[0].IsWork) ||
                                     (reservationResourceAdditionalTimes.Count == 0 && reservationResourceTimesOfBooking.Count > 0);

                if (reservationResourceBookings.Count > 0 || (affiliateIsWork && reservationResourceIsWork))
                {
                    column.Events = GetCardsByColumn(reservationResource, affiliate, column.ListTimes, reservationResourceBookings,
                        affiliateAdditionalTimes, affiliateTimesOfBooking, reservationResourceTimesOfBooking,
                        reservationResourceAdditionalTimes);
                }
                else
                    column.IsNotWork = true;

                //column.Events.ForEach(x => x.ColumnId = column.Id);

                model.Columns.Add(column);
            }

            if (bookings.Any(x => !x.ReservationResourceId.HasValue))
            {
                var noReservationResourceBookings =
                    bookings.Where(x => !x.ReservationResourceId.HasValue).ToList();

                var column = new BookingsJournalColumnModel()
                {
                    Name = "Ресурс не указан",
                    Date = columnDate,
                    TotalEventsCount = noReservationResourceBookings.Count,
                    BookingDuration = new TimeSpan(0, affiliate.BookingIntervalMinutes, 0),
                    ListTimes = affiliateListTimes
                };

                column.Events = GetCardsByColumn(null, affiliate, column.ListTimes, noReservationResourceBookings,
                    affiliateAdditionalTimes, affiliateTimesOfBooking, null, null);

                //column.Events.ForEach(x => x.ColumnId = column.Id);

                model.Columns.Add(column);
            }

            SetMinMaxTime(model, bookings, affiliateAdditionalTimes, affiliateTimesOfBooking);

            model.Columns = model.Columns.OrderBy(x => x.IsNotWork ? 1 : 0).ToList();

            stopWatch.Stop();
            model.ElapsedMilliseconds = stopWatch.ElapsedMilliseconds;

            return model;
        }

        public List<BookingJournalModel> GetEvents()
        {
            //if (_filter.ColumnId.IsNullOrEmpty())
            //    return new List<BookingJournalModel>();

            Core.Services.Booking.Affiliate affiliate;
            List<AffiliateTimeOfBooking> affiliateTimesOfBooking;
            List<AffiliateAdditionalTime> affiliateAdditionalTimes;
            List<Tuple<TimeSpan, TimeSpan>> affiliateListTimes;
            LoadAffiliateData(out affiliate, out affiliateListTimes, out affiliateAdditionalTimes, out affiliateTimesOfBooking);

            if (_showByAccess && !AffiliateService.CheckAccess(affiliate))
                return null;

            var reservationResource = ReservationResourceService.Get(_filter.ColumnId.TryParseInt());

            if (_showByAccess && reservationResource != null && !ReservationResourceService.CheckAccess(reservationResource, affiliate, _currentManager))
                return null;

            var bookings = BookingService.GetListByDateAndReservationResource(_filter.AffiliateFilterId, _filter.Date,
                reservationResource != null ? (int?) reservationResource.Id : null);

            var reservationResourceTimesOfBooking = reservationResource != null
                ? ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliate.Id, reservationResource.Id, _filter.Date.DayOfWeek)
                : null;

            var reservationResourceAdditionalTimes = reservationResource != null
                ? ReservationResourceAdditionalTimeService.GetByDate(affiliate.Id, reservationResource.Id, _filter.Date)
                : null;

            var reservationResourceBookingIntervalMinutes = reservationResource != null
                ? ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id,
                    reservationResource.Id)
                : null;

            var listTimes = reservationResource == null ||
                            reservationResourceBookingIntervalMinutes == null ||
                            reservationResourceBookingIntervalMinutes.Value == affiliate.BookingIntervalMinutes
                ? affiliateListTimes
                : BookingService.GetListTimes(reservationResourceBookingIntervalMinutes.Value);

            var events = GetCardsByColumn(reservationResource, affiliate, listTimes, bookings, affiliateAdditionalTimes,
                affiliateTimesOfBooking, reservationResourceTimesOfBooking, reservationResourceAdditionalTimes);

            //events.ForEach(x => x.ColumnId = _filter.ColumnId);

            return events;
        }

        private List<BookingJournalModel> GetCardsByColumn(ReservationResource reservationResource, Core.Services.Booking.Affiliate affiliate, List<Tuple<TimeSpan, TimeSpan>> listTimes,
            List<Core.Services.Booking.Booking> reservationResourceBookings, List<AffiliateAdditionalTime> affiliateAdditionalTimes, List<AffiliateTimeOfBooking> affiliateTimesOfBooking, 
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBooking, List<ReservationResourceAdditionalTime> reservationResourceAdditionalTimes)
        {
            var events = new List<BookingJournalModel>();

            var eventsBooking = reservationResourceBookings.Select(BookingJournalModel.CreateFromBooking).ToList();
            eventsBooking.ForEach(x =>
                x.IsWork = x.Start >= DateTime.Today
                    ? reservationResourceAdditionalTimes != null && reservationResourceTimesOfBooking != null
                        ? ReservationResourceService.CheckDateRangeIsWork(x.Start, x.End,
                            new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>() { { _filter.Date.Date, reservationResourceAdditionalTimes } },
                            new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>() { { _filter.Date.DayOfWeek, reservationResourceTimesOfBooking } },
                            new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { { _filter.Date.Date, affiliateAdditionalTimes } },
                            new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { { _filter.Date.DayOfWeek, affiliateTimesOfBooking } })
                        : AffiliateService.CheckDateRangeIsWork(x.Start, x.End,
                            new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { { _filter.Date.Date, affiliateAdditionalTimes } },
                            new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { { _filter.Date.DayOfWeek, affiliateTimesOfBooking } })
                    : (bool?) null);

            eventsBooking.Where(x => x.IsWork == false).ForEach(x => x.ClassName += " booking-nowork");

            events.AddRange(eventsBooking);

            events.AddRange(
                GetNoWorkTimes(reservationResource, affiliate, listTimes, affiliateAdditionalTimes, affiliateTimesOfBooking,
                    reservationResourceTimesOfBooking, reservationResourceAdditionalTimes));

            return events;
        }

        private List<BookingJournalModel> GetNoWorkTimes(ReservationResource reservationResource, Core.Services.Booking.Affiliate affiliate, List<Tuple<TimeSpan, TimeSpan>> listTimes, 
            List<AffiliateAdditionalTime> affiliateAdditionalTimes,
            List<AffiliateTimeOfBooking> affiliateTimesOfBooking, 
            List<ReservationResourceTimeOfBooking> reservationResourceTimesOfBooking,
            List<ReservationResourceAdditionalTime> reservationResourceAdditionalTimes)
        {
            var events = new List<BookingJournalModel>();

            Tuple<TimeSpan, TimeSpan> prevTime = null;
            BookingJournalModel @event = null;

            foreach (var listTime in listTimes)
            {
                var isWork = reservationResourceAdditionalTimes != null && reservationResourceTimesOfBooking != null
                    ? ReservationResourceService.CheckDateRangeIsWork(_filter.Date.Date + listTime.Item1,//ExistDateRangeInTimeOfBooking
                        _filter.Date.Date + listTime.Item2,
                        new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>() { { _filter.Date.Date, reservationResourceAdditionalTimes } },
                        new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>() { { _filter.Date.DayOfWeek, reservationResourceTimesOfBooking } },
                        new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { { _filter.Date.Date, affiliateAdditionalTimes } },
                        new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { { _filter.Date.DayOfWeek, affiliateTimesOfBooking } })
                    : AffiliateService.CheckDateRangeIsWork(_filter.Date.Date + listTime.Item1,//ExistDateRangeInTimeOfBooking
                        _filter.Date.Date + listTime.Item2,
                        new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { { _filter.Date.Date, affiliateAdditionalTimes } },
                        new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { { _filter.Date.DayOfWeek, affiliateTimesOfBooking } });

                if (isWork)
                {
                    if (@event != null)
                    {
                        @event.End = _filter.Date.Date + prevTime.Item2;
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
                            Start = _filter.Date.Date + listTime.Item1,
                            End = _filter.Date.Date + listTime.Item2,
                            Title = "No Work",
                            EnRendering = EnRendering.Background,
                            ClassName = "no-work",
                            IsWork = false
                        };
                    }
                    else if (prevTime.Item2 != listTime.Item1)
                    {
                        @event.End = _filter.Date.Date + prevTime.Item2;
                        events.Add(@event);

                        @event = new BookingJournalModel()
                        {
                            AffiliateId = affiliate.Id,
                            ReservationResourceId = reservationResource != null ? (int?)reservationResource.Id : null,
                            Start = _filter.Date.Date + listTime.Item1,
                            End = _filter.Date.Date + listTime.Item2,
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
                @event.End = _filter.Date.Date + prevTime.Item2;
                events.Add(@event);
            }

            return events;
        }

        private void LoadAffiliateData(out Core.Services.Booking.Affiliate affiliate, out List<Tuple<TimeSpan, TimeSpan>> affiliateListTimes,
            out List<AffiliateAdditionalTime> affiliateAdditionalTimes, out List<AffiliateTimeOfBooking> affiliateTimesOfBooking)
        {
            affiliate = AffiliateService.Get(_filter.AffiliateFilterId);
            affiliateListTimes = BookingService.GetListTimes(affiliate.BookingIntervalMinutes);
            affiliateTimesOfBooking = AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_filter.AffiliateFilterId,
                _filter.Date.DayOfWeek);
            affiliateAdditionalTimes = AffiliateAdditionalTimeService.GetByAffiliateAndDate(_filter.AffiliateFilterId,
                _filter.Date);
        }

        private List<ReservationResource> GetReservationResources(Core.Services.Booking.Affiliate affiliate)
        {
            var paging = new SqlPaging()
            {
                ItemsPerPage = Int32.MaxValue,
                CurrentPageIndex = 1
            };

            paging.Select(
                "ReservationResource.Id",
                "ReservationResource.ManagerId",
                "ReservationResource.Name",
                "ReservationResource.Description",
                "ReservationResource.Image",
                "ReservationResource.SortOrder",
                "ReservationResource.Enabled"
                );

            paging.From("[Booking].[ReservationResource]");
            paging.Inner_Join("[Booking].[AffiliateReservationResource] ON AffiliateReservationResource.ReservationResourceId = ReservationResource.Id");

            paging.Where("AffiliateReservationResource.AffiliateId = {0}", _filter.AffiliateFilterId);
            paging.Where("ReservationResource.Enabled = {0}", true);
            
            if (_showByAccess && !affiliate.AccessForAll && !_currentCustomer.IsAdmin)
            {
                if (_currentManager != null && !affiliate.ManagerIds.Contains(_currentManager.ManagerId))
                    paging.Where("ReservationResource.ManagerId = {0}", _currentManager.ManagerId);
            }

            if (_filter.Search.IsNotEmpty())
            {
                paging.Where("(ReservationResource.Name LIKE '%' + {0} + '%' OR " +
                             "EXISTS(SELECT [ReservationResourceTagsMap].[ReservationResourceId] FROM [Booking].[ReservationResourceTag] INNER JOIN [Booking].[ReservationResourceTagsMap] ON [ReservationResourceTagsMap].[ReservationResourceTagId] = [ReservationResourceTag].[Id] AND [ReservationResourceTagsMap].[ReservationResourceId] = ReservationResource.Id WHERE [ReservationResourceTag].[Name] LIKE '%' + {0} + '%'))", 
                             _filter.Search);
            }

            paging.OrderBy("ReservationResource.SortOrder", "ReservationResource.Name");


            return paging.PageItemsList<ReservationResource>();
        }

        private void SetMinMaxTime(BookingsJournalModel model, List<Core.Services.Booking.Booking> bookings,
            List<AffiliateAdditionalTime> affiliateAdditionalTimes, List<AffiliateTimeOfBooking> affiliateTimesOfBooking)
        {
            if (model.Columns.Count > 0)
            {
                if (_filter.CompactView)
                {
                    DateTime? minTime;
                    DateTime? maxTime;

                    foreach (var column in model.Columns)
                    {
                        if (!column.IsNotWork)
                        {
                            GetMinMaxTimeCompactView(column, bookings, affiliateAdditionalTimes, affiliateTimesOfBooking, out minTime, out maxTime);

                            DateTime columnMinTime;
                            DateTime columnMaxTime;
                            CalcColumnMinMaxTime(column, minTime, maxTime, out columnMinTime, out columnMaxTime);

                            column.MinDateTime = columnMinTime;
                            column.MaxDateTime = columnMaxTime;
                        }
                    }
                }
                else
                {
                    DateTime? minTime;
                    DateTime? maxTime;

                    var columnsReservationResourceIds = model.Columns.Where(x => x.Id.IsNotEmpty()).Select(x => x.Id).ToList();
                    var bookingsByColums =
                        bookings.Where(b => b.ReservationResourceId == null || columnsReservationResourceIds.Contains(b.ReservationResourceId.ToString()))
                            .ToList();
                    GetMinMaxTimeNoCompactView(model, bookingsByColums, affiliateAdditionalTimes, affiliateTimesOfBooking, out minTime, out maxTime);

                    foreach (var column in model.Columns)
                    {
                        DateTime columnMinTime;
                        DateTime columnMaxTime;
                        CalcColumnMinMaxTime(column, minTime, maxTime, out columnMinTime, out columnMaxTime);
                        
                        column.MinDateTime = columnMinTime;
                        column.MaxDateTime = columnMaxTime;
                    }
                }
            }
        }

        private void CalcColumnMinMaxTime(BookingsJournalColumnModel column, DateTime? minTime,
            DateTime? maxTime, out DateTime columnMinTime, out DateTime columnMaxTime)
        {
            if (minTime.HasValue)
            {
                var calcTime = minTime.Value - _filter.Date.Date;
                columnMinTime = _filter.Date.Date + column.ListTimes.First(t =>
                    TimeCrossMin(t.Item1, t.Item2, calcTime)).Item1;
            }
            else
                columnMinTime = _filter.Date.Date + column.ListTimes[0].Item1;

            if (maxTime.HasValue)
            {
                var calcTime = maxTime.Value - _filter.Date.Date;
                columnMaxTime = _filter.Date.Date + column.ListTimes.Last(t =>
                    TimeCrossMax(t.Item1, t.Item2, calcTime)).Item2;
            }
            else
                columnMaxTime = _filter.Date.Date + column.ListTimes[column.ListTimes.Count - 1].Item2;
        }

        private void GetMinMaxTimeCompactView(BookingsJournalColumnModel column, List<Core.Services.Booking.Booking> bookings, List<AffiliateAdditionalTime> affiliateAdditionalTimes,
            List<AffiliateTimeOfBooking> affiliateTimesOfBooking, out DateTime? minTime, out DateTime? maxTime)
        {
            if (column.TimesOfBooking != null && column.AdditionalTimes != null)
            {
                DateTime? minTimeByBooking;
                DateTime? maxTimeByBooking;
                GetMinMaxTimeByBooking(bookings.Where(b => b.ReservationResourceId == column.Id.TryParseInt(true)).ToList(),
                    out minTimeByBooking, out maxTimeByBooking);

                DateTime? minTimeByReservationResourceTime;
                DateTime? maxTimeByReservationResourceTime;
                GetMinMaxTimeByReservationResourceTime(column, out minTimeByReservationResourceTime,
                    out maxTimeByReservationResourceTime);


                minTime = null;
                maxTime = null;

                if (minTimeByBooking.HasValue || minTimeByReservationResourceTime.HasValue)
                {
                    if (minTimeByReservationResourceTime.HasValue)
                    {
                        minTime = !minTimeByBooking.HasValue || minTimeByReservationResourceTime < minTimeByBooking
                            ? minTimeByReservationResourceTime
                            : minTimeByBooking;
                    }
                    else
                        minTime = minTimeByBooking;
                }

                if (maxTimeByBooking.HasValue || maxTimeByReservationResourceTime.HasValue)
                {
                    if (maxTimeByReservationResourceTime.HasValue)
                    {
                        maxTime = !maxTimeByBooking.HasValue || maxTimeByReservationResourceTime > maxTimeByBooking
                            ? maxTimeByReservationResourceTime
                            : maxTimeByBooking;
                    }
                    else
                        maxTime = maxTimeByBooking.Value;
                }
            }
            else
                GetMinMaxTimeByAffiliateWorkTimes(affiliateAdditionalTimes, affiliateTimesOfBooking, out minTime, out maxTime);
        }

        private void GetMinMaxTimeByReservationResourceTime(BookingsJournalColumnModel column,
            out DateTime? minTime, out DateTime? maxTime)
        {
            if (column.AdditionalTimes.Count > 0)
            {
                if (!column.AdditionalTimes[0].IsWork)
                    minTime = (DateTime?)null;
                else
                    minTime = column.AdditionalTimes.Min(x => x.StartTime);
            }
            else
                minTime = column.TimesOfBooking.Count > 0
                    ? _filter.Date.Date + column.TimesOfBooking.Min(x => x.StartTime)
                    : (DateTime?)null;

            if (column.AdditionalTimes.Count > 0)
            {
                if (!column.AdditionalTimes[0].IsWork)
                    maxTime = (DateTime?)null;
                else
                    maxTime = column.AdditionalTimes.Max(x => x.EndTime);
            }
            else
                maxTime = column.TimesOfBooking.Count > 0
                    ? _filter.Date.Date + column.TimesOfBooking.Max(x => x.EndTime)
                    : (DateTime?)null;
        }

        private void GetMinMaxTimeNoCompactView(BookingsJournalModel model, List<Core.Services.Booking.Booking> bookings,
            List<AffiliateAdditionalTime> affiliateAdditionalTimes, List<AffiliateTimeOfBooking> affiliateTimesOfBooking,
            out DateTime? minTime, out DateTime? maxTime)
        {
            DateTime? minTimeByReservationResourceTime;
            DateTime? maxTimeByReservationResourceTime;
            GetMinMaxTimeByReservationResourceTime(model, out minTimeByReservationResourceTime, out maxTimeByReservationResourceTime);

            DateTime? minTimeByAffiliateWorkTimes = null;
            DateTime? maxTimeByAffiliateWorkTimes = null;

            if (minTimeByReservationResourceTime.HasValue || maxTimeByReservationResourceTime.HasValue)
                GetMinMaxTimeByAffiliateWorkTimes(affiliateAdditionalTimes, affiliateTimesOfBooking,
                    out minTimeByAffiliateWorkTimes, out maxTimeByAffiliateWorkTimes);

            DateTime? minTimeByBooking;
            DateTime? maxTimeByBooking;
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

        private void GetMinMaxTimeByReservationResourceTime(BookingsJournalModel model, out DateTime? minTime, out DateTime? maxTime)
        {
            minTime = model.Columns.Where(c => c.TimesOfBooking != null && c.AdditionalTimes != null).Min(c =>
            {
                if (c.AdditionalTimes.Count > 0)
                {
                    if (!c.AdditionalTimes[0].IsWork)
                        return (DateTime?)null;
                    else
                        return c.AdditionalTimes.Min(x => x.StartTime);
                }
                else
                    return c.TimesOfBooking.Count > 0 ? _filter.Date.Date + c.TimesOfBooking.Min(x => x.StartTime) : (DateTime?)null;
            });

            maxTime = model.Columns.Where(c => c.TimesOfBooking != null && c.AdditionalTimes != null).Max(c =>
            {
                if (c.AdditionalTimes.Count > 0)
                {
                    if (!c.AdditionalTimes[0].IsWork)
                        return (DateTime?) null;
                    else
                        return c.AdditionalTimes.Max(x => x.EndTime);
                }
                else
                    return c.TimesOfBooking.Count > 0 ? _filter.Date.Date + c.TimesOfBooking.Max(x => x.EndTime) : (DateTime?)null;
            });
        }

        private void GetMinMaxTimeByAffiliateWorkTimes(List<AffiliateAdditionalTime> affiliateAdditionalTimes, List<AffiliateTimeOfBooking> affiliateTimesOfBooking, out DateTime? minTime, out DateTime? maxTime)
        {
            var affiliateWorkDay = affiliateAdditionalTimes.Count > 0
                ? affiliateAdditionalTimes[0].IsWork
                : affiliateTimesOfBooking.Count > 0;

            if (affiliateWorkDay)
            {
                minTime =
                    affiliateAdditionalTimes.Count > 0
                        ? affiliateAdditionalTimes[0].IsWork
                            ? (DateTime?) affiliateAdditionalTimes.Min(x => x.StartTime)
                            : null
                        : _filter.Date.Date + affiliateTimesOfBooking.Min(x => x.StartTime);

                maxTime =
                    affiliateAdditionalTimes.Count > 0
                        ? affiliateAdditionalTimes[0].IsWork
                            ? (DateTime?) affiliateAdditionalTimes.Max(x => x.EndTime)
                            : null
                        : _filter.Date.Date + affiliateTimesOfBooking.Max(x => x.EndTime);
            }
            else
            {
                minTime = null;
                maxTime = null;
            }
        }

        private void GetMinMaxTimeByBooking(List<Core.Services.Booking.Booking> bookings, out DateTime? minTime, out DateTime? maxTime)
        {
            minTime = bookings.Count > 0 ? bookings.Aggregate((x1, x2) => x1.BeginDate < x2.BeginDate ? x1 : x2).BeginDate : (DateTime?)null;
            maxTime = bookings.Count > 0 ? bookings.Aggregate((x1, x2) => x1.EndDate > x2.EndDate ? x1 : x2).EndDate : (DateTime?)null;

            if (minTime.HasValue && minTime.Value.Date != _filter.Date.Date)
                minTime = null;
            if (maxTime.HasValue && maxTime.Value.Date != _filter.Date.Date)
                maxTime = null;
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, DateTime start, DateTime end)
        {
            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            if (endTime < startTime)
                endTime += new TimeSpan(1, 0, 0, 0);

            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            return startTime < to && from < endTime;
            //return (start <= from && from < end) || (start < to && to <= end);
        }

        private bool TimeCrossMin(TimeSpan from, TimeSpan to, TimeSpan min)
        {
            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            return from <= min && min < to;
        }

        private bool TimeCrossMax(TimeSpan from, TimeSpan to, TimeSpan max)
        {
            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            return from < max && max <= to;
        }

    }
}
