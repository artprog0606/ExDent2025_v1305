using System;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class ProcessMoneyHandler : AbstractCommandHandler
    {
        private readonly int _id;
        private readonly decimal _amount;
        private readonly string _basis;
        private readonly bool _isRewardPayout;
        private readonly DateTime? _rewardPeriodTo;

        private Partner _partner;

        public ProcessMoneyHandler(int partnerId, decimal amount, string basis, bool isRewardPayout = false, DateTime? rewardPeriodTo = null)
        {
            _id = partnerId;
            _amount = amount;
            _basis = basis;
            _isRewardPayout = isRewardPayout;
            _rewardPeriodTo = rewardPeriodTo;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
        }

        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
            if (_amount == 0)
                throw new BlException("Не указана сумма");
        }

        protected override void Handle()
        {
            PartnerService.ProcessMoney(_id, _amount, _basis, isRewardPayout: _isRewardPayout, rewardPeriodTo: _rewardPeriodTo);
        }
    }
}
