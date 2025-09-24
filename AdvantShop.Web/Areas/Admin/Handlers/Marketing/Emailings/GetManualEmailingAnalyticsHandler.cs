using System;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetManualEmailingAnalyticsHandler : BaseGetEmailingAnalyticsHandler<EmailingAnalyticsModel>
    {
        private Guid _id;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;

        public GetManualEmailingAnalyticsHandler(Guid id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            _id = id;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public override EmailingAnalyticsModel Execute()
        {            
            var cacheName = CacheNames.AdvantShopMail + "Manual_" + _id + "_" + _dateFrom + "_" + _dateTo;
            return CacheManager.Get(cacheName, () =>
            {
                var data = AdvantShopMailService.GetEmailingAnalytics(_id, _dateFrom, _dateTo);
                return GetModel(data);
            });
        }
    }
}
