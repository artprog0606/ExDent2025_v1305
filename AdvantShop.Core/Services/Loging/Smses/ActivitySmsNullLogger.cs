using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public class ActivitySmsNullLogger : ISmsLogger
    {
        public virtual void LogSms(SmsLog message)
        {
        }

        public virtual List<SmsLogDto> GetSms(Guid customerId, long phone)
        {
            return null;
        }
    }
}