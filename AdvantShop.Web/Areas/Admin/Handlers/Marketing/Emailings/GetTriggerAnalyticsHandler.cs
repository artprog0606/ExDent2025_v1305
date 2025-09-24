using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetTriggerAnalyticsHandler : BaseGetEmailingAnalyticsHandler<EmailingAnalyticsModel>
    {
        private readonly int _triggerId;
        private Guid? _emailingId;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        public GetTriggerAnalyticsHandler(int triggerId, DateTime dateFrom, DateTime dateTo, Guid? emailingId)
        {
            _triggerId = triggerId;
            _emailingId = emailingId;
            _dateFrom = dateFrom.Date;
            _dateTo = dateTo.Date;
        }

        public override EmailingAnalyticsModel Execute()
        {
            var cacheName = CacheNames.AdvantShopMail + $"Trigger_{_triggerId}_{_dateFrom.Ticks}_{_dateTo.Ticks}";
            
            var result = CacheManager.Get(cacheName, 0.5, () =>
            {
                var statisticsList = AdvantShopMailService.GetTriggerAnalytics(_triggerId, _dateFrom, _dateTo);

                var item =
                    statisticsList.FirstOrDefault(x =>
                        x.EmailingId != null && (!_emailingId.HasValue || x.EmailingId == _emailingId.Value));
                return GetModel(item);
            });

            return result;
        }
    }
}
