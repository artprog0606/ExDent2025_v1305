using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Configuration;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Localization;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Core;
using AdvantShop.Core.Common;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Infrastructure.Controllers
{
    public abstract partial class BaseController : Controller
    {
        /// <summary>Redirects to the specified action using the action name.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        protected new RedirectToRouteResult RedirectToAction(string actionName)
        {
            return RedirectToAction(actionName, (RouteValueDictionary)null);
        }

        /// <summary>Redirects to the specified action using the action name and route values.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">The parameters for a route.</param>
        protected new RedirectToRouteResult RedirectToAction(string actionName, object routeValues)
        {
            return RedirectToAction(actionName, TypeHelper.ObjectToDictionary(routeValues));
        }

        /// <summary>Redirects to the specified action using the action name and route dictionary.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">The parameters for a route.</param>
        protected new RedirectToRouteResult RedirectToAction(
            string actionName,
            RouteValueDictionary routeValues)
        {
            return RedirectToAction(actionName, null, routeValues);
        }

        /// <summary>Redirects to the specified action using the action name and controller name.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        protected new RedirectToRouteResult RedirectToAction(
            string actionName,
            string controllerName)
        {
            return RedirectToAction(actionName, controllerName, null);
        }

        /// <summary>Redirects to the specified action using the action name, controller name, and route dictionary.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The parameters for a route.</param>
        protected new RedirectToRouteResult RedirectToAction(
            string actionName,
            string controllerName,
            object routeValues)
        {
            return RedirectToAction(actionName, controllerName, TypeHelper.ObjectToDictionary(routeValues));
        }

        /// <summary>Redirects to the specified action using the action name, controller name, and route values.</summary>
        /// <returns>The redirect result object.</returns>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The parameters for a route.</param>
        protected new virtual RedirectToRouteResult RedirectToAction(
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues)
        {
            var routeValueDictionary = RouteData != null
                ? MergeRouteValues(actionName, controllerName, RouteData.Values, routeValues,
                    true)
                : MergeRouteValues(actionName, controllerName, null,
                    routeValues, true);

            ExecuteResult(ControllerContext, null, routeValueDictionary, false);
            return null;
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, controller name, and route values.</returns>
        /// <param name="actionName">The action name.</param>
        protected new RedirectToRouteResult RedirectToActionPermanent(string actionName)
        {
            return RedirectToActionPermanent(actionName, (RouteValueDictionary)null);
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and route values.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and route values.</returns>
        /// <param name="actionName">The action name.</param>
        /// <param name="routeValues">The route values.</param>
        protected new RedirectToRouteResult RedirectToActionPermanent(
            string actionName,
            object routeValues)
        {
            return RedirectToActionPermanent(actionName, TypeHelper.ObjectToDictionary(routeValues));
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and route values.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and route values.</returns>
        /// <param name="actionName">The action name.</param>
        /// <param name="routeValues">The route values.</param>
        protected new RedirectToRouteResult RedirectToActionPermanent(
            string actionName,
            RouteValueDictionary routeValues)
        {
            return RedirectToActionPermanent(actionName, null, routeValues);
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and controller name.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, and controller name.</returns>
        /// <param name="actionName">The action name.</param>
        /// <param name="controllerName">The controller name.</param>
        protected new RedirectToRouteResult RedirectToActionPermanent(
            string actionName,
            string controllerName)
        {
            return RedirectToActionPermanent(actionName, controllerName, null);
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, controller name, and route values.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, controller name, and route values.</returns>
        /// <param name="actionName">The action name.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="routeValues">The route values.</param>
        protected new RedirectToRouteResult RedirectToActionPermanent(
            string actionName,
            string controllerName,
            object routeValues)
        {
            return RedirectToActionPermanent(actionName, controllerName,
                TypeHelper.ObjectToDictionary(routeValues));
        }

        /// <summary>Returns an instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, controller name, and route values.</summary>
        /// <returns>An instance of the <see cref="T:System.Web.Mvc.RedirectResult" /> class with the Permanent property set to true using the specified action name, controller name, and route values.</returns>
        /// <param name="actionName">The action name.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="routeValues">The route values.</param>
        protected new virtual RedirectToRouteResult RedirectToActionPermanent(
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues)
        {
            var values = RouteData?.Values;

            ExecuteResult(ControllerContext, null,
                MergeRouteValues(actionName, controllerName, values, routeValues, true), true);

            return null;
        }

        private static RouteValueDictionary MergeRouteValues(
            string actionName,
            string controllerName,
            RouteValueDictionary implicitRouteValues,
            RouteValueDictionary routeValues,
            bool includeImplicitMvcValues)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (includeImplicitMvcValues)
            {
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("action", out var obj))
                    routeValueDictionary["action"] = obj;
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("controller", out obj))
                    routeValueDictionary["controller"] = obj;
            }

            if (routeValues != null)
            {
                foreach (var routeValue in GetRouteValues(routeValues))
                    routeValueDictionary[routeValue.Key] = routeValue.Value;
            }

            if (actionName != null)
                routeValueDictionary["action"] = actionName;
            if (controllerName != null)
                routeValueDictionary["controller"] = controllerName;
            return routeValueDictionary;
        }

        private static RouteValueDictionary
            GetRouteValues(RouteValueDictionary routeValues)
        {
            return routeValues == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(routeValues);
        }

        private static void ExecuteResult(
            ControllerContext context,
            string routeName,
            RouteValueDictionary routeValues,
            bool permanent)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.IsChildAction)
                throw new InvalidOperationException();
            var url = UrlHelper.GenerateUrl(routeName, null, null, routeValues, RouteTable.Routes,
                context.RequestContext, false);
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException();
            context.Controller.TempData.Keep();
            
            var uri = new Uri(new Uri(UrlService.GenerateBaseUrl(appendApplicationPath: false).TrimEnd('/') + "/"), url.TrimStart('/'));
            if (permanent)
                context.HttpContext.Response.RedirectPermanent(uri.ToString(), false);
            else
                context.HttpContext.Response.Redirect(uri.ToString(), false);
        }
    }
}