using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class GetReservationResourceFormDataHandler
    {
        private readonly int? _Id;
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly int? _bookingIntervalMinutes;
        private readonly bool _fistLoad;

        public GetReservationResourceFormDataHandler(int? id, Core.Services.Booking.Affiliate affiliate,
            int? bookingIntervalMinutes, bool fistLoad)
        {
            _Id = id;
            _affiliate = affiliate;
            _bookingIntervalMinutes = bookingIntervalMinutes;
            _fistLoad = fistLoad;
        }

        public ReservationResourceFormData Execute()
        {
            var model = new ReservationResourceFormData
            {
                Tags = TagService.GetList().Select(x => x.Name).ToList()
            };

            if (_affiliate != null)
            {

                ReservationResource reservationResource = null;

                if (_Id != null)
                    reservationResource = ReservationResourceService.Get(_Id.Value);


                var bookingIntervalMinutes = _affiliate.BookingIntervalMinutes;
                if (_fistLoad)
                {
                    if (reservationResource != null)
                    {
                        var reservationResourceBookingIntervalMinutes =
                            ReservationResourceService.GetBookingIntervalMinutesForAffiliate(_affiliate.Id,
                                reservationResource.Id);

                        if (reservationResourceBookingIntervalMinutes.HasValue)
                            bookingIntervalMinutes = reservationResourceBookingIntervalMinutes.Value;
                    }
                }
                else if (_bookingIntervalMinutes.HasValue)
                    bookingIntervalMinutes = _bookingIntervalMinutes.Value;

                IEnumerable<Manager> managers = ManagerService.GetManagers(RoleAction.Booking);
                //managers = reservationResource == null
                //    ? managers.Where(manager => AffiliateService.CheckAccess(_affiliate, manager, checkByReservationResources: false))
                //    : managers.Where(manager => ReservationResourceService.CheckAccess(reservationResource, _affiliate, manager));

                var managersItems = managers.Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
                managersItems.Insert(0, new SelectItemModel("-", null));

                if (reservationResource != null && reservationResource.ManagerId.HasValue && !managersItems.Any(x => x.value == reservationResource.ManagerId.Value.ToString()))
                {
                    var m = ManagerService.GetManager(reservationResource.ManagerId.Value);
                    if (m != null)
                        managersItems.Add(new SelectItemModel(m.FullName, m.ManagerId));
                }

                model.Managers = managersItems;

                var listTimes = BookingService.GetListTimes(bookingIntervalMinutes);

                var affiliateTimes = AffiliateTimeOfBookingService.GetByAffiliate(_affiliate.Id);

                var reservationResourceTimes = reservationResource != null
                    ? ReservationResourceTimeOfBookingService.GetBy(_affiliate.Id, reservationResource.Id)
                    : new List<ReservationResourceTimeOfBooking>();

                model.AffiliateBookingIntervalMinutes = _affiliate.BookingIntervalMinutes;
                model.Times = listTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.MondayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Monday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.TuesdayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Tuesday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.WednesdayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Wednesday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.ThursdayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Thursday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.FridayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Friday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.SaturdayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Saturday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.SundayWorkTimes =
                    GetWorkTimes(listTimes, affiliateTimes.Where(x => x.DayOfWeek == DayOfWeek.Sunday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.MondayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Monday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.TuesdayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Tuesday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.WednesdayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Wednesday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.ThursdayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Thursday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.FridayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Friday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.SaturdayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Saturday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

                model.SundayTimes =
                    GetCross(listTimes, reservationResourceTimes.Where(x => x.DayOfWeek == DayOfWeek.Sunday).ToList())
                        .Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList();

            }
            

            if (model.Managers == null)
            {
                model.Managers =
                    ManagerService.GetManagers(RoleAction.Booking)
                        .Select(x => new SelectItemModel(x.FullName, x.ManagerId.ToString()))
                        .ToList();

            }

            return model;
        }

        private List<Tuple<TimeSpan, TimeSpan>> GetWorkTimes(List<Tuple<TimeSpan, TimeSpan>> list,
            List<AffiliateTimeOfBooking> affiliateTimeOfBookings)
        {
            return list
                .Where(listTime => AffiliateService.CheckDateRangeIsWorkByDayOfWeek(listTime.Item1, listTime.Item2, affiliateTimeOfBookings))
                .ToList();
        }

        private List<Tuple<TimeSpan, TimeSpan>> GetCross(List<Tuple<TimeSpan, TimeSpan>> list,
            List<ReservationResourceTimeOfBooking> reservationResourceTimeOfBookings)
        {
            return list
                .Where(listTime =>
                    //affiliateTimeOfBookings.Any(affiliateTime => affiliateTime.StartTime == listTime.Item1 && affiliateTime.EndTime == listTime.Item2) ||
                    reservationResourceTimeOfBookings.Any(affiliateTime => TimeCross(listTime.Item1, listTime.Item2, affiliateTime.StartTime, affiliateTime.EndTime)))
                .ToList();
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, TimeSpan start, TimeSpan end)
        {
            if (to < from)
                to += new TimeSpan(1, 0, 0, 0);

            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return start < to && from < end;
            //return (start <= from && from < end) || (start < to && to <= end);
        }

    }
}
