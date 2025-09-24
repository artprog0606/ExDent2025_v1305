using AdvantShop.Core.Common.Extensions;
using System;

namespace AdvantShop.Configuration
{
    public class SettingsCustomers
    {
        public static bool IsRegistrationAsLegalEntity
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsRegistrationAsLegalEntity"]);
            set => SettingProvider.Items["IsRegistrationAsLegalEntity"] = value.ToString();
        }
        public static bool IsRegistrationAsPhysicalEntity
        {
            get {
                var value = SettingProvider.Items["IsRegistrationAsPhysicalEntity"];
                if(value == null)
                    return true;
                return Convert.ToBoolean(SettingProvider.Items["IsRegistrationAsPhysicalEntity"]);
            }
            set => SettingProvider.Items["IsRegistrationAsPhysicalEntity"] = value.ToString();
        }

        public static string CustomerFieldsEnabledBeforeDisabled
        {
            get => SettingProvider.Items["CustomerFieldsEnabledBeforeDisabled"];
            set => SettingProvider.Items["CustomerFieldsEnabledBeforeDisabled"] = value;
        }

        public static float? MinimalOrderPriceForPhysicalEntity
        {
            get => SettingProvider.Items["MinimalOrderPriceForPhysicalEntity"].TryParseFloat(true);
            set => SettingProvider.Items["MinimalOrderPriceForPhysicalEntity"] = value.ToString();
        }

        public static float? MinimalOrderPriceForLegalEntity
        {
            get => SettingProvider.Items["MinimalOrderPriceForLegalEntity"].TryParseFloat(true);
            set => SettingProvider.Items["MinimalOrderPriceForLegalEntity"] = value.ToString();
        }
    }
}
