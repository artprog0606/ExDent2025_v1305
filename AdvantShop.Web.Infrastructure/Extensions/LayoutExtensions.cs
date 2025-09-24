using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Controllers;
using Newtonsoft.Json;
using static AdvantShop.Core.AssetsTool;

namespace AdvantShop.Web.Infrastructure.Extensions
{
    public static class LayoutExtensions
    {
        private const string BundlesContextKey = "Page_Bundle_";

        #region Page title, keywords, description

        public static string Title
        {
            get { return HttpContext.Current.Items["Page_Title"] as string; }
            set { HttpContext.Current.Items["Page_Title"] = value; }
        }

        public static string H1
        {
            get { return HttpContext.Current.Items["Page_H1"] as string; }
            set { HttpContext.Current.Items["Page_H1"] = value; }
        }

        public static string MobileTitleText
        {
            get { return HttpContext.Current.Items["Page_TitleText"] as string; }
            set { HttpContext.Current.Items["Page_TitleText"] = value; }
        }

        public static NgControllers.NgControllersTypes NgController
        {
            get
            {
                return HttpContext.Current.Items.Contains("Page_NgController")
                    ? (NgControllers.NgControllersTypes)HttpContext.Current.Items["Page_NgController"]
                    : NgControllers.NgControllersTypes.AppCtrl;
            }
            set { HttpContext.Current.Items["Page_NgController"] = value; }
        }

        public static string Description
        {
            get { return HttpContext.Current.Items["Page_Description"] as string; }
            set { HttpContext.Current.Items["Page_Description"] = value; }
        }

        public static string Keywords
        {
            get { return HttpContext.Current.Items["Page_Keywords"] as string; }
            set { HttpContext.Current.Items["Page_Keywords"] = value; }
        }

        public static bool NoFollow
        {
            get { return Convert.ToBoolean(HttpContext.Current.Items["Page_NoFollow"]); }
            set { HttpContext.Current.Items["Page_NoFollow"] = value; }
        }

        public static bool NoIndex
        {
            get { return Convert.ToBoolean(HttpContext.Current.Items["Page_NoIndex"]); }
            set { HttpContext.Current.Items["Page_NoIndex"] = value; }
        }

        /// <summary>
        /// Current page in paging. Start from 1..n
        /// </summary>
        public static int CurrentPage
        {
            get { return Convert.ToInt32(HttpContext.Current.Items["Page_CurrentPage"]); }
            set { HttpContext.Current.Items["Page_CurrentPage"] = value; }
        }

        /// <summary>
        /// Total pages in paging
        /// </summary>
        public static int TotalPages
        {
            get { return Convert.ToInt32(HttpContext.Current.Items["Page_TotalPages"]); }
            set { HttpContext.Current.Items["Page_TotalPages"] = value; }
        }

        public static MetaType MetaType
        {
            get
            {
                MetaType metaType = MetaType.Default;
                var type = HttpContext.Current.Items["Page_MetaType"] as string;
                if (Enum.TryParse(type, false, out metaType))
                    return metaType;

                return MetaType.Default;
            }
            set { HttpContext.Current.Items["Page_MetaType"] = value; }
        }

        public static string NotifyMessages
        {
            get { return HttpContext.Current.Items["Page_NotifyMessages"] as string; }
            set { HttpContext.Current.Items["Page_NotifyMessages"] = value; }
        }

        public static IHtmlString GetPageTitle(this HtmlHelper helper)
        {
            return new HtmlString(Title.HtmlEncode());
        }

        public static IHtmlString GetPageH1(this HtmlHelper helper)
        {
            return new HtmlString(H1.HtmlEncode().TrimEnd("\\", StringComparison.Ordinal));
        }

        public static IHtmlString GetMobilePageTitleText(this HtmlHelper helper)
        {
            return new HtmlString(MobileTitleText.HtmlEncode());
        }

        public static IHtmlString GetNgController(this HtmlHelper helper)
        {
            return new HtmlString(NgControllers.GetNgControllerInitString(NgController));
        }

        public static IHtmlString GetPageDescription(this HtmlHelper helper, bool asMetaLink = false)
        {
            var description = Description.HtmlEncode();
            if (!asMetaLink)
                return new HtmlString(description);

            return description.IsNullOrEmpty()
                ? new HtmlString(null)
                : new HtmlString(string.Format("<meta name=\"Description\" content=\"{0}\" />", description));
        }

        public static IHtmlString GetPageKeywords(this HtmlHelper helper, bool asMetaLink = false)
        {
            var keywords = Keywords.HtmlEncode();
            if (!asMetaLink)
                return new HtmlString(keywords);

            return keywords.IsNullOrEmpty()
                ? new HtmlString(null)
                : new HtmlString(string.Format("<meta name=\"Keywords\" content=\"{0}\" />", keywords));
        }


        public static IHtmlString GetAdvId(this HtmlHelper helper)
        {
            return new HtmlString(SettingsLic.AdvId);
        }

        public static IHtmlString GetAdvTemplate(this HtmlHelper helper)
        {
            return new HtmlString((SettingsDesign.Template ?? "").ToLower());
        }

        public static IHtmlString GetCanonicalTag(this HtmlHelper helper)
        {
            if (HttpContext.Current != null && CurrentPage != 0 && TotalPages != 0)
            {
                var query = HttpContext.Current.Request.QueryString;
                if (query.Count == 0 || (query.Count == 1 && query["page"] != null) || (CurrentPage > 0 && TotalPages > 1))
                {
                    var tag = string.Format("<link rel=\"canonical\" href=\"{0}\" />", UrlService.GetCanonicalUrl(true));

                    var queryParams = new List<string>
                    {
                        query["brand"] != null ? "brand=" + query["brand"] : null,
                        query["indepth"] != null ? "indepth=" + query["indepth"] : null
                    }.Where(p => p.IsNotEmpty()).ToList();

                    if (CurrentPage >= 2)
                    {
                        var prevQueryParams = CurrentPage == 2
                            ? new List<string>(queryParams)
                            : new List<string> { "page=" + (CurrentPage - 1) }.Union(queryParams).ToList();
                        tag += string.Format("\n<link rel=\"prev\" href=\"{0}{1}\" />", UrlService.GetCanonicalUrl(),
                            prevQueryParams.Any() ? "?" + prevQueryParams.AggregateString("&") : "");
                    }

                    if (CurrentPage < TotalPages)
                    {
                        var nextQueryParams = new List<string> { "page=" + (CurrentPage + 1) }.Union(queryParams).ToList();
                        tag += string.Format("\n<link rel=\"next\" href=\"{0}{1}\" />",
                            UrlService.GetCanonicalUrl(),
                            nextQueryParams.Any() ? "?" + nextQueryParams.AggregateString("&") : "");
                    }

                    return new HtmlString(tag);
                }
            }

            return new HtmlString("<link rel=\"canonical\" href=\"" + UrlService.GetCanonicalUrl() + "\" />");
        }

        public static IHtmlString GetNoFollowTag(this HtmlHelper helper)
        {
            if (!NoIndex && !NoFollow)
                return new HtmlString(null);

            if (NoIndex && NoFollow)
                return new HtmlString("<meta name=\"robots\" content=\"noindex, nofollow\" />");

            return new HtmlString("<meta name=\"robots\" content=\"" + (NoIndex ? "noindex" : "nofollow") + "\" />");
        }

        public static MvcHtmlString GetNotifyMessages(this HtmlHelper helper)
        {
            List<Notify> list = null;

            var str = helper.ViewContext.TempData[BaseController.NotifyMessages] as string;
            if (!string.IsNullOrEmpty(str))
            {
                helper.ViewContext.TempData[BaseController.NotifyMessages] = string.Empty;
                list = JsonConvert.DeserializeObject<List<Notify>>(str);
            }

            return helper.Partial("~/Views/Shared/_Notify.cshtml", list ?? new List<Notify>());
        }

        public static MvcHtmlString GetNotifications(this HtmlHelper helper)
        {
            List<Notify> list = null;

            var str = helper.ViewContext.TempData[BaseController.Notifications] as string;
            if (!string.IsNullOrEmpty(str))
            {
                helper.ViewContext.TempData[BaseController.Notifications] = string.Empty;
                list = JsonConvert.DeserializeObject<List<Notify>>(str);
            }

            return helper.Partial("~/Areas/Admin/Views/Shared/_Notifications.cshtml", list ?? new List<Notify>());
        }

        #endregion

        #region Bundles

        public static void AddBundles(this HtmlHelper helper, List<string> list, string bundleName)
        {
            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string> ?? new List<string>();
            bundles.AddRange(list);

            HttpContext.Current.Items[BundlesContextKey + bundleName] = bundles;
        }

        public static void AddBundle(this HtmlHelper helper, string listItem, string bundleName)
        {
            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string> ?? new List<string>();
            bundles.Add(listItem);

            HttpContext.Current.Items[BundlesContextKey + bundleName] = bundles;
        }

        public static void RemoveBundle(this HtmlHelper helper, string name, string bundleName)
        {
            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string> ?? new List<string>();

            var b = bundles.Find(x => x == name);
            if (b != null)
                bundles.Remove(b);

            HttpContext.Current.Items[BundlesContextKey + bundleName] = bundles;
        }

        public static void ReplaceBundle(this HtmlHelper helper, string name, string newname, string bundleName)
        {
            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string> ?? new List<string>();

            var index = bundles.FindIndex(x => x == name);
            if (index != -1)
            {
                bundles.RemoveAt(index);
                bundles.Insert(index, newname);
            }
            else
            {
                bundles.Add(newname);
            }

            HttpContext.Current.Items[BundlesContextKey + bundleName] = bundles;
        }

        public static IHtmlString RenderCssBundle(this HtmlHelper helper, string bundleName, string outputFolder = null, bool inline = false)
        {

            if (DebugMode.IsDebugMode(eDebugMode.CriticalCss) && inline)
            {
                return new HtmlString("");
            }

            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string>;

            if (bundles == null || bundles.Count == 0)
                return new HtmlString("");

            return new HtmlString(JsCssTool.MiniCss(bundles, bundleName, outputFolder, inline));
        }

        public static IHtmlString RenderJsBundle(this HtmlHelper helper, string bundleName, string outputFolder = null)
        {
            var bundles = HttpContext.Current.Items[BundlesContextKey + bundleName] as List<string>;

            if (bundles == null || bundles.Count == 0)
                return new HtmlString("");

            return new HtmlString(JsCssTool.MiniJs(bundles, bundleName, outputFolder));
        }

        public static IHtmlString RenderModuleCssBundle(this HtmlHelper helper, string bundleName, bool inline = false)
        {
            var bundles = new List<string>();

            var modules = AttachedModules.GetModuleInstances<IModuleBundles>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var moduleBundles = module.GetCssBundles();
                    if (moduleBundles != null && moduleBundles.Count > 0)
                        bundles.AddRange(moduleBundles);
                }
            }

            if (bundles.Count == 0)
                return new HtmlString("");

            return new HtmlString(JsCssTool.MiniCss(bundles, bundleName, inline: inline));
        }

        public static IHtmlString RenderModuleJsBundle(this HtmlHelper helper, string bundleName)
        {
            var bundles = new List<string>();

            var modules = AttachedModules.GetModuleInstances<IModuleBundles>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var moduleBundles = module.GetJsBundles();
                    if (moduleBundles != null && moduleBundles.Count > 0)
                        bundles.AddRange(moduleBundles);
                }
            }

            if (bundles.Count == 0)
                return new HtmlString("");

            return new HtmlString(JsCssTool.MiniJs(bundles, bundleName));
        }

        #endregion

        #region Assets

        private static string GetCurrentArea(this HtmlHelper helper)
        {
            var areaName = GetAreaName(helper.ViewContext.RouteData);
            if (areaName.IsNullOrEmpty() && SettingsDesign.IsMobileTemplate)
                areaName = "Mobile";
            return areaName;
        }

        private static string GetAreaName(RouteData routeData)
        {
            object area;
            if (routeData.DataTokens.TryGetValue("area", out area))
                return area as string;
            var routeWithArea = routeData.Route as IRouteWithArea;
            if (routeWithArea != null)
                return routeWithArea.Area;
            var castRoute = routeData.Route as Route;
            if (castRoute != null && castRoute.DataTokens != null)
                return castRoute.DataTokens["area"] as string;
            return null;
        }

        public static void AddAsset(this HtmlHelper helper, string name, bool insertBegin = false)
        {
            var str = HttpContext.Current.Items["Page_Assets"] as string;
            var assetsList = str.IsNotEmpty() ? JsonConvert.DeserializeObject<List<string>>(str) : new List<string>() {};
            if (insertBegin)
            {
                assetsList.Insert(0, name);
            }
            else
            {
                assetsList.Add(name);
            }
            HttpContext.Current.Items["Page_Assets"] = JsonConvert.SerializeObject(assetsList);
        }

        public static void AddModuleAsset(this HtmlHelper helper, string moduleName, string name)
        {
            var str = HttpContext.Current.Items["Page_ModuleAssets"] as string;
            var assets = str.IsNotEmpty() ? JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(str) : new Dictionary<string, List<string>>();
            if (assets.ContainsKey(moduleName))
                assets[moduleName].Add(name);
            else
                assets.Add(moduleName, new List<string> { name });
            HttpContext.Current.Items["Page_ModuleAssets"] = JsonConvert.SerializeObject(assets);
        }

        private static List<string> CurrentAssets
        {
            get
            {
                var json = HttpContext.Current.Items["Page_Assets"] as string;
                return json.IsNotEmpty() ? JsonConvert.DeserializeObject<List<string>>(json) : new List<string>();
            }
        }

        private static Dictionary<string, List<string>> CurrentModuleAssets
        {
            get
            {
                var json = HttpContext.Current.Items["Page_ModuleAssets"] as string;
                return json.IsNotEmpty() ? JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) : new Dictionary<string, List<string>>();
            }
        }

        public static IHtmlString RenderAssets(this HtmlHelper helper, AssetFileType? renderOnlyType = null, bool stylesLazyLoad = true)
        {
            var assets = AssetsTool.GetAssets(helper.GetCurrentArea(), CurrentModuleAssets, CurrentAssets.ToArray());
            if (assets == null || !assets.Any())
                return null;

            bool lazyStyles = DebugMode.IsDebugMode(eDebugMode.CriticalCss) == false && stylesLazyLoad;
            var cssTags = new List<string>();
            var jsTags = new List<string>();
            bool renderLinks = renderOnlyType == null || renderOnlyType.Value == AssetFileType.Link;
            bool renderScripts = renderOnlyType == null || renderOnlyType.Value == AssetFileType.Script;
            foreach (var assetName in assets.Keys)
            {
                if (renderLinks && assets[assetName].Data != null && assets[assetName].Data.CssFiles != null)
                {
                    cssTags.AddRange(assets[assetName].Data.CssFiles
                        .Select(x => string.Format("<link data-html2canvas-ignore href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" {1}>", assets[assetName].GetUrl(x), lazyStyles ? "media=\"print\" onload=\"window._advantshopStylesLoadedMark()\"" : "")));
                }
                if (renderScripts && assets[assetName].Data != null && assets[assetName].Data.JsFiles != null)
                {
                    jsTags.AddRange(assets[assetName].Data.JsFiles
                        .Select(x => string.Format("<script src=\"{0}\"></script>", assets[assetName].GetUrl(x))));
                }
            }

            var cssTagsUnique = cssTags.Distinct();
            var cssTagsHtml = string.Join("\n", cssTagsUnique);

            if (lazyStyles && renderLinks)
            {
                cssTagsHtml = "<script>(function(){'use strict'; let _countLoaded=0, _countStyles=" + cssTagsUnique.Count() +
                              $@";document.documentElement._advantshopVariables = {{stylesLoaded: false}};
                                window.whenAdvantshopStylesLoaded = function () {{
                                    return new Promise(function (resolve, reject) {{
                                        if (document.documentElement._advantshopVariables.stylesLoaded) {{
                                            resolve()
                                        }}
                                        else {{
                                            document.addEventListener('advantshopStylesLoaded', (e) => {{
                                                resolve()
                                            }}, false)
                                        }}
                                    }})
                                }}
                                  window._advantshopStylesLoadedMark = function(){{ 
                                    _countLoaded += 1; 
                                    if (_countStyles === _countLoaded && document.documentElement._advantshopVariables.stylesLoaded === false) {{ 
                                        Array.from(document.getElementById('linksLazy').children).forEach(link => link.setAttribute('media', 'all'));
                                        document.documentElement._advantshopVariables.stylesLoaded = true;

                                        const styleLoaded = new CustomEvent('advantshopStylesLoaded');
                                        document.dispatchEvent(styleLoaded);
                                    }};
                                  }}"+
                              "})() </script>\n<div id=\"linksLazy\">\n" + cssTagsHtml + "\n</div>\n";
            }

            return new HtmlString(cssTagsHtml + string.Join("\n", jsTags.Distinct()));
        }

        public static IHtmlString RenderAssetFilesList(this HtmlHelper helper, string assetName)
        {
            return helper.RenderAssetFilesList(assetName, helper.GetCurrentArea());
        }

        public static IHtmlString RenderAssetFilesList(this HtmlHelper helper, string assetName, string areaName)
        {
            var assets = AssetsTool.GetAssets(areaName, CurrentModuleAssets, assetName);
            if (assets == null || !assets.Any())
                return null;

            var files = new List<string>();
            if (assets[assetName].Data != null && assets[assetName].Data.CssFiles != null)
                files.AddRange(assets[assetName].Data.CssFiles.Select(x => string.Format("'{0}'", assets[assetName].GetUrl(x))));

            if (assets[assetName].Data != null && assets[assetName].Data.JsFiles != null)
                files.AddRange(assets[assetName].Data.JsFiles.Select(x => string.Format("'{0}'", assets[assetName].GetUrl(x))));

            return new HtmlString(string.Join(",", files));
        }

        public static IHtmlString RenderAssetFilesListModule(this HtmlHelper helper, string moduleName, string moduleAssetName)
        {
            var modulePathData = new PathData(helper.GetCurrentArea(), moduleName);
            var files = new List<string>();
            
            if (modulePathData.Bundles != null)
            {
                var asset = modulePathData.Bundles.EntryPoints[moduleAssetName];

                if (asset == null)
                    return null;

                asset.SetPathData(modulePathData);
                
                if (asset.Data != null && asset.Data.CssFiles != null)
                    files.AddRange(asset.Data.CssFiles.Select(x => string.Format("'{0}'", asset.GetUrl(x))));

                if (asset.Data != null && asset.Data.JsFiles != null)
                    files.AddRange(asset.Data.JsFiles.Select(x => string.Format("'{0}'", asset.GetUrl(x))));
            }

            return new HtmlString(string.Join(",", files));
        }

        public static IHtmlString RenderAssetFilesAsHtml(this HtmlHelper helper, string assetName)
        {
            return helper.RenderAssetFilesAsHtml(assetName, helper.GetCurrentArea());
        }

        public static IHtmlString RenderAssetFilesAsHtml(this HtmlHelper helper, string assetName, AssetFileType renderOnlyType)
        {
            return helper.RenderAssetFilesAsHtml(assetName, helper.GetCurrentArea(), renderOnlyType);
        }
        
        public static IHtmlString RenderAssetFilesAsHtml(this HtmlHelper helper, string assetName, string areaName, AssetFileType? renderOnlyType = null)
        {
            var assets = AssetsTool.GetAssets(areaName, CurrentModuleAssets, assetName);
            if (assets == null || assets.Count == 0)
                return null;
            
            bool renderLinks = renderOnlyType == null || renderOnlyType.Value == AssetFileType.Link;
            bool renderScripts = renderOnlyType == null || renderOnlyType.Value == AssetFileType.Script;
            
            var files = new List<string>();
            if (renderLinks && assets[assetName].Data != null && assets[assetName].Data.CssFiles != null)
                files.AddRange(assets[assetName].Data.CssFiles.Select(x => string.Format("<link rel=\"stylesheet\" href=\"{0}\">", assets[assetName].GetUrl(x))));

            if (renderScripts && assets[assetName].Data != null && assets[assetName].Data.JsFiles != null)
                files.AddRange(assets[assetName].Data.JsFiles.Select(x => string.Format("<script src=\"{0}\"></script>", assets[assetName].GetUrl(x))));

            return new HtmlString(string.Join("", files));
        }

        public static IHtmlString RenderAssetsCriticalCss(this HtmlHelper helper)
        {
            if (DebugMode.IsDebugMode(eDebugMode.CriticalCss))
            {
                return new HtmlString("");
            }

            var angularCriticalCss = @"[ng\:cloak], [ng-cloak], [data-ng-cloak], [x-ng-cloak], .ng-cloak, .x-ng-cloak, .ng-hide:not(.ng-hide-animate) {display: none !important;}" +
                                     @"ng\:form {display: block;}" +
                                     @".ng-animate-shim {visibility: hidden;}" +
                                     @".ng-anchor {position: absolute;}";
            var content = AssetsTool.GetCriticalCss(helper.GetCurrentArea(), CurrentModuleAssets, CurrentAssets.ToArray());
            return content.IsNotEmpty() ? new HtmlString(angularCriticalCss + content) : null;
        }

        public static string GetAssetPath(this HtmlHelper helper, string filename, string area = null)
        {
            return new AssetsTool.PathData(area ?? helper.GetCurrentArea()).GetPathByOriginalFileName(filename);   
        }
        
        #endregion

        public static IHtmlString GetMiniProfiler(this HtmlHelper helper)
        {
            if (SettingProvider.GetConfigSettingValue("Profiling") != "true" || (!Customers.CustomerContext.CurrentCustomer.IsAdmin && !Customers.CustomerContext.CurrentCustomer.IsVirtual))
                return new HtmlString("");

            var sb = new StringBuilder();

            var profilerSql = HttpContext.Current.Items["MiniProfiler_Sql"] as List<Profiling>;


            sb.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\">", UrlService.GetUrl("styles/profiler.css"));

            if (profilerSql != null)
            {
                sb.AppendFormat(
                    "<div class=\"pf\" style=\"display:none\"> <div class=\"pf-title\">SQL queries: <span>(count: {0} time: {1} ms)</span></div>",
                    profilerSql.Count, profilerSql.Sum(x => x.Time).ToString("F2"));

                foreach (var pf in profilerSql)
                {
                    sb.AppendFormat(
                        "<div class=\"pf-item\">" +
                        "<div class=\"pf-name\">{0}<div class=\"pf-params\">{1}</div></div>" +
                        "<div class=\"pf-time\">{2} <span>ms</span></div>" +
                        "</div>",
                        pf.Command,
                        pf.Parameters.Aggregate("", (current, x) => current + string.Format("{0}: {1};", x.Key, x.Value)),
                        pf.Time.ToString("F2"));
                }
                sb.Append("</div>");
            }

            var profilerActions = HttpContext.Current.Items["MiniProfiler_Actions"] as List<Profiling>;
            if (profilerActions != null)
            {
                sb.AppendFormat(
                    "<div class=\"pf\" style=\"display:none\"> <div class=\"pf-title\">Actions <span>(count: {0} time: {1} ms)</span></div>",
                    profilerActions.Count, profilerActions.Sum(x => x.Time).ToString("F2"));

                foreach (var pf in profilerActions)
                {
                    sb.AppendFormat(
                        "<div class=\"pf-item\">" +
                        "<div class=\"pf-name\">{0}</div>" +
                        "<div class=\"pf-time\">{1} <span>ms</span></div>" +
                        "</div>",
                        pf.Command,
                        pf.Time.ToString("F3"));
                }
                sb.Append("</div>");
            }

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString RenderLabels(this HtmlHelper helper, bool recommended, bool sales, bool best,
            bool news, Discount discount, int labelCount = 5, List<string> customLabels = null, bool warranty = false, bool notAvailabel = false)
        {
            var labels = new StringBuilder();

            if (warranty && labelCount-- > 0)
                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-warranty\">{0}</span></div>",
                    LocalizationService.GetResource("Catalog.Label.Warranty"));

            if (recommended && labelCount-- > 0)
                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-recommend\">{0}</span></div>",
                    LocalizationService.GetResource("Catalog.Label.Recomended"));

            if (sales && labelCount-- > 0)
                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-sales\">{0}</span></div>",
                    LocalizationService.GetResource("Catalog.Label.Sales"));

            if (best && labelCount-- > 0)
                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-best\">{0}</span></div>",
                    LocalizationService.GetResource("Catalog.Label.Best"));

            if (notAvailabel)
                labels.AppendFormat(
                     "<div class=\"products-view-label ng-hide\" data-ng-show=\"productViewItem.offer != null && productViewItem.offer.Amount <= 0\">" +
                        "<span class=\"products-view-label-inner products-view-label-not-available\">{0}</span>" +
                     "</div>",
                    LocalizationService.GetResource("Product.NotAvailable"));

            if (discount != null && discount.HasValue && labelCount-- > 0)
            {
                var group = CustomerContext.CurrentCustomer.CustomerGroup;

                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-discount\">{0} {1}</span></div>",
                    group != null && group.GroupDiscount > 0
                        ? LocalizationService.GetResource("Catalog.Label.YourDiscount")
                        : LocalizationService.GetResource("Catalog.Label.Discount"),
                    discount.Type == DiscountType.Percent
                        ? discount.Percent.FormatPriceInvariant() + "%"
                        : discount.Amount.FormatPrice()
                );
            }

            if (customLabels != null)
                foreach (var customLabel in customLabels)
                    labels.Append(
                        "<div class=\"products-view-label\"><span class='products-view-label-inner products-view-label-best products-view-label-custom'>" +
                        customLabel + "</span></div>");

            if (news && labelCount > 0)
                labels.AppendFormat(
                    "<div class=\"products-view-label\"><span class=\"products-view-label-inner products-view-label-new\">{0}</span></div>",
                    LocalizationService.GetResource("Catalog.Label.New"));

            return new HtmlString(labels.ToString());
        }

        public static IHtmlString RenderAmountTable(this HtmlHelper helper, ProductItem productItem)
        {
            var items = 
                PriceRuleService.GetPriceRuleAmountListItems(productItem, CustomerContext.CurrentCustomer.CustomerGroup);

            if (items == null || items.Count == 0)
                return new HtmlString(null);

            var sb = new StringBuilder();
            
            sb.Append("<div class=\"price-amount-list\">");

            sb.AppendFormat(
                "<div class=\"price-amount-list__row price-amount-list__row--head\"> <div class=\"price-amount-list__col price-amount-list__col--head\">{0}</div> <div class=\"price-amount-list__col price-amount-list__col--head\">{1}</div>  </div>",
                LocalizationService.GetResource("Catalog.AmountTable.Amount"), 
                LocalizationService.GetResource("Catalog.AmountTable.Price"));

            foreach (var item in items)
            {
                sb.AppendFormat(
                    "<div class=\"price-amount-list__row\"> <div class=\"price-amount-list__col\">{0}</div> <div class=\"price-amount-list__col\">{1}</div>  </div>",
                    item.Amount, item.Price);
            }
            
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        [Obsolete("Will be removed in future version. Use RenderAmountTable(productItem)")]
        public static IHtmlString RenderAmountTable(this HtmlHelper helper, int offerId, float multiplicity, float productCurrencyRate)
        {
            if (multiplicity == 0)
                multiplicity = 1;
            
            var items = 
                PriceRuleService.GetPriceRuleAmountListItems(offerId, CustomerContext.CurrentCustomer.CustomerGroupId, multiplicity, productCurrencyRate);

            if (items == null || items.Count == 0)
                return new HtmlString(null);

            var sb = new StringBuilder();
            
            sb.Append("<div class=\"price-amount-list\">");

            sb.AppendFormat(
                "<div class=\"price-amount-list__row price-amount-list__row--head\"> <div class=\"price-amount-list__col price-amount-list__col--head\">{0}</div> <div class=\"price-amount-list__col price-amount-list__col--head\">{1}</div>  </div>",
                LocalizationService.GetResource("Catalog.AmountTable.Amount"), 
                LocalizationService.GetResource("Catalog.AmountTable.Price"));

            foreach (var item in items)
            {
                sb.AppendFormat(
                    "<div class=\"price-amount-list__row\"> <div class=\"price-amount-list__col\">{0}</div> <div class=\"price-amount-list__col\">{1}</div>  </div>",
                    item.Amount, item.Price);
            }
            
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        public static MvcHtmlString DisableChromeAutoFill(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<input type=\"password\" name=\"disablingChromeAutoFill\" autocomplete=\"new-password\" hidden />");
        }
        public static HtmlString PhoneMask(this HtmlHelper helper)
        {
            return new HtmlString(SettingsMain.EnablePhoneMask ? "data-mask-control data-mask-control-preset=\"phone\"" : "");
        }

        public static MvcHtmlString SvgSprite(this HtmlHelper helper, string name, string cssClass, object svgAttributes = null, string spriteFileName = "spritemap")
        {
            var svgAttributesSerialized = "";
            if (svgAttributes != null)
            {
                var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(svgAttributes);
                var sb = new StringBuilder();
                foreach (var kvp in attrs)
                {
                    sb.Append(" ");
                    sb.Append(string.Format("{0}=\"{1}\"", kvp.Key, kvp.Value));
                }

                svgAttributesSerialized = sb.ToString();
            }

            return helper.Action("SvgSprite", "Common", new { name, cssClass, svgAttributes = svgAttributesSerialized, spriteFileName, areaName = helper.GetCurrentArea(), area = ""  });
        }
    }
}