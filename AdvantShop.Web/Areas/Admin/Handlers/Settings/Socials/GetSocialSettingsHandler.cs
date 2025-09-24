using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.Telegram;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Socials
{
    public class GetSocialSettingsHandler
    {
        public SocialSettingsModel Execute()
        {
            return new SocialSettingsModel
            {
                #region SocialCommon

                SocialShareCustomCode = SettingsSocial.SocialShareCustomCode,
                SocialShareEnabled = SettingsSocial.SocialShareEnabled,

                VkTemplateActive = SettingsDesign.IsVkTemplateActive,
                FbTemplateActive = SettingsDesign.IsFbTemplateActive,

                SocialLinkVk = SettingsMain.SiteUrl.Replace("www.", "").Replace("http://", "http://vk.").Replace("https://", "https://vk."),
                SocialLinkFb = SettingsMain.SiteUrl.Replace("www.", "").Replace("http://", "http://fb.").Replace("https://", "https://fb."),

                LinkVk = SettingsSocial.LinkVk,
                LinkFacebook = SettingsSocial.LinkFacebook,
                LinkInstagramm = SettingsSocial.LinkInstagramm,
                LinkOk = SettingsSocial.LinkOk,
                LinkTwitter = SettingsSocial.LinkTwitter,
                LinkTelegram = SettingsSocial.LinkTelegram,
                LinkYoutube = SettingsSocial.LinkYoutube,
                LinkRutube = SettingsSocial.LinkRutube,
                LinkYandexZen = SettingsSocial.LinkYandexZen,

                LinkVkActive = SettingsSocial.LinkVkActive,
                LinkFacebookActive = SettingsSocial.LinkFacebookActive,
                LinkInstagrammActive = SettingsSocial.LinkInstagrammActive,
                LinkOkActive = SettingsSocial.LinkOkActive,
                LinkTwitterActive = SettingsSocial.LinkTwitterActive,
                LinkTelegramActive = SettingsSocial.LinkTelegramActive,
                LinkYoutubeActive = SettingsSocial.LinkYoutubeActive,
                LinkRutubeActive = SettingsSocial.LinkRutubeActive,
                LinkYandexZenActive = SettingsSocial.LinkYandexZenActive,

                #endregion SocialCommon

                #region Widgets

                WidgetVk = SettingsSocial.WidgetVk,
                WidgetFacebook = SettingsSocial.WidgetFacebook,
                WidgetInstagramm = SettingsSocial.WidgetInstagramm,
                WidgetOk = SettingsSocial.WidgetOk,
                WidgetTwitter = SettingsSocial.WidgetTwitter,
                WidgetTelegram = SettingsSocial.WidgetTelegram,
                WidgetYoutube = SettingsSocial.WidgetYoutube,

                WidgetVkActive = SettingsSocial.WidgetVkActive,
                WidgetFacebookActive = SettingsSocial.WidgetFacebookActive,
                WidgetInstagrammActive = SettingsSocial.WidgetInstagrammActive,
                WidgetOkActive = SettingsSocial.WidgetOkActive,
                WidgetTwitterActive = SettingsSocial.WidgetTwitterActive,
                WidgetTelegramActive = SettingsSocial.WidgetTelegramActive,
                WidgetYoutubeActive = SettingsSocial.WidgetYoutubeActive,

                #endregion Widgets

                #region SocialWidget

                SocialWidgetIsActive = SettingsSocialWidget.IsActive,

                SocialWidgetVkShowInDesktop = SettingsSocialWidget.IsShowVkInDesktop,
                SocialWidgetVkShowInMobile = SettingsSocialWidget.IsShowVkInMobile,

                SocialWidgetFbShowInDesktop = SettingsSocialWidget.IsShowFbInDesktop,
                SocialWidgetFbShowInMobile = SettingsSocialWidget.IsShowFbInMobile,

                SocialWidgetJivositeShowInDesktop = SettingsSocialWidget.IsShowJivositeInDesktop,
                SocialWidgetJivositeShowInMobile = SettingsSocialWidget.IsShowJivositeInMobile,

                SocialWidgetCallbackShowInDesktop = SettingsSocialWidget.IsShowCallbackInDesktop,
                SocialWidgetCallbackShowInMobile = SettingsSocialWidget.IsShowCallbackInMobile,

                SocialWidgetWhatsAppShowInMobile = SettingsSocialWidget.IsShowWhatsAppInMobile,
                SocialWidgetWhatsAppShowInDesktop = SettingsSocialWidget.IsShowWhatsAppInDesktop,
                SocialWidgetWhatsAppPhone = SettingsSocialWidget.WhatsAppPhone,

                SocialWidgetViberShowInMobile = SettingsSocialWidget.IsShowViberInMobile,
                SocialWidgetViberShowInDesktop = SettingsSocialWidget.IsShowViberInDesktop,
                SocialWidgetViberPhone = SettingsSocialWidget.ViberPhone,

                SocialWidgetOdnoklassnikiShowInDesktop = SettingsSocialWidget.IsShowOdnoklassnikiInDesktop,
                SocialWidgetOdnoklassnikiShowInMobile = SettingsSocialWidget.IsShowOdnoklassnikiInMobile,
                SocialWidgetLinkOdnoklassniki = SettingsSocialWidget.LinkOdnoklassniki,

                SocialWidgetTelegramShowInDesktop = SettingsSocialWidget.IsShowTelegramInDesktop,
                SocialWidgetTelegramShowInMobile = SettingsSocialWidget.IsShowTelegramInMobile,
                SocialWidgetLinkTelegram =
                SettingsSocialWidget.LinkTelegram.IsNullOrEmpty()
                    ? (new TelegramApiService().IsActive() && SettingsTelegram.BotUser != null
                        ? "https://t.me/" + SettingsTelegram.BotUser.Username
                        : string.Empty)
                    : SettingsSocialWidget.LinkTelegram,

                SocialWidgetCustomLink1 = SettingsSocialWidget.CustomLink1,
                SocialWidgetCustomLinkText1 = SettingsSocialWidget.CustomLinkText1,
                SocialWidgetCustomIcon1 = SettingsSocialWidget.CustomLinkIcon1,

                SocialWidgetCustomLink2 = SettingsSocialWidget.CustomLink2,
                SocialWidgetCustomLinkText2 = SettingsSocialWidget.CustomLinkText2,
                SocialWidgetCustomIcon2 = SettingsSocialWidget.CustomLinkIcon2,

                SocialWidgetCustomLink3 = SettingsSocialWidget.CustomLink3,
                SocialWidgetCustomLinkText3 = SettingsSocialWidget.CustomLinkText3,
                SocialWidgetCustomIcon3 = SettingsSocialWidget.CustomLinkIcon3,

                SocialWidgetShowPopover = SettingsSocialWidget.ShowPopover,
                SocialWidgetPopoverText = SettingsSocialWidget.PopoverText,
                #endregion SocialWidget
            };
        }
    }
}