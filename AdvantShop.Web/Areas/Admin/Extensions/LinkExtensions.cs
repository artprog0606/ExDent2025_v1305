using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Admin.Extensions
{
    public static class LinkExtensions
    {
        public static MvcHtmlString
            AbsoluteActionLink(this HtmlHelper htmlHelper, string linkText, string actionName) =>
            AbsoluteActionLink(htmlHelper, linkText, actionName, null, null, null);

        public static MvcHtmlString AbsoluteActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName) =>
            AbsoluteActionLink(htmlHelper, linkText, actionName, controllerName, null, null);

        public static MvcHtmlString AbsoluteActionLink(
            this HtmlHelper htmlHelper,
            string linkText,
            string actionName,
            string controllerName,
            object routeValues,
            object htmlAttributes)
        {
            var url = UrlHelper.GenerateUrl(null, actionName, controllerName, ObjectToDictionary(routeValues),
                RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
            var tagBuilder = new TagBuilder("a")
            {
                InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty
            };
            tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            var uri = new Uri(new Uri(UrlService.GenerateBaseUrl(appendApplicationPath: false).TrimEnd('/') + "/"),
                url.TrimStart('/'));
            tagBuilder.MergeAttribute("href", uri.ToString());
            var s = tagBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(s);
        }

        private static RouteValueDictionary ObjectToDictionary(object value)
        {
            var dictionary = new RouteValueDictionary();
            if (value == null) return dictionary;
            foreach (var property in PropertyHelper.GetProperties(value))
                dictionary.Add(property.Name, property.GetValue(value));
            return dictionary;
        }
    }
}