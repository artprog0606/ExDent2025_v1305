using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Crm.Vk
{
    public class VkUserTokenTimer
    {
        private static readonly object Lock = new object();
        
        public static bool NeedRefresh()
        {
            lock (Lock)
            {
                if (SettingsVk.IsOldIntegration)
                    return false;
                
                var lastRefreshTimeUtc = SettingsVk.UserTokenLastRefreshTimeUtc;

                if (lastRefreshTimeUtc.HasValue)
                    return (DateTime.UtcNow - lastRefreshTimeUtc.Value).TotalMinutes > 50;

                SettingsVk.UserTokenLastRefreshTimeUtc = DateTime.UtcNow;

                return false;
            }
        } 
    }
}