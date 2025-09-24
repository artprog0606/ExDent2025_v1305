using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Affiliate;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate
{
    public class GetAdditionalTimeFromDataHandler
    {
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly DateTime? _date;
        private readonly DateTime? _dateFrom;
        private readonly DateTime? _dateBy;

        public GetAdditionalTimeFromDataHandler(Core.Services.Booking.Affiliate affiliate)
        {
            _affiliate = affiliate;
        }

        public GetAdditionalTimeFromDataHandler(Core.Services.Booking.Affiliate affiliate, DateTime date) : this(affiliate)
        {
            _date = date;
        }

        public GetAdditionalTimeFromDataHandler(Core.Services.Booking.Affiliate affiliate, DateTime dateFrom, DateTime dateBy) : this(affiliate)
        {
            _dateFrom = dateFrom;
            _dateBy = dateBy;
        }

        public AdditionalTimeFromDataModel Execute()
        {
            var listTimes = BookingService.GetListTimes(_affiliate.BookingIntervalMinutes);

            var workTimes = _date.HasValue ? AffiliateTimeOfBookingService.GetByAffiliateAndDayOfWeek(_affiliate.Id, _date.Value.DayOfWeek) : new List<AffiliateTimeOfBooking>();

            var existAddTimes = _date.HasValue || _dateFrom.HasValue // есть временные промежутки
                ? _date.HasValue // указан конкретный день
                    ? AffiliateAdditionalTimeService.Exists(_affiliate.Id, _date.Value)
                    : AffiliateAdditionalTimeService.Exists(_affiliate.Id, _dateFrom.Value.Date, _dateBy.Value.Date.AddDays(1))
                : false;

            return new AdditionalTimeFromDataModel
            {
                Times = listTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.Item1, x.Item2)).ToList(),
                WorkTimes = workTimes.Select(x => string.Format("{0:hh\\:mm}-{1:hh\\:mm}", x.StartTime, x.EndTime)).ToList(),
                ExistAdditionalTimes = existAddTimes
            };
        }
    }
}
