using System.Linq;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class GetBonuses : AbstractCommandHandler<GetBonusesResponse>
    {
        private readonly long _cardId;
        private Card _card;

        public GetBonuses(long cardId)
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

        protected override GetBonusesResponse Handle()
        {
            var bonuses = BonusService.Actual(_card.CardId).Select(x => new BonusModel()
            {
                Id = x.Id,
                Amount = x.Amount,
                Name = x.Name,
                Description = x.Description,
                EndDate = x.EndDate,
                StartDate = x.StartDate,
                Status = x.Status,
            }).ToList();

            return new GetBonusesResponse(bonuses);
        }
    }
}