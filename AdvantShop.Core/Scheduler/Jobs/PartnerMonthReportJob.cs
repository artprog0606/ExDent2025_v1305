//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Partners;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class PartnerMonthReportJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            context.TryRun(PartnerReportService.SendMonthReports);
            context.TryRun(() => PartnerReportService.GeneratePartnersPayoutReport());
            context.TryRun(PartnerReportService.GenerateAndSendActReports);

            context.WriteLastRun();
        }
    }
}