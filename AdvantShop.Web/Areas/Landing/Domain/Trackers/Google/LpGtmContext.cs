using System.Web;
using AdvantShop.App.Landing.Domain.Settings;
using AdvantShop.Core.Services.SEO;

namespace AdvantShop.App.Landing.Domain.Trackers.Google
{
    public static class LpGtmContext
    {
        private const string GtmContextKey = "LpGtmContext";

        public static GoogleTagManager Current
        {
            get
            {
                if (string.IsNullOrEmpty(LSiteSettings.GoogleTagManagerId))
                    return null;

                if (HttpContext.Current == null)
                    return new GoogleTagManager(LSiteSettings.GoogleTagManagerId, true);

                var gtmContext = HttpContext.Current.Items[GtmContextKey] as GoogleTagManager;
                if (gtmContext != null)
                    return gtmContext;

                var gtm = new GoogleTagManager(LSiteSettings.GoogleTagManagerId, true);

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
