using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public enum ECongratulationsDashboardStep
    {
        StoreInfo = 0,
        Product = 1,
        Design = 2,
        Domain = 3,
    }

    public class SettingsCongratulationsDashboard
    {
        public static bool SkipCongratulationsDashboard
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.SkipCongratulationsDashboard"]);
            set => SettingProvider.Items["SettingsCD.SkipCongratulationsDashboard"] = value.ToString();
        }

        public static bool AllDone => StoreInfoDone && ProductDone && DesignDone && DomainDone;

        public static bool DesignDone
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.DesignDone"]);
            set => SettingProvider.Items["SettingsCD.DesignDone"] = value.ToString();
        }

        public static bool ProductDone
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.ProductDone"]);
            set => SettingProvider.Items["SettingsCD.ProductDone"] = value.ToString();
        }

        public static bool DomainDone
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.DomenDone"]);
            set => SettingProvider.Items["SettingsCD.DomenDone"] = value.ToString();
        }

        public static bool StoreInfoDone
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.StoreInfoDone"]);
            set => SettingProvider.Items["SettingsCD.StoreInfoDone"] = value.ToString();
        }

        public static bool NotFirstVisit
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsCD.NotFirstVisit"]);
            set => SettingProvider.Items["SettingsCD.NotFirstVisit"] = value.ToString();
        }
    }
}