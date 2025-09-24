using System.Data;
using AdvantShop.Core.SQL;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    //[DisallowConcurrentExecution]
    public class ReindexJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_Reindex]", CommandType.StoredProcedure, 60*5);
        }
    }
}