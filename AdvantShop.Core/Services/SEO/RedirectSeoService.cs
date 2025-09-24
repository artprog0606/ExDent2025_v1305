//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;

namespace AdvantShop.SEO
{
    public static class RedirectSeoService
    {
        private const string RedirectsCahcheName = "redirectsCache";
        private const string RedirectsCahcheNameRedirects = RedirectsCahcheName + "Redirects";
        private const string RedirectsCahcheNameRedirectsWithStar = RedirectsCahcheName + "RedirectsWithStar";

        private static void CleanRedirectsCache()
        {
            CacheManager.RemoveByPattern(RedirectsCahcheName);
        }

        public static RedirectSeo GetRedirectSeoById(int id)
        {
            return SQLDataAccess.ExecuteReadOne<RedirectSeo>(
                "SELECT * FROM [Settings].[Redirect] WHERE ID = @ID",
                CommandType.Text,
                GetRedirectSeoFromReader,
                new SqlParameter("@ID", id));
        }

        public static IEnumerable<RedirectSeo> GetRedirectsSeo()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<RedirectSeo>(
                "SELECT * FROM [Settings].[Redirect]",
                CommandType.Text,
                GetRedirectSeoFromReader);
        }


        public static RedirectSeo GetByInputUrl(string relativeUrl, string absoluteUrl)
        {
            List<RedirectSeo> redirectsWithStar;
            Dictionary<string, RedirectSeo> redirects;

            if (CacheManager.Contains(RedirectsCahcheNameRedirects) && CacheManager.Contains(RedirectsCahcheNameRedirectsWithStar))
            {
                redirects = CacheManager.Get<Dictionary<string, RedirectSeo>>(RedirectsCahcheNameRedirects);
                redirectsWithStar = CacheManager.Get<List<RedirectSeo>>(RedirectsCahcheNameRedirectsWithStar);
            }
            else
            {
                redirects = new Dictionary<string, RedirectSeo>();
                redirectsWithStar = new List<RedirectSeo>();

                var allRedirects = SQLDataAccess.ExecuteReadIEnumerable("SELECT * FROM [Settings].[Redirect]", CommandType.Text, GetRedirectSeoFromReader);

                foreach (var redirectSeo in allRedirects)
                {
                    redirectSeo.RedirectFrom = redirectSeo.RedirectFrom.TrimStart('/');
                    if (!redirectSeo.RedirectFrom.Contains('*'))
                    {
                        if (!redirects.ContainsKey(redirectSeo.RedirectFrom))
                            redirects.Add(redirectSeo.RedirectFrom, redirectSeo);
                    }
                    else
                    {
                        redirectsWithStar.Add(redirectSeo);
                    }
                }

                CleanRedirectsCache();
                CacheManager.Insert(RedirectsCahcheNameRedirects, redirects, 60);
                CacheManager.Insert(RedirectsCahcheNameRedirectsWithStar, redirectsWithStar, 60);
            }

            var redirectWithStar = redirectsWithStar.Find(x => ImitateSqlLike(relativeUrl, x.RedirectFrom)
                                                            || ImitateSqlLike(absoluteUrl, x.RedirectFrom)
                                                            || ImitateSqlLike(HttpUtility.UrlPathEncode(relativeUrl), x.RedirectFrom)
                                                            || ImitateSqlLike(HttpUtility.UrlPathEncode(absoluteUrl), x.RedirectFrom));
            if (redirectWithStar != null)
            {
                return new RedirectSeo
                {
                    ID = redirectWithStar.ID,
                    ProductArtNo = redirectWithStar.ProductArtNo,
                    RedirectFrom = redirectWithStar.RedirectFrom.Replace("*", ""),
                    RedirectTo = redirectWithStar.RedirectTo
                };
            }

            RedirectSeo redirect = null;
            if (redirects.TryGetValue(relativeUrl, out redirect)
                || redirects.TryGetValue(absoluteUrl, out redirect)
                || redirects.TryGetValue(HttpUtility.UrlPathEncode(relativeUrl), out redirect) 
                || redirects.TryGetValue(HttpUtility.UrlPathEncode(absoluteUrl), out redirect)
                || redirects.TryGetValue(HttpUtility.UrlDecode(relativeUrl), out redirect)
                )
            {
                return new RedirectSeo
                {
                    ID = redirect.ID,
                    ProductArtNo = redirect.ProductArtNo,
                    RedirectFrom = redirect.RedirectFrom,
                    RedirectTo = redirect.RedirectTo
                };
            }

            return null;
        }

        public static RedirectSeo GetRedirectsSeoByRedirectFrom(string redirectFrom)
        {
            return SQLDataAccess.ExecuteReadOne<RedirectSeo>(
                "SELECT * FROM [Settings].[Redirect] WHERE RedirectFrom = @RedirectFrom",
                CommandType.Text,
                GetRedirectSeoFromReader,
                new SqlParameter("@RedirectFrom", redirectFrom));
        }


        private static bool ImitateSqlLike(string input, string source)
        {
            var cleanSource = source.Replace("*", "");
            var index = source.IndexOf('*');

            if (index == 0)
                return input.EndsWith(cleanSource);

            if (index == source.Length - 1)
                return input.StartsWith(cleanSource);

            return input.Contains(cleanSource);
        }

        private static RedirectSeo GetRedirectSeoFromReader(SqlDataReader reader)
        {
            return new RedirectSeo
            {
                ID = SQLDataHelper.GetInt(reader, "ID"),
                RedirectFrom = SQLDataHelper.GetString(reader, "RedirectFrom").ToLower(),
                RedirectTo = SQLDataHelper.GetString(reader, "RedirectTo").ToLower(),
                ProductArtNo = SQLDataHelper.GetString(reader, "ProductArtNo"),
                Created = SQLDataHelper.GetDateTime(reader, "Created"),
                Edited = SQLDataHelper.GetDateTime(reader, "Edited")
            };
        }

        public static void AddRedirectSeo(RedirectSeo redirectSeo)
        {
            redirectSeo.ID =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[Redirect] ([RedirectFrom], [RedirectTo], [ProductArtNo], [Created], [Edited]) VALUES (@RedirectFrom, @RedirectTo, @ProductArtNo, @Created, @Edited); SELECT SCOPE_IDENTITY();",
                    CommandType.Text,
                    new SqlParameter("@RedirectFrom", redirectSeo.RedirectFrom),
                    new SqlParameter("@RedirectTo", redirectSeo.RedirectTo),
                    new SqlParameter("@ProductArtNo", redirectSeo.ProductArtNo),
                    new SqlParameter("@Created", System.DateTime.Now),
                    new SqlParameter("@Edited", System.DateTime.Now)
                    );
            CleanRedirectsCache();
        }

        public static void UpdateRedirectSeo(RedirectSeo redirectSeo)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[Redirect] SET RedirectFrom = @RedirectFrom, RedirectTo = @RedirectTo, ProductArtNo = @ProductArtNo, Edited = @Edited WHERE ID = @ID", CommandType.Text,
                                                new SqlParameter("@ID", redirectSeo.ID),
                                                new SqlParameter("@RedirectFrom", redirectSeo.RedirectFrom),
                                                new SqlParameter("@RedirectTo", redirectSeo.RedirectTo),
                                                new SqlParameter("@ProductArtNo", redirectSeo.ProductArtNo),
                                                new SqlParameter("@Edited", System.DateTime.Now)
                                         );
            CleanRedirectsCache();
        }

        public static void DeleteRedirectSeo(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[Redirect] WHERE ID = @ID", CommandType.Text, new SqlParameter("@ID", id));
            CleanRedirectsCache();
        }
        
        /// <summary>
        /// Проверка на количество редиректов.
        /// Если редиректов больше или равно 10, то возвращает true.
        /// </summary>
        public static bool IsToManyRedirects(RedirectSeo redirectSeo)
        {
            List<RedirectSeo> redirectsWithStar;
            Dictionary<string, RedirectSeo> redirects;


            //если кэш есть, очищаем
            if (CacheManager.Contains(RedirectsCahcheNameRedirects) && CacheManager.Contains(RedirectsCahcheNameRedirectsWithStar))
            {
                CleanRedirectsCache();
            }
            
            redirects = new Dictionary<string, RedirectSeo>();
            redirectsWithStar = new List<RedirectSeo>();
            
            //берём все редиректы
            var allRedirects = GetRedirectsSeo();

            //разделяем их
            foreach (var redirect in allRedirects)
            {
                if (!redirect.RedirectFrom.Contains('*'))
                {
                    if (!redirects.ContainsKey(redirect.RedirectFrom))
                        redirects.Add(redirect.RedirectFrom, redirect);
                }
                else
                {
                    redirectsWithStar.Add(redirect);
                }
            }

            redirectSeo.RedirectFrom = HttpUtility.UrlDecode(redirectSeo.RedirectFrom);
            redirectSeo.RedirectTo = HttpUtility.UrlDecode(redirectSeo.RedirectTo);

            //таже добавляем к редиректам добавленные/обновлённые данные
            if (redirectSeo.RedirectFrom.Contains('*'))
            {
                if (redirectsWithStar.FirstOrDefault(x => x.ID == redirectSeo.ID) == null)
                {
                    //если нет, добавляем для проверки новый
                    redirectsWithStar.Add(redirectSeo);
                }
                else
                {
                    //если есть, то удаляем, и добавляем новый чтобы обновить запись
                    redirectsWithStar.Remove(redirectsWithStar.FirstOrDefault(x => x.ID == redirectSeo.ID));
                    redirectsWithStar.Add(redirectSeo);
                }
            }
            else
            {
                //проверяем содержится ли в массиве
                if (redirects.FirstOrDefault(x => x.Value.ID == redirectSeo.ID).Value == null)
                {
                    //если нет, добавляем для проверки новый
                    redirects.Add(redirectSeo.RedirectFrom, redirectSeo);
                }
                else
                {
                    //если есть, то удаляем, и добавляем новый чтобы обновить запись
                    redirects.Remove(redirects.FirstOrDefault(x => x.Value.ID == redirectSeo.ID).Key);
                    redirects.Add(redirectSeo.RedirectFrom, redirectSeo);
                }
            }

            //добавляем в кэш
            //для того, чтобы были актуальные данные по редиректам
            CleanRedirectsCache();
            CacheManager.Insert(RedirectsCahcheNameRedirects, redirects, 1);
            CacheManager.Insert(RedirectsCahcheNameRedirectsWithStar, redirectsWithStar, 1);

            var redirectTo = redirectSeo.RedirectTo;
            int counter;

            //с помощью цикла проверяем существуют ли переадресации
            for (counter = 0; counter < 10; counter++)
            {
                var relativeUrl = string.Empty;
                //если это абсолютные url, то получаем из нее относительную
                //если это относительная url, то делаем из нее абсолютную
                if (redirectTo.Contains("http://") || redirectTo.Contains("https://"))
                {
                    relativeUrl = HttpUtility.UrlDecode(new System.Uri(redirectTo).AbsolutePath);
                }
                else
                {
                    relativeUrl = HttpUtility.UrlDecode(redirectTo);
                    redirectTo = UrlService.GenerateBaseUrl() + "/" + relativeUrl.Trim('/');
                }

                //проверяем на редиректы
                var redirect = UrlService.GetRedirect301(relativeUrl.Trim('/'), redirectTo.Trim('/'));
                if (redirect.IsNullOrEmpty())
                {
                    CleanRedirectsCache();
                    return false;
                }
                else
                {
                    redirectTo = redirect;
                }
            }
            
            CleanRedirectsCache();
            return true;
        }

        /// <summary>
        /// Проверка на системный url
        /// </summary>
        /// <returns>Возвращает true если url относися к системным</returns>
        public static bool CheckOnSystemUrl(string url)
        {
            var systemUrls = GetSystemUrls();
            Uri uri;
            return systemUrls.Any(x => url.ToLower().Contains(x.ToLower())) || (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri) && SettingsMain.IsTechDomain(uri));
        }

        private static List<string> GetSystemUrls()
        {
            return new List<string>
            {
                "location/getcurrentzone",
                "buymore/minicartmessage",
                "webhook/advantshopevents/processdeferredtasks",
            };
        }
    }
}