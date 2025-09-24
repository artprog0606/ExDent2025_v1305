//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Globalization;
using AdvantShop.Helpers;
using AdvantShop.Permission;
using AdvantShop.Core.Common.Extensions;
using System;
using AdvantShop.Diagnostics;
using System.Web;

namespace AdvantShop.Configuration
{
    public class SettingsLic
    {
        public static string LicKey
        {
            get => SettingProvider.Items["LicKey"].Trim();
            set => SettingProvider.Items["LicKey"] = value.Trim();
        }

        public static bool ActiveLic
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ActiveLic"]);
            set => SettingProvider.Items["ActiveLic"] = value.ToString(CultureInfo.InvariantCulture);
        }

        public static string AdvId => LicKey.Substring(0, LicKey.Length >8 ? 8 : LicKey.Length);

        public static string ClientCode
        {
            get => SettingProvider.Items["ClientCode"];
            set => SettingProvider.Items["ClientCode"] = value;
        }


        public static bool ShowAdvantshopJivoSiteForm
        {
            get => SQLDataHelper.GetBoolean(SettingProvider.Items["ShowAdvantshopJivoSiteForm"]);
            set => SettingProvider.Items["ShowAdvantshopJivoSiteForm"] = value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool Activate(string key = null)
        {
            try
            {
                var accountNumber = PermissionAccsess.ActiveDailyLic(key ?? LicKey, 
                    HttpUtility.UrlEncode(SettingsMain.SiteUrl), HttpUtility.UrlEncode(SettingsMain.ShopName), SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev, SettingsGeneral.AbsoluteUrlPath);

                if (accountNumber != "false")
                {
                    if (accountNumber.IsNotEmpty())
                        ClientCode = accountNumber;
                    if (key != null)
                        LicKey = key;
                    ActiveLic = true;
                    return true;
                }

                if (key != null)
                    LicKey = key;
                ActiveLic = false;

                return false;
            }
            catch (Exception ex)
            {
                Debug.Log.Error("Error at license check", ex);
                return true;
            }
        }

        public static string BasePlatformUrl => SettingProvider.GetConfigSettingValue("BasePlatformUrl");

        public static string DomainServiceUrl => SettingProvider.GetConfigSettingValue("DomainServiceUrl");

        public static string AccountPlatformUrl => SettingProvider.GetConfigSettingValue("AccountPlatformUrl");

        public static string GeoIPServiceUrl => SettingProvider.GetInternalSetting("GeoIPServiceUrl");

        public static string EmailServiceUrl => SettingProvider.GetInternalSetting("EmailServiceUrl");

        public static string ImageServiceUrl => SettingProvider.GetInternalSetting("ImageServiceUrl");

        public static string WebpDownGraderServiceUrl => SettingProvider.GetInternalSetting("WebpDownGraderServiceUrl");
    }
}