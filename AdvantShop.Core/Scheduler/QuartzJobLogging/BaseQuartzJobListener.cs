using System;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using Quartz;
using Quartz.Impl;

namespace AdvantShop.Core.Scheduler.QuartzJobLogging
{
    internal class BaseQuartzJobListener : IJobListener
    {
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            var contextJobDetail = context.JobDetail as JobDetailImpl;
            QuartzJobsLoggingService.AddQuartzJobRun(new QuartzJobRun
            {
                Id = context.FireInstanceId,
                Name = contextJobDetail is null
                    ? context.JobDetail.JobType.FullName
                    : contextJobDetail.FullName,
                Group = context.JobDetail.Key.Group,
                Initiator = "Scheduler",
                Status = EQuartzJobStatus.Running,
                StartDate = DateTime.Now
            });
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            if (!QuartzJobsLoggingService.IsExistsQuartzJobRun(context.FireInstanceId))
                return;

            var quartzJobRun = QuartzJobsLoggingService.GetQuartzJobRun(context.FireInstanceId);
            quartzJobRun.Status = EQuartzJobStatus.Vetoed;
            QuartzJobsLoggingService.UpdateQuartzJobRun(quartzJobRun);
            context.LogWarning("Job is vetoed");
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (!QuartzJobsLoggingService.IsExistsQuartzJobRun(context.FireInstanceId))
                return;

            if (jobException != null)
            {
                var innerException = GetInnerException(jobException);
                context.LogCritical($"{innerException.Message}{innerException.StackTrace}");
            }

            var quartzJobRun = QuartzJobsLoggingService.GetQuartzJobRun(context.FireInstanceId);

            quartzJobRun.Status = jobException is null
                ? EQuartzJobStatus.Complete
                : EQuartzJobStatus.CompleteWithError;
            quartzJobRun.EndDate = DateTime.Now;
            QuartzJobsLoggingService.UpdateQuartzJobRun(quartzJobRun);
        }

        public string Name => nameof(BaseQuartzJobListener);

        private static Exception GetInnerException(Exception exception)
        {
            while (true)
            {
                if ((exception is JobExecutionException || exception is SchedulerException) && exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                else
                    return exception;
            }
        }
    }
}
