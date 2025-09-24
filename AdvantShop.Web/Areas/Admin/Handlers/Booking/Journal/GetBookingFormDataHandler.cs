using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class GetBookingFormDataHandler
    {
        private readonly GetBookingFormModel _model;
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public List<string> Errors { get; set; }

        public GetBookingFormDataHandler(GetBookingFormModel model)
        {
            _model = model;
            Errors = new List<string>();
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public BookingFormData Execute()
        {
            if (!_model.AffiliateId.HasValue)
            {
                Errors.Add("Не указан филиал");
                return null;
            }

            var affiliate = AffiliateService.Get(_model.AffiliateId.Value);
            if (affiliate == null)
            {
                Errors.Add("Указанный филиал не найден");
                return null;
            }

            if (!AffiliateService.CheckAccess(affiliate))
            {
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));
                return null;
            }

            var reservationResources = ReservationResourceService.GetByAffiliate(affiliate.Id, false).Where(x => ReservationResourceService.CheckAccess(x, affiliate, _currentManager)).ToList();
            var selectedReservationResource = _model.SelectedReservationResourceId.HasValue
                ? reservationResources.FirstOrDefault(x => x.Id == _model.SelectedReservationResourceId.Value)
                : null;

            IEnumerable<Manager> managers = ManagerService.GetManagers(RoleAction.Booking);
            managers = selectedReservationResource == null
                ? managers.Where(manager => AffiliateService.CheckAccess(affiliate, manager, checkByReservationResources: false))
                : managers.Where(manager => ReservationResourceService.CheckAccess(selectedReservationResource, affiliate, manager));

            var managersItems = managers.Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();
            managersItems.Insert(0, new SelectItemModel("-", null));

            var currentBooking = _model.Id.HasValue ? BookingService.Get(_model.Id.Value) : null;
            if (currentBooking != null && currentBooking.ManagerId.HasValue && !managersItems.Any(x => x.value == currentBooking.ManagerId.Value.ToString()))
            {
                var m = ManagerService.GetManager(currentBooking.ManagerId.Value);
                if (m != null)
                    managersItems.Add(new SelectItemModel(m.FullName, m.ManagerId));
            }

            var defaultBookingSource = OrderSourceService.GetOrderSource(OrderType.None).Id;
            var bookingSources =
                OrderSourceService.GetOrderSources()
                    .Select(x => new SelectItemModel(x.Name, x.Id))
                    .ToList();

            var listTimes =
                BookingService.GetListTimes(selectedReservationResource != null
                    ? ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id,
                        selectedReservationResource.Id) ?? affiliate.BookingIntervalMinutes
                    : affiliate.BookingIntervalMinutes);

            var workTimes = new List<Tuple<TimeSpan, TimeSpan>>();

            if (_model.SelectedDate.HasValue)
            {
                var affiliateAdditionalTimes = AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliate.Id, _model.SelectedDate.Value);
                var affiliateTimesOfBooking = AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliate.Id, _model.SelectedDate.Value.DayOfWeek);

                var nullTime = new TimeSpan(0, 0, 0);
                var oneDayTime = new TimeSpan(1, 0, 0, 0);

                var affiliateIsWork = (affiliateAdditionalTimes.Count > 0 && affiliateAdditionalTimes[0].IsWork) ||
                                      (affiliateAdditionalTimes.Count == 0 && affiliateTimesOfBooking.Count > 0);

                if (affiliateIsWork) { 
                    if (selectedReservationResource != null)
                    {
                        var reservationResourceAdditionalTime = ReservationResourceAdditionalTimeService.GetByDate(affiliate.Id, selectedReservationResource.Id, _model.SelectedDate.Value);
                        if (reservationResourceAdditionalTime.Count > 0)
                        {
                            if (reservationResourceAdditionalTime[0].IsWork)
                                workTimes.AddRange(reservationResourceAdditionalTime.Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime.TimeOfDay, x.EndTime.TimeOfDay == nullTime ? oneDayTime : x.EndTime.TimeOfDay)));
                        }
                        else
                        {
                            workTimes.AddRange(ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliate.Id, selectedReservationResource.Id, _model.SelectedDate.Value.DayOfWeek)
                                .Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime, x.EndTime)));
                        }
                    }
                    else
                    {
                        if (affiliateAdditionalTimes.Count > 0)
                            workTimes.AddRange(affiliateAdditionalTimes.Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime.TimeOfDay, x.EndTime.TimeOfDay == nullTime ? oneDayTime : x.EndTime.TimeOfDay)));
                        else
                            workTimes.AddRange(affiliateTimesOfBooking.Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime, x.EndTime)));
                    }
                }

                var bookingListBySelectedDate =
                    selectedReservationResource == null
                        ? BookingService.GetListByDate(affiliate.Id, _model.SelectedDate.Value.Date)
                        : BookingService.GetListByDateAndReservationResource(affiliate.Id, _model.SelectedDate.Value.Date, selectedReservationResource.Id);

                if (bookingListBySelectedDate.Count > 0)
                {
                    listTimes = listTimes.Where(
                        listTime =>
                            !bookingListBySelectedDate.Any(
                                booking =>
                                    TimeCross(listTime.Item1, listTime.Item2, booking.BeginDate.TimeOfDay, booking.EndDate.TimeOfDay)))
                        .ToList();

                    workTimes = workTimes.Where(
                        workTime =>
                            !bookingListBySelectedDate.Any(
                                booking =>
                                    TimeCross(workTime.Item1, workTime.Item2, booking.BeginDate.TimeOfDay, booking.EndDate.TimeOfDay)))
                        .ToList();
                }

            }

            return new BookingFormData
            {
                Times = listTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                WorkTimes = workTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                ReservationResources = reservationResources.Select(x => new SelectItemModel(x.Name, x.Id)).ToList(),
                CurrentManager = _currentManager != null ? _currentManager.ManagerId : (int?)null,
                Managers = managersItems,
                BookingSourceNone = defaultBookingSource,
                BookingSources = bookingSources
            };
        }

        private bool TimeCross(TimeSpan from, TimeSpan to, TimeSpan start, TimeSpan end)
        {
            if (end < start)
                end += new TimeSpan(1, 0, 0, 0);

            return start < to && from < end;
            //return (start <= from && from < end) || (start < to && to <= end);
        }
    }
}
