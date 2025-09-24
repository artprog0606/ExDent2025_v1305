using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class SettingsTracking
    {
        public static DateTime? AdminAreaVisitDate
        {
            get => SettingProvider.Items["SettingsTracking.AdminAreaVisitDate"].TryParseDateTime(isNullable: true);
            set => SettingProvider.Items["SettingsTracking.AdminAreaVisitDate"] = value.HasValue ? value.ToString() : string.Empty;
        }

        public static DateTime? TrialCreatedDate
        {
            get => SettingProvider.Items["SettingsTracking.TrialCreatedDate"].TryParseDateTime(isNullable: true);
            set => SettingProvider.Items["SettingsTracking.TrialCreatedDate"] = value.HasValue ? value.ToString() : string.Empty;
        }

        public static List<string> TrackedEvents
        {
            get
            {
                var settingsValue = SettingProvider.Items["SettingsTracking.TrackedEvents"];
                return settingsValue.IsNullOrEmpty() 
                    ? new List<string>() 
                    : settingsValue.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            }
            set =>
                SettingProvider.Items["SettingsTracking.TrackedEvents"] = value != null && value.Any() 
                    ? value.AggregateString(',')
                    : string.Empty;
        }

        public static int MaxEventsPerHour => SettingProvider.Items["SettingsTracking.MaxEventsPerHour"].TryParseInt(defaultValue: 10);
    }
}