//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using Quartz;

namespace AdvantShop.Core.Services.TrafficStatistics
{
    //[DisallowConcurrentExecution]
    public class TrafficStatisticsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            context.TryRun(() => TrafficStatisticsService.DeleteExpiredDateTime());

            context.WriteLastRun();
        }
    }
}
