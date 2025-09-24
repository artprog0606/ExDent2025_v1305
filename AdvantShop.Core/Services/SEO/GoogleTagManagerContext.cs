using System.Web;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.SEO
{
    public static class GoogleTagManagerContext
    {
        private const string GtmContextKey = "GtmContextKey";

        public static GoogleTagManager Current
        {
            get
            {
                if (HttpContext.Current == null)
                    return new GoogleTagManager(SettingsSEO.GTMContainerID, SettingsSEO.UseGTM);

                var gtmContext = HttpContext.Current.Items[GtmContextKey] as GoogleTagManager;
                if (gtmContext != null)
                    return gtmContext;

                var gtm = new GoogleTagManager(SettingsSEO.GTMContainerID, SettingsSEO.UseGTM);

                HttpContext.Current.Items[GtmContextKey] = gtm;

                return gtm;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[GtmContextKey] = value;
            }
        }
    }
}
