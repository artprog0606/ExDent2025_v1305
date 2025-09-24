using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;

namespace AdvantShop.Configuration
{
    public class SettingsManager
    {
        public static ManagersOrderConstraint ManagersOrderConstraint
        {
            get => (ManagersOrderConstraint)SettingProvider.Items["ManagersOrderConstraint"].TryParseInt();
            set => SettingProvider.Items["ManagersOrderConstraint"] = ((int)value).ToString();
        }

        public static ManagersLeadConstraint ManagersLeadConstraint
        {
            get => (ManagersLeadConstraint)SettingProvider.Items["ManagersLeadConstraint"].TryParseInt();
            set => SettingProvider.Items["ManagersLeadConstraint"] = ((int)value).ToString();
        }

        public static ManagersCustomerConstraint ManagersCustomerConstraint
        {
            get => (ManagersCustomerConstraint)SettingProvider.Items["ManagersCustomerConstraint"].TryParseInt();
            set => SettingProvider.Items["ManagersCustomerConstraint"] = ((int)value).ToString();
        }

        public static ManagersTaskConstraint ManagersTaskConstraint
        {
            get => (ManagersTaskConstraint)SettingProvider.Items["ManagersTaskConstraint"].TryParseInt();
            set => SettingProvider.Items["ManagersTaskConstraint"] = ((int)value).ToString();
        }
        
        public static string ActiveTwoFactorAuthModule
        {
            get => SettingProvider.Items["ActiveTwoFactorAuthModule"];
            set => SettingProvider.Items["ActiveTwoFactorAuthModule"] = value;
        }
    }
}