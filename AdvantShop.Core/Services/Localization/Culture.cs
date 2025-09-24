//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Localization
{

    public class Culture
    {
        public enum SupportLanguage
        {
            Russian = 0,
            English = 1,
            Ukrainian = 2,
            Other = 3
        }
        
        public static SupportLanguage Language
        {
            get
            {
                switch (SettingsMain.Language)
                {
                    case "en":
                    case "en-US":
                        return SupportLanguage.English;
                    case "ru":
                    case "ru-RU":
                        return SupportLanguage.Russian;
                    case "uk":
                    case "uk-UA":
                        return SupportLanguage.Ukrainian;
                    default:
                        return SupportLanguage.Other;
                }
            }
        }

        public static SupportLanguage CurrentCulture
        {
            get
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "en":
                    case "en-US":
                        return SupportLanguage.English;
                    case "ru":
                    case "ru-RU":
                        return SupportLanguage.Russian;
                    case "uk":
                    case "uk-UA":
                        return SupportLanguage.Ukrainian;
                    default:
                        return SupportLanguage.Other;
                }
            }
        }


        public static void InitializeCulture()
        {
            Thread.CurrentThread.SetCulture();
        }

        public static void InitializeCulture(string langValue)
        {
            var lang = langValue;
            if (string.IsNullOrEmpty(lang)) return;
            Thread.CurrentThread.SetCulture(langValue);
        }

        public static CultureInfo GetCulture(string lang = "")
        {
            if (string.IsNullOrWhiteSpace(lang))
            {
                return new CultureInfo(SettingsMain.Language);
            }
            return new CultureInfo(lang);
        }
        
        public static string ConvertDate(DateTime d)
        {
            return d.ToString(SettingsMain.AdminDateFormat);
        }

        public static string ConvertDateWithoutHours(DateTime d)
        {
            return d.ToString(SettingsMain.AdminDateFormat.Split(' ')[0]);
        }

        public static string ConvertShortDate(DateTime d)
        {
            try
            {
                return d.ToString(SettingsMain.ShortDateFormat);
            }
            catch (FormatException)
            {
                return d.ToShortDateString();
            }
        }

        public static string ConvertDateFromString(string s)
        {
            var d = DateTime.Parse(s, CultureInfo.GetCultureInfo(SettingsMain.Language));  // GetStringLangByEnum(Language)
            return d.ToString(SettingsMain.AdminDateFormat);
        }
    }
}
