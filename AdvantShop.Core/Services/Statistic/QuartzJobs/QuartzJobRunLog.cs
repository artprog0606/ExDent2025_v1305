using System;

namespace AdvantShop.Core.Services.Statistic.QuartzJobs
{
    internal class QuartzJobRunLog
    {
        public int Id { get; set; }
        public string JobRunId { get; set; }
        public EQuartzJobEvent Event { get; set; }
        public string Message { get; set; }
        public DateTime AddDate { get; set; }
    }
}
