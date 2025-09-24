using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Crm.Instagram;
using AdvantShop.Core.Services.Crm.Vk;
using AdvantShop.Core.Services.Crm.Vk.VkMarket.Export;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.Scheduler
{
    public static class JobActivationManager
    {
        private static readonly Dictionary<string, Func<bool>> ActivationKeys = new Dictionary<string, Func<bool>>
        {
            { "IsSaas", () => SaasDataService.IsSaasEnabled },
            { "IsNotTrial", () => TrialService.IsTrialEnabled is false },
            { "IsNotDemo", () => Demo.IsDemoEnabled is false },
            { "DeferredTasks", () => SaasDataService.IsEnabledFeature(ESaasProperty.BizProcess) },
            { "CustomerSegmets", () => SaasDataService.IsEnabledFeature(ESaasProperty.HaveCustomerSegmets) },
            { "Booking", () => SettingsMain.BookingActive },
            { "Partner", () => SettingsMain.PartnersActive },
            { "BonusSystem", () => BonusSystem.IsActive },
            { "Landings", () => SettingsLandingPage.ActiveLandingPage },
            { "DeferredMails", () => SettingsLandingPage.ActiveLandingPage && SettingsLandingPage.UseCrossSellLandingsInCheckout },
            { "TaskReminder", () => SettingsTasks.ReminderActive },
            { "AutoUpdateCurrencies", () => SettingsMain.EnableAutoUpdateCurrencies },
            { "Imap", () => SettingsMail.ImapHost.IsNotEmpty()  },
            { "VkMessages", () => SettingsMain.VkChannelActive && VkApiService.IsVkActive() },
            { "VkOrders", () => SettingsMain.VkChannelActive && SettingsVk.SyncOrdersFromVk && VkApiService.IsVkActive() },
            { "VkMarket", () => SettingsMain.VkChannelActive && VkMarketExportSettings.ExportOnShedule && VkApiService.IsVkActive() },
            { "OkMarket", () => SettingsMain.OkChannelActive && SettingsOk.IsOkSettingsConfigured },
            { "Instagram", () => SettingsMain.InstagramChannelActive && Instagram.Instance.IsActive() },
            { "Triggers", () => SettingsMain.TriggersActive },
            { "CheckSendingMails", () => SaasDataService.IsSaasEnabled && Demo.IsDemoEnabled is false }
        };

        public static void SettingUpdated() => Task.Run(TaskManager.TaskManagerInstance().InitWebConfigJobs);

        public static bool GetActivityStatus(string key)
        {
            var activatorExists = ActivationKeys.TryGetValue(key, out var activator);
            return activatorExists && activator.Invoke();
        }
    }
}
