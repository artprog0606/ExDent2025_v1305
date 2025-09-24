using System;
using System.Globalization;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Localization;

namespace AdvantShop.Core.Services.TemplatesDocx.Templates
{
    public abstract class BaseTemplate
    {
        [TemplateDocxProperty("DateNow", LocalizeDescription = "Текущая дата")]
        public DateTime DateNow { get { return DateTime.Now; } }

        [TemplateDocxProperty("Date", LocalizeDescription = "Текущая дата")]
        public string Date { get { return Culture.ConvertShortDate(DateTime.Now); } }

        [TemplateDocxProperty("DateNow Day", LocalizeDescription = "Текущий день")]
        public string DateNowDay { get { return DateTime.Now.Day.ToString(); } }

        [TemplateDocxProperty("DateNow Month", LocalizeDescription = "Название текущего месяца")]
        public string DateNowMonth { get { return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[DateTime.Now.Month - 1]; } }

        [TemplateDocxProperty("DateNow Month Genitive", LocalizeDescription = "Название текущего месяца в родительном падеже")]
        public string DateNowMonthGenitive { get { return CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames[DateTime.Now.Month - 1]; } }

        [TemplateDocxProperty("DateNow Year", LocalizeDescription = "Текущий год")]
        public string DateNowYear { get { return DateTime.Now.Year.ToString(); } }

        [TemplateDocxProperty("DateNow Time", LocalizeDescription = "Текущие время")]
        public string DateNowTime { get { return DateTime.Now.ToLongTimeString(); } }

        [TemplateDocxProperty("Shop Name", LocalizeDescription = "Название магазина")]
        public string ShopName { get { return SettingsMain.ShopName; } }
    }
}
