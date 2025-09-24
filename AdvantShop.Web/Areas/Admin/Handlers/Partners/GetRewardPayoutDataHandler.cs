using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetRewardPayoutDataHandler : AbstractCommandHandler<object>
    {
        private readonly int _id;
        private readonly DateTime _rewardPeriodTo;
        private Partner _partner;

        public GetRewardPayoutDataHandler(int id, DateTime? rewardPeriodTo)
        {
            _id = id;
            var date = rewardPeriodTo.HasValue ? rewardPeriodTo.Value.AddMonths(1) : DateTime.Now;
            _rewardPeriodTo = new DateTime(date.Year, date.Month, 1).AddDays(-1);
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
        }
        
        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
        }

        protected override object Handle()
        {
            var lastRewardPayout = TransactionService.GetLastRewardPayout(_id);
            var dateFrom = lastRewardPayout != null && lastRewardPayout.RewardPeriodTo.HasValue ? lastRewardPayout.RewardPeriodTo.Value.AddDays(1) : (DateTime?)null;

            var amount = TransactionService.GetRewardAmount(_id, dateFrom, _rewardPeriodTo);

            return new
            {
                amount = amount.RoundConvertToDefault(),
                rewardPeriodFrom = dateFrom.HasValue ? dateFrom.Value.ToString("dd.MM.yyyy") : "С регистрации",
                rewardPeriodTo = _rewardPeriodTo.ToString("yyyy-MM-dd"),
                basis = string.Format("Выплата партнерского вознаграждения за период с {0} по {1}",
                    dateFrom.HasValue ? lastRewardPayout.RewardPeriodTo.Value.ToString("dd.MM.yyyy") : "регистрации",
                    _rewardPeriodTo.ToString("dd.MM.yyyy"))
            };
        }
    }
}
