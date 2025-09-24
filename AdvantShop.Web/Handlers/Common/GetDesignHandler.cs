using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Trial;

namespace AdvantShop.Handlers.Common
{
    public class GetDesignHandler
    {
        public object Get()
        {
            object obj = new
            {
                Backgrounds = DesignService.GetDesigns(eDesign.Background),
                Themes = DesignService.GetDesigns(eDesign.Theme),
                Colors = DesignService.GetDesigns(eDesign.Color),
                Structures = GetStructures(),
                DesignCurrent = GetCurrentDesign(),
                isTrial = TrialService.IsTrialEnabled && CommonHelper.GetCookieString("isTrialService").IsNullOrEmpty(),
                isTemplate = SettingsDesign.Template != TemplateService.DefaultTemplateId
            };

            CommonHelper.SetCookie("isTrialService", TrialService.IsTrialEnabled.ToString(CultureInfo.InvariantCulture));

            return obj;
        }

        private object GetCurrentDesign()
        {
            var isDemoEnabled = Demo.IsDemoEnabled;
            return new
            {
                Background =
                    isDemoEnabled && CommonHelper.GetCookieString("background").IsNotEmpty()
                        ? CommonHelper.GetCookieString("background")
                        : SettingsDesign.Background,
                Theme =
                    isDemoEnabled && CommonHelper.GetCookieString("theme").IsNotEmpty()
                        ? CommonHelper.GetCookieString("theme")
                        : SettingsDesign.Theme,
                ColorScheme =
                    isDemoEnabled && CommonHelper.GetCookieString("colorscheme").IsNotEmpty()
                        ? CommonHelper.GetCookieString("colorscheme")
                        : SettingsDesign.ColorScheme,
                Structure =
                    isDemoEnabled && CommonHelper.GetCookieString("structure").IsNotEmpty()
                        ? CommonHelper.GetCookieString("structure")
                        : SettingsDesign.MainPageMode.ToString(),
                Template =
                    SettingsDesign.Template != TemplateService.DefaultTemplateId
                        ? SettingsDesign.Template
                        : string.Empty
            };
        }

        private List<string> GetStructures()
        {
            var structures = Enum.GetNames(typeof(SettingsDesign.eMainPageMode)).ToList();

            var settings = TemplateSettingsProvider.GetTemplateSettingsBox();
            if (settings == null || settings.Settings == null || settings.Settings.Count == 0)
                return structures;

            var mainPageModeSetting = settings.Settings.Find(x => x.Name == "MainPageMode");
            if (mainPageModeSetting == null || mainPageModeSetting.Options == null ||
                mainPageModeSetting.Options.Count == 0)
                return structures;

            var structuresByTemplate = new List<string>();

            foreach (var optionSetting in mainPageModeSetting.Options)
            {
                if (structures.Contains(optionSetting.Value))
                    structuresByTemplate.Add(optionSetting.Value);
            }

            return structuresByTemplate.Count != 0 ? structuresByTemplate : structures;
        }
    }
}