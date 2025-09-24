using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Extensions
{
    public static partial class HtmlExtensions
    {
        public static MvcForm BeginAbsoluteUrlForm(this HtmlHelper htmlHelper, object routeValues)
        {
            return BeginAbsoluteUrlForm(htmlHelper, null, null, TypeHelper.ObjectToDictionary(routeValues),
                FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValues)
        {
            return BeginAbsoluteUrlForm(htmlHelper, null, null, routeValues, FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(),
                FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            object routeValues)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName,
                TypeHelper.ObjectToDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, routeValues, FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            FormMethod method)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method,
                new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            object routeValues,
            FormMethod method)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName,
                TypeHelper.ObjectToDictionary(routeValues), method, new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues,
            FormMethod method)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, routeValues, method,
                new RouteValueDictionary());
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            FormMethod method,
            object htmlAttributes)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            return BeginAbsoluteUrlForm(htmlHelper, actionName, controllerName, new RouteValueDictionary(), method,
                htmlAttributes);
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            object routeValues,
            FormMethod method,
            object htmlAttributes)
        {
            return FormExtensions.BeginForm(htmlHelper, actionName, controllerName,
                TypeHelper.ObjectToDictionary(routeValues), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginAbsoluteUrlForm(
            this HtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues,
            FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            var url = UrlService.GenerateBaseUrl(appendApplicationPath: false) + UrlHelper.GenerateUrl(null, actionName,
                controllerName, routeValues,
                htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);

            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("action", url);
            tagBuilder.MergeAttribute(nameof(method), HtmlHelper.GetFormMethodString(method), true);
            var flag = htmlHelper.ViewContext.ClientValidationEnabled &&
                       !htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled;
            if (flag)
                tagBuilder.GenerateId(Guid.NewGuid().ToString());
            htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            var mvcForm = new MvcForm(htmlHelper.ViewContext);
            if (!flag)
                return mvcForm;
            htmlHelper.ViewContext.FormContext.FormId = tagBuilder.Attributes["id"];
            return mvcForm;
        }
    }
}