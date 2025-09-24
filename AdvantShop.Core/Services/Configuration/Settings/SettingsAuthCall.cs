using AdvantShop.Configuration;
using AdvantShop.Core.Services.Auth.Calls;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsAuthCall
    {
        public static string ActiveAuthCallModule
        {
            get => SettingProvider.Items["ActiveAuthCallModule"];
            set => SettingProvider.Items["ActiveAuthCallModule"] = value;
        }

        public static EAuthCall AuthCallMode
        {
            get => (EAuthCall)SQLDataHelper.GetInt(SettingProvider.Items["AuthCallMode"]);
            set => SettingProvider.Items["AuthCallMode"] = ((int)value).ToString();
        }
    }
}