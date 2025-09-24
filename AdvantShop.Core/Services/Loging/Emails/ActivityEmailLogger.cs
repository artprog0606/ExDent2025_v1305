using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public sealed class ActivityEmailLogger : ActivityEmailNullLogger
    {
        private string ActivityEmailLogBaseUrl => SettingProvider.GetInternalSetting("ActivityEmailLogServiceUrl");
        
        public override void LogEmail(EmailLog emailLog)
        {
            Task.Run(() => new ActivityRequest(ActivityEmailLogBaseUrl).Post("api/v1/log", emailLog));
        }

        public override List<EmailLogDto> GetEmails(Guid customerId, string email)
        {
            return 
                new ActivityRequest(ActivityEmailLogBaseUrl)
                    .Get<List<EmailLogDto>>($"api/v1/log/?customerId={customerId}&email={email}");
        }
    }
}
