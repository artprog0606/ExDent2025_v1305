using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Design;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Infrastructure.Templates
{
    public class RazorThemeViewEngine : RazorViewEngine
    {
        // format is ":ViewCacheEntry:{prefix}:{name}:{controllerName}:{areaName}:{themeName}:"
        private const string CacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";
        private const string CacheKeyPrefixMaster = "Master";
        private const string CacheKeyPrefixPartial = "Partial";
        private const string CacheKeyPrefixView = "View";
        private static readonly string[] _emptyLocations = new string[0];

        public RazorThemeViewEngine() : base()
        {
            base.MasterLocationFormats = new[]
            {
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Views/{1}/_Layout.cshtml",
                "~/Areas/{2}/Views/Shared/_Layout.cshtml",

                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
            base.ViewLocationFormats = new[]
            {
                "~/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",

                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
            base.PartialViewLocationFormats = new[]
            {
                "~/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Templates/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{2}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml",

                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
            };
            base.AreaMasterLocationFormats = new[]
            {
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            base.AreaViewLocationFormats = new[]
            {
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            base.AreaPartialViewLocationFormats = new[]
            {
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Templates/{4}/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Templates/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Templates/{3}/Areas/{2}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Templates/{3}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Templates/{3}/Views/Shared/{0}.cshtml",

                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            base.FileExtensions = new[] {"cshtml"};
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            
            if (String.IsNullOrEmpty(viewName))
                throw new ArgumentException("viewName must be specified.", "viewName");

            var themeName = GetThemeToUse();
            var mobileTemplate = "";
            var areaName = (GetAreaName(controllerContext.RouteData) ?? "").ToLower();

            if (string.IsNullOrEmpty(masterName))
            {
                var layout = GetMasterLayout(controllerContext.RouteData);
                if (!string.IsNullOrEmpty(layout))
                    masterName = layout;
            }

            if (SettingsDesign.IsMobileTemplate && areaName != "adminv3" && areaName != "adminv2")
            {
                if (masterName != "_LayoutEmpty")
                    masterName = "_Layout";

                if (!string.IsNullOrEmpty(SettingsDesign.MobileTemplate))
                {
                    mobileTemplate = themeName;
                    themeName = SettingsDesign.MobileTemplate;
                }
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            var requiredString = controllerContext.RouteData.GetRequiredString("controller");

            var viewPath = GetPath(controllerContext, ViewLocationFormats, AreaViewLocationFormats, viewName, themeName, mobileTemplate, requiredString, CacheKeyPrefixView, useCache, out viewLocationsSearched, areaName); // "ViewLocationFormats"
            var masterPath = GetPath(controllerContext, MasterLocationFormats, AreaMasterLocationFormats, masterName, themeName, mobileTemplate, requiredString, CacheKeyPrefixMaster, useCache, out masterLocationsSearched, areaName);  // "MasterLocationFormats"
            
            if (!String.IsNullOrEmpty(viewPath) && (!String.IsNullOrEmpty(masterPath) || String.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
            }

            if (masterLocationsSearched == null)
                return new ViewEngineResult(viewLocationsSearched);

            return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            
            if (String.IsNullOrEmpty(partialViewName))
                throw new ArgumentException("partialViewName must be specified.", "partialViewName");
            
            var themeName = GetThemeToUse();
            var mobileTemplate = "";

            if (SettingsDesign.IsMobileTemplate && !string.IsNullOrEmpty(SettingsDesign.MobileTemplate))
            {
                mobileTemplate = themeName;
                themeName = SettingsDesign.MobileTemplate;
            }

            string[] searched;
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");
            var areaName = (GetAreaName(controllerContext.RouteData) ?? "").ToLower();

            var partialPath = GetPath(controllerContext, PartialViewLocationFormats, AreaPartialViewLocationFormats, partialViewName, themeName, mobileTemplate, controllerName, CacheKeyPrefixPartial, useCache, out searched, areaName); // "PartialViewLocationFormats"

            if (String.IsNullOrEmpty(partialPath))
                return new ViewEngineResult(searched);

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }
        
        private static string GetThemeToUse()
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
                return "";

            return SettingsDesign.Template != TemplateService.DefaultTemplateId ? SettingsDesign.Template : "";
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations,
                                string name, string themeName, string mobileTemplate, string controllerName, string cacheKeyPrefix, bool useCache,
                                out string[] searchedLocations, string areaName)
        {
            searchedLocations = _emptyLocations;

            if (string.IsNullOrEmpty(name))
                return string.Empty;

            List<ViewLocation> viewLocations = null;
            
            var usingAreas = !String.IsNullOrEmpty(areaName);
            if (usingAreas)
            {
                areaName = areaName.ToLower().Replace("adminv2", "admin");

                if (areaName == "adminv3")
                {
                    var areaLocations2 = new List<string>();

                    if (areaLocations == AreaMasterLocationFormats)
                    {
                        areaLocations2 = new List<string>()
                        {
                            "~/Areas/Admin/Templates/{2}/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Templates/{2}/Views/Shared/{0}.cshtml",

                            "~/Areas/Admin/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Views/Shared/{0}.cshtml",
                        };
                    }
                    else if (areaLocations == AreaViewLocationFormats)
                    {
                        areaLocations2 = new List<string>()
                        {
                            "~/Areas/Admin/Templates/{2}/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Templates/{2}/Views/Shared/{0}.cshtml",

                            "~/Areas/Admin/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Views/Shared/{0}.cshtml",
                        };
                    }
                    else if (areaLocations == AreaPartialViewLocationFormats)
                    {
                        areaLocations2 = new List<string>()
                        {
                            "~/Areas/Admin/Templates/{2}/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Templates/{2}/Views/Shared/{0}.cshtml",

                            "~/Areas/Admin/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Views/Shared/{0}.cshtml",
                        };
                    }

                    if (SettingsDesign.IsMobileTemplate)
                    {
                        areaLocations2.InsertRange(0, new List<string>()
                        {
                            "~/Areas/Admin/Templates/Mobile/Views/{1}/{0}.cshtml",
                            "~/Areas/Admin/Templates/Mobile/Views/Shared/{0}.cshtml",
                        });
                        mobileTemplate = "mobile";
                    }

                    viewLocations = GetViewLocations(locations, areaLocations2.ToArray());
                }
            }
            else if (SettingsDesign.IsMobileTemplate)
            {
                areaName = "mobile";
                usingAreas = true;
            }

            if (viewLocations == null)
                viewLocations = GetViewLocations(locations, usingAreas ? areaLocations : null);

            if (viewLocations.Count == 0)
                throw new InvalidOperationException("Properties cannot be null or empty.");
            
            var nameRepresentsPath = IsSpecificPath(name);
            var cacheKey = CreateCacheKey(cacheKeyPrefix, name, nameRepresentsPath ? string.Empty : controllerName, areaName, themeName, mobileTemplate);

            if (useCache)
            {
                var cachedLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
                if (cachedLocation != null)
                {
                    return cachedLocation;
                }
            }

            return !nameRepresentsPath
                    ? GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, themeName, mobileTemplate, cacheKey, ref searchedLocations)
                    : GetPathFromSpecificName(controllerContext, name, themeName, mobileTemplate, cacheKey, ref searchedLocations);
        }

        private static string GetAreaName(RouteBase route)
        {
            var routeWithArea = route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;

            var castRoute = route as Route;
            if (castRoute != null && castRoute.DataTokens != null)
                return castRoute.DataTokens["area"] as string;

            return null;
        }

        private static string GetAreaName(RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
                return area as string;

            return GetAreaName(routeData.Route);
        }
        
        private static string GetMasterLayout(RouteData routeData)
        {
            object layout;
            if (routeData.DataTokens.TryGetValue("MasterLayout", out layout))
                return layout as string;

            return null;
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats)
        {
            var allLocations = new List<ViewLocation>();
            
            if (areaViewLocationFormats != null)
                foreach (string areaViewLocationFormat in areaViewLocationFormats)
                {
                    allLocations.Add(new AreaAwareViewLocation(areaViewLocationFormat));
                }

            if (viewLocationFormats != null)
                foreach (string viewLocationFormat in viewLocationFormats)
                {
                    allLocations.Add(new ViewLocation(viewLocationFormat));
                }

            return allLocations;
        }

        private static bool IsSpecificPath(string name)
        {
            var ch = name[0];
            return ch == '~' || ch == '/';
        }

        internal string CreateCacheKey(string prefix, string name, string controllerName, string areaName, string themeName, string mobileTemplate)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat, prefix, name, controllerName, areaName, themeName, mobileTemplate);
        }

        internal static string AppendDisplayModeToCacheKey(string cacheKey, string displayMode)
        {
            // key format is ":ViewCacheEntry:{cacheType}:{prefix}:{name}:{controllerName}:{areaName}:"
            // so append "{displayMode}:" to the key
            return cacheKey + displayMode + ":";
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name,
            string controllerName, string areaName, string themeName, string mobileTemplate, string cacheKey, ref string[] searchedLocations)
        {
            string result = String.Empty;
            searchedLocations = new string[locations.Count];

            for (int i = 0; i < locations.Count; i++)
            {
                ViewLocation location = locations[i];
                string virtualPath = location.Format(name, controllerName, areaName, themeName, mobileTemplate);

                //if(virtualPath.Contains("Templates") && areaName != null)
                //    virtualPath = virtualPath.Replace(areaName, themeName + "/Areas/" + areaName);

                if (FileExists(controllerContext, virtualPath))
                {
                    searchedLocations = _emptyLocations;
                    result = virtualPath;
                    this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
                    return result;
                }
                
                searchedLocations[i] = virtualPath;
            }

            return result;
        }


        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string themeName, string mobileTemplate,
                                                string cacheKey, ref string[] searchedLocations)
        {
            var virtualPath = name;

            if (!string.IsNullOrEmpty(themeName) && virtualPath.StartsWith("~/modules/", true, CultureInfo.InvariantCulture))
            {
                var modulesName = name.Replace("~/", "~/Templates/" + themeName + "/");

                if (FileExists(controllerContext, modulesName))
                {
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, modulesName);
                    return modulesName;
                }
            }

            if (!FileExists(controllerContext, name))
            {
                virtualPath = string.Empty;
                searchedLocations = new[] {name};
            }
            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }

        private class AreaAwareViewLocation : ViewLocation
        {
            public AreaAwareViewLocation(string virtualPathFormatString) : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string themeName, string mobileTemplate)
            {
                return String.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, areaName, themeName, mobileTemplate);
            }
        }

        private class ViewLocation
        {
            protected string _virtualPathFormatString;

            public ViewLocation(string virtualPathFormatString)
            {
                _virtualPathFormatString = virtualPathFormatString;
            }

            public virtual string Format(string viewName, string controllerName, string areaName, string themeName, string mobileTemplate)
            {
                return String.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, themeName, mobileTemplate);
            }

            public string Test()
            {
                return _virtualPathFormatString;
            }
        }
    }
}