using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;

namespace AdvantShop.Configuration
{
    public class SettingsTasks
    {
        public static bool TasksActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["TasksActive"]);
            set => SettingProvider.Items["TasksActive"] = value.ToString();
        }

        public static bool ReminderActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["ReminderActive"]);
            set
            {
                SettingProvider.Items["ReminderActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }
    }
}