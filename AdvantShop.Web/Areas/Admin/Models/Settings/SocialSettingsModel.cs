using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SocialSettingsModel
    {
        #region SocialCommon

        public string SocialLinkVk { get; set; }
        public string SocialLinkFb { get; set; }

        public bool VkTemplateActive { get; set; }
        public bool FbTemplateActive { get; set; }

        public bool SocialShareEnabled { get; set; }
        public bool SocialShareCustomEnabled { get; set; }
        public string SocialShareCustomCode { get; set; }


        // new model

        public string LinkVk { get; set; }
        public string LinkFacebook { get; set; }
        public string LinkInstagramm { get; set; }
        public string LinkOk { get; set; }
        public string LinkTwitter { get; set; }
        public string LinkTelegram { get; set; }
        public string LinkYoutube { get; set; }
        public string LinkYandexZen { get; set; }
        public string LinkRutube { get; set; }

        public bool LinkVkActive { get; set; }
        public bool LinkFacebookActive { get; set; }
        public bool LinkInstagrammActive { get; set; }
        public bool LinkOkActive { get; set; }
        public bool LinkTwitterActive { get; set; }
        public bool LinkTelegramActive { get; set; }
        public bool LinkYoutubeActive { get; set; }
        public bool LinkYandexZenActive { get; set; }
        public bool LinkRutubeActive { get; set; }

        #endregion

        #region Widgets

        public string WidgetVk { get; set; }
        public string WidgetFacebook { get; set; }
        public string WidgetInstagramm { get; set; }
        public string WidgetOk { get; set; }
        public string WidgetTwitter { get; set; }
        public string WidgetTelegram { get; set; }
        public string WidgetYoutube { get; set; }

        public bool WidgetVkActive { get; set; }
        public bool WidgetFacebookActive { get; set; }
        public bool WidgetInstagrammActive { get; set; }
        public bool WidgetOkActive { get; set; }
        public bool WidgetTwitterActive { get; set; }
        public bool WidgetTelegramActive { get; set; }
        public bool WidgetYoutubeActive { get; set; }

        #endregion

        #region CommunicationsWidget

        public bool SocialWidgetIsActive { get; set; }

        public bool SocialWidgetVkShowInDesktop { get; set; }
        public bool SocialWidgetVkShowInMobile { get; set; }

        public bool SocialWidgetFbShowInDesktop { get; set; }
        public bool SocialWidgetFbShowInMobile { get; set; }

        public bool SocialWidgetJivositeShowInDesktop { get; set; }
        public bool SocialWidgetJivositeShowInMobile { get; set; }

        public bool SocialWidgetCallbackShowInDesktop { get; set; }
        public bool SocialWidgetCallbackShowInMobile { get; set; }
        
        public bool SocialWidgetWhatsAppShowInMobile { get; set; }
        public bool SocialWidgetWhatsAppShowInDesktop { get; set; }
        public string SocialWidgetWhatsAppPhone { get; set; }
        
        public bool SocialWidgetViberShowInMobile { get; set; }
        public bool SocialWidgetViberShowInDesktop { get; set; }
        public string SocialWidgetViberPhone { get; set; }

        public bool SocialWidgetOdnoklassnikiShowInDesktop { get; set; }
        public bool SocialWidgetOdnoklassnikiShowInMobile { get; set; }
        public string SocialWidgetLinkOdnoklassniki { get; set; }

        public bool SocialWidgetTelegramShowInDesktop { get; set; }
        public bool SocialWidgetTelegramShowInMobile { get; set; }
        public string SocialWidgetLinkTelegram { get; set; }

        public string SocialWidgetCustomLink1 { get; set; }
        public string SocialWidgetCustomLinkText1 { get; set; }
        public string SocialWidgetCustomIcon1 { get; set; }
        public string SocialWidgetCustomIcon1Src
        {
            get
            {
                return !string.IsNullOrEmpty(SocialWidgetCustomIcon1)
                    ? FoldersHelper.GetPath(FolderType.SocialWidget, SocialWidgetCustomIcon1, false)
                    : null;
            }
        }

        public string SocialWidgetCustomLink2 { get; set; }
        public string SocialWidgetCustomLinkText2 { get; set; }
        public string SocialWidgetCustomIcon2 { get; set; }
        public string SocialWidgetCustomIcon2Src
        {
            get
            {
                return !string.IsNullOrEmpty(SocialWidgetCustomIcon2)
                    ? FoldersHelper.GetPath(FolderType.SocialWidget, SocialWidgetCustomIcon2, false)
                    : null;
            }
        }

        public string SocialWidgetCustomLink3 { get; set; }
        public string SocialWidgetCustomLinkText3 { get; set; }
        public string SocialWidgetCustomIcon3 { get; set; }
        public string SocialWidgetCustomIcon3Src
        {
            get
            {
                return !string.IsNullOrEmpty(SocialWidgetCustomIcon3)
                    ? FoldersHelper.GetPath(FolderType.SocialWidget, SocialWidgetCustomIcon3, false)
                    : null;
            }
        }

        public bool SocialWidgetShowPopover { get; set; }
        public string SocialWidgetPopoverText { get; set; }
        
        #endregion
    }
}