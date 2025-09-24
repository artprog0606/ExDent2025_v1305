using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;

namespace AdvantShop.Configuration
{
    public class SettingsOk
    {
        public static bool IsOkSettingsConfigured =>
            !string.IsNullOrEmpty(ApplicationPublicKey)
            && !string.IsNullOrEmpty(ApplicationAccessToken) 
            && !string.IsNullOrEmpty(ApplicationSessionSecretKey);
        
        public static string ApplicationPublicKey
        {
            get => SettingProvider.Items["SettingsOk.ApplicationPublicKey"];
            set
            {
                SettingProvider.Items["SettingsOk.ApplicationPublicKey"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        public static string ApplicationAccessToken
        {
            get => SettingProvider.Items["SettingsOk.ApplicationAccessToken"];
            set
            {
                SettingProvider.Items["SettingsOk.ApplicationAccessToken"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        public static string ApplicationSessionSecretKey
        {
            get => SettingProvider.Items["SettingsOk.ApplicationSessionSecretKey"];
            set
            {
                SettingProvider.Items["SettingsOk.ApplicationSessionSecretKey"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        public static string GroupSocialAccessToken
        {
            get => SettingProvider.Items["SettingsOk.GroupSocialAccessToken"];
            set => SettingProvider.Items["SettingsOk.GroupSocialAccessToken"] = value;
        }

        public static string GroupId
        {
            get => SettingProvider.Items["SettingsOk.GroupId"];
            set => SettingProvider.Items["SettingsOk.GroupId"] = value;
        }

        public static string GroupName
        {
            get => SettingProvider.Items["SettingsOk.GroupName"];
            set => SettingProvider.Items["SettingsOk.GroupName"] = value;
        }
        
        public static bool OkSubscribeToMessages
        {
            get => SettingProvider.Items["OkSubscribeToMessages"].TryParseBool(true) ?? true;
            set => SettingProvider.Items["OkSubscribeToMessages"] = value.ToString();
        }
    }
}