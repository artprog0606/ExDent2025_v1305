using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Core
{
    public class AssetsTool
    {
        public enum AssetFileType
        {
            Link = 0,
            Script = 1
        }

        private const string CacheKeyPrefix = "assets_";

        public class Bundles
        {
            public Bundles()
            {
                EntryPoints = new Assets();
                Files = new Dictionary<string, string>();
            }
            
            public Assets EntryPoints { get; set; }
            public Dictionary<string, string> Files { get; set; }
        }

        public class Assets : Dictionary<string, Asset>
        {
        }

        public class Asset
        {
            private PathData _pathData;

            public void SetPathData(PathData pathData)
            {
                _pathData = pathData;
            }
            
            [JsonProperty("assets")] public AsssetData Data { get; set; }

            public string GetPath(string fileName, string folder = "dist")
            {
                return _pathData.GetPath(fileName, folder);
            }

            public string GetPathAbs(string fileName, string folder = "dist")
            {
                return _pathData.GetPathAbs(fileName, folder);
            }

            public string GetUrl(string fileName, string folder = "dist")
            {
                return UrlService.GetUrl(GetPath(fileName, folder));
            }

            public string GetCacheKey(string fileName = null)
            {
                return _pathData.GetCacheKey(fileName);
            }

            public string BundlesFilePath
            {
                get { return _pathData.GetPath("bundles.json"); }
            }

            public string BundlesFilePathAbs
            {
                get { return _pathData.GetPathAbs("bundles.json"); }
            }
        }

        public class AsssetData
        {
            [JsonProperty("js")] public List<string> JsFiles { get; set; }

            [JsonProperty("css")] public List<string> CssFiles { get; set; }
            //[JsonProperty("criticalCSS")]
            //public List<string> CriticalCSS { get; set; }
        }

        public class PathData
        {
            public PathData()
            {
            }

            public PathData(string areaName, string moduleName = null)
            {
                if (moduleName.IsNullOrEmpty())
                {
                    AreaName = areaName == "AdminV2" ? "AdminV3" : areaName;
                    
                    if (areaName.IsNullOrEmpty() ||
                        string.Equals(areaName, "Mobile", StringComparison.OrdinalIgnoreCase))
                    {
                        Template = SettingsDesign.Template != TemplateService.DefaultTemplateId
                            ? SettingsDesign.Template
                            : null;
                    }
                    else if (AreaName == "AdminV3" && !SettingsDesign.IsMobileTemplate)
                    {
                        SubTemplate = "AdminV3";
                    }
                    else if (AreaName == "AdminV3")
                    {
                        AreaName = "Admin";
                        SubTemplate = "Mobile";
                    }

                    //if (string.Equals(areaName, "Mobile", StringComparison.OrdinalIgnoreCase))
                    //    SubTemplate = SettingsDesign.MobileTemplate;
                }
                else
                    ModuleName = moduleName;
            }

            public string AreaName { get; set; }
            public string Template { get; set; }
            public string SubTemplate { get; set; }

            public string ModuleName { get; set; }

            /// <summary>
            /// 1) with template and subtemplate
            /// 2) without template
            /// 3) without subtemplate
            /// 4) null
            /// </summary>
            public PathData Default
            {
                get
                {
                    if (Template.IsNotEmpty())
                        return new PathData
                        {
                            AreaName = this.AreaName,
                            SubTemplate = this.SubTemplate
                        };
                    if (SubTemplate.IsNotEmpty())
                        return new PathData
                        {
                            AreaName = this.AreaName
                        };
                    return null;
                }
            }

            private Bundles _bundles;

            public Bundles Bundles
            {
                get
                {
                    if (_bundles != null)
                        return _bundles;

                    var cacheName = GetCacheKey();
                    var bundles = CacheManager.Get<Bundles>(cacheName);
                    if (bundles != null)
                        return (_bundles = bundles);

                    var filePath = GetPathAbs("bundles.json");
                    bundles = GetBundles(filePath);
                    if (bundles != null)
                        CacheManager.Insert(cacheName, bundles, 60, new CacheDependency(filePath), CacheItemPriority.Default);

                    return (_bundles = bundles);
                }
            }

            public string GetPathAbs(string fileName, string folder = "dist")
            {
                return HostingEnvironment.MapPath("~/" + GetPath(fileName, folder));
            }

            public string GetDirectoryAbs()
            {
                return HostingEnvironment.MapPath("~/" + GetDirectory());
            }
            
            public string GetPath(string fileName, string folder = "dist", string area = null)
            {
                string _area = area ?? AreaName;
                if (fileName.IsNullOrEmpty())
                    throw new ArgumentNullException("fileName");

                //TODO удалить после перемещения adminv3 как основную админку
                var isAdminMobile = SettingsDesign.IsMobileTemplate && (_area == "AdminV3" || _area == "AdminV2");
                if (fileName == "runtime.js" && _area == "AdminV3" && isAdminMobile)
                {
                    return string.Format("{0}{1}/{2}",
                        "Areas/Admin/",
                        folder,
                        fileName);   
                }
                
                if (ModuleName.IsNotEmpty())
                    return string.Format("Modules/{0}/{1}/{2}", ModuleName, folder, fileName);
                return string.Format("{0}{1}/{2}",
                    GetDirectory(_area),
                    folder,
                    fileName);
            }

            public string GetPathByOriginalFileName(string fileName, string folder = "dist")
            {
                string _fileName = fileName.ToLower();
                string result = null;
                string fileNameFounded = null;
                string areaUsing = null;

                if (Bundles != null && Bundles.Files != null)
                {
                    fileNameFounded = Bundles.Files.FirstOrDefault(x => x.Key.ToLower() == _fileName).Value;
                    
                    if (fileNameFounded != null)
                    {
                        result = GetPath(fileNameFounded, folder);
                        areaUsing = AreaName;
                    }
                }

                if (result.IsNullOrEmpty())
                {
                    fileNameFounded = Default.Bundles.Files.FirstOrDefault(x => x.Key.ToLower() == _fileName).Value;
                    
                    if (fileNameFounded != null)
                    {
                        result = Default.GetPath(fileNameFounded, folder);
                        areaUsing = Default.AreaName;
                    }
                }

                return result.IsNotEmpty() ? UrlService.GetUrl() + result : result;
            }
            
            public string GetDirectory(string area = null)
            {
                string _area = area ?? AreaName;
                if (ModuleName.IsNotEmpty())
                    return string.Format("Modules/{0}/", ModuleName);
                var isAdminMobile = SettingsDesign.IsMobileTemplate && (_area == "AdminV3" || _area == "AdminV2");
                
                return string.Format("{0}{1}{2}",
                    Template.IsNotEmpty() ? string.Format("Templates/{0}/", Template) : null,
                    _area.IsNotEmpty()
                        ? string.Format("Areas/{0}/{1}",
                            _area == "AdminV3"
                                ? "Admin"
                                : (_area == "AdminV2" && isAdminMobile ? "Admin" : _area),
                            isAdminMobile ? "Templates/Mobile/" : "")
                        : null,
                    SubTemplate.IsNotEmpty() ? string.Format("Templates/{0}/", SubTemplate) : null);
            }

            public string GetCacheKey(string fileName = null)
            {
                return
                    $"{CacheKeyPrefix}_{AreaName}_{Template}_{SubTemplate}_{ModuleName}_{fileName}"; //new List<string> { CacheKeyPrefix, AreaName, Template, SubTemplate, ModuleName, fileName }.Where(x => x.IsNotEmpty()).AggregateString('_');
            }
        }

        protected static Bundles GetBundles(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Bundles>(json);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("AssetsTool.GetBundles, file: " + filePath, ex);
            }

            return null;
        }

        public static Assets GetAssets(string areaName, Dictionary<string, List<string>> moduleAssets,
            params string[] assetNames)
        {
            return GetAssets(areaName, true, moduleAssets, assetNames);
        }

        private static Assets GetAssets(string areaName, bool useDefaultBundles,
            Dictionary<string, List<string>> moduleAssets, params string[] assetNames)
        {
            if ((assetNames == null || !assetNames.Any()) && (moduleAssets == null || !moduleAssets.Any()))
                return null;

            var pathData = GetPathDataWithBundlesOrDefault(new PathData(areaName));
            if (pathData == null)
                return null;
            //{
            //    if (pathData.Default?.Bundles != null)
            //        pathData = pathData.Default;
            //    else if (pathData.Default?.Default?.Bundles != null)
            //        pathData = pathData.Default.Default;
            //    else
            //        return null;
            //}

            var assets = new Assets();
            foreach (var assetName in assetNames)
            {
                if (assets.ContainsKey(assetName))
                    continue;

                assets.Add(assetName, GetAssetOrDefault(pathData, assetName) ?? new Asset());
                //Asset value;
                //if (pathData.Bundles.EntryPoints.TryGetValue(assetName, out value))
                //{
                //    value.SetPathData(pathData);
                //    assets.Add(assetName, value);
                //}
                //else if (pathData.Default != null && pathData.Default.Bundles != null &&
                //    pathData.Default.Bundles.EntryPoints.TryGetValue(assetName, out value))
                //{
                //    value.SetPathData(pathData.Default);
                //    assets.Add(assetName, value);
                //}
                //else if (pathData.Default != null && pathData.Default.Default != null && pathData.Default.Default.Bundles != null &&
                //    pathData.Default.Default.Bundles.EntryPoints.TryGetValue(assetName, out value))
                //{
                //    value.SetPathData(pathData.Default.Default);
                //    assets.Add(assetName, value);
                //}
                //else
                //    assets.Add(assetName, new Asset());
            }

            if (moduleAssets != null && moduleAssets.Any())
            {
                foreach (var moduleName in moduleAssets.Keys)
                {
                    var modulePathData = new PathData(areaName, moduleName);
                    if (modulePathData.Bundles == null)
                        continue;
                    foreach (var assetName in moduleAssets[moduleName])
                    {
                        if (assets.ContainsKey(assetName))
                            continue;
                        if (modulePathData.Bundles.EntryPoints.TryGetValue(assetName, out Asset value))
                        {
                            value.SetPathData(modulePathData);
                            assets.Add(assetName, value);
                        }
                        else
                            assets.Add(assetName, new Asset());
                    }
                }
            }

            return assets;
        }

        private static PathData GetPathDataWithBundlesOrDefault(PathData pathData)
        {
            if (pathData?.Bundles != null)
                return pathData;

            return pathData != null ? GetPathDataWithBundlesOrDefault(pathData.Default) : null;
        }

        private static Asset GetAssetOrDefault(PathData pathData, string assetName)
        {
            if (pathData?.Bundles == null)
                return null;
            if (pathData.Bundles.EntryPoints.TryGetValue(assetName, out Asset asset))
            {
                asset.SetPathData(pathData);
                return asset;
            }

            return GetAssetOrDefault(pathData.Default, assetName);
        }

        public static string GetCriticalCss(string areaName, Dictionary<string, List<string>> moduleAssets,
            params string[] assetNames)
        {
            if ((assetNames == null || assetNames.Length == 0) && (moduleAssets == null || moduleAssets.Count == 0))
                return null;

            var processed = new List<string>();
            var cssSb = new StringBuilder();

            if (assetNames != null && assetNames.Length > 0)
            {
                var pathData = new PathData(areaName);
                foreach (var assetName in assetNames)
                {
                    var content = GetCssFileContent(assetName, pathData, processed);
                    if (content.IsNotEmpty())
                        cssSb.Append(content);
                }

                // if no critical css, take from home
                if (cssSb.Length == 0 && !assetNames.Contains("home"))
                {
                    var content = GetCssFileContent("home", pathData, processed);
                    if (content.IsNotEmpty())
                        cssSb.Append(content);
                }

                //if is mobile and not override in template
                if (pathData.AreaName == "Mobile" && pathData.Template.IsNotEmpty() && cssSb.Length == 0)
                {
                    string cachekey = $"{pathData.AreaName}_{pathData.Template}_IsExist";

                    bool isExistMobileInTemplate =
                        CacheManager.Get(cachekey, () => Directory.Exists(pathData.GetDirectoryAbs()));

                    if (!isExistMobileInTemplate)
                    {
                        foreach (var assetName in assetNames)
                        {
                            var content = GetCssFileContent(assetName, pathData.Default, processed);
                            if (content.IsNotEmpty())
                                cssSb.Append(content);
                        }
                    }
                }
            }

            if (moduleAssets != null && moduleAssets.Count > 0)
            {
                foreach (var moduleName in moduleAssets.Keys)
                {
                    var modulePathData = new PathData(areaName, moduleName);
                    foreach (var assetName in moduleAssets[moduleName])
                    {
                        var content = GetCssFileContent(assetName, modulePathData, processed);
                        if (content.IsNotEmpty())
                            cssSb.Append(content);
                    }
                }
            }

            return cssSb.ToString();
        }

        private static string GetCssFileContent(string assetName, PathData pathData, List<string> processed)
        {
            var cssFileName = $"{assetName}.critical.css";

            var cacheKey = pathData.GetCacheKey(cssFileName);
            if (processed.Contains(cacheKey))
                return null;
            processed.Add(cacheKey);

            var css = "";

            if (CacheManager.TryGetValue(cacheKey, out css))
                return css;

            var file = pathData.GetPathAbs(cssFileName, "_criticalcss");
            if (!File.Exists(file))
            {
                CacheManager.Insert(cacheKey, "", 20);
                return null;
            }

            string content = null;
            try
            {
                content = File.ReadAllText(file);
            }
            catch (Exception ex)
            {
                Debug.Log.Error("AssetsTool, GetCriticalCss, file: " + file, ex);
            }

            if (content != null)
                CacheManager.Insert(cacheKey, content, 60, new CacheDependency(file), CacheItemPriority.Default);

            return content;
        }

        //public static string GetCriticalCssOld(string areaName, Dictionary<string, List<string>> moduleAssets, params string[] assetNames)
        //{
        //    var assets = GetAssets(areaName, false, moduleAssets, assetNames);
        //    if (assets == null)
        //        return null;

        //    var neededAssets = new Dictionary<string, Asset>(); // <assetName, Asset>
        //    foreach (var assetName in assets.Keys)
        //    {
        //        if (assets[assetName].Data == null || assets[assetName].Data.CriticalCSS == null || !assets[assetName].Data.CriticalCSS.Any() || neededAssets.ContainsKey(assetName))
        //            continue;

        //        neededAssets.Add(assetName, assets[assetName]);
        //    }

        //    // if no critical css, take from home
        //    if (!neededAssets.Any() && !assetNames.Contains("home"))
        //    {
        //        var defaultAssets = GetAssets(areaName, null, "home");
        //        var homeAsset = defaultAssets != null ? defaultAssets["home"] : null;
        //        if (homeAsset != null && homeAsset.Data != null && homeAsset.Data.CriticalCSS != null)
        //            neededAssets.Add("home", homeAsset);
        //    }

        //    if (!neededAssets.Any())
        //        return null;

        //    var processedFiles = new List<string>();
        //    var cssSb = new StringBuilder();
        //    foreach (var assetName in neededAssets.Keys)
        //    {
        //        foreach (var cssFileName in neededAssets[assetName].Data.CriticalCSS)
        //        {
        //            var file = neededAssets[assetName].GetPathAbs(cssFileName, "_criticalcss");
        //            if (!File.Exists(file) || processedFiles.Contains(file))
        //                continue;

        //            var cacheKey = neededAssets[assetName].GetCacheKey(cssFileName);
        //            var css = CacheManager.Get<string>(cacheKey);
        //            if (css != null)
        //            {
        //                cssSb.Append(css);
        //                continue;
        //            }

        //            string content = null;
        //            try
        //            {
        //                content = File.ReadAllText(file);
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.Log.Error("AssetsTool, GetCriticalCss, file: " + file, ex);
        //            }

        //            if (content.IsNotEmpty())
        //            {
        //                var assetsFile = neededAssets[assetName].BundlesFilePathAbs;
        //                CacheManager.Insert(cacheKey, content.ToString(), 60, new CacheDependency(new string[] { file, assetsFile }), CacheItemPriority.Default);
        //            }
        //            cssSb.Append(content);
        //        }
        //    }
        //    return cssSb.ToString();
        //}

        public static void DeleteNotUsedFiles()
        {
            if (SettingsGeneral.LastBundlesCleanup == SettingsGeneral.SiteVersionDev)
                return;

            var tplPaths = new List<string>
            {
                HostingEnvironment.MapPath("~/") // default
            };
            if (Directory.Exists(HostingEnvironment.MapPath("~/Templates/")))
                tplPaths.AddRange(Directory.GetDirectories(HostingEnvironment.MapPath("~/Templates/"))
                    .Select(dir => dir + '/'));

            var bundlesPaths = new List<string>();
            foreach (var path in tplPaths)
            {
                bundlesPaths.Add(path + "dist/bundles.json");
                bundlesPaths.Add(path + "Areas/Mobile/dist/bundles.json");
                var mobileTplsPath = path + "Areas/Mobile/Templates/";
                if (Directory.Exists(mobileTplsPath))
                    bundlesPaths.AddRange(Directory.GetDirectories(mobileTplsPath)
                        .Select(dir => dir + "/dist/bundles.json"));
            }

            foreach (var bundlesFile in bundlesPaths)
            {
                if (!File.Exists(bundlesFile))
                    continue;
                var bundles = GetBundles(bundlesFile);
                if (bundles == null)
                    continue;

                var neededFiles = new List<string> {"bundles.json"};
                //var criticalCssFiles = new List<string>();
                if (bundles.Files != null)
                    neededFiles.AddRange(bundles.Files.Values);
                if (bundles.EntryPoints != null)
                {
                    var assets = bundles.EntryPoints.Keys.Select(key => bundles.EntryPoints[key]).ToList();
                    neededFiles.AddRange(assets.Where(asset => asset.Data != null).SelectMany(asset =>
                        (asset.Data.JsFiles ?? new List<string>())
                        .Concat(asset.Data.CssFiles ?? new List<string>())
                    ).Distinct().Where(x => !neededFiles.Contains(x)));
                    //criticalCssFiles.AddRange(assets.Where(asset => asset.Data != null).SelectMany(asset =>
                    //    asset.Data.CriticalCSS ?? new List<string>()
                    //    ).Distinct().Where(x => !criticalCssFiles.Contains(x)));
                }

                foreach (var file in Directory.GetFiles(Path.GetDirectoryName(bundlesFile)))
                {
                    var fileName = Path.GetFileName(file);
                    if (neededFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log.Warn(ex);
                    }
                }
                //var dInfo = new DirectoryInfo(Path.GetDirectoryName(bundlesFile));
                //var criticalCssDir = dInfo.Parent != null ? dInfo.Parent.FullName + "/_criticalcss/" : HostingEnvironment.MapPath("~/_criticalcss/");
                //if (Directory.Exists(criticalCssDir))
                //{
                //    foreach (var file in Directory.GetFiles(criticalCssDir))
                //    {
                //        var fileName = Path.GetFileName(file);
                //        if (criticalCssFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                //            continue;
                //        try
                //        {
                //            File.Delete(file);
                //        }
                //        catch (Exception ex)
                //        {
                //            Debug.Log.Warn(ex);
                //        }
                //    }
                //}
            }

            SettingsGeneral.LastBundlesCleanup = SettingsGeneral.SiteVersionDev;
        }
    }
}