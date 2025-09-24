using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.Services.SEO
{
    public class CopyrightService
    {
        private const string RequestUrlGetCopyright = "http://modules.advantshop.net/copyright?lickey={0}&type={1}";

        private const string CopyrightCacheKey = "AdvantshopModules_Copyrigh";

        public static bool IsCopyrightLocked => 
            SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.DisableCopyrightEdit
            || TrialService.IsTrialEnabled;

        public static string GetFormattedCopyrightText(ECopyrightType copyrightType)
        {
            var copyright = copyrightType == ECopyrightType.Site
                ? SettingsDesign.ShowCustomCopyright
                    ? SettingsDesign.CopyrightText
                    : SettingsDesign.DefaultCopyright
                : SettingsLandingPage.CopyrightHtmlContent;

            return !string.IsNullOrEmpty(copyright)
                ? copyright.Replace("#CURRENT_YEAR#", DateTime.Now.Year.ToString())
                : string.Empty;
        }

        private static string GetActualCopyright(ECopyrightType copyrightType, int timeoutSeconds = 1)
        {
            return CacheManager.Get($"{CopyrightCacheKey}_{copyrightType}", 60, () => {
                if (!string.IsNullOrWhiteSpace(SettingsLic.LicKey))
                {
                    try
                    {
                        var url = string.Format(RequestUrlGetCopyright, SettingsLic.LicKey, copyrightType.ToString());
                        var response = RequestHelper.MakeRequest<string>(url, method: ERequestMethod.GET, timeoutSeconds: timeoutSeconds);
                        return response;
                    }
                    catch (Exception exception)
                    {
                        Debug.Log.Error("Не удалось получить актуальный copyright", exception);
                    }
                }

                return string.Empty;
            });
        }

        private static readonly object LocObject = new object();
        public static string UpdateCopyRight(ECopyrightType copyrightType, int timeoutSeconds = 1)
        {
            lock (LocObject)
            {
                var value = GetActualCopyright(copyrightType, timeoutSeconds);
                if (string.IsNullOrWhiteSpace(value))
                    return string.Empty;
                
                switch (copyrightType)
                {
                    case ECopyrightType.Site:
                        SettingsDesign.DefaultCopyright = value;
                        break;
                    case ECopyrightType.Landing:
                        SettingsLandingPage.CopyrightHtmlContent = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(copyrightType), copyrightType, null);
                }
                return value;
            }
        }
    }
}
