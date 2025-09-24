using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public interface IEmailLogger : IAdvantShopLogger
    {
        void LogEmail(EmailLog email);

        List<EmailLogDto> GetEmails(Guid customerId, string email);
    }
}
