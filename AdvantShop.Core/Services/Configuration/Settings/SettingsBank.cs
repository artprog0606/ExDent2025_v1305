//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Configuration
{

    public class SettingsBank
    {
        public static string INN
        {
            get => SettingProvider.Items["INN"];
            set => SettingProvider.Items["INN"] = value;
        }
        public static string RS
        {
            get => SettingProvider.Items["RS"];
            set => SettingProvider.Items["RS"] = value;
        }
        public static string Director
        {
            get => SettingProvider.Items["Director"];
            set => SettingProvider.Items["Director"] = value;
        }
        public static string Manager
        {
            get => SettingProvider.Items["Manager"];
            set => SettingProvider.Items["Manager"] = value;
        }
        public static string Accountant
        {
            get => SettingProvider.Items["Accountant"];
            set => SettingProvider.Items["Accountant"] = value;
        }

        public static string StampImageName
        {
            get => SettingProvider.Items["StampImage"];
            set => SettingProvider.Items["StampImage"] = value;
        }

        public static string CompanyName
        {
            get => SettingProvider.Items["CompanyName"];
            set => SettingProvider.Items["CompanyName"] = value;
        }
        public static string KPP
        {
            get => SettingProvider.Items["KPP"];
            set => SettingProvider.Items["KPP"] = value;
        }
        public static string BankName
        {
            get => SettingProvider.Items["BankName"];
            set => SettingProvider.Items["BankName"] = value;
        }
        public static string KS
        {
            get => SettingProvider.Items["KS"];
            set => SettingProvider.Items["KS"] = value;
        }
        public static string BIK
        {
            get => SettingProvider.Items["BIK"];
            set => SettingProvider.Items["BIK"] = value;
        }
        public static string Address
        {
            get => SettingProvider.Items["Address"];
            set => SettingProvider.Items["Address"] = value;
        }
    }
}
