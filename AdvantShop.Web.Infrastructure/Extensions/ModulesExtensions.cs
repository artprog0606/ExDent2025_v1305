using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace AdvantShop.Web.Infrastructure.Extensions
{
    public static class ModulesExtensions
    {
        public static List<ModuleRoute> GetModuleRoutes(string key, object routeValues = null, string area = "")
        {
            var model = new List<ModuleRoute>();

            var routeValueDictionary = new RouteValueDictionary();
            if (routeValues != null)
                routeValueDictionary = TypeHelper.ObjectToDictionary(routeValues);

            if (!routeValueDictionary.ContainsKey("area"))
                routeValueDictionary.Add("area", "");
            
            var modules = AttachedModules.GetModuleInstances<IRenderModuleByKey>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var moduleRoutes = module.GetModuleRoutes();
                    if (moduleRoutes == null) 
                        continue;
                    
                    foreach (var moduleRoute in moduleRoutes.Where(m => m.Key == key))
                    {
                        if (moduleRoute.RouteValues == null)
                            moduleRoute.RouteValues = new RouteValueDictionary();

                        foreach (var routeValue in routeValueDictionary)
                        {
                            moduleRoute.RouteValues.Add(routeValue.Key, routeValue.Value);
                        }

                        model.Add(moduleRoute);
                    }
                }
            }

            return model;
        }


        public static MvcHtmlString RenderModules(this HtmlHelper helper, string key, object routeValues = null)
        {
            if (DebugMode.IsDebugMode(eDebugMode.Modules))
                return new MvcHtmlString("");

            return helper.Partial("_Module", GetModuleRoutes(key, routeValues));
        }

        public static MvcHtmlString RenderMarketplaceButtons(this HtmlHelper helper, int productId, int? offerId = null, bool isAdminPart = false)
        {
            if (DebugMode.IsDebugMode(eDebugMode.Modules))
                return new MvcHtmlString(string.Empty);
            var model = new List<ModuleRoute>();
            foreach (var module in AttachedModules.GetModules<IMarketplaceModule>())
            {
                var instance = (IMarketplaceModule)Activator.CreateInstance(module);
                var moduleRoute = instance.GetProductButtonRoute(productId, offerId, isAdminPart);
                if (moduleRoute == null)
                    continue;

                if (moduleRoute.RouteValues == null)
                    moduleRoute.RouteValues = new RouteValueDictionary();
                moduleRoute.RouteValues.Add("area", string.Empty);

                model.Add(moduleRoute);
            }

            return helper.Partial("_Module", model);
        }

        public static string GetModuleVersion(this HtmlHelper helper, string moduleId)
        {
            var version = "";

            var module = ModulesRepository.GetModulesFromDb().FirstOrDefault(x => x.StringId == moduleId);
            if (module != null)
                version = module.Version;

            if (string.IsNullOrEmpty(version) || version.TryParseFloat() == 0)
                version = "rnd=" + new Random().Next(1000);

            return version;
        }

        public static IHtmlString GetModuleVersionHtmlString(this HtmlHelper helper, string moduleId)
        {
            return new HtmlString(GetModuleVersion(helper, moduleId));
        }
    }
}