using System;
using System.Threading.Tasks;
using System.Web;

namespace AdvantShop.Core.Common
{
    public enum DelayType
    {
        Minutes,
        Seconds,
        Milliseconds,
    }
    
    public static class DelayHelper
    {
        public static void Wait(float interval, DelayType delayType, Action action)
        {
            var context = HttpContext.Current;
            
            Task.Delay(GetDelay(interval, delayType))
                .ContinueWith(_ =>
                {
                    HttpContext.Current = context;
                    HttpContext.Current.Items["IsFromTask"] = "true";
                    
                    action();
                });
        }

        private static TimeSpan GetDelay(float interval, DelayType delayType)
        {
            switch (delayType)
            {
                case DelayType.Minutes:
                    return TimeSpan.FromMinutes(interval);
                case DelayType.Seconds:
                    return TimeSpan.FromSeconds(interval);
                case DelayType.Milliseconds:
                    return TimeSpan.FromMilliseconds(interval);
                default:
                    return TimeSpan.FromMilliseconds(0);
            }
        }
    }
}