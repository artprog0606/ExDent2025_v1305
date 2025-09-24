//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Repository.Currencies;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    //[DisallowConcurrentExecution]
    public class UpdateCurrencyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            CurrencyService.UpdateCurrenciesFromCentralBank();
        }
    }
}