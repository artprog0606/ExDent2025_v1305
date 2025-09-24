using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;

namespace AdvantShop.App.Landing.Domain.Blocks
{
    public interface ILpBlockDefaultSettings
    {
        dynamic GetSettings(dynamic settings, LpConfiguration configuration = null);
    }

    public class BaseLpBlockDefaultSettings : ILpBlockDefaultSettings
    {
        public virtual dynamic GetSettings(dynamic settings, LpConfiguration configuration = null)
        {
            if (settings == null)
                return null;
            
            settings.vk_enabled = SettingsSocial.LinkVkActive;
            settings.vk_url = SettingsSocial.LinkVk;
            settings.fb_enabled = SettingsSocial.LinkFacebookActive;
            settings.fb_url = SettingsSocial.LinkFacebook;
            settings.youtube_enabled = SettingsSocial.LinkYoutubeActive;
            settings.youtube_url = SettingsSocial.LinkYoutube;
            settings.twitter_enabled = SettingsSocial.LinkTwitterActive;
            settings.twitter_url = SettingsSocial.LinkTwitter;
            settings.instagram_enabled = SettingsSocial.LinkInstagrammActive;
            settings.instagram_url = SettingsSocial.LinkInstagramm;
            settings.telegram_enabled = SettingsSocial.LinkTelegramActive;
            settings.telegram_url = SettingsSocial.LinkTelegram;
            settings.odnoklassniki_enabled = SettingsSocial.LinkOkActive;
            settings.odnoklassniki_url = SettingsSocial.LinkOk;
            settings.rutube_enabled = SettingsSocial.LinkRutubeActive;
            settings.rutube_url = SettingsSocial.LinkRutube;
            settings.yandexzen_enabled = SettingsSocial.LinkYandexZenActive;
            settings.yandexzen_url = SettingsSocial.LinkYandexZen;
            return settings;
        }
    }
}
