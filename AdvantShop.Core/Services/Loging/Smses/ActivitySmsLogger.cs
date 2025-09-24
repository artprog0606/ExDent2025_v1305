using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public sealed class ActivitySmsLogger : ActivitySmsNullLogger
    {
        private string ActivitySmsLogBaseUrl => SettingProvider.GetInternalSetting("ActivitySmsLogServiceUrl");
        
        public override void LogSms(SmsLog smsLog)
        {
            Task.Run(() => new ActivityRequest(ActivitySmsLogBaseUrl).Post("api/v1/log", smsLog));
        }

        public override List<SmsLogDto> GetSms(Guid customerId, long phone)
        {
            return 
                new ActivityRequest(ActivitySmsLogBaseUrl)
                    .Get<List<SmsLogDto>>($"api/v1/log/?customerId={customerId}&phone={phone}");
        }
    }
}
