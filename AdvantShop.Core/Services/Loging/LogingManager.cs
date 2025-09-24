using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Loging.Calls;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Loging.Events;
using AdvantShop.Core.Services.Loging.Smses;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Saas;

namespace AdvantShop.Core.Services.Loging
{
    public class LoggingManager
    {
        public static IEmailLogger GetEmailLogger()
        {
            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
                return new ActivityEmailLogger();

            return new ActivityEmailNullLogger();
        }

        public static ICallLogger GetCallLogger()
        {
            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
                return new ActivityCallLogger();

            return new ActivityCallNullLogger();
        }

        public static ISmsLogger GetSmsLogger()
        {
            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
                return new ActivitySmsLogger();

            return new ActivitySmsNullLogger();
        }

        public static IEventLogger GetEventLogger()
        {
            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
                return new ActivityEventLogger();

            return new ActivityEventNullLogger();
        }
        
        public static ICustomerAction GetCustomerActionLogger()
        {
            if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
                return new ActivityEventLogger();

            return null;
        }

        public static ITrafficSourceLogger GetTrafficSourceLogger()
        {
            //if (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveCustomerLog)
            //    return new ActivityTrafficSourceLogger();

            return new ActivityTrafficSourceNullLogger();
        }
    }
}