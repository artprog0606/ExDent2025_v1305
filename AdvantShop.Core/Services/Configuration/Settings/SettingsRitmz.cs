//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Configuration
{
    public class SettingsRitmz
    {
        public static string RitmzLogin
        {
            get => SettingProvider.Items["RitmzLogin"];
            set => SettingProvider.Items["RitmzLogin"] = value;
        }
        public static string RitmzPassword
        {
            get => SettingProvider.Items["RitmzPassword"];
            set => SettingProvider.Items["RitmzPassword"] = value;
        }
    }
}
