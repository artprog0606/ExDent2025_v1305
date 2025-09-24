using System;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class CheckSendingMailsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                WebhookExecuter.MakeSystemRequest("advantshopevents/checkSendingMails", async: false);
            }
            catch (Exception exception)
            {
                context.LogCritical(exception.Message);
                Debug.Log.Error("Check sending mails error", exception);
            }
        }
    }
}
