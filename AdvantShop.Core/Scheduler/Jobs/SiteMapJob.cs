//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.ExportImport;
using Quartz;
using System.Text;
using AdvantShop.Core.Scheduler.QuartzJobLogging;

namespace AdvantShop.Core.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class SiteMapJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {                  
            context.TryRun(() => new ExportHtmlMap().Create());
            context.TryRun(() => new ExportXmlMap().Create());
            
            context.WriteLastRun();
        }
    }
}