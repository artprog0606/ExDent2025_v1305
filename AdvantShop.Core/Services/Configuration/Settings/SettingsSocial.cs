//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Configuration
{
    public class SettingsSocial
    {
        public static string SocialShareCustomCode
        {
            get => SettingProvider.Items["SocialShareCustomCode"];
            set => SettingProvider.Items["SocialShareCustomCode"] = value;
        }

        public static bool SocialShareEnabled
        {
            get => Convert.ToBoolean(SettingProvider.Items["SocialShareEnabled"]);
            set => SettingProvider.Items["SocialShareEnabled"] = value.ToString();
        }
        
        public static string LinkVk
        {
            get => SettingProvider.Items["LinkVk"];
            set => SettingProvider.Items["LinkVk"] = value;
        }
        public static string LinkFacebook
        {
            get => SettingProvider.Items["LinkFacebook"];
            set => SettingProvider.Items["LinkFacebook"] = value;
        }
        public static string LinkInstagramm
        {
            get => SettingProvider.Items["LinkInstagramm"];
            set => SettingProvider.Items["LinkInstagramm"] = value;
        }
        public static string LinkOk
        {
            get => SettingProvider.Items["LinkOk"];
            set => SettingProvider.Items["LinkOk"] = value;
        }
        public static string LinkTwitter
        {
            get => SettingProvider.Items["LinkTwitter"];
            set => SettingProvider.Items["LinkTwitter"] = value;
        }
        public static string LinkTelegram
        {
            get => SettingProvider.Items["LinkTelegram"];
            set => SettingProvider.Items["LinkTelegram"] = value;
        }
        public static string LinkYoutube
        {
            get => SettingProvider.Items["LinkYoutube"];
            set => SettingProvider.Items["LinkYoutube"] = value;
        }
        public static string LinkRutube
        {
            get => SettingProvider.Items["LinkRutube"];
            set => SettingProvider.Items["LinkRutube"] = value;
        }
        public static string LinkYandexZen
        {
            get => SettingProvider.Items["LinkYandexZen"];
            set => SettingProvider.Items["LinkYandexZen"] = value;
        }

        public static bool LinkVkActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkVkActive"]);
            set => SettingProvider.Items["LinkVkActive"] = value.ToString();
        }
        public static bool LinkFacebookActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkFacebookActive"]);
            set => SettingProvider.Items["LinkFacebookActive"] = value.ToString();
        }
        public static bool LinkInstagrammActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkInstagrammActive"]);
            set => SettingProvider.Items["LinkInstagrammActive"] = value.ToString();
        }
        public static bool LinkOkActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkOkActive"]);
            set => SettingProvider.Items["LinkOkActive"] = value.ToString();
        }
        public static bool LinkTwitterActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkTwitterActive"]);
            set => SettingProvider.Items["LinkTwitterActive"] = value.ToString();
        }
        public static bool LinkTelegramActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkTelegramActive"]);
            set => SettingProvider.Items["LinkTelegramActive"] = value.ToString();
        }
        public static bool LinkYoutubeActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkYoutubeActive"]);
            set => SettingProvider.Items["LinkYoutubeActive"] = value.ToString();
        }
        public static bool LinkRutubeActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkRutubeActive"]);
            set => SettingProvider.Items["LinkRutubeActive"] = value.ToString();
        }
        public static bool LinkYandexZenActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["LinkYandexZenActive"]);
            set => SettingProvider.Items["LinkYandexZenActive"] = value.ToString();
        }

        public static string WidgetVk
        {
            get => SettingProvider.Items["WidgetVk"];
            set => SettingProvider.Items["WidgetVk"] = value;
        }
        public static string WidgetFacebook
        {
            get => SettingProvider.Items["WidgetFacebook"];
            set => SettingProvider.Items["WidgetFacebook"] = value;
        }
        public static string WidgetInstagramm
        {
            get => SettingProvider.Items["WidgetInstagramm"];
            set => SettingProvider.Items["WidgetInstagramm"] = value;
        }
        public static string WidgetOk
        {
            get => SettingProvider.Items["WidgetOk"];
            set => SettingProvider.Items["WidgetOk"] = value;
        }
        public static string WidgetTwitter
        {
            get => SettingProvider.Items["WidgetTwitter"];
            set => SettingProvider.Items["WidgetTwitter"] = value;
        }
        public static string WidgetTelegram
        {
            get => SettingProvider.Items["WidgetTelegram"];
            set => SettingProvider.Items["WidgetTelegram"] = value;
        }
        public static string WidgetYoutube
        {
            get => SettingProvider.Items["WidgetYoutube"];
            set => SettingProvider.Items["WidgetYoutube"] = value;
        }

        public static bool WidgetVkActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetVkActive"]);
            set => SettingProvider.Items["WidgetVkActive"] = value.ToString();
        }
        public static bool WidgetFacebookActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetFacebookActive"]);
            set => SettingProvider.Items["WidgetFacebookActive"] = value.ToString();
        }
        public static bool WidgetInstagrammActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetInstagrammActive"]);
            set => SettingProvider.Items["WidgetInstagrammActive"] = value.ToString();
        }
        public static bool WidgetOkActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetOkActive"]);
            set => SettingProvider.Items["WidgetOkActive"] = value.ToString();
        }
        public static bool WidgetTwitterActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetTwitterActive"]);
            set => SettingProvider.Items["WidgetTwitterActive"] = value.ToString();
        }
        public static bool WidgetTelegramActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetTelegramActive"]);
            set => SettingProvider.Items["WidgetTelegramActive"] = value.ToString();
        }
        public static bool WidgetYoutubeActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["WidgetYoutubeActive"]);
            set => SettingProvider.Items["WidgetYoutubeActive"] = value.ToString();
        }
    }
}