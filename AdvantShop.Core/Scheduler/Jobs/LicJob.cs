//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class LicJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SettingsLic.Activate();
        }
    }
}