using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.Services.Bonuses.Notification.Template;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class SubstractFreeAmountBonuses : AbstractCommandHandler<ApiResponse>
    {
        private readonly long _cardId;
        private readonly SubstractFreeAmountBonusModel _model;
        private Card _card;

        public SubstractFreeAmountBonuses(long cardId, SubstractFreeAmountBonusModel model)
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

            if (_card.BonusesTotalAmount < _model.Amount)
                throw new BlException(T("Admin.Cards.SubstractMainBonusHandler.Error.MoreSubstractThatHave"));
        }
  
        protected override ApiResponse Handle()
        {
            BonusSystemService.SubtractBonuses(
                _card.CardId,
                _model.Amount,
                _model.Reason);

            SendNotificationOnSubtractBonuses();

            return new ApiResponse();
        }

        private void SendNotificationOnSubtractBonuses()
        {
            if (!_model.SendSms)
                return;

            var customer = CustomerService.GetCustomer(_card.CardId);
            if (customer is null)
                return;
            
            if (customer.StandardPhone is null 
                && customer.EMail.IsNullOrEmpty())
                return;

            NotificationService.Process(_card.CardId, ENotifcationType.OnSubtractBonus, new OnSubtractBonusTempalte
            {
                Bonus = _model.Amount,
                CompanyName = SettingsMain.ShopName,
                Balance = BonusService.ActualSum(_card.CardId),
                Basis = _model.Reason
            });
        }

    }
}