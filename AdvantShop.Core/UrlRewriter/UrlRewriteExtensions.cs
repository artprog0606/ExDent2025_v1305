using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.MobileApp;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.UrlRewriter
{
    public static class UrlRewriteExtensions
    {
        private const string _header = "If-Modified-Since";

        private static readonly List<string> AlowedUrlsForTechDomainClosed = new List<string>()
        {
            "/adminv2/",
            "/adminv3/",
            "/areas/admin/",
            "/combine/",
            "/fonts/",
            "/modules/",
            "/vendors/",
            "/adv-admin.aspx"
        };

        public static readonly List<string> AlowedExtensionsForTechDomainClosed = new List<string>() { ".csv", ".yml", ".xml", ".ashx" };

        public const string TechnicalHeaderName = "letitbe";

        public static void StaticFile304(this HttpApplication app)
        {
            if (!app.Request.Url.AbsolutePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return;

            var lastString = app.Request.Headers[_header];
            if (string.IsNullOrWhiteSpace(lastString)) return;

            var fileName = app.Request.PhysicalPath;
            
            var lastModified = File.GetLastWriteTime(fileName);
            var ifModifiedSince = lastString.TryParseDateTimeGMT();

            if (!ifModifiedSince.HasValue)
            {
                app.Request.Headers.Remove(_header);
            }
            if (ifModifiedSince.HasValue && (ifModifiedSince.Value >= lastModified))
            {
                app.Response.StatusCode = 304;
                app.Response.SuppressContent = true;
                return;
            }
            app.Response.Cache.SetLastModified(lastModified);
        }

        public static DateTime? TryParseDateTimeGMT(this string val)
        {
            DateTime intval;
            if (DateTime.TryParseExact(val, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out intval))
            {
                return intval.ToLocalTime();
            }
            return null;
        }

        public static void RewriteTo404(this HttpApplication app)
        {
            try
            {
                app.Context.Response.Clear();
                app.Context.Response.TrySkipIisCustomErrors = true;
                app.Context.Response.StatusCode = 404;
                app.Context.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(404);
                app.Context.Response.End();
            }
            catch
            {
            }
        }
        
        public static void RewriteTo403(this HttpApplication app, string statusDescription = null)
        {
            try
            {
                app.Context.Response.Clear();
                app.Context.Response.TrySkipIisCustomErrors = true;
                app.Context.Response.StatusCode = 403;
                if (statusDescription.IsNotEmpty()) 
                {
                    app.Context.Response.StatusDescription = statusDescription;
                    app.Context.Response.Write(statusDescription);
                }
                app.Context.Response.End();
            }
            catch
            {
            }
        }

        public static Uri GetUrlReferrer(this HttpRequest request)
        {
            try
            {
                return request.UrlReferrer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static Uri GetUrlReferrer(this HttpRequestBase request)
        {
            try
            {
                return request.UrlReferrer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Is Chrome-Lighthouse useragent for PageSpeed Insights
        /// </summary>
        public static bool IsLighthouse(this HttpRequest request)
        {
            return !string.IsNullOrEmpty(request.UserAgent) && 
                   (request.UserAgent.Contains("Chrome-Lighthouse") ||
                    request.UserAgent.Contains("PTST"));
        }

        /// <summary>
        /// Is Chrome-Lighthouse useragent for PageSpeed Insights
        /// </summary>
        public static bool IsLighthouse(this HttpRequestBase request)
        {
            return !string.IsNullOrEmpty(request.UserAgent) &&
                   (request.UserAgent.Contains("Chrome-Lighthouse") ||
                    request.UserAgent.Contains("PTST"));
        }
        
        public static bool IsMobileApp(this HttpRequest request)
        {
            if (!string.IsNullOrEmpty(request.UserAgent) && request.UserAgent.Contains("Dart/"))
                return true;
            
            var cookie = CommonHelper.GetCookie(MobileAppConst.CookieName);
            if (cookie != null && cookie.Value == "true")
                return true;
            
            return false;
        }
        
        public static bool IsMobileApp(this HttpRequestBase request)
        {
            if (!string.IsNullOrEmpty(request.UserAgent) && request.UserAgent.Contains("Dart/"))
                return true;
            
            var cookie = CommonHelper.GetCookie(MobileAppConst.CookieName);
            if (cookie != null && cookie.Value == "true")
                return true;
            
            return false;
        }

        public static bool IsTechnicalHeader(this HttpRequest request)
        {
            var header = request.Headers[TechnicalHeaderName];
            return !string.IsNullOrEmpty(header) && header == SettingsLic.AdvId;
        }

        public static bool IsTechnicalHeader(this HttpRequestBase request)
        {
            var header = request.Headers[TechnicalHeaderName];
            return !string.IsNullOrEmpty(header) && header == SettingsLic.AdvId;
        }

        public static bool AllowedUrlForTechDomainClosed(this HttpRequest request)
        {
            var url = request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            var pathWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

            return AlowedUrlsForTechDomainClosed.Any(pathWhitoutQuery.Contains);
        }

        public static bool AllowedUrlForTechDomainClosed(this HttpRequestBase request)
        {
            var url = request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            var pathWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

            return AlowedUrlsForTechDomainClosed.Any(pathWhitoutQuery.Contains);
        }
        
        public static bool IsTechDomainClosed(this HttpRequest request, string path)
        {
            var isPicture = path.Contains("/pictures/");
            if (isPicture)
            {
                if (SaasDataService.IsSaasEnabled)
                    return false;

                if (!TrialService.IsTrialEnabled)
                    return false;
                
                if (SettingsMain.IsTechDomainPicturesAllowed)
                    return false;
            }

            return request.IsTechDomainClosed();
        }
        
        public static bool IsTechDomainClosed(this HttpRequest request)
        {
            return (TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled) && 
                   SettingsMain.IsTechDomain(request.Url) &&
                   !CustomerContext.CurrentCustomer.IsAdmin && !CustomerContext.CurrentCustomer.IsModerator &&
                   !CustomerService.IsTechDomainGuest() &&
                   !request.AllowedUrlForTechDomainClosed() &&
                   !request.IsTechnicalHeader() &&
                   !request.IsLighthouse() &&
                   !request.IsMobileApp();
        }

        public static bool IsTechDomainClosed(this HttpRequestBase request)
        {
            return (TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled)
                   && SettingsMain.IsTechDomain(request.Url) &&
                   !CustomerContext.CurrentCustomer.IsAdmin && !CustomerContext.CurrentCustomer.IsModerator && !CustomerService.IsTechDomainGuest() &&
                   !request.AllowedUrlForTechDomainClosed() &&
                   !request.IsTechnicalHeader() &&
                   !request.IsLighthouse() && 
                   !request.IsMobileApp();
        }

        public static bool IsAppBlock(this HttpRequest request)
        {
            if (!IsAppBlock())
                return false;

            if (BrowsersHelper.IsBot() && !request.RawUrl.StartsWith("/admin"))
                return false;

            return true;
        }
        
        public static bool IsAppBlock()
        {
            return File.Exists(HostingEnvironment.MapPath("~/app_block.htm"));
        }

        public static bool IsMobileAppNotAvailable(this HttpRequestBase request)
        {
            var isMobileApp = string.Equals(request.QueryString["utm_source"], "mobileApp",
                StringComparison.InvariantCultureIgnoreCase);
            return isMobileApp && SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveMobileApp;
        }
        
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private static readonly List<string> StrictForAdminTechDomainUrls = new List<string>
        {
            "/areas/admin/",
            "/adv-admin.aspx",
            "/admin",
            "/adminv2",
            "/adminv3",
            "/extra_admin.css",
        };
        
        private static readonly List<string> UrlForAnyTechDomain = new List<string>
        {
            "/account/",
            "/vendors/",
            "/dist/",
            "/userfiles/",
            "/signalr/",
            "/pictures/",
            "/modules/",
            "/module/",
            "/combine/",
            "/fonts/",
            "/lp/",
            "/areas/landing/",
            "/integration",
            "/TelegramWebHook/",
            "/facebookWebHook/",
            "/OKWebHook/",
            "/images/",
            "/BotDetectCaptcha.ashx",
            "/Templates/",
            "/common/",
            "/landinginplace/",
            "/location/",
            "/cart/",
            "/myaccount/",
            "/export/",
            "/content/",
            "/logogenerator/",
        };

        private static string ParseRequest(HttpRequest request)
        {
            var url = request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            return index > 0 ? url.Substring(0, index) : url;
        }
        
        public static bool IsStrictForAdminTechDomain(this HttpRequest request)
        {
            var pathWithoutQuery = ParseRequest(request);
            return StrictForAdminTechDomainUrls.Any(path => pathWithoutQuery.Contains(path, StringComparison.OrdinalIgnoreCase));
        }
        public static bool IsForAnyTechDomain(this HttpRequest request)
        {
            var pathWithoutQuery = ParseRequest(request);
            return UrlForAnyTechDomain.Any(path => pathWithoutQuery.Contains(path, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAdminTechDomain(this HttpRequest request)
        {
            if (SettingsMain.IsTechDomainsReady == false)
                return false;

            var currentDomain = request.Url.Host.ToLower();
            if (!string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]))
                currentDomain = request.Headers["X-Forwarded-Host"].ToLower();
            return currentDomain.Equals(new Uri(SettingsMain.TechDomainAdminPanel).Host, StringComparison.OrdinalIgnoreCase);
        }
            

        public static bool IsStoreTechDomain(this HttpRequest request)
        {
            if (SettingsMain.IsTechDomainsReady == false)
                return false;

            var currentDomain = request.Url.Host.ToLower();
            if (!string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]))
                currentDomain = request.Headers["X-Forwarded-Host"].ToLower();
            return currentDomain.Equals(new Uri(SettingsMain.TechDomainStore).Host, StringComparison.OrdinalIgnoreCase);
        }

        public static string BuildTechDomainUrl(Uri uri, string techDomain) => 
            $"{techDomain}/{uri.PathAndQuery.TrimStart('/')}";
    }
}
