using System;

namespace AdvantShop.Core.Services.Statistic.QuartzJobs
{
    internal class QuartzJobRun
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Initiator { get; set; }
        public EQuartzJobStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
