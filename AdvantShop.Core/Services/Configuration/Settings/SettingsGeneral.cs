//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Saas;

namespace AdvantShop.Configuration
{
    public class SettingsGeneral
    {
        private static readonly Object ThisLock = new Object();

        private const string CheckedIpsCacheKey = "CheckedIps_";

        public static string AbsoluteUrlPath
        {
            get
            {
                // check Handler instead of Request (Request may throw error "Request is not available in this context")
                if (HttpContext.Current != null && HttpContext.Current.Handler != null)
                {
                    if (HttpContext.Current.Request != null)
                        return (HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath).ToLower();
                }
                return SettingsMain.SiteUrl.ToLower();
            }
        }
        

        private static string _absolutePath;

        public static string AbsolutePath
        {
            get
            {
                if (_absolutePath.IsNullOrEmpty())
                    SetAbsolutePath(System.Web.Hosting.HostingEnvironment.MapPath("~/"));
                return _absolutePath;
            }
        }

        public static void SetAbsolutePath(string st)
        {
            lock (ThisLock)
            {
                _absolutePath = st;
            }
        }

        public static string SiteVersion => SettingProvider.GetConfigSettingValue("PublicVersion");

        public static string SiteVersionDev => SettingProvider.GetConfigSettingValue("Version");

        public static string Release => SettingProvider.GetConfigSettingValue("Release");

        public static string CurrentSaasId
        {
            get => SettingProvider.Items["LicKey"];
            set => SettingProvider.Items["LicKey"] = value;
        }

        public static string CsvSeparator
        {
            get => SettingProvider.Items["CsvSeparator"];
            set => SettingProvider.Items["CsvSeparator"] = value;
        }

        public static string CsvEnconing
        {
            get => SettingProvider.Items["CsvEnconing"];
            set => SettingProvider.Items["CsvEnconing"] = value;
        }

        public static string CsvColumSeparator
        {
            get => SettingProvider.Items["CsvColumSeparator"];
            set => SettingProvider.Items["CsvColumSeparator"] = value;
        }

        public static string CsvPropertySeparator
        {
            get => SettingProvider.Items["CsvPropertySeparator"];
            set => SettingProvider.Items["CsvPropertySeparator"] = value;
        }

        public static bool CsvExportNoInCategory
        {
            get => SettingProvider.Items["CsvExportNoInCategory"].TryParseBool();
            set => SettingProvider.Items["CsvExportNoInCategory"] = value.ToString();
        }

        public static List<string> BannedIpList
        {
            get
            {
                var item = SettingProvider.Items["BannedIp"];
                if (item.IsNullOrEmpty())
                    return new List<string>();
                return item.Split(",").ToList();
            }
            set
            {
                if (value == null)
                    SettingProvider.Items["BannedIp"] = string.Empty;
                else
                    SettingProvider.Items["BannedIp"] = string.Join(",", value);
                CacheManager.RemoveByPattern(CheckedIpsCacheKey);
            }
        }

        public static ConcurrentDictionary<string, bool> GetCheckedIps(SaasData saasData)
        {
            var key = $"{CheckedIpsCacheKey}{saasData.IpBlackListValues?.GetHashCode()}_{saasData.IpWhiteListValues?.GetHashCode()}"; // HashCode чтобы при изменение черного или белого списка кэш сбросился 
            if (!CacheManager.TryGetValue(key, out ConcurrentDictionary<string, bool> checkedIps))
            {
                CacheManager.RemoveByPattern(CheckedIpsCacheKey);
                checkedIps = new ConcurrentDictionary<string, bool>();
                CacheManager.Insert(key, checkedIps, 60 * 24);
            }

            return checkedIps;
        }

        public static bool AllowSearchBotsFromOtherCountries
        {
            get => SettingProvider.Items["AllowSearchBotsFromOtherCountries"].TryParseBool();
            set => SettingProvider.Items["AllowSearchBotsFromOtherCountries"] = value.ToString();
        }
        
        public static bool BackupPhotosBeforeDeleting => SettingProvider.Items["BackupPhotosBeforeDeleting"].TryParseBool();

        public static bool DisableXFrameOptionsHeader => SettingProvider.Items["DisableXFrameOptionsHeader"].TryParseBool();

        public static bool? UseCDNFonts => SettingProvider.Items["UseCDNFonts"].TryParseBool(true);
        public static bool? UseCDNDesign => SettingProvider.Items["UseCDNDesign"].TryParseBool(true);
        public static string LastBundlesCleanup
        {
            get => SettingProvider.Items["LastBundlesCleanup"];
            set => SettingProvider.Items["LastBundlesCleanup"] = value;
        }
        
        public static bool SkipResizeOriginalPhotos => SettingProvider.Items["SkipResizeOriginalPhotos"].TryParseBool(true) ?? false;
    }
}

