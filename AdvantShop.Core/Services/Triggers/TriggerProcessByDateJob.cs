using Quartz;

namespace AdvantShop.Core.Services.Triggers
{
    /// <summary>
    /// Обработка триггеров, которые привязаны к дате
    /// </summary>
    public class TriggerProcessByDateJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            TriggerProcessService.ProcessTriggersByDatetime();
        }
    }
}
