using AdvantShop.Configuration;
using System;
using System.Globalization;

namespace AdvantShop.Core.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Is this time between 2 datetimes
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool TimeBetween(this DateTime dateTime, DateTime from, DateTime to)
        {
            var start = new TimeSpan(from.Hour, from.Minute, 0);
            var end = new TimeSpan(to.Hour, to.Minute, 0);
            var now = dateTime.TimeOfDay;

            if (start < end)
                return start <= now && now <= end;

            return !(end < now && now < start);
        }


        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static string ToShortDateTime(this DateTime dt)
        {
            return dt.ToString(SettingsMain.ShortDateFormat);
        }

        public static DateTime ToDateTimeFromUnixTime(this long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
        }

        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static void GetDuration(this DateTime dateTime, DateTime dt2, out int years, out int mouths, out int days)
        {
            years = 0;
            mouths = 0;
            var tmp = dateTime;

            while (tmp < dt2)
            {
                years++;
                tmp = tmp.AddYears(1);
            }
            years--;
            tmp = dateTime.AddYears(years);

            while (tmp < dt2)
            {
                mouths++;
                tmp = tmp.AddMonths(1);
            }
            mouths--;

            days = (dt2 - dateTime.AddYears(years).AddMonths(mouths)).Days;
        }

        public static string GetDurationString(this DateTime dateTime, DateTime dt2)
        {
            int years = 0, months = 0, days = 0;
            dateTime.GetDuration(dt2, out years, out months, out days);
            return (years > 0 ? years + "г. " : "") + (months > 0 ? months + "м. " : "") + days + "д.";
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static DateTime GetFirstDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            return GetFirstDayOfWeek(dayInWeek, firstDay);
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetLastDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, firstDay);
            return firstDayInWeek.AddDays(7);
        }

        public static DateTime GetLastDayOfWeek(this DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, cultureInfo);
            return firstDayInWeek.AddDays(7);
        }

        public static bool IsValidForSql(this DateTime dateTime)
        {            
            var minDateTime = new DateTime(1753, 1, 1);
            var maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);

            return dateTime > minDateTime && dateTime < maxDateTime;
        }

        public static DateTime GetEndDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Преобразует DateTime в DateTimeOffset, со смещением указанным в настройках времени приема заказов
        /// </summary>
        /// <param name="dateTime">UTC или локальная дата</param>
        /// <returns>Дата со смещением <br /> null - если время не определено ни как локальное, ни как время UTC</returns>
        public static DateTimeOffset? GetDateTimeOffset(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                return null;
            return ((DateTimeOffset)TimeZoneInfo.ConvertTimeToUtc(dateTime)).ToOffset(SettingsCheckout.TimeZoneWorkingTime);
        }
    }
}