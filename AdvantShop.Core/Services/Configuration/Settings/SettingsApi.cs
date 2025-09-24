//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsApi
    {
        public static string ApiKey
        {
            get => SettingProvider.Items["Api_ApiKey"];
            set => SettingProvider.Items["Api_ApiKey"] = value;
        }
        
        public static string ApiKeyAuth
        {
            get => SettingProvider.Items["Api_ApiKeyAuth"];
            set => SettingProvider.Items["Api_ApiKeyAuth"] = value;
        }
        
        public static string AndroidPublishedAppUrl
        {
            get => SettingProvider.Items["Api_AndroidPublishedAppUrl"];
            set => SettingProvider.Items["Api_AndroidPublishedAppUrl"] = value;
        }
        
        public static string IosPublishedAppUrl
        {
            get => SettingProvider.Items["Api_IosPublishedAppUrl"];
            set => SettingProvider.Items["Api_IosPublishedAppUrl"] = value;
        }
        
        public static string RustorePublishedAppUrl
        {
            get => SettingProvider.Items["Api_RustorePublishedAppUrl"];
            set => SettingProvider.Items["Api_RustorePublishedAppUrl"] = value;
        }

        public static string AppGalleryPublishedAppUrl
        {
            get => SettingProvider.Items["Api_AppGalleryPublishedAppUrl"];
            set => SettingProvider.Items["Api_AppGalleryPublishedAppUrl"] = value;
        }

        public static Guid? TestAccountCustomerId
        {
            get => SettingProvider.Items["Api_TestAccountCustomerId"].TryParseGuid(true);
            set => SettingProvider.Items["Api_TestAccountCustomerId"] = value?.ToString();
        }
        
        public static string TestAccountSmsVerificationCode
        {
            get => SettingProvider.Items["Api_TestAccountSmsVerificationCode"];
            set => SettingProvider.Items["Api_TestAccountSmsVerificationCode"] = value;
        }

        public static string GoogleAccessToken
        {
            get => SettingProvider.Items["Api_GoogleAccessToken"];
            set => SettingProvider.Items["Api_GoogleAccessToken"] = value;
        }

        public static long GoogleAccessTokenExpiresDate
        {
            get => SettingProvider.Items["Api_GoogleAccessTokenExpiresDate"].TryParseLong();
            set => SettingProvider.Items["Api_GoogleAccessTokenExpiresDate"] = value.ToString();
        }

        public static string TypeGoogleAccessToken
        {
            get => SettingProvider.Items["Api_TypeGoogleAccessToken"];
            set => SettingProvider.Items["Api_TypeGoogleAccessToken"] = value;
        }

        public static string GooglePrivateKey
        {
            get => SettingProvider.Items["Api_GooglePrivateKey"];
            set => SettingProvider.Items["Api_GooglePrivateKey"] = value;
        }

        public static string GoogleClientEmail
        {
            get => SettingProvider.Items["Api_GoogleClientEmail"];
            set => SettingProvider.Items["Api_GoogleClientEmail"] = value;
        }
        
        public static string GoogleProjectId
        {
            get => SettingProvider.Items["Api_GoogleProjectId"];
            set => SettingProvider.Items["Api_GoogleProjectId"] = value;
        }
        public static string MobileAppName
        {
            get => SettingProvider.Items["Mobile_App_Name"];
            set => SettingProvider.Items["Mobile_App_Name"] = value;
        }
        public static string MobileAppDescription
        {
            get => SettingProvider.Items["Mobile_App_Description"];
            set => SettingProvider.Items["Mobile_App_Description"] = value;
        }
        
        public static string MobileAppIcon
        {
            get => SettingProvider.Items["Mobile_App_Icon"];
            set => SettingProvider.Items["Mobile_App_Icon"] = value;
        }
        
        public static bool MobileAppBannerVisibility
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["Mobile_App_Banner_Visibility"]);
            set => SettingProvider.Items["Mobile_App_Banner_Visibility"] = value.ToString();
        }
    }
}
