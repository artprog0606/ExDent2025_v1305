using System.Web.Mvc;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Admin.Extensions
{
    internal static class UrlExtensions
    {
        public static string AbsoluteClientRouteUrl(this UrlHelper url, string routeName, object routeValues = null) =>
            GetClientRouteUrl(url, url.RouteUrl(routeName, routeValues), true);
        
        public static string AbsoluteClientRouteUrl(this UrlHelper url, object routeValues = null) =>
            GetClientRouteUrl(url, url.RouteUrl(routeValues), true);
        
        public static string RelativeClientRouteUrl(this UrlHelper url, string routeName, object routeValues = null) =>
            GetClientRouteUrl(url, url.RouteUrl(routeName, routeValues), false);
        
        public static string RelativeClientRouteUrl(this UrlHelper url, object routeValues = null) =>
            GetClientRouteUrl(url, url.RouteUrl(routeValues), false);
        
        private static string GetClientRouteUrl(UrlHelper url, string routeUrl, bool isAbsolute)
        {
            var applicationPath = url.RequestContext?.HttpContext?.Request?.ApplicationPath;
            
            if (!string.IsNullOrWhiteSpace(routeUrl) 
                && !string.IsNullOrWhiteSpace(applicationPath) 
                && !applicationPath.Equals("/"))
                routeUrl = routeUrl.Replace(applicationPath, string.Empty);

            return isAbsolute 
                ? $"{SettingsMain.SiteUrl.TrimEnd('/')}/{routeUrl?.TrimStart('/')}" 
                : routeUrl;
        }
    }
}