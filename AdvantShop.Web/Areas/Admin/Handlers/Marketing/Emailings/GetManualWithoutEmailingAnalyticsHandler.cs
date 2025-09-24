using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;
using System;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetManualWithoutEmailingAnalyticsHandler : BaseGetEmailingAnalyticsHandler<EmailingAnalyticsModel>
    {
        private int? _id;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;

        public GetManualWithoutEmailingAnalyticsHandler(int? id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            _id = id;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public override EmailingAnalyticsModel Execute()
        {
            var cacheName = CacheNames.AdvantShopMail + 
                string.Format("Manual_Without_{0}_{1}_{2}", 
                    _id, 
                    _dateFrom.HasValue ? _dateFrom.Value.Ticks.ToString() : string.Empty, 
                    _dateTo.HasValue ? _dateTo.Value.Ticks.ToString() : string.Empty);
            return CacheManager.Get(cacheName, () =>
            {
                var data = AdvantShopMailService.GetWithoutEmailingAnalytics(_id, _dateFrom, _dateTo);
                return GetModel(data);
            });
        }
    }
}