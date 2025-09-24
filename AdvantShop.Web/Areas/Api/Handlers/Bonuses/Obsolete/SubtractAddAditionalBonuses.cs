using System;
using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    [Obsolete]
    public class SubtractAddAditionalBonuses : AbstractCommandHandler<ApiResponse>
    {
        private readonly long _cardId;
        private readonly SubctractAdditionalBonusModel _model;
        private Card _card;
        private Bonus _bonus;

        public SubtractAddAditionalBonuses(long cardId, SubctractAdditionalBonusModel model)
        {
            _cardId = cardId;
            _model = model;
        }

        protected override void Validate()
        {
            _card = CardService.Get(_cardId);

            if (_card == null) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));

            if (_card.Blocked) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));

            _bonus = BonusService.Get(_model.AdditionalBonusId);

            if (_bonus == null) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.BonusNotExist"));

            if (_bonus.Amount < _model.Amount) 
                throw new BlException(T("Admin.Cards.SubstractMainBonusHandler.Error.MoreSubstractThatHave"));
        }

        protected override ApiResponse Handle()
        {
            BonusSystemService.SubtractBonuses(_bonus, _card.CardId, _model.Amount, _model.Reason, _model.SendSms);
            return new ApiResponse();
        }
    }
}