using System;
using System.Collections.Generic;

namespace AdvantShop.Core.Services.Loging.Emails
{
    public class ActivityEmailNullLogger : IEmailLogger
    {
        public virtual void LogEmail(EmailLog email)
        {
        }

        public virtual List<EmailLogDto> GetEmails(Guid customerId, string email)
        {
            return null;
        }
    }
}
