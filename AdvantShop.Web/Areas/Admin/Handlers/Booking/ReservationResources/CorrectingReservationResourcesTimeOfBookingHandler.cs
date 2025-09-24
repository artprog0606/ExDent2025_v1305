using System.Linq;
using AdvantShop.Core.Services.Booking;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{

    /// <summary>
    /// Удаляет время у сотрудников не рабочее для филиала
    /// </summary>
    public class CorrectingReservationResourcesTimeOfBookingHandler
    {
        private readonly int _affiliateId;
        private readonly TypeCorrectingReservationResourcesTimes _typeCorrecting;

        public CorrectingReservationResourcesTimeOfBookingHandler(int affiliateId, TypeCorrectingReservationResourcesTimes typeCorrecting)
        {
            _affiliateId = affiliateId;
            _typeCorrecting = typeCorrecting;
        }

        public void Execute()
        {
            var affiliateTimesOfBooking = AffiliateTimeOfBookingService.GetByAffiliate(_affiliateId);

            foreach (var reservationResource in ReservationResourceService.GetByAffiliate(_affiliateId))
            {
                var reservationResourceBookingIntervalMinutes =
                    ReservationResourceService.GetBookingIntervalMinutesForAffiliate(_affiliateId,
                        reservationResource.Id);

                var isDefaultIntervalMinutes =
                    reservationResourceBookingIntervalMinutes == null &&
                    (_typeCorrecting == TypeCorrectingReservationResourcesTimes.All ||
                     _typeCorrecting == TypeCorrectingReservationResourcesTimes.WithDefaultInterval);

                var isIndividualIntervalMinutes =
                    reservationResourceBookingIntervalMinutes != null &&
                    (_typeCorrecting == TypeCorrectingReservationResourcesTimes.All ||
                     _typeCorrecting == TypeCorrectingReservationResourcesTimes.WithIndividualInterval);

                if (isDefaultIntervalMinutes == false && isIndividualIntervalMinutes == false)
                    continue;

                var reservationResourceTimesOfBooking =
                    ReservationResourceTimeOfBookingService.GetBy(_affiliateId, reservationResource.Id).GroupBy(x => x.DayOfWeek).ToList();

                if (isDefaultIntervalMinutes)
                {
                    foreach (var dayOfWeekTimesOfBooking in reservationResourceTimesOfBooking)
                    {
                        var affiliateTimesOfBookingDayOfWeek =
                            affiliateTimesOfBooking.Where(x => x.DayOfWeek == dayOfWeekTimesOfBooking.Key).ToList();

                        foreach (var reservationResourceTimeOfBooking in dayOfWeekTimesOfBooking)
                            if (!AffiliateService.ExistDateRangeInTimeOfBookingByDayOfWeek(
                                reservationResourceTimeOfBooking.StartTime,
                                reservationResourceTimeOfBooking.EndTime,
                                affiliateTimesOfBookingDayOfWeek))
                            {
                                ReservationResourceTimeOfBookingService.Delete(reservationResourceTimeOfBooking.Id);
                            }
                    }
                }

                if (isIndividualIntervalMinutes)
                {
                    foreach (var dayOfWeekTimesOfBooking in reservationResourceTimesOfBooking)
                    {
                        var affiliateTimesOfBookingDayOfWeek =
                            affiliateTimesOfBooking.Where(x => x.DayOfWeek == dayOfWeekTimesOfBooking.Key).ToList();

                        foreach (var reservationResourceTimeOfBooking in dayOfWeekTimesOfBooking)
                            if (!AffiliateService.CheckDateRangeIsWorkByDayOfWeek(
                                reservationResourceTimeOfBooking.StartTime,
                                reservationResourceTimeOfBooking.EndTime,
                                affiliateTimesOfBookingDayOfWeek))
                            {
                                ReservationResourceTimeOfBookingService.Delete(reservationResourceTimeOfBooking.Id);
                            }
                    }
                }
            }
        }
    }
}
