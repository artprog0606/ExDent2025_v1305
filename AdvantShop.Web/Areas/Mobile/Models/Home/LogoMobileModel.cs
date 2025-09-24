using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Mobile.Models.Home
{
    public class LogoMobileModel
    {
        public string ImgSource { get; set; }

        public string LogoAlt { get; set; }

        public SettingsMobile.eLogoType LogoType { get; set; }

        public string Text { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}