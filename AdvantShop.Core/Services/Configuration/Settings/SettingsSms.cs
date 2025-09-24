using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsSms
    {
        public static string AdminPhone
        {
            get => SettingProvider.Items["AdminPhone"];
            set => SettingProvider.Items["AdminPhone"] = value;
        }
        
        public static bool SendSmsToCustomerOnNewOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToCustomerOnNewOrder"]);
            set => SettingProvider.Items["SendSmsToCustomerOnNewOrder"] = value.ToString();
        }

        public static bool SendSmsToAdminOnNewOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnNewOrder"]);
            set => SettingProvider.Items["SendSmsToAdminOnNewOrder"] = value.ToString();
        }

        public static bool SendSmsToAdminOnNewLead
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnNewLead"]);
            set => SettingProvider.Items["SendSmsToAdminOnNewLead"] = value.ToString();
        }

        public static string SmsTextOnNewOrder
        {
            get => SettingProvider.Items["SmsTextOnNewOrder"];
            set => SettingProvider.Items["SmsTextOnNewOrder"] = value;
        }

        public static string SmsTextOnNewLead
        {
            get => SettingProvider.Items["SmsTextOnNewLead"];
            set => SettingProvider.Items["SmsTextOnNewLead"] = value;
        }


        public static bool SendSmsToCustomerOnOrderStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToCustomerOnOrderStatusChanging"]);
            set => SettingProvider.Items["SendSmsToCustomerOnOrderStatusChanging"] = value.ToString();
        }

        public static bool SendSmsToAdminOnOrderStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnOrderStatusChanging"]);
            set => SettingProvider.Items["SendSmsToAdminOnOrderStatusChanging"] = value.ToString();
        }
        
        public static string SmsTextOnOrderPayStatusChanging
        {
            get => SettingProvider.Items["SmsTextOnOrderPayStatusChanging"];
            set => SettingProvider.Items["SmsTextOnOrderPayStatusChanging"] = value;
        }
        
        public static bool SendSmsToCustomerOnOrderPayStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToCustomerOnOrderPayStatusChanging"]);
            set => SettingProvider.Items["SendSmsToCustomerOnOrderPayStatusChanging"] = value.ToString();
        }
        
        public static bool SendSmsToAdminOnOrderPayStatusChanging
        {
            get => Convert.ToBoolean(SettingProvider.Items["SendSmsToAdminOnOrderPayStatusChanging"]);
            set => SettingProvider.Items["SendSmsToAdminOnOrderPayStatusChanging"] = value.ToString();
        }

        public static string ActiveSmsModule
        {
            get => SettingProvider.Items["ActiveSmsModule"];
            set => SettingProvider.Items["ActiveSmsModule"] = value;
        }
        
        public static bool SmsTesting
        {
            get => Convert.ToBoolean(SettingProvider.Items["SmsTesting"]);
            set => SettingProvider.Items["SmsTesting"] = value.ToString();
        }

        public static SmsBanLevel SmsBanLevel
        {
            get => (SmsBanLevel)Convert.ToInt32(SettingProvider.Items["SmsBanLevel"]);
            set => SettingProvider.Items["SmsBanLevel"] = ((int)value).ToString();
        }
    }

    public enum SmsBanLevel
    {
        [Localize("Core.SettingsSms.SmsBanLevel.Normal")]
        Normal = 0,
        
        [Localize("Core.SettingsSms.SmsBanLevel.Middle")]
        Middle = 1,
        
        [Localize("Core.SettingsSms.SmsBanLevel.High")]
        High = 2
    }
}
