//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.Services.Crm;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class DeferredTasksJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            TaskService.ProcessDeferredTasks();

            context.WriteLastRun();
        }
    }
}