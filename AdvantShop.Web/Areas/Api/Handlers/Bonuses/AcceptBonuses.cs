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
    public class AcceptBonuses : AbstractCommandHandler<ApiResponse>
    {
        private readonly long _cardId;
        private readonly AcceptBonusModel _model;
        private Card _card;

        public AcceptBonuses(long cardId, AcceptBonusModel model)
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

            if (_model.Amount < 0)
                throw new BlException("Не может быть отрицательным");

            if (_model.StartDate != null && _model.EndDate != null && (_model.StartDate > _model.EndDate))
                throw new BlException("Дата начала больше даты окончания");            

            if (_model.EndDate != null && _model.EndDate < DateTime.Today)
                throw new BlException("Срок действия бонуса истек");            
        }

        protected override ApiResponse Handle()
        {
            BonusSystemService.AcceptBonuses(_card.CardId, _model.Amount, _model.Reason, _model.Name, _model.StartDate, _model.EndDate, null, _model.SendSms);
            return new ApiResponse();
        }
    }
}