using AdvantShop.Configuration;
using AdvantShop.Core.Services.Helpers;
using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Configuration
{
    public class CapSettingProvider
    {
        private static Dictionary<string, string> _settings;
        private static string urlHost = SettingsLic.EmailServiceUrl;

        public sealed class CapShopSettingIndexer
        {
            public string this[string name]
            {
                get { return GetCapSettingValue(name); }
                //set { SetCapSettingValue(name, value); }
            }
        }

        private static CapShopSettingIndexer _staticIndexer;
        public static CapShopSettingIndexer Items
        {
            get { return _staticIndexer ?? (_staticIndexer = new CapShopSettingIndexer()); }
        }

        private static Dictionary<string, string> GetAllSettings()
        {
            var urlAction = "v1/shop/settings/" + SettingsLic.LicKey;
            var temp = RequestHelper.MakeRequest<Dictionary<string, string>>(urlHost + urlAction, method: ERequestMethod.GET);
            var tempIgnoreCase = new Dictionary<string, string>(temp, StringComparer.OrdinalIgnoreCase);
            return tempIgnoreCase;
        }

        public static void SetCapSettingValue(string name, string value)
        {
            var urlAction = "v1/shop/settings/" + SettingsLic.LicKey;
            RequestHelper.MakeRequest<string>(urlHost + urlAction, new KeyValuePair<string, string>(name, value));
            _settings = null;
        }

        public static string GetCapSettingValue(string key)
        {
            var settings = _settings ?? (_settings = GetAllSettings());

            string value;

            if (settings != null && settings.TryGetValue(key.ToLower(), out value))
                return value;

            return null;
        }

        public static void Reset()
        {
            _settings = null;
        }
    }
}
