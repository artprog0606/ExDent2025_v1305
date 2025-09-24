using System.Web;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Core.Services.Landing
{
    public static class LpConstants
    {
        public const string EVENT_LANDING_VIEWPAGE = "landing_view";
        public const string EVENT_LANDING_GOAL = "landing_goal";

        public static double LpCacheTime = (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled) || Trial.TrialService.IsTrialEnabled  ? 0 : 30;
        public static double LpBlocksCacheTime = (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled) || Trial.TrialService.IsTrialEnabled ? 0 : 30;

        public const string LandingCachePrefix = "LandingPage_";
        
        public const string LpCachePrefix = LandingCachePrefix + "lp_"; // + lpId
        public const string LpBlockConfigCachePrefix = LandingCachePrefix + "blockconfig_";
        public const string LpTemplatesCachePrefix = LandingCachePrefix + "templates_"; // + template name

        public const string BlockCachePrefix = LandingCachePrefix + "block_";         // + blockId
        public const string BlockListCachePrefix = LandingCachePrefix + "blocklist_"; // + lpId

        public const string SubBlockCachePrefix = LandingCachePrefix + "subblock_"; // + Id

        public static string BlockRenderedCachePrefix
        {
            get { return LandingCachePrefix + "blockrendered_{0}_{1}_{2}_{3}" + GetPrefixDomain; }
        }

        public static string SubBlockRenderedCachePrefix
        {
            get { return LandingCachePrefix + "subblockrendered_{0}_{1}_{2}_{3}" + GetPrefixDomain; }
        }

        public static string SubBlockRendered2CachePrefix
        {
            get { return LandingCachePrefix + "subblockrendered2_{0}_{1}_{2}_{3}" + GetPrefixDomain; }
        }

        public static string GetPrefixDomain
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                    return null;

                return (UrlService.IsSecureConnection(context.Request) ? "https" : "") + context.Request.Url.Host;
            }
        }


    }
}
