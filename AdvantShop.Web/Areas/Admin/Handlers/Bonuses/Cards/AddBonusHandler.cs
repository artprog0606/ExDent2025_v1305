using AdvantShop.Core;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class AddBonusHandler: AbstractCommandHandler<bool>
    {
        private readonly AddBonusModel _model;

        public AddBonusHandler(AddBonusModel model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.Amount <= 0)
                throw new BlException("Сумма бонуса должна быть > 0");
            
            var card = CardService.Get(_model.CardId);
            
            if (card == null) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardNotExist"));
            
            if (card.Blocked) 
                throw new BlException(T("Admin.Cards.AddMainBonusHandler.Error.CardIsBlock"));
        }

        protected override bool Handle()
        {
            BonusSystemService.AcceptBonuses(_model.CardId, _model.Amount, _model.Reason, _model.Name, _model.StartDate, _model.EndDate, null, _model.SendSms);

            return true;
        }        
    }
}
