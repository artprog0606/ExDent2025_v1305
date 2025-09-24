using System;
using System.Web;
using AdvantShop.Core.UrlRewriter;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Infrastructure.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteRouteUrl(this UrlHelper url, string routeName, object routeValues = null) =>
            UrlService.GenerateBaseUrl(appendApplicationPath: false).TrimEnd('/') + "/" + url.RouteUrl(routeName, routeValues)?.TrimStart('/');

        public static string AbsoluteRouteUrl(this UrlHelper url, object routeValues = null) =>
            UrlService.GenerateBaseUrl(appendApplicationPath: false).TrimEnd('/') + "/" + url.RouteUrl(routeValues)?.TrimStart('/');

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName) =>
            AbsoluteActionUrl(url, actionName, null, null);

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName, object routeValues) =>
            AbsoluteActionUrl(url, actionName, null, routeValues);

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName,
            RouteValueDictionary routeValues) =>
            AbsoluteActionUrl(url, actionName, null, routeValues);

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName, string controllerName) =>
            AbsoluteActionUrl(url, actionName, controllerName, null);

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName, string controllerName,
            object routeValues)
        {
            var link = url.Action(actionName, controllerName, routeValues,
                UrlService.IsSecureConnection(url.RequestContext.HttpContext.Request) ? "https" : "http");

            if (link != null && SettingsMain.IsTechDomainsReady &&
                HttpContext.Current.Request.IsStrictForAdminTechDomain())
            {
                var uri = new Uri(link);

                link = SettingsMain.TechDomainAdminPanel + "/" + uri.PathAndQuery.TrimStart('/');
            }

            return link;
        }
    }
}