//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Saas;

namespace AdvantShop.Configuration
{
    /// <summary>
    /// Setting provider
    /// </summary>
    /// <remarks></remarks>
    public class SettingProvider
    {
        private const string ConfigSettingValueCacheKey = "ConfigSettingValue_";
        private const string InternalSettingValueCacheKey = "InternalSettingValue_";
        private static LazyWithoutExceptionsCashing<ConcurrentDictionary<string, string>> _settings = new LazyWithoutExceptionsCashing<ConcurrentDictionary<string, string>>(GetAllSettings);
        //private static object _lock = new object();

        private static readonly ConcurrentDictionary<string, object> SyncLocks = new ConcurrentDictionary<string, object>();

        public sealed class SettingIndexer
        {
            public string this[string name]
            {
                get => GetSqlSettingValue(name);
                set => SetSqlSettingValue(name, value);
            }
        }

        private static SettingIndexer _staticIndexer;
        public static SettingIndexer Items => _staticIndexer ?? (_staticIndexer = new SettingIndexer());

        #region  SQL storage

        /// <summary>
        /// Save settings into DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <remarks></remarks>
        public static void SetSqlSettingValue(string name, string value)
        {
            var syncObj = SyncLocks.GetOrAdd(name, _ => new object());
            lock (syncObj)
            {
                if (value == null)
                {
                    SQLDataAccess.ExecuteNonQuery("Delete from settings.settings where Name=@Name", CommandType.Text,
                        new SqlParameter("@Name", name.Trim()));

                    if (_settings.IsValueCreated && _settings.Value != null && _settings.Value.ContainsKey(name))
                        _settings.Value.TryRemove(name, out _);
                }
                else
                {
                    SQLDataAccess.ExecuteNonQuery("[Settings].[sp_UpdateSettings]", CommandType.StoredProcedure,
                        new SqlParameter("@Name", name.Trim()), new SqlParameter("@Value", value));

                    if (_settings.IsValueCreated && _settings.Value != null)
                        _settings.Value[name] = value;
                }
            }
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSqlSettingValue(string key)
        {
            var settings = _settings.Value;

            if (settings.TryGetValue(key, out var value))
                return value;

            return null;
        }

        public static string GetSqlSettingValue(EProviderSetting key)
        {
            if (key == EProviderSetting.StoreActive &&
                SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.DisableStore)
            {
                return "False";
            }
            
            return GetSqlSettingValue(GetSettingName(key));
        }

        public static void SetSqlSettingValue(EProviderSetting key, string value)
        {
            if (key == EProviderSetting.StoreActive && 
                value.Equals("True", StringComparison.OrdinalIgnoreCase) &&
                SaasDataService.IsSaasEnabled && SaasDataService.CurrentSaasData.DisableStore)
            {
                throw new BlException(LocalizationService.GetResource("Core.SettingProvider.FunctionIsNotAvailable"));
            }
            
            SetSqlSettingValue(GetSettingName(key), value);
        }

        private static string GetSettingName(EProviderSetting key)
        {
            switch (key)
            {
                case EProviderSetting.StoreActive:
                    return "StoreActive";
                case EProviderSetting.ActiveLandingPage:
                    return "ActiveLandingPage";
                case EProviderSetting.ActiveBonusSystem:
                    return "BonusAppActive";
                case EProviderSetting.CrmActive:
                    return "CrmActive";
                case EProviderSetting.TasksActive:
                    return "TasksActive";
                case EProviderSetting.BookingActive:
                    return "BookingActive";
                case EProviderSetting.TriggersActive:
                    return "TriggersActive";
                case EProviderSetting.PartnersActive:
                    return "PartnersActive";                
                case EProviderSetting.FunnelBlocks:
                    return "Features.EnableFunnelBlocks";
                case EProviderSetting.ExperimentalFeatures:
                    return "Features.EnableExperimentalFeatures";
                default:
                    return key.ToString();
            }
        }

        private static ConcurrentDictionary<string, string> GetAllSettings()
        {
            var settings = new ConcurrentDictionary<string, string>();
            SQLDataAccess.ExecuteForeach("SELECT [Name],[Value] FROM [Settings].[Settings]",
                    CommandType.Text, (reader) =>
                        settings.TryAdd(SQLDataHelper.GetString(reader, "Name"), SQLDataHelper.GetString(reader, "Value")));

            return settings;
        }

        public static void ClearSettings()
        {
            _settings = new LazyWithoutExceptionsCashing<ConcurrentDictionary<string, string>>(GetAllSettings);
        }


        #endregion


        #region Internal Settings

        public static string GetInternalSetting(string key)
        {
            return CacheManager.Get<string>(InternalSettingValueCacheKey + key,
                () => SQLDataAccess.ExecuteScalar<string>("[Settings].[sp_GetInternalSetting]", CommandType.StoredProcedure, new SqlParameter("@settingKey", key)));
        }

        #endregion


        #region  Web.config storage

        /// <summary>
        /// Read settings from appSettings node web.config file.
        /// On Err: Function will return an empty string
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetConfigSettingValue(string strKey)
        {
            var cacheKey = ConfigSettingValueCacheKey + strKey;

            if (!CacheManager.TryGetValue(cacheKey, out string value))
            {
                var config = new AppSettingsReader();
                value = config.GetValue(strKey, typeof(String)).ToString();

                if (value != null)
                    CacheManager.Insert(cacheKey, value);
            }

            return value;
        }

        public static T GetConfigSettingValue<T>(string strKey)
        {
            var cacheKey = ConfigSettingValueCacheKey + strKey;

            if (!CacheManager.TryGetValue(cacheKey, out T value))
            {
                var config = new AppSettingsReader();
                value = (T)config.GetValue(strKey, typeof(T));

                if (value != null)
                    CacheManager.Insert(cacheKey, value);
            }

            return value;
        }

        /// <summary>
        /// Save settings from appSettings node web.config
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strValue"></param>
        /// <remarks></remarks>
        public static bool SetConfigSettingValue(string strKey, string strValue)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            var myAppSettings = (AppSettingsSection)config.GetSection("appSettings");
            myAppSettings.Settings[strKey].Value = strValue;
            config.Save();

            return true;
        }

        #endregion

    }
}