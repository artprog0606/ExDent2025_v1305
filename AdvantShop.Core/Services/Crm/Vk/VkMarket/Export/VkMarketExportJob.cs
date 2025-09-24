using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using Quartz;

namespace AdvantShop.Core.Services.Crm.Vk.VkMarket.Export
{
    [DisallowConcurrentExecution]
    public class VkMarketExportJob : IJob
    {
        private readonly VkApiService _vkService = new VkApiService();

        public void Execute(IJobExecutionContext context)
        {
            if (VkMarketExportState.IsRun)
            {
                context.LogInformation("VkMarketExportState.IsRun is still true");
                return;
            }

            new VkMarketExportService().StartExport();
            
            context.WriteLastRun();
        }
    }
}
