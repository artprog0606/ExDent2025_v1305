using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class GetAdditionalTimeFromDataHandler
    {
        private readonly int _affiliateId;
        private readonly ReservationResource _reservationResource;
        private readonly DateTime _date;

        public GetAdditionalTimeFromDataHandler(int affiliateId, ReservationResource reservationResource, DateTime date)
        {
            _affiliateId = affiliateId;
            _reservationResource = reservationResource;
            _date = date;
        }

        public AdditionalTimeFromDataModel Execute()
        {
            var listTimes = new List<Tuple<TimeSpan, TimeSpan>>();
            var affiliate = AffiliateService.Get(_affiliateId);
            var additionalTime = AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliate.Id, _date);
            var reservationResourceBookingIntervalMinutes =
                ReservationResourceService.GetBookingIntervalMinutesForAffiliate(affiliate.Id, _reservationResource.Id);

            if (reservationResourceBookingIntervalMinutes == null ||
                reservationResourceBookingIntervalMinutes.Value == affiliate.BookingIntervalMinutes)
            {
                if (additionalTime.Count != 0)
                {
                    if (additionalTime[0].IsWork)
                    {
                        listTimes =
                            additionalTime.Select(
                                x => new Tuple<TimeSpan, TimeSpan>(x.StartTime.TimeOfDay, x.EndTime.TimeOfDay))
                                .ToList();
                    }
                }
                else
                {
                    listTimes = AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliate.Id, _date.DayOfWeek)
                        .Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime, x.EndTime))
                        .ToList();
                }
            }
            else
            {
                var affiliateAdditionalTime = AffiliateAdditionalTimeService.GetByAffiliateAndDate(affiliate.Id, _date);
                var affiliateTimesOfBookingDayOfWeek =
                    AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(affiliate.Id, _date.DayOfWeek);
                var date = _date.Date;

                var time = new TimeSpan();
                var timeEnd = new TimeSpan(1, 0, 0, 0);

                while (time < timeEnd)
                {
                    var start = time;
                    var end = (time = time.Add(new TimeSpan(0, reservationResourceBookingIntervalMinutes.Value, 0)));

                    if (AffiliateService.CheckDateRangeIsWork(date + start, date + end, 
                        new SortedDictionary<DateTime, List<AffiliateAdditionalTime>>() { {date, affiliateAdditionalTime} }, 
                        new SortedDictionary<DayOfWeek, List<AffiliateTimeOfBooking>>() { {date.DayOfWeek, affiliateTimesOfBookingDayOfWeek} }))
                        listTimes.Add(new Tuple<TimeSpan, TimeSpan>(start, end));
                }

            }

            var workTimes = ReservationResourceTimeOfBookingService.GetByDayOfWeek(affiliate.Id, _reservationResource.Id, _date.DayOfWeek)
                .Select(x => new Tuple<TimeSpan, TimeSpan>(x.StartTime, x.EndTime))
                .ToList();

            return new AdditionalTimeFromDataModel
            {
                Times = listTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                WorkTimes = workTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                ExistAdditionalTimes = ReservationResourceAdditionalTimeService.Exists(affiliate.Id, _reservationResource.Id, _date)
            };
        }
    }
}
