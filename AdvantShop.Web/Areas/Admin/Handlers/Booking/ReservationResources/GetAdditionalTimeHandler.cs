using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.ReservationResources;

namespace AdvantShop.Web.Admin.Handlers.Booking.ReservationResources
{
    public class GetAdditionalTimeHandler
    {
        private readonly int _affiliateId;
        private readonly ReservationResource _reservationResource;
        private readonly DateTime _date;

        public GetAdditionalTimeHandler(int affiliateId, ReservationResource reservationResource, DateTime date)
        {
            _affiliateId = affiliateId;
            _reservationResource = reservationResource;
            _date = date;
        }

        public AdditionalTimeModel Execute()
        {
            var affiliateTimes = ReservationResourceAdditionalTimeService.GetByDate(_affiliateId, _reservationResource.Id, _date);

            return new AdditionalTimeModel
            {
                Times = affiliateTimes.Count == 0 || !affiliateTimes[0].IsWork ? new List<string>() : affiliateTimes.Select(x => string.Format("{0:HH\\:mm}-{1:HH\\:mm}", x.StartTime, x.EndTime)).ToList()
            };
        }
    }
}
