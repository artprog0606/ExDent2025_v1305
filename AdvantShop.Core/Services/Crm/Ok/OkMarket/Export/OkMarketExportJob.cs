using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using Quartz;

namespace AdvantShop.Core.Services.Crm.Ok.OkMarket.Export
{
    [DisallowConcurrentExecution]
    public class OkMarketExportJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (OkMarketExportSettings.ExportOnShedule is false)
            {
                context.LogInformation("OkMarketExportState.ExportOnShedule is false");
                return;
            } 
        
            if (OkMarketExportState.IsRun)
            {
                context.LogInformation("OkMarketExportState.IsRun is still true");
                return;
            }

            new OkMarketExport().StartExport();
            
            context.WriteLastRun();
        }
    }
}