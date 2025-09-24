using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Affiliate;

namespace AdvantShop.Web.Admin.Handlers.Booking.Affiliate
{
    public class GetAdditionalTimeHandler
    {
        private readonly Core.Services.Booking.Affiliate _affiliate;
        private readonly DateTime _date;

        public GetAdditionalTimeHandler(Core.Services.Booking.Affiliate affiliate, DateTime date)
        {
            _affiliate = affiliate;
            _date = date;
        }

        public AdditionalTimeModel Execute()
        {
            var affiliateTimes = AffiliateAdditionalTimeService.GetByAffiliateAndDate(_affiliate.Id, _date);

            return new AdditionalTimeModel
            {
                Times = affiliateTimes.Count == 0 || !affiliateTimes[0].IsWork ? new List<string>() : affiliateTimes.Select(x => string.Format("{0:HH\\:mm}-{1:HH\\:mm}", x.StartTime, x.EndTime)).ToList()
            };
        }
    }
}
