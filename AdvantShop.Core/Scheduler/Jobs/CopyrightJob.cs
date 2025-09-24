using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.SEO;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    public class CopyrightJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var actualCopyright = CopyrightService.UpdateCopyRight(ECopyrightType.Site, 60);
            context.LogInformation($"Got copyright: {actualCopyright} for site");
            
            actualCopyright = CopyrightService.UpdateCopyRight(ECopyrightType.Landing, 60);
            context.LogInformation($"Got copyright: {actualCopyright} for landings");
            context.WriteLastRun();
        }
    }
}
