//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Auth;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public enum EAuthByCodeMode
    {
        [Localize("Core.Settings.SettingsOAuth.EAuthByCodeMethod.Sms")]
        Sms = 1,
        
        [Localize("Core.Settings.SettingsOAuth.EAuthByCodeMethod.Call")]
        Call = 2
    }
    
    public class SettingsOAuth
    {

        public static bool YandexActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderYandexActive"]);
            set => SettingProvider.Items["OpenIdProviderYandexActive"] = value.ToString();
        }
        public static bool GoogleActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderGoogleActive"]);
            set => SettingProvider.Items["OpenIdProviderGoogleActive"] = value.ToString();
        }

        public static bool MailActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderMailActive"]);
            set => SettingProvider.Items["OpenIdProviderMailActive"] = value.ToString();
        }

        public static bool VkontakteActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderVkontakteActive"]);
            set => SettingProvider.Items["OpenIdProviderVkontakteActive"] = value.ToString();
        }
        
        public static bool VkIdActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderVkIdActive"]);
            set => SettingProvider.Items["OpenIdProviderVkIdActive"] = value.ToString();
        }

        public static bool FacebookActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderFacebookActive"]);
            set => SettingProvider.Items["OpenIdProviderFacebookActive"] = value.ToString();
        }

        public static bool OdnoklassnikiActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderOdnoklassnikiActive"]);
            set => SettingProvider.Items["OpenIdProviderOdnoklassnikiActive"] = value.ToString();
        }

        public static bool AuthByCodeActive
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["OpenIdProviderAuthByCodeActive"]);
            set => SettingProvider.Items["OpenIdProviderAuthByCodeActive"] = value.ToString();
        }

        public static EAuthByCodeMode AuthByCodeMethod
        {
            get
            {
                if (SQLDataHelper.GetInt(SettingProvider.Items["OpenIdProviderAuthByCodeMethod"]) == 0)
                    return EAuthByCodeMode.Sms;

                return (EAuthByCodeMode)SQLDataHelper.GetInt(SettingProvider.Items["OpenIdProviderAuthByCodeMethod"]);
            }
            set => SettingProvider.Items["OpenIdProviderAuthByCodeMethod"] = ((int)value).ToString();
        }

        public static string FacebookClientId
        {
            get => SettingProvider.Items["oidFacebookClientId"];
            set => SettingProvider.Items["oidFacebookClientId"] = value;
        }

        public static string FacebookApplicationSecret
        {
            get => SettingProvider.Items["oidFacebookApplicationSecret"];
            set => SettingProvider.Items["oidFacebookApplicationSecret"] = value;
        }

        public static string VkontakeClientId
        {
            get => SettingProvider.Items["oidVkontakeClientId"];
            set => SettingProvider.Items["oidVkontakeClientId"] = value;
        }

        public static string VkontakeSecret
        {
            get => SettingProvider.Items["oidVkontakeSecret"];
            set => SettingProvider.Items["oidVkontakeSecret"] = value;
        }
        
        public static string VkIdClientId
        {
            get => SettingProvider.Items["oidVkIdClientId"];
            set => SettingProvider.Items["oidVkIdClientId"] = value;
        }

        public static string OdnoklassnikiClientId
        {
            get => SettingProvider.Items["oidOdnoklassnikiClientId"];
            set => SettingProvider.Items["oidOdnoklassnikiClientId"] = value;
        }

        public static string OdnoklassnikiPublicApiKey
        {
            get => SettingProvider.Items["oidOdnoklassnikiPublicApiKey"];
            set => SettingProvider.Items["oidOdnoklassnikiPublicApiKey"] = value;
        }

        public static string OdnoklassnikiSecret
        {
            get => SettingProvider.Items["oidOdnoklassnikiSecret"];
            set => SettingProvider.Items["oidOdnoklassnikiSecret"] = value;
        }

        public static string GoogleClientId
        {
            get => SettingProvider.Items["oidGoogleClientId"];
            set => SettingProvider.Items["oidGoogleClientId"] = value;
        }

        public static string GoogleClientSecret
        {
            get => SettingProvider.Items["oidGoogleClientSecret"];
            set => SettingProvider.Items["oidGoogleClientSecret"] = value;
        }

        public static string GoogleAnalyticsAccessToken
        {
            get => SettingProvider.Items["GoogleAnalyticsAccessToken"];
            set => SettingProvider.Items["GoogleAnalyticsAccessToken"] = value;
        }


        public static string YandexClientId
        {
            get => SettingProvider.Items["oauthYandexClientId"];
            set => SettingProvider.Items["oauthYandexClientId"] = value;
        }
        public static string YandexClientSecret
        {
            get => SettingProvider.Items["oauthYandexClientSecret"];
            set => SettingProvider.Items["oauthYandexClientSecret"] = value;
        }

        public static bool SmsUseCapthca
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["oauthSmsUseCapthca"]);
            set => SettingProvider.Items["oauthSmsUseCapthca"] = value.ToString();
        }


        public static string MailAppId
        {
            get => SettingProvider.Items["oauthMailAppId"];
            set => SettingProvider.Items["oauthMailAppId"] = value;
        }
        public static string MailClientSecret
        {
            get => SettingProvider.Items["oauthMailClientSecret"];
            set => SettingProvider.Items["oauthMailClientSecret"] = value;
        }

        public static EAuthMethod AuthMethod
        {
            get => SettingProvider.Items["AuthMethod"] != null 
                ? (EAuthMethod)Convert.ToInt32(SettingProvider.Items["AuthMethod"]) 
                : EAuthMethod.Email;
            set => SettingProvider.Items["AuthMethod"] = ((int)value).ToString();
        }

        public static bool IsOauthEnabled()
        {
            return FacebookActive || GoogleActive || MailActive || OdnoklassnikiActive || VkontakteActive || YandexActive || VkIdActive;
        }

    }
}