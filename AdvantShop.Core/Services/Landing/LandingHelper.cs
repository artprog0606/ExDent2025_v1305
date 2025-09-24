using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Landing
{
    public static class LandingHelper
    {
        private const string CacheKey = "LandingPage_lp_domains";
        private const string PageCacheKey = "LandingPage_lp_domains_page_";

        private static List<string> excludeUrls = new List<string>() { "lp/", "landing", "areas/landing/", "product/", "productext/",  "common/", "commonext/", "cart/",
                                                                       "checkout/", "pay/", "coupon/", "reviews/", "location/", "printorder/", "bonuses/", "catalog/",
                                                                       "myaccount/", "user/", "signalr/", ".well-known/",
                                                                       "paymentnotification/", "paymentreturnurl/", "paymentreceipt/", "fail",
                                                                       "zadarma/", "yandextelephony/", "sipuni/", "mangoadvantshop/", "webhook/", "api/",
                                                                       "registration", "forgotpassword", "moduleroute/", "logout", "logogenerator/", "preorder/lp/",
                                                                       "module/", "cancel", "buy/", "errorext/"
                                                                     };

        public static bool IsLandingUrl(HttpApplication app, Uri uri, string extention, out string rewriteUrl)
        {
            rewriteUrl = null;

            if (SaasDataService.IsSaasEnabled && !SaasDataService.IsEnabledFeature(ESaasProperty.HaveLandingFunnel))
                return false;

            if (!string.IsNullOrEmpty(extention))
                return false;

            var landingSiteId = 0;

            var domains = GetLandingDomains();
            if (domains.Count != 0)
            {
                var host = uri.Host;

                var domain =
                    domains.FirstOrDefault(x =>
                        host.Equals(x.DomainUrl, StringComparison.OrdinalIgnoreCase)
                        || host == StringHelper.ToPuny(x.DomainUrl));

                if (domain == null)
                    return false;

                landingSiteId = domain.LandingSiteId;
            }
            else if (domains.Count == 0 && !SettingsMain.StoreActive && !app.Request.IsAdminTechDomain())
            {
                landingSiteId = GetFirstLandingSiteId();
            }

            if (landingSiteId == 0)
                return false;

            var pageUrl = uri.AbsolutePath.ToLower();
            if (!string.IsNullOrWhiteSpace(app.Request.ApplicationPath) && app.Request.ApplicationPath != "/") 
                pageUrl = pageUrl.Replace(app.Request.ApplicationPath.ToLower(), "");
            pageUrl = pageUrl.Trim('/');

            if (excludeUrls.Any(x => pageUrl.StartsWith(x)))
                return false;

            if (pageUrl.StartsWith("admin"))
            {
                return false;
            }

            int lpId;
            if (!string.IsNullOrEmpty(pageUrl))
            {
                lpId = GetLpPageId(landingSiteId, pageUrl);
                if (lpId == 0)
                {
                    rewriteUrl = "~/landing/landing/error404page";
                    return true;
                }
            }
            else
            {
                lpId = GetMainLpPageId(landingSiteId);
            }

            rewriteUrl = "~/landing/landing?landingId=" + lpId +
                         (!string.IsNullOrEmpty(uri.Query) ? "&" + uri.Query.TrimStart('?') : "");

            return true;
        }

        public static bool IsLandingDomain(Uri uri, out int lpSiteId)
        {
            lpSiteId = 0;
            
            var domains = GetLandingDomains();
            if (domains.Count == 0)
                return false;

            var host = uri.Host; //.Replace("www.", "");

            var domain = domains.FirstOrDefault(x => host.Equals(x.DomainUrl, StringComparison.OrdinalIgnoreCase)  || host == StringHelper.ToPuny(x.DomainUrl));
            if (domain == null)
                return false;

            lpSiteId = domain.LandingSiteId;

            return true;
        }

        private static List<LpDomain> GetLandingDomains()
        {
            return CacheManager.Get(CacheKey, 6*60, () => SQLDataAccess.Query<LpDomain>("Select * From [CMS].[LandingDomain]").ToList());
        }

        private static int GetLpPageId(int siteId, string pageUrl)
        {
            return CacheManager.Get(PageCacheKey + siteId + "_" + pageUrl, 6*60,
                () =>
                    SQLDataAccess.Query<int>(
                        "Select top(1) Id From [CMS].[Landing] Where LandingSiteId=@siteId and Url=@pageUrl",
                        new {siteId, pageUrl})
                        .FirstOrDefault());
        }

        private static int GetMainLpPageId(int siteId)
        {
            return CacheManager.Get(PageCacheKey + siteId + "ismain", 6 * 60,
                () =>
                    SQLDataAccess.Query<int>(
                        "Select top(1) Id From [CMS].[Landing] Where LandingSiteId=@siteId and IsMain=1",
                        new { siteId })
                        .FirstOrDefault());
        }

        private static int GetFirstLandingSiteId()
        {
            return CacheManager.Get(PageCacheKey + "first", 6 * 60,
                () =>
                    SQLDataAccess
                        .Query<int>("Select top(1) Id From [CMS].[LandingSite] Where [Enabled] = 1")
                        .FirstOrDefault());
        }

        public static string LandingRedirectUrl
        {
            get { return HttpContext.Current != null ? HttpContext.Current.Items["LandingRedirectUrl"] as string : null; }
            set { HttpContext.Current.Items["LandingRedirectUrl"] = value; }
        }
    }
}
