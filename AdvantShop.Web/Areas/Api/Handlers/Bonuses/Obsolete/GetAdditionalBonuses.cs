using System;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;
using System.Linq;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    [Obsolete]
    public class GetAdditionalBonuses : AbstractCommandHandler<GetAdditionBonusesResponse>
    {
        private readonly long _cardId;
        private Card _card;

        public GetAdditionalBonuses(long cardId)
        {
            _cardId = cardId;
        }

        protected override void Validate()
        {
            _card = CardService.Get(_cardId);

            if (_card == null) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));

            if (_card.Blocked) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));
        }

        protected override GetAdditionBonusesResponse Handle()
        {
            var bonuses = BonusService.Actual(_card.CardId).Select(x => new AdditionalBonusModel()
            {
                Id = x.Id,
                Amount = x.Amount,
                Name = x.Name,
                Description = x.Description,
                EndDate = x.EndDate,
                StartDate = x.StartDate,
                Status = x.Status,
            }).ToList();

            return new GetAdditionBonusesResponse(bonuses);
        }
    }
}