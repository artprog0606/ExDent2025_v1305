using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Trial;

namespace AdvantShop.Handlers.Common
{
    public class SaveDesignHandler
    {
        public void Save(string theme, string colorscheme, string structure, string background)
        {
            var mainPageMode = (SettingsDesign.eMainPageMode) Enum.Parse(typeof(SettingsDesign.eMainPageMode), structure);
            var isMainPageModeChanged = false;

            if (Demo.IsDemoEnabled)
            {
                CommonHelper.SetCookie("theme", theme);
                CommonHelper.SetCookie("background", background);
                CommonHelper.SetCookie("colorscheme", colorscheme);
                CommonHelper.SetCookie("structure", structure);
            }
            else
            {
                SettingsDesign.Theme = theme;
                SettingsDesign.Background = background;
                SettingsDesign.ColorScheme = colorscheme;

                isMainPageModeChanged = SettingsDesign.MainPageMode != mainPageMode;
                SettingsDesign.MainPageMode = mainPageMode;
            }

            TrialService.TrackEvent(TrialEvents.ChangeColorScheme, colorscheme);
            TrialService.TrackEvent(TrialEvents.ChangeBackGround, background);
            TrialService.TrackEvent(TrialEvents.ChangeTheme, theme);
            TrialService.TrackEvent(TrialEvents.ChangeMainPageMode, structure);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_ChangeDesignTransformer);
            

            if (!Demo.IsDemoEnabled && isMainPageModeChanged)
            {
                if (SettingsDesign.MainPageMode == SettingsDesign.eMainPageMode.Default)
                {
                    SettingsDesign.CountMainPageProductInLine = 4;
                    SettingsDesign.CountMainPageProductInSection = 4;
                }
                else if (SettingsDesign.MainPageMode == SettingsDesign.eMainPageMode.TwoColumns)
                {
                    SettingsDesign.CountMainPageProductInLine = 3;
                    SettingsDesign.CountMainPageProductInSection = 3;
                }
            }

            var mobileBrowserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
            if (mobileBrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
            {
                var curColorScheme = DesignService.GetCurrenDesign(eDesign.Color);
                SettingsMobile.BrowserColor = curColorScheme.Color;
            }

            CacheManager.Clean();

            new ScreenshotService().UpdateStoreScreenShotInBackground();
        }
    }
}