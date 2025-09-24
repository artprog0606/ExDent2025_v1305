namespace AdvantShop.Core.Services.Statistic.QuartzJobs
{
    public enum EQuartzJobStatus
    {
        Running,
        Complete,
        CompleteWithError,
        Vetoed,
        BrokenOnAppRestart
    }
}
