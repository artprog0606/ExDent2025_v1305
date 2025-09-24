using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Crm.BusinessProcesses;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Booking.Journal;

namespace AdvantShop.Web.Admin.Handlers.Booking.Journal
{
    public class UpdateAfterDragBookingHandler
    {
        private readonly UpdateAfterDragBookingModel _model;

        public bool UserConfirmIsRequired { get; set; }
        public string ConfirmMessageIsRequired { get; set; }
        public string ConfirmMessage { get; set; }
        public string ConfirmButtomText { get; set; }
        public List<string> Errors { get; set; }
        private readonly Customer _currentCustomer;
        private readonly Manager _currentManager;

        public UpdateAfterDragBookingHandler(UpdateAfterDragBookingModel model)
        {
            _model = model;
            Errors = new List<string>();
            _currentCustomer = CustomerContext.CurrentCustomer;
            _currentManager = _currentCustomer.IsManager ? ManagerService.GetManager(_currentCustomer.Id) : null;
        }

        public bool Execute()
        {
            var booking = BookingService.Get(_model.Id);

            if (booking == null)
            {
                Errors.Add("Бронь не найдена");
                return false;
            }

            if (!BookingService.CheckAccessToEditing(booking, _currentManager))
                Errors.Add(LocalizationService.GetResource("Admin.Booking.NoAccess"));

            if (Errors.Count == 0)
            {
                if (!BookingService.Exist(booking.AffiliateId, _model.ReservationResourceId, _model.BeginDate, _model.EndDate, booking.Id))
                {
                    if (!_model.UserConfirmed && !CheckTimeIsWork(booking.AffiliateId))
                    {
                        UserConfirmIsRequired = true;
                        ConfirmMessage = "Указанное время является нерабочим. Продолжить?";
                        ConfirmButtomText = "Да, продолжить";
                        return false;
                    }

                    booking.BeginDate = _model.BeginDate;
                    booking.EndDate = _model.EndDate;
                    booking.ReservationResourceId = _model.ReservationResourceId;

                    BookingService.Update(booking);

                    BizProcessExecuter.BookingChanged(booking);

                    return true;
                }
                else
                {
                    Errors.Add("Пересечение броней по времени");
                }
            }
            return Errors.Count == 0;
        }

        private bool CheckTimeIsWork(int affiliateId)
        {
            ReservationResource reservationResource = _model.ReservationResourceId.HasValue
                ? ReservationResourceService.Get(_model.ReservationResourceId.Value)
                : null;

            var oneDay = _model.BeginDate.Date == _model.EndDate.Date;

            var affiliateAdditionalTime = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(affiliateId, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                    .GroupBy(x => x.StartTime.Date)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            var affiliateTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                (oneDay
                    ? AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliateId, _model.BeginDate.DayOfWeek)
                    : AffiliateTimeOfBookingService.GetByAffiliate(affiliateId))

                    .GroupBy(x => x.DayOfWeek)
                    .ToDictionary(x => x.Key, x => x.ToList())
                );

            if (reservationResource != null)
            {
                var reservationResourceAdditionalTime = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                    ReservationResourceAdditionalTimeService.GetByDateFromTo(affiliateId, reservationResource.Id, _model.BeginDate.Date, _model.EndDate.Date.AddDays(1))
                        .GroupBy(x => x.StartTime.Date)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );

                var reservationResourceTimesOfBookingDayOfWeek = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                    (oneDay
                        ? ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliateId,
                            reservationResource.Id, _model.BeginDate.DayOfWeek)
                        : ReservationResourceTimeOfBookingService.GetBy(affiliateId, reservationResource.Id))

                        .GroupBy(x => x.DayOfWeek)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    );

                return ReservationResourceService.CheckDateRangeIsWork(_model.BeginDate, _model.EndDate,
                    reservationResourceAdditionalTime,
                    reservationResourceTimesOfBookingDayOfWeek,
                    affiliateAdditionalTime,
                    affiliateTimesOfBookingDayOfWeek);
            }
            else
            {
                return AffiliateService.CheckDateRangeIsWork(_model.BeginDate, _model.EndDate,
                    affiliateAdditionalTime,
                    affiliateTimesOfBookingDayOfWeek);
            }
        }
    }
}
