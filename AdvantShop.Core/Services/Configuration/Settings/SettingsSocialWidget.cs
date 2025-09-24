using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsSocialWidget
    {
        public static bool IsActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsActive"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsActive"] = value.ToString();
        }

        public static bool IsShowVkInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowVkInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowVkInDesktop"] = value.ToString();
        }
        public static bool IsShowVkInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowVkInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowVkInMobile"] = value.ToString();
        }

        public static bool IsShowFbInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowFbInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowFbInDesktop"] = value.ToString();
        }
        public static bool IsShowFbInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowFbInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowFbInMobile"] = value.ToString();
        }

        public static bool IsShowJivositeInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowJivositeInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowJivositeInDesktop"] = value.ToString();
        }
        public static bool IsShowJivositeInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowJivositeInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowJivositeInMobile"] = value.ToString();
        }

        public static bool IsShowCallbackInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowCallbackInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowCallbackInDesktop"] = value.ToString();
        }
        public static bool IsShowCallbackInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowCallbackInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowCallbackInMobile"] = value.ToString();
        }
        
        public static bool IsShowWhatsAppInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowWhatsAppInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowWhatsAppInMobile"] = value.ToString();
        }
        public static bool IsShowWhatsAppInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowWhatsAppInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowWhatsAppInDesktop"] = value.ToString();
        }
        public static string WhatsAppPhone
        {
            get => SettingProvider.Items["SettingsSocialWidget.WhatsAppPhone"];
            set => SettingProvider.Items["SettingsSocialWidget.WhatsAppPhone"] = value;
        }
        
        public static bool IsShowViberInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowViberInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowViberInMobile"] = value.ToString();
        }
        public static bool IsShowViberInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowViberInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowViberInDesktop"] = value.ToString();
        }
        public static string ViberPhone
        {
            get => SettingProvider.Items["SettingsSocialWidget.ViberPhone"];
            set => SettingProvider.Items["SettingsSocialWidget.ViberPhone"] = value;
        }

        public static bool IsShowOdnoklassnikiInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowOdnoklassnikiInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowOdnoklassnikiInDesktop"] = value.ToString();
        }
        public static bool IsShowOdnoklassnikiInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowOdnoklassnikiInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowOdnoklassnikiInMobile"] = value.ToString();
        }
        public static string LinkOdnoklassniki
        {
            get => SettingProvider.Items["SettingsSocialWidget.LinkOdnoklassniki"];
            set => SettingProvider.Items["SettingsSocialWidget.LinkOdnoklassniki"] = value;
        }

        public static bool IsShowTelegramInDesktop
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowTelegramInDesktop"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowTelegramInDesktop"] = value.ToString();
        }
        public static bool IsShowTelegramInMobile
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.IsShowTelegramInMobile"]);
            set => SettingProvider.Items["SettingsSocialWidget.IsShowTelegramInMobile"] = value.ToString();
        }
        public static string LinkTelegram
        {
            get => SettingProvider.Items["SettingsSocialWidget.LinkTelegram"];
            set => SettingProvider.Items["SettingsSocialWidget.LinkTelegram"] = value;
        }

        public static string CustomLink1
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLink1"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLink1"] = value;
        }

        public static string CustomLinkText1
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkText1"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkText1"] = value;
        }

        public static string CustomLinkIcon1
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon1"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon1"] = value;
        }

        public static string CustomLink2
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLink2"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLink2"] = value;
        }

        public static string CustomLinkText2
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkText2"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkText2"] = value;
        }

        public static string CustomLinkIcon2
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon2"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon2"] = value;
        }

        public static string CustomLink3
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLink3"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLink3"] = value;
        }

        public static string CustomLinkText3
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkText3"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkText3"] = value;
        }
        
        public static string CustomLinkIcon3
        {
            get => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon3"];
            set => SettingProvider.Items["SettingsSocialWidget.CustomLinkIcon3"] = value;
        }

        public static bool ShowPopover
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsSocialWidget.ShowPopover"]);
            set => SettingProvider.Items["SettingsSocialWidget.ShowPopover"] = value.ToString();    
        }
        
        public static string PopoverText
        {
            get => SettingProvider.Items["SettingsSocialWidget.PopoverText"];
            set => SettingProvider.Items["SettingsSocialWidget.PopoverText"] = value;
        }

    }
}
