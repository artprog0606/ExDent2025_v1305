using AdvantShop.Core.Services.Triggers.DeferredDatas;
using Quartz;

namespace AdvantShop.Core.Services.Triggers
{
    /// <summary>
    /// Обработка отложенных данных для триггеров
    /// </summary>
    [DisallowConcurrentExecution]
    public class TriggerProcessJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var deferredDatas = TriggerDeferredDataService.GetList();

            foreach (var deferredData in deferredDatas)
            {
                TriggerProcessService.ProcessDeferredData(deferredData);
            }
        }
    }
}
