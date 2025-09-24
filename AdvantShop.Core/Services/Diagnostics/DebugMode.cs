using System;
using System.Web;
using System.Linq;
using AdvantShop.Core.UrlRewriter;


namespace AdvantShop.Core.Services.Diagnostics
{
    public enum eDebugMode
    {
        Normal = 0,
        Css = 1,
        Js = 2,
        StaticBlocks = 3,
        Modules = 4,
        Redirects = 5,
        All = 6,
        CriticalCss = 7,
        CdnFonts = 8,
        Html = 9,
        CdnDesign = 10
    }

    public class DebugMode
    {
        public static bool IsDebugMode(eDebugMode mode)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request["DebugMode"] == null)
            {
                if (HttpContext.Current.Request.GetUrlReferrer() == null ||
                    HttpUtility.ParseQueryString(HttpContext.Current.Request.GetUrlReferrer().Query)["DebugMode"] == null)
                {
                    return false;
                }

                var urlModesReferrer = HttpUtility.ParseQueryString(HttpContext.Current.Request.GetUrlReferrer().Query)["DebugMode"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return urlModesReferrer.Any(m => string.Equals(m, eDebugMode.All.ToString(), StringComparison.OrdinalIgnoreCase)
                                                 || string.Equals(m, mode.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            var urlModes = HttpContext.Current.Request["DebugMode"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return urlModes.Any(m => string.Equals(m, eDebugMode.All.ToString(), StringComparison.OrdinalIgnoreCase)
                                || string.Equals(m, mode.ToString(), StringComparison.OrdinalIgnoreCase));
            
        }
    }

}

