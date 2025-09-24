using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Taxes;

namespace AdvantShop.Configuration
{
    public class SettingsCertificates
    {
        public static ePaymentMethodType PaymentMethodType
        {
            get
            {
                int settingValue;
                return int.TryParse(SettingProvider.Items["SettingsCertificates.PaymentMethodType"], out settingValue)
                    ? (ePaymentMethodType)settingValue
                    : ePaymentMethodType.advance;
            }
            set => SettingProvider.Items["SettingsCertificates.PaymentMethodType"] = ((int)value).ToString();
        }

        public static ePaymentSubjectType PaymentSubjectType
        {
            get
            {
                int settingValue;
                return int.TryParse(SettingProvider.Items["SettingsCertificates.PaymentSubjectType"], out settingValue)
                    ? (ePaymentSubjectType)settingValue
                    : ePaymentSubjectType.payment;
            }
            set => SettingProvider.Items["SettingsCertificates.PaymentSubjectType"] = ((int)value).ToString();
        }

        public static bool ShowCertificatePaymentMetodOnlyCoversSum
        {
            get => SettingProvider.Items["ShowCertificatePaymentMetodOnlyCoversSum"].TryParseBool();
            set => SettingProvider.Items["ShowCertificatePaymentMetodOnlyCoversSum"] = value.ToString();
        }
    }
}
