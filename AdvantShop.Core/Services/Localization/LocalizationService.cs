using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Localization
{
    public static class LocalizationService
    {
        private static Func<Dictionary<string, LocalizedSetPair>> GetLocalizedSetsFunc =>
            () => LanguageService.GetList().ToDictionary(language => language.LanguageCode,
                language => new LocalizedSetPair { Culture = language.LanguageCode });

        private static LazyWithoutExceptionsCashing<Dictionary<string, LocalizedSetPair>> _localizedSets =
            new LazyWithoutExceptionsCashing<Dictionary<string, LocalizedSetPair>>(GetLocalizedSetsFunc);

        private static readonly object LockObj = new object();

        public static void ReloadLocalizedSets()
        {
            _localizedSets = new LazyWithoutExceptionsCashing<Dictionary<string, LocalizedSetPair>>(GetLocalizedSetsFunc);
        }
        
        public static string GetResource(string resourceKey)
        {
            var cultureName = Thread.CurrentThread.CurrentUICulture.Name;

            return GetResource(resourceKey, cultureName);
        }

        public static string GetResource(string resourceKey, string cultureName)
        {
            if (_localizedSets.Value[cultureName].Resources.TryGetValue(resourceKey, out var value))
                return value;
            
            return resourceKey; // or empty string?
        }

        public static string GetResourceFormat(string resourceKey, object param1)
        {
            return string.Format(GetResource(resourceKey), param1);
        }
        
        public static string GetResourceFormat(string resourceKey, object param1, object param2)
        {
            return string.Format(GetResource(resourceKey), param1, param2);
        }

        public static string GetResourceFormat(string resourceKey, params object[] parametres)
        {
            return string.Format(GetResource(resourceKey), parametres);
        }

        public static ConcurrentDictionary<string, string> GetResources(string cultureName)
        {
            var dict = new ConcurrentDictionary<string, string>();
            SQLDataAccess.ExecuteForeach(
                "Select ResourceKey,ResourceValue From Settings.Localization " +
                "Left Join [Settings].[Language] On [Language].[LanguageID] = [Localization].[LanguageId] " +
                "Where LanguageCode=@cultureName",
                CommandType.Text, (reader) =>
                    dict.TryAdd(
                        SQLDataHelper.GetString(reader, "ResourceKey"),
                        SQLDataHelper.GetString(reader, "ResourceValue")),
                new SqlParameter("@cultureName", cultureName));

            return dict;
        }

        public static void AddOrUpdateResource(int languageId, string resourceKey, string resourceValue, string modifiedBy = null)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_AddUpdateLocalization]",
                CommandType.StoredProcedure,
                new SqlParameter("@LanguageId", languageId),
                new SqlParameter("@ResourceKey", resourceKey),
                new SqlParameter("@ResourceValue", resourceValue),
                new SqlParameter("@ModifiedBy", modifiedBy ?? (object)DBNull.Value));

            if (_localizedSets.IsValueCreated)
            {
                var language = LanguageService.GetLanguage(languageId);
                if (language != null)
                    _localizedSets.Value[language.LanguageCode].Resources[resourceKey] = resourceValue;
            }
        }

        public static void RemoveByPattern(string key)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From [Settings].[Localization] Where ResourceKey Like '" + key.ToLower() + "%'", CommandType.Text);
            
            if (_localizedSets.IsValueCreated)
                foreach (var localizedSetPair in _localizedSets.Value.Values)
                    foreach (var pair in localizedSetPair.Resources)
                        if (pair.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                            localizedSetPair.Resources.TryRemove(pair.Key, out _);
        }

        public static void GenerateJsResourcesFile()
        {
            var cultureName = SettingsMain.Language.ToLower();

            lock (LockObj)
            {
                GenerateJsResourcesFile(HostingEnvironment.MapPath("~/userfiles/"), cultureName, false);
                GenerateJsResourcesFile(HostingEnvironment.MapPath("~/userfiles/"), cultureName, true);
            }
        }

        private static void GenerateJsResourcesFile(string localizationDirPath, string cultureName, bool isAdmin)
        {
            var localizationFilePath = localizationDirPath + "\\" + (isAdmin ? "admin_" : "") + cultureName + ".js";

            FileHelpers.CreateDirectory(localizationDirPath);

            Dictionary<string, string> jsResources;

            try
            {
                jsResources = SQLDataAccess.ExecuteReadDictionary<string, string>(
                    "Select ResourceKey,ResourceValue From Settings.Localization " +
                    "Left Join [Settings].[Language] On [Language].[LanguageID] = [Localization].[LanguageId] " +
                    "Where LanguageCode=@CultureName and ResourceKey like " + (!isAdmin ? "'Js.%'" : "'Admin.Js.%'"),
                    CommandType.Text,
                    "ResourceKey", "ResourceValue",
                    new SqlParameter("@CultureName", cultureName));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return;
            }

            var jsResourceObject =
                string.Format(isAdmin ? "window.AdvantshopAdminResource = {0};" : "window.AdvantshopResource = {0};",
                    JsonConvert.SerializeObject(jsResources));

            using (var sw = new StreamWriter(localizationFilePath))
            {
                sw.Write(jsResourceObject);
            }
        }
    }
}
