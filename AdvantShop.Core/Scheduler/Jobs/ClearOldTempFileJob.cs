using AdvantShop.Configuration;
using AdvantShop.Helpers;
using Quartz;
using System;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ClearOldTempFileJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var directory = $"{SettingsGeneral.AbsolutePath}App_TempData\\";
            FileHelpers.ClearDirectoriesByDate(directory, DateTime.Now.AddMonths(-1));
            context.WriteLastRun();
        }
    }
}
