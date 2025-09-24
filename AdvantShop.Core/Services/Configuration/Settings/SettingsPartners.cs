using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class SettingsPartners
    {
        public static float DefaultRewardPercent
        {
            get => SettingProvider.Items["Partners.DefaultRewardPercent"].TryParseFloat();
            set => SettingProvider.Items["Partners.DefaultRewardPercent"] = value.ToString();
        }

        public static int PayoutMinCustomersCount
        {
            get => SettingProvider.Items["Partners.PayoutMinCustomersCount"].TryParseInt();
            set => SettingProvider.Items["Partners.PayoutMinCustomersCount"] = value.ToString();
        }

        public static decimal PayoutMinBalance
        {
            get => SettingProvider.Items["Partners.PayoutMinBalance"].TryParseDecimal();
            set => SettingProvider.Items["Partners.PayoutMinBalance"] = value.ToString();
        }

        public static decimal PayoutCommissionNaturalPerson
        {
            get => SettingProvider.Items["Partners.PayoutCommissionNaturalPerson"].TryParseDecimal();
            set => SettingProvider.Items["Partners.PayoutCommissionNaturalPerson"] = value.ToString();
        }

        public static string ActReportTplDocxLegalEntity
        {
            get => SettingProvider.Items["Partners.ActReportTplDocxLegalEntity"];
            set => SettingProvider.Items["Partners.ActReportTplDocxLegalEntity"] = value;
        }

        public static string ActReportTplDocxNaturalPerson
        {
            get => SettingProvider.Items["Partners.ActReportTplDocxNaturalPerson"];
            set => SettingProvider.Items["Partners.ActReportTplDocxNaturalPerson"] = value;
        }

        public static bool AutoApplyPartnerCoupon
        {
            get => SettingProvider.Items["Partners.AutoApplyPartnerCoupon"].TryParseBool();
            set => SettingProvider.Items["Partners.AutoApplyPartnerCoupon"] = value.ToString();
        }

        public static bool EnableCaptchaInRegistrationPartners
        {
            get => SettingProvider.Items["Partners.EnableCaptchaInRegistrationPartners"].TryParseBool();
            set => SettingProvider.Items["Partners.EnableCaptchaInRegistrationPartners"] = value.ToString();
        }

        public static bool ActivatePartners
        {
            get
            {
                var activate = SettingProvider.Items["Partners.ActivatePartners"];

                if (!string.IsNullOrEmpty(activate))
                    return activate.TryParseBool();
                return true;
            }
            set => SettingProvider.Items["Partners.ActivatePartners"] = value.ToString();
        }

        public static string EmailNotification
        {
            get => SettingProvider.Items["Partners.EmailNotification"];
            set => SettingProvider.Items["Partners.EmailNotification"] = value;
        }
        
        public static bool SkipRegPasportStepForIndividualEntity
        {
            get => SettingProvider.Items["Partners.SkipRegPasportStepForIndividualEntity"].TryParseBool();
            set => SettingProvider.Items["Partners.SkipRegPasportStepForIndividualEntity"] = value.ToString();
        }
    }
}