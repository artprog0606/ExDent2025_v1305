using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Settings
{
    public class SettingsResponse : List<SettingItemResponse>, IApiResponse
    {
        public SettingsResponse(List<SettingItemResponse> items)
        {
            this.AddRange(items);
        }
    }
    
    public class SettingsResponseV2 : Dictionary<string, object>, IApiResponse
    {
        public SettingsResponseV2(List<SettingItemResponse> items)
        {
            foreach (var item in items)
                if (!this.ContainsKey(item.Key))
                    this.Add(item.Key, item.Value);
        }
    }
    
    public class SettingItemResponse
    {
        public string Key { get; private set; }
        public object Value { get; private set; }

        public SettingItemResponse(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}