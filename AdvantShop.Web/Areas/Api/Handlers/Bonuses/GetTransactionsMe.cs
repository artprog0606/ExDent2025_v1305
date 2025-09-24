using System;
using System.Linq;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public sealed class GetTransactionsMe : AbstractCommandHandler<BonusCardTransactionsResponse>
    {
        private Card _bonusCard;
        
        protected override void Load()
        {
            _bonusCard = BonusSystemService.GetCard(CustomerContext.CustomerId);
        }

        protected override void Validate()
        {
            if (_bonusCard == null)
                throw new BlException("Бонусная карта не найдена");
        }

        protected override BonusCardTransactionsResponse Handle()
        {
            var transactions =
                TransactionService.GetLastUnitedByDateAndType(_bonusCard.CardId)
                    .Select(x => new BonusCardTransaction(x))
                    .ToList();

            return new BonusCardTransactionsResponse(transactions);
        }
    }
}