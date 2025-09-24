using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Smses
{
    public interface ISmsLogger : IAdvantShopLogger
    {
        void LogSms(SmsLog message);

        List<SmsLogDto> GetSms(Guid customerId, long phone);
    }
}
