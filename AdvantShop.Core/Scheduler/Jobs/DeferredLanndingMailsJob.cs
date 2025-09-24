using System;
using AdvantShop.Core.Services.Landing.LandingEmails;
using AdvantShop.Core.Services.Mails;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class DeferredLanndingMailsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = new LandingDeferredEmailService();

            var deferredMails = service.GetList(DateTime.Now);
            if (deferredMails.Count > 0)
            {
                foreach (var mail in deferredMails)
                {
                    MailService.SendMailNow(mail.CustomerId, mail.Email, mail.Subject, mail.Body, true);
                    service.Delete(mail.Id);
                }
            }
            context.WriteLastRun();
        }
    }
}
