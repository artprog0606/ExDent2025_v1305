using AdvantShop.Areas.Api.Models.Bonuses;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Bonuses
{
    public class GetBonusSystemSettings : AbstractCommandHandler<BonusSystemSettings>
    {
        public GetBonusSystemSettings()
        {
        }

        protected override void Validate()
        {
        }

        protected override BonusSystemSettings Handle()
        {
            var settings = new BonusSystemSettings()
            {
                IsEnabled = BonusSystem.IsEnabled,
                BonusGradeId = BonusSystem.DefaultGrade,
                CardNumberFrom = BonusSystem.CardFrom,
                CardNumberTo = BonusSystem.CardTo,
                MaxOrderPercent = BonusSystem.MaxOrderPercent,
                BonusType = BonusSystem.BonusType,
                BonusTextBlock = BonusSystem.BonusTextBlock,
                BonusRightTextBlock = BonusSystem.BonusRightTextBlock,
                DisallowUseWithCoupon = BonusSystem.ForbidOnCoupon
            };

            foreach (var method in BonusSystem.EnabledNotificationMethods)
                switch (method)
                {
                    case EBonusNotificationMethod.Sms:
                        settings.SmsNotificationEnabled = true;
                        break;
                    case EBonusNotificationMethod.Email:
                        settings.EmailNotificationEnabled = true;
                        break;
                    case EBonusNotificationMethod.Push:
                        settings.PushNotificationEnabled = true;
                        break;
                }

            var grade = settings.Grades.Find(x => x.Value == settings.BonusGradeId.ToString());
            if (grade != null)
                grade.Selected = true;

            var bonusType = settings.BonusTypes.Find(x => x.Value == settings.BonusType.ToString());
            if (bonusType != null)
                bonusType.Selected = true;

            return settings;
        }
    }
}