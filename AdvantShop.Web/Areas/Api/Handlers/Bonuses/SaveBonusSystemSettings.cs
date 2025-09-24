using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Web.Infrastructure.Handlers;
using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class SaveBonusSystemSettings : AbstractCommandHandler<ApiResponse>
    {
        private readonly BonusSystemSettings _model;

        public SaveBonusSystemSettings(BonusSystemSettings model)
        {
            _model = model;
        }

        protected override void Validate()
        {
            if (_model.CardNumberTo <= _model.CardNumberFrom)
                throw new BlException("Проверьте диапазон карт");

            if (_model.MaxOrderPercent < 0)
                _model.MaxOrderPercent = 0;

            if (_model.MaxOrderPercent > 100)
                _model.MaxOrderPercent = 100;
        }

        protected override ApiResponse Handle()
        {
            BonusSystem.IsEnabled = _model.IsEnabled;
            BonusSystem.DefaultGrade = _model.BonusGradeId;
            BonusSystem.CardFrom = _model.CardNumberFrom;
            BonusSystem.CardTo = _model.CardNumberTo;
            BonusSystem.MaxOrderPercent = _model.MaxOrderPercent;
            BonusSystem.BonusType = _model.BonusType;
            BonusSystem.BonusTextBlock = _model.BonusTextBlock;
            BonusSystem.BonusRightTextBlock = _model.BonusRightTextBlock;
            BonusSystem.ForbidOnCoupon = _model.DisallowUseWithCoupon;
            var notificationMethods = new List<EBonusNotificationMethod>();
            if (_model.SmsNotificationEnabled)
                notificationMethods.Add(EBonusNotificationMethod.Sms);
            if (_model.EmailNotificationEnabled)
                notificationMethods.Add(EBonusNotificationMethod.Email);
            if (_model.PushNotificationEnabled)
                notificationMethods.Add(EBonusNotificationMethod.Push);
            BonusSystem.EnabledNotificationMethods = notificationMethods;

            return new ApiResponse();
        }
    }
}