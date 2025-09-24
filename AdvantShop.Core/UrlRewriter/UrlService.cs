//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Core.Services.Files;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.SEO;

namespace AdvantShop.Core.UrlRewriter
{
    public enum ParamType
    {
        None,
        Product,
        Category,
        StaticPage,
        News,
        Brand,
        NewsCategory,
        Tag,
        Warehouse,
        ProductList
    }

    public static class UrlService
    {
        public class UrlStruct
        {
            public int ObjId { get; set; }
            public string UrlPath { get; set; }
            public ParamType Type { get; set; }
        }

        private const string ProductsWord = "products";
        private const string CategoriesWord = "categories";
        private const string PagesWord = "pages";
        private const string NewsWord = "news";
        private const string NewscategoryWord = "newscategory";
        private const string ManufacturersWord = "manufacturers";
        private const string WarehousesWord = "warehouses";

       public enum ESocialType
        {
            none = 0,
            vk = 1,
            fb = 2,
            ok = 3
        }

        public static readonly List<string> ExtentionNotToRedirect = new List<string>
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".ico",
            ".css",
            ".js",
            //".html",
            ".htc",
            ".ashx",
            ".xls",
            ".xlsx",
            ".xml",
            ".yml",
            ".txt",
            ".zip",
            ".pdf",
            ".swf",
            ".tpl",
		    ".axd",
            ".csv",
        };

        public static readonly List<string> ExtentionOpenInBrowser = new List<string>
        {
            ".csv",
            ".xml",
            ".yml"
        };

        public static readonly Dictionary<ParamType, string> NamesAndIds = new Dictionary<ParamType, string>
        {
            {ParamType.StaticPage, "StaticPageId"},
            {ParamType.Category, "CategoryId"},
            {ParamType.Product, "ProductId"},
            {ParamType.NewsCategory, "NewsCategoryId"},
            {ParamType.News, "NewsId"},
            {ParamType.Brand, "BrandId"},
            {ParamType.Tag, "Id"},
            {ParamType.Warehouse, "Id"},
            {ParamType.ProductList, "Id"},
            {ParamType.None, string.Empty}
        };

        public static readonly Dictionary<ParamType, string> NamesAndWords = new Dictionary<ParamType, string>
        {
            {ParamType.StaticPage, PagesWord},
            {ParamType.Category, CategoriesWord},
            {ParamType.Product, ProductsWord},
            {ParamType.NewsCategory, NewscategoryWord},
            {ParamType.News, NewsWord},
            {ParamType.Brand, ManufacturersWord},
            {ParamType.Warehouse, WarehousesWord},
            {ParamType.None, string.Empty}
        };

        public static readonly Dictionary<ParamType, string> NamesAndDb = new Dictionary<ParamType, string>
        {
            {ParamType.StaticPage, "CMS.StaticPage"},
            {ParamType.Category, "Catalog.Category"},
            {ParamType.Product, "Catalog.Product"},
            {ParamType.NewsCategory, "Settings.NewsCategory"},
            {ParamType.News, "Settings.News"},
            {ParamType.Brand, "Catalog.Brand"},
            {ParamType.Tag, "Catalog.Tag"},
            {ParamType.Warehouse, "Catalog.Warehouse"},
            {ParamType.ProductList, "Catalog.ProductList"},
            {ParamType.None, string.Empty}
        };

        public static readonly List<string> UnAvailableWords = new List<string>
        {
            ProductsWord,
            CategoriesWord,
            PagesWord,
            NewscategoryWord,
            NewsWord,
            ManufacturersWord,
            WarehousesWord,
            "admin",
            "adminv2",
            "areas",
            "bin",
            "combine",
            "content",
            "fonts",
            "images",
            "landings",
            "modules",
            "pictures",
            "scripts",
            "styles",
            "templates",
            "tools",
            "userfiles",
            "vendors",
            "views"
        };

        private static List<string> TwoDotsDominZones = new List<string>()
        {
            "abkhazia.su",
            "abkhazia.su",
            "adygeya.ru",
            "adygeya.su",
            "ae.org",
            "aktyubinsk.su",
            "arkhangelsk.su",
            "armenia.su",
            "ashgabad.su",
            "azerbaijan.su",
            "balashov.su",
            "bashkiria.ru",
            "bashkiria.su",
            "bir.ru",
            "biz.ki",
            "biz.pl",
            "biz.ua",
            "br.com",
            "bryansk.su",
            "bukhara.su",
            "cbg.ru",
            "cherkassy.ua",
            "cherkasy.ua",
            "chernivtsi.ua",
            "chernovtsy.ua",
            "chimkent.su",
            "ck.ua",
            "club.tw",
            "cn.com",
            "cn.ua",
            "co.ag",
            "co.am",
            "co.at",
            "co.bz",
            "co.cm",
            "co.com",
            "co.gl",
            "co.gy",
            "co.im",
            "co.in",
            "co.lc",
            "co.nz",
            "co.ua",
            "co.uk",
            "co.ve",
            "co.za",
            "com.af",
            "com.ag",
            "com.am",
            "com.bz",
            "com.cm",
            "com.co",
            "com.de",
            "com.ec",
            "com.es",
            "com.gl",
            "com.gy",
            "com.hn",
            "com.ht",
            "com.im",
            "com.ki",
            "com.kz",
            "com.lc",
            "com.lv",
            "com.ly",
            "com.mu",
            "com.mx",
            "com.my",
            "com.nf",
            "com.ng",
            "com.pe",
            "com.ph",
            "com.pl",
            "com.ro",
            "com.ru",
            "com.sb",
            "com.sc",
            "com.tr",
            "com.tw",
            "com.ua",
            "com.vc",
            "com.ve",
            "crimea.ua",
            "cv.ua",
            "dagestan.ru",
            "dagestan.su",
            "de.com",
            "dn.ua",
            "donetsk.ua",
            "dp.ua",
            "east-kazakhstan.su",
            "ebiz.tw",
            "edu.pl",
            "eu.com",
            "exnet.su",
            "fin.ec",
            "firm.in",
            "game.tw",
            "gb.net",
            "gen.in",
            "georgia.su",
            "gr.com",
            "grozny.ru",
            "grozny.su",
            "hu.net",
            "idv.tw",
            "if.ua",
            "in.net",
            "in.ua",
            "ind.in",
            "info.ec",
            "info.ht",
            "info.ki",
            "info.pl",
            "ivano-frankivsk.ua",
            "ivanovo.su",
            "jambyl.su",
            "jp.net",
            "jpn.com",
            "kalmykia.ru",
            "kalmykia.su",
            "kaluga.su",
            "karacol.su",
            "karaganda.su",
            "karelia.su",
            "khakassia.su",
            "kherson.ua",
            "khmelnitskiy.ua",
            "kiev.ua",
            "kirovograd.ua",
            "km.ua",
            "kr.ua",
            "krasnodar.su",
            "ks.ua",
            "kurgan.su",
            "kustanai.ru",
            "kustanai.su",
            "kyiv.ua",
            "lenug.su",
            "lg.ua",
            "lt.ua",
            "lugansk.ua",
            "lutsk.ua",
            "mangyshlak.su",
            "marine.ru",
            "me.uk",
            "med.ec",
            "mex.com",
            "mk.ua",
            "mordovia.ru",
            "mordovia.su",
            "msk.ru",
            "msk.su",
            "murmansk.su",
            "mytis.ru",
            "nalchik.ru",
            "nalchik.su",
            "navoi.su",
            "net.af",
            "net.ag",
            "net.am",
            "net.bz",
            "net.cm",
            "net.co",
            "net.ec",
            "net.gl",
            "net.gy",
            "net.hn",
            "net.ht",
            "net.im",
            "net.in",
            "net.ki",
            "net.lc",
            "net.lv",
            "net.mu",
            "net.my",
            "net.nf",
            "net.nz",
            "net.pe",
            "net.ph",
            "net.pl",
            "net.ru",
            "net.sb",
            "net.sc",
            "net.so",
            "net.ua",
            "net.vc",
            "net.za",
            "nikolaev.ua",
            "nom.ag",
            "nom.co",
            "nom.es",
            "nom.pl",
            "north-kazakhstan.su",
            "nov.ru",
            "nov.su",
            "obninsk.su",
            "od.ua",
            "odesa.ua",
            "odessa.ua",
            "or.at",
            "org.af",
            "org.ag",
            "org.am",
            "org.bz",
            "org.es",
            "org.gl",
            "org.hn",
            "org.ht",
            "org.im",
            "org.in",
            "org.ki",
            "org.kz",
            "org.lc",
            "org.lv",
            "org.mu",
            "org.my",
            "org.nf",
            "org.nz",
            "org.pe",
            "org.ph",
            "org.pl",
            "org.ru",
            "org.sb",
            "org.sc",
            "org.uk",
            "org.vc",
            "org.za",
            "penza.su",
            "pl.ua",
            "pokrovsk.su",
            "poltava.ua",
            "pp.ru",
            "pp.ua",
            "pro.ec",
            "pyatigorsk.ru",
            "radio.am",
            "radio.fm",
            "rivne.ua",
            "rovno.ua",
            "ru.com",
            "ru.net",
            "rv.ua",
            "sa.com",
            "se.net",
            "sebastopol.ua",
            "shop.pl",
            "sm.ua",
            "sochi.su",
            "spb.ru",
            "spb.su",
            "sumy.ua",
            "tashkent.su",
            "te.ua",
            "termez.su",
            "ternopil.ua",
            "togliatti.su",
            "troitsk.su",
            "tselinograd.su",
            "tula.su",
            "tuva.su",
            "uk.com",
            "uk.net",
            "us.com",
            "us.org",
            "uzhgorod.ua",
            "vinnica.ua",
            "vladikavkaz.ru",
            "vladikavkaz.su",
            "vladimir.ru",
            "vladimir.su",
            "vn.ua",
            "vologda.su",
            "volyn.ua",
            "waw.pl",
            "web.za",
            "yalta.ua",
            "za.com",
            "zaporizhzhe.ua",
            "zhitomir.ua",
            "zp.ua",
            "zt.ua",
            "балаклава.рф",
            "донузлав.рф",
            "евпатория.рф",
            "инкерман.рф",
            "казантип.рф",
            "коктебель.рф",
            "массандра.рф",
            "ореанда.рф",
            "севастополь.рф",
            "симфи.рф",
            "тарханкут.рф",
            "ялта.рф",
            "minsk.by",
            "com.by",
            "net.by",
            "at.by",
        };

        private static HashSet<string> _fileInDbPaths;

        /// <summary>
        /// Warning!!! if we can't urlpath on databing
        /// </summary>
        public static string GetLinkDB(ParamType type, int objId)
        {
            var objUrl = GetObjUrlFromDb(type, objId);
            return GetLink(type, objUrl, objId);
        }

        /// <summary>
        /// get url from db by id and type
        /// </summary>
        public static string GetObjUrlFromDb(ParamType type, int objId)
        {
            if (type == ParamType.None) return string.Empty;
            return SQLDataAccess.ExecuteScalar<string>(string.Format("select urlPath from {0} where {1}=@id", NamesAndDb[type], NamesAndIds[type]),
                                                        CommandType.Text, new SqlParameter { ParameterName = "@id", Value = objId });
        }

        /// <summary>
        /// create url-string
        /// </summary>
        public static string GetLink(ParamType type, string objUrl, int objId)
        {
            return GetLink(type, objUrl, objId, string.Empty);
        }

        /// <summary>
        /// return url link
        /// </summary>
        public static string GetLink(ParamType type, string objUrl, int objId, string query)
        {
            return NamesAndWords[type] + '/' + HttpUtility.UrlEncode(objUrl) + (string.IsNullOrEmpty(query) ? string.Empty : '?' + query);
        }
        
        public static string GetLink(ParamType type, string objUrl)
        {
            return NamesAndWords[type] + '/' + HttpUtility.UrlEncode(objUrl);
        }

        public static string GetAbsoluteBaseLink(bool appendApplicationPath = true)
        {
            if (HttpContext.Current == null)
                return SettingsMain.SiteUrl;

            var request = HttpContext.Current.Request;
            if (!string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]))
            {
                var forwardedHostHeader = request.Headers["X-Forwarded-Host"].TrimEnd('/');
                var forwardedHostPathHeader = request.Headers["X-Forwarded-Host-Path"].TrimStart('/');
                var forwardedProtoSecureHeader = request.Headers["X-Forwarded-Proto-Secure"];

                var scheme = forwardedProtoSecureHeader == "1" ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                return $"{scheme}{Uri.SchemeDelimiter}{forwardedHostHeader}/{forwardedHostPathHeader}/";
            }

            var requestUrl = request.Url;
            var port = requestUrl.IsDefaultPort
                ? string.Empty
                : ":" + Convert.ToString(requestUrl.Port, CultureInfo.InvariantCulture);
            var url = string.Empty;
            if (appendApplicationPath)
            {
                url = request.ApplicationPath == "/" ? request.ApplicationPath : request.ApplicationPath + "/";
            }

            return ((IsSecureConnection(request) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp) + Uri.SchemeDelimiter +
                    requestUrl.Host + port + url).ToLower();
        }
        
        public static string GetAbsoluteLink(string link)
        {
            if (link.Contains("http://") || link.Contains("https://")) return link;

            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return link;

            return GetAbsoluteBaseLink() + link.TrimStart('/');
        }

        /// <summary>
        /// Helps to get link to file with hash in query
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="addCacheDependencyOnFile">Add CacheDependency for cache entry</param>
        /// <returns>Absolute link on file with hash string in query</returns>
        /// <exception cref="ArgumentNullException">If filePath is null or empty</exception>
        public static string GetAbsoluteFileLinkWithHashInQuery(string filePath, bool addCacheDependencyOnFile = false)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var cacheKey = nameof(GetAbsoluteFileLinkWithHashInQuery) + "_" + filePath;
            var hashString = CacheManager.Get<string>(cacheKey);
            if (string.IsNullOrWhiteSpace(hashString))
            {
                var path = filePath;
                if (!path.StartsWith("~/"))
                {
                    path = "~/" + path.TrimStart('/');
                }

                path = HostingEnvironment.MapPath(path);

                hashString = FileHelpers.GetHashByContent(path);
                CacheManager.Insert(
                    cacheKey,
                    hashString,
                    60,
                    addCacheDependencyOnFile
                        ? new CacheDependency(path)
                        : null,
                    CacheItemPriority.Default
                );
            }

            return GetAbsoluteLink(filePath + "?hash=" + hashString);
        }

        public static string GetAdminAbsoluteLink(string link)
        {
            if (link.Contains("http://") || link.Contains("https://")) return link;
            return GetAbsoluteBaseLink() + "admin/"+ link.TrimStart('/');
        }

        /// <summary>
        /// Get if url is avalible
        /// </summary>
        public static bool IsAvailableUrl(int objId, ParamType type, string objUrl)
        {
            if (string.IsNullOrWhiteSpace(objUrl)) return true;
            var temp = objUrl.ToLower();
            // find in unavalible words
            if (UnAvailableWords.FirstOrDefault(x => x == temp) != null) return false;
            //find  in database
            if (GetUrlCount(temp, type, objId) > 0) return false;

            return true;
        }

        public static bool IsAvailableUrl(ParamType type, string objUrl)
        {
            if (string.IsNullOrWhiteSpace(objUrl)) return true;
            // find in unavalible words
            var temp = objUrl.ToLower();
            if (UnAvailableWords.FirstOrDefault(x => x == temp) != null) return false;
            //find  in database
            if (GetUrlCount(temp, type, 0) > 0) return false;

            return true;
        }

        /// <summary>
        /// Get count of objUrl in database by type
        /// </summary>
        private static int GetUrlCount(string objUrl, ParamType type, int objId)
        {
            return SQLDataAccess.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM {0} WHERE UrlPath=@UrlPath AND {1} <> @id", NamesAndDb[type], NamesAndIds[type]),
                                                    CommandType.Text,
                                                    new SqlParameter { ParameterName = "@UrlPath", Value = objUrl },
                                                    new SqlParameter { ParameterName = "@id", Value = objId }
                                                    );
        }

        public static bool IsDebugUrl(string url)
        {
            // Add here more adress if you need it
            return url.Contains("/tools/") ||
                   url.Contains("/techdemos/") ||
                   url.Contains("/content/info/") ||
                   url.Contains("/.well-known/");

        }

        public static ESocialType IsSocialUrl(string url)
        {
            foreach (ESocialType item in new ESocialType[] {ESocialType.vk, ESocialType.fb})
            {
                if (url.StartsWith("https://" + item + ".") || url.StartsWith("http://" + item + "."))
                    return item;
            }

            return ESocialType.none;
        }

        public static string GetRedirect301(string fromUrl, string reqAbsoluteUri)
        {
            var index = reqAbsoluteUri.IndexOf('?');
            var absoluteUri = index > 0
                                ? reqAbsoluteUri.Substring(0, index).ToLower() + reqAbsoluteUri.Substring(index).ToLower()
                                : reqAbsoluteUri.ToLower();

            absoluteUri = HttpUtility.UrlDecode(absoluteUri);

            var redirect = RedirectSeoService.GetByInputUrl(fromUrl, absoluteUri);
            if (redirect == null)
                return null;

            var uri = new Uri(redirect.RedirectFrom, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                redirect.RedirectFrom = GetUrl(redirect.RedirectFrom);
            }
            uri = new Uri(redirect.RedirectTo, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                redirect.RedirectTo = GetUrl(redirect.RedirectTo);
            }

            string location = string.Empty;
            if (!string.IsNullOrEmpty(redirect.ProductArtNo))
            {
                var product = Catalog.ProductService.GetProduct(redirect.ProductArtNo);

                location = product != null ? GetAbsoluteLink(GetLink(ParamType.Product, product.UrlPath, product.ProductId))
                                           : absoluteUri.Replace(redirect.RedirectFrom, redirect.RedirectTo);
            }
            else
            {
                var absoluteUrlEncoded = absoluteUri.Split('/').Select(HttpUtility.UrlDecode).AggregateString("/").ToLower();
                var absoluteUrlPathEncoded = absoluteUri.Split('/').Select(HttpUtility.UrlPathEncode).AggregateString("/").ToLower();
                if (absoluteUrlEncoded.Contains(redirect.RedirectFrom) || absoluteUrlEncoded.Replace(" ", "+").Contains(redirect.RedirectFrom))
                {
                    location = absoluteUrlEncoded.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
                if (absoluteUrlPathEncoded.Contains(redirect.RedirectFrom) || absoluteUrlPathEncoded.Replace(" ", "+").Contains(redirect.RedirectFrom))
                {
                    location = absoluteUrlPathEncoded.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
                else if (absoluteUri.Contains(redirect.RedirectFrom))
                {
                    location = absoluteUri.Replace(redirect.RedirectFrom, redirect.RedirectTo);
                }
            }
            return location.Replace("*", string.Empty);
        }

        public static string GetAvailableValidUrl(int objId, ParamType type, string prevUrl)
        {
            var j = 1;
            var transformUrl = !SettingsMain.EnableCyrillicUrl
                ? StringHelper.TransformUrl(StringHelper.Translit(prevUrl))
                : StringHelper.TransformUrl(prevUrl ?? "");

            if (transformUrl.Length > 150)
                transformUrl = transformUrl.Substring(0, 150);

            var url = transformUrl;
            if (url.IsNullOrEmpty())
                url = transformUrl = type.ToString().ToLower();

            while (!IsAvailableUrl(objId, type, url))
            {
                url = StringHelper.TransformUrl(transformUrl);
                url =
                    (url.Length + j.ToString().Length + 1 > 150
                        ? url.Substring(0, url.Length - j.ToString().Length - 1)
                        : url) +
                    "-" + j++;
            }

            return url;
        }

        public static bool IsValidUrl(string url, ParamType type)
        {
            var pattern = !SettingsMain.EnableCyrillicUrl ? "^[a-zA-Z0-9_\\-]+$" : "^[a-zA-Zа-яА-Я0-9_\\-]+$";
            var reg = new Regex(pattern);

            return reg.IsMatch(url) && IsAvailableUrl(type, url);
        }

        public static string GenerateBaseUrl(bool appendApplicationPath = true)
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl.TrimEnd('/') + "/";

            var contextKey = "BaseUrl_withPath" + appendApplicationPath;
            if (HttpContext.Current.Items[contextKey] != null)
                return (string)HttpContext.Current.Items[contextKey];

            var baseUrl = GetAbsoluteBaseLink(appendApplicationPath: appendApplicationPath);
            HttpContext.Current.Items[contextKey] = baseUrl;
            return baseUrl;
        }

        public static string GetUrl()
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl + "/";

            return GenerateBaseUrl(); 
        }
        
        public static string GetUrl(string path)
        {
            var appPath = HttpContext.Current?.Request.ApplicationPath;
            if (!string.IsNullOrWhiteSpace(appPath) && appPath != "/")
            {
                if (path.StartsWith(appPath)) path = path.Substring(appPath.Length);
                if (path.StartsWith(appPath.Trim('/'))) path = path.Substring(appPath.Trim('/').Length);
            }
            
            return GenerateBaseUrl() + path.TrimStart('/');
        }

        /// <summary>
        /// Admin webforms base url
        /// </summary>
        /// <returns></returns>
        [Obsolete("use GetAdminUrl")]
        public static string GetAdminBaseUrl()
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl + "/adminv2/";

            return GenerateBaseUrl() + HttpContext.Current.Request.Url.PathAndQuery;
        }

        public static string GetAdminUrl(bool baseUrlfromSettings = false, bool useAdminAreaTemplates = true)
        {
            if (HttpContext.Current == null 
                || baseUrlfromSettings == true
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl + "/adminv2/";

            string adminUrl;
            if (SettingsMain.IsTechDomainsReady)
            {
                adminUrl = SettingsMain.TechDomainAdminPanel + "/";
            }
            else
            {
                adminUrl = GetAbsoluteBaseLink();
            }

            if (useAdminAreaTemplates && AdminAreaTemplate.Current != null)
                adminUrl += AdminAreaTemplate.Current + "/";
            else
                adminUrl+= "adminv2/";

            return adminUrl;
        }

        public static string GetAdminUrl(string path, bool baseUrlfromSettings = false, bool useAdminAreaTemplates = true)
        {
            return GetAdminUrl(baseUrlfromSettings, useAdminAreaTemplates: useAdminAreaTemplates) + path;
        }

        public static string GetAdminStaticPath()
        {
            return "areas/admin/content/";
        }

        public static string GetAdminStaticUrl()
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return GetAdminStaticPath();

            return GetAbsoluteBaseLink() + GetAdminStaticPath();
        }

        public static string GetCanonicalUrl(bool withPage = false)
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl + "/";
            string query = null;
            if (withPage)
            {
                var page = HttpContext.Current.Request.QueryString["page"];
                query = page.IsNullOrEmpty() ? string.Empty : $"?page={page}";
            }
            return $"{GenerateBaseUrl(appendApplicationPath: false).TrimEnd('/')}/{HttpContext.Current.Request.Url.AbsolutePath.TrimStart('/')}{query}".ToLower();
        }

        public static string GetCanonicalUrlAdminArea()
        {
            if (!SettingsMain.IsTechDomainsReady)
                return GetCanonicalUrl();
            
            return $"{SettingsMain.TechDomainAdminPanel.TrimEnd('/')}/{HttpContext.Current.Request.Url.AbsolutePath.TrimStart('/')}"
                .ToLower();
        }

        public static string GetCurrentUrl(string url = null)
        {
            if (HttpContext.Current == null
                || string.Compare((string)HttpContext.Current.Items["IsFromTask"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                return SettingsMain.SiteUrl + "/";
            
            var request = HttpContext.Current.Request;
            if (!string.IsNullOrWhiteSpace(request.Headers["X-Forwarded-Host"]))
            {
                var forwardedHostHeader = request.Headers["X-Forwarded-Host"].TrimEnd('/');
                var forwardedHostPathHeader = request.Headers["X-Forwarded-Host-Path"].TrimStart('/');
                var forwardedProtoSecureHeader = request.Headers["X-Forwarded-Proto-Secure"];

                var scheme = forwardedProtoSecureHeader == "1" ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                return $"{scheme}{Uri.SchemeDelimiter}{forwardedHostHeader}/{forwardedHostPathHeader}/{url?.TrimStart('/')}";
            }

            return HttpContext.Current.Request.RawUrl + url;
        }

        public static bool IsSecureConnection(HttpRequest request)
        {
            if (request == null)
                return false;

            return request.IsSecureConnection || request.Headers["x-forwarded-proto"] == "https" || request.Headers["X-Forwarded-Proto-Secure"] == "1";
        }

        public static bool IsSecureConnection(HttpRequestBase request)
        {
            if (request == null)
                return false;

            return request.IsSecureConnection || request.Headers["x-forwarded-proto"] == "https" || request.Headers["X-Forwarded-Proto-Secure"] == "1";
        }

        public static bool IsIpBanned(string ip)
        {
            var saasData = SaasDataService.CurrentSaasData;
            var checkedIps = SettingsGeneral.GetCheckedIps(saasData);
            if (checkedIps.TryGetValue(ip, out bool banned))
                return banned;

            try
            {
                if (saasData.IpWhiteListValues?.Any(x => IsInSubnet(ip, x)) is true)
                    return false;

                var ipList = SettingsGeneral.BannedIpList;
                if (saasData.IpBlackListValues != null)
                    ipList.AddRange(saasData.IpBlackListValues);

                if (ipList.Any(x => IsInSubnet(ip, x)))
                    banned = true;
                checkedIps.TryAdd(ip, banned);

                return banned;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return false;
            }
        }

        public static bool IsInSubnet(string ip, string subnetMask)
        {
            var slashIdx = subnetMask.IndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (slashIdx == -1)
                return ip == subnetMask;
            if (!IPAddress.TryParse(ip, out IPAddress address))
                return false;

            // First parse the address of the netmask before the prefix length.
            if (!IPAddress.TryParse(subnetMask.Substring(0, slashIdx), out IPAddress maskAddress))
                return false;

            if (maskAddress.AddressFamily != address.AddressFamily)
                // We got something like an IPV4-Address for an IPv6-Mask. This is not valid.
                return false;

            // Now find out how long the prefix is.
            var maskLength = int.Parse(subnetMask.Substring(slashIdx + 1));

            if (maskLength == 0)
                return true;
            else if (maskLength < 0)
                throw new NotSupportedException("A Subnetmask should not be less than 0.");

            switch (maskAddress.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    {
                        // Convert the mask address to an unsigned integer.
                        var maskAddressBits = BitConverter.ToUInt32(maskAddress.GetAddressBytes().Reverse().ToArray(), 0);

                        // And convert the IpAddress to an unsigned integer.
                        var ipAddressBits = BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

                        // Get the mask/network address as unsigned integer.
                        var mask = uint.MaxValue << (32 - maskLength);

                        // https://stackoverflow.com/a/1499284/3085985
                        // Bitwise AND mask and MaskAddress, this should be the same as mask and IpAddress
                        // as the end of the mask is 0000 which leads to both addresses to end with 0000
                        // and to start with the prefix.
                        return (maskAddressBits & mask) == (ipAddressBits & mask);
                    }
                case AddressFamily.InterNetworkV6:
                    {
                        // Convert the mask address to a BitArray. Reverse the BitArray to compare the bits of each byte in the right order.
                        var maskAddressBits = new BitArray(maskAddress.GetAddressBytes().Reverse().ToArray());

                        // And convert the IpAddress to a BitArray. Reverse the BitArray to compare the bits of each byte in the right order.
                        var ipAddressBits = new BitArray(address.GetAddressBytes().Reverse().ToArray());
                        var ipAddressLength = ipAddressBits.Length;

                        if (maskAddressBits.Length != ipAddressBits.Length)
                            throw new ArgumentException("Length of IP Address and Subnet Mask do not match.");

                        // Compare the prefix bits.
                        for (var i = ipAddressLength - 1; i >= ipAddressLength - maskLength; i--)
                            if (ipAddressBits[i] != maskAddressBits[i])
                                return false;

                        return true;
                    }
                default:
                    throw new NotSupportedException("Only InterNetworkV6 or InterNetwork address families are supported.");
            }
        }

        public static bool IsCurrentUrl(string url)
        {
            if (HttpContext.Current == null || url.IsNullOrEmpty())
                return false;
            var currentUri = HttpContext.Current.Request.Url;
            if (currentUri.LocalPath == "/")
                return false;

            url = url.ToLower();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = GetUrl(url.TrimStart('/'));

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri) && uri.Host != currentUri.Host)
                return false;

            var path = uri != null ? uri.LocalPath : url;
            return path.IsNotEmpty() && path.Equals(currentUri.LocalPath, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryResponseFileFromDb(string path, HttpResponse response)
        {
            if (_fileInDbPaths == null)
                _fileInDbPaths = FileInDbService.GetPaths();

            if (_fileInDbPaths == null || !_fileInDbPaths.Contains(path))
                return false;

            var fileInDb = FileInDbService.Get(path);
            if (fileInDb == null)
                return false;

            response.Clear();
            response.ContentType = fileInDb.ContentType + 
                                   (!string.IsNullOrEmpty(fileInDb.Charset) ? "; charset=" + fileInDb.Charset : "");
            response.AddHeader("content-disposition", "inline;filename=" + fileInDb.Name);
            response.BinaryWrite(fileInDb.Content);
            response.End();

            return true;
        }

        public static void UpdateFileInDbPaths()
        {
            _fileInDbPaths = FileInDbService.GetPaths();
        }

        public static string GetUrlForAuthFromAdmin(string domain, string path = null)
        {
            var pathQuery = path is null ? string.Empty : $"&path={path}";
            return GetUrl($"adminv3/account/redirectwithauth?domain={domain}{pathQuery}");
        }

        public static bool IsOnSubdomain(Uri url)
        {
            return GetDomainLevel(url) >= 2;
        }
        
        public static int GetDomainLevel(Uri uri)
        {
            var safeHost = uri.DnsSafeHost.ToLower();
            var dotsCount = safeHost.ToCharArray().Count(c => c == '.');

            if (TwoDotsDominZones.Any(zone => safeHost.EndsWith(zone.ToLower()))) dotsCount--;
            if (safeHost.StartsWith("www")) dotsCount--;

            return dotsCount;
        }
        
        public static string RemoveSubdomain(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                return null;

            var dotsCount = TwoDotsDominZones.Any(domainName.EndsWith) ? 3 : 2;

            for (var i = domainName.Length - 1; i >= 0; i--)
            {
                if (domainName[i] == '.')
                {
                    dotsCount--;
                }
                if (dotsCount == 0)
                {
                    return domainName.Substring(i + 1);
                }
            }
            return domainName;
        }

        public static string GetSubDomain(Uri uri)
        {
            if (!IsOnSubdomain(uri))
                return null;

            var domainName = uri.DnsSafeHost;
            if (string.IsNullOrWhiteSpace(domainName))
                return null;

            var dotsCount = TwoDotsDominZones.Any(domainName.EndsWith) ? 3 : 2;
            var isWwwPrefixed = false;
            if (domainName.StartsWith("www"))
            {
                dotsCount++;
                isWwwPrefixed = true;
            }

            for (var i = domainName.Length - 1; i >= 0; i--)
            {
                if (domainName[i] == '.')
                {
                    dotsCount--;
                }

                if (dotsCount > 0)
                    continue;

                var startIndex = 0;
                if (isWwwPrefixed)
                {
                    startIndex = 4;
                }

                return domainName.Substring(startIndex, i - startIndex);
            }

            return null;
        }
    }
}