using System.Configuration;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.CDN
{

    public enum eCDN
    {
        Fonts = 0,
        Design = 1
    }


    static public class CDNService
    {
        static public bool UseCDN(eCDN cdn)
        {
            bool result = false;

            switch (cdn)
            {
                case eCDN.Fonts:
                    result = !DebugMode.IsDebugMode(eDebugMode.CdnFonts)
                                       && (SettingsGeneral.UseCDNFonts == true || ((SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled) && SettingsGeneral.UseCDNFonts != false));
                    break;
                case eCDN.Design:
                    result = !DebugMode.IsDebugMode(eDebugMode.CdnDesign)
                                       && (SettingsGeneral.UseCDNDesign == true || ((SaasDataService.IsSaasEnabled || TrialService.IsTrialEnabled) && SettingsGeneral.UseCDNDesign != false));
                    break;
            }

#if debug
result = false;
#endif

            return result;

        }

        static public string GetCDNUrl(eCDN cdn)
        {
            string result = "";
            switch (cdn)
            {
                case eCDN.Fonts:
                    result = SettingProvider.GetConfigSettingValue("CdnFontsUrl");
                    break;
                case eCDN.Design:
                    result = SettingProvider.GetConfigSettingValue("CdnDesignUrl");
                    break;
            }

            return result;
        }
    }
}