using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.App.Landing.Models;
using AdvantShop.Core;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.App.Landing.Handlers.Booking
{
    public class GetBookingByTimeFreeDayHandler : ICommandHandler<string>
    {
        private readonly GetBookingByTimeFreeDayDto _model;

        public GetBookingByTimeFreeDayHandler(GetBookingByTimeFreeDayDto model)
        {
            _model = model;
        }

        public string Execute()
        {

            var affiliate = AffiliateService.Get(_model.AffiliateId);
            if (affiliate == null)
                throw new BlException("Филиал не найден");

            var resource = ReservationResourceService.Get(_model.ResourceId);
            if (resource == null)
                throw new BlException("Ресурс не найден");

            var dateFrom = DateTime.Now.Date;
            var dateTo = DateTime.Now.Date.AddMonths(3);

            var affiliateTimesOfBooking = new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>(
                AffiliateTimeOfBookingService.GetByAffiliate(_model.AffiliateId)
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var affiliateAdditionalTimes = new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>(
                AffiliateAdditionalTimeService.GetByAffiliateAndDateFromTo(_model.AffiliateId, dateFrom, dateTo)
                .GroupBy(x => x.StartTime.Date)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var resourceTimesOfBooking = new SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>>(
                ReservationResourceTimeOfBookingService.GetBy(_model.AffiliateId, _model.ResourceId)
                .GroupBy(x => x.DayOfWeek)
                .ToDictionary(x => x.Key, x => x.ToList()));

            var resourceAdditionalTimes = new SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>>(
                ReservationResourceAdditionalTimeService.GetByDateFromTo(_model.AffiliateId, _model.ResourceId, dateFrom, dateTo)
                .GroupBy(x => x.StartTime.Date)
                .ToDictionary(x => x.Key, x => x.ToList()));

            DateTime? freeDate = null;

            var workDates = GetWorkDates(DateTime.Now.Date, DateTime.Now.Date.AddMonths(3), affiliateAdditionalTimes, affiliateTimesOfBooking, resourceAdditionalTimes, resourceTimesOfBooking);
            var bookings = BookingService.GetListByDateFromToAndReservationResource(_model.AffiliateId, dateFrom, dateTo, _model.ResourceId);

            var timesHandler = new GetBookingTimesHandler(_model.ResourceId, _model.AffiliateId, dateFrom/*не влияет*/, _model.SelectedServices);
            foreach (var workDate in workDates)
            {
                var times = timesHandler.GetBookingTimes(
                    workDate,
                    affiliateAdditionalTimes.ContainsKey(workDate) ? affiliateAdditionalTimes[workDate] : new List<AffiliateAdditionalTime>(),
                    affiliateTimesOfBooking.ContainsKey(workDate.DayOfWeek) ? affiliateTimesOfBooking[workDate.DayOfWeek] : new List<AffiliateTimeOfBooking>(),
                    resourceAdditionalTimes.ContainsKey(workDate) ? resourceAdditionalTimes[workDate] : new List<ReservationResourceAdditionalTime>(),
                    resourceTimesOfBooking.ContainsKey(workDate.DayOfWeek) ? resourceTimesOfBooking[workDate.DayOfWeek] : new List<ReservationResourceTimeOfBooking>(),
                    bookings.Where(x => x.BeginDate < workDate.AddDays(1) && x.EndDate > workDate).ToList());
                if (times.Any())
                {
                    freeDate = workDate;
                    break;
                }
            }
            if (!freeDate.HasValue)
                freeDate = workDates.FirstOrDefault();

            return freeDate.Value.ToString("yyyy-MM-dd");
        }

        private List<DateTime> GetWorkDates(DateTime dateFrom, DateTime dateTo,
            SortedDictionary<DateTime, List<AffiliateAdditionalTime>> affiliateAdditionalTimes,
            SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>> affiliateTimesOfBooking,
            SortedDictionary<DateTime, List<ReservationResourceAdditionalTime>> resourceAdditionalTimes,
            SortedDictionary<DayOfWeek, List<ReservationResourceTimeOfBooking>> resourceTimesOfBooking)
        {
            var result = new List<DateTime>();
            var date = dateFrom;
            do
            {
                var isWork = false;

                if (affiliateAdditionalTimes.ContainsKey(date))
                {
                    if (affiliateAdditionalTimes[date][0].IsWork)
                        isWork = true;
                }
                else if (affiliateTimesOfBooking.ContainsKey(date.DayOfWeek))
                    isWork = true;

                if (resourceAdditionalTimes.ContainsKey(date))
                {
                    if (resourceAdditionalTimes[date][0].IsWork)
                        isWork = true;
                }
                else if (resourceTimesOfBooking.ContainsKey(date.DayOfWeek))
                    isWork = true;

                if (isWork)
                    result.Add(date);

                date = date.AddDays(1);
            } while (date < dateTo);

            return result;
        }
    }
}
