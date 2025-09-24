using System;

namespace AdvantShop.Core.Common
{
    [Flags]
    public enum FlagDayOfWeek : byte
    {
        Sunday = 0b1 << DayOfWeek.Sunday,
        Monday = 0b1 << DayOfWeek.Monday,
        Tuesday = 0b1 << DayOfWeek.Tuesday,
        Wednesday = 0b1 << DayOfWeek.Wednesday,
        Thursday = 0b1 << DayOfWeek.Thursday,
        Friday = 0b1 << DayOfWeek.Friday,
        Saturday = 0b1 << DayOfWeek.Saturday,
    }

    public static class FlagDayOfWeekExtensions
    {
        public static bool HasDayOfWeek(this FlagDayOfWeek flagDayOfWeek, DayOfWeek dayOfWeek) 
            => flagDayOfWeek.HasFlag(dayOfWeek.GetFlagDayOfWeek());
        
        public static DayOfWeek GetDayOfWeek(this FlagDayOfWeek flagDayOfWeek) 
        {
            switch (flagDayOfWeek)
            {
                case FlagDayOfWeek.Sunday:
                    return DayOfWeek.Sunday;
                case FlagDayOfWeek.Monday:
                    return DayOfWeek.Monday;
                case FlagDayOfWeek.Tuesday:
                    return DayOfWeek.Tuesday;
                case FlagDayOfWeek.Wednesday:
                    return DayOfWeek.Wednesday;
                case FlagDayOfWeek.Thursday:
                    return DayOfWeek.Thursday;
                case FlagDayOfWeek.Friday:
                    return DayOfWeek.Friday;
                case FlagDayOfWeek.Saturday:
                    return DayOfWeek.Saturday;
            }

            throw new ArgumentOutOfRangeException(nameof(flagDayOfWeek));
        }
    }
    
    public static class DayOfWeekExtensionsForFlagDayOfWeek
    {
        public static FlagDayOfWeek GetFlagDayOfWeek(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return FlagDayOfWeek.Sunday;
                case DayOfWeek.Monday:
                    return FlagDayOfWeek.Monday;
                case DayOfWeek.Tuesday:
                    return FlagDayOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return FlagDayOfWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return FlagDayOfWeek.Thursday;
                case DayOfWeek.Friday:
                    return FlagDayOfWeek.Friday;
                case DayOfWeek.Saturday:
                    return FlagDayOfWeek.Saturday;
            }

            throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
        }
    }
}