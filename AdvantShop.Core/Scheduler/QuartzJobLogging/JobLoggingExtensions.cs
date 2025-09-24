using System;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using Quartz;

namespace AdvantShop.Core.Scheduler.QuartzJobLogging
{
    public static class JobLoggingExtensions
    {
        /// <remarks>
        /// Better to use other extensions<br/>
        /// <see cref="LogInformation">LogInformation</see><br/>
        /// <see cref="LogWarning">LogWarning</see><br/>
        /// <see cref="LogError">LogError</see><br/>
        /// <see cref="LogCritical">LogCritical</see><br/>
        /// </remarks>
        public static void LogJobEvent(this IJobExecutionContext context, EQuartzJobEvent @event, string message)
        {
            QuartzJobsLoggingService.AddQuartzJobRunLog(new QuartzJobRunLog
            {
                JobRunId = context.FireInstanceId,
                Event = @event,
                Message = message
            });
        }

        public static void LogInformation(this IJobExecutionContext context, string message)
        {
            context.LogJobEvent(EQuartzJobEvent.Information, message);
        }

        public static void LogWarning(this IJobExecutionContext context, string message)
        {
            context.LogJobEvent(EQuartzJobEvent.Warning, message);
        }

        public static void LogError(this IJobExecutionContext context, string message)
        {
            context.LogJobEvent(EQuartzJobEvent.Error, message);
        }

        public static void LogCritical(this IJobExecutionContext context, string message)
        {
            context.LogJobEvent(EQuartzJobEvent.Critical, message);
        }

        public static void TryRun(this IJobExecutionContext context, Action action)
        {
            string methodName = null;
            if (action.Method.DeclaringType != null && !action.Method.DeclaringType.Name.Contains("<>"))
                methodName = $"{action.Method.DeclaringType.Name}.{action.Method.Name}";

            try
            {
                action.Invoke();
                context.LogInformation($"Done: {methodName} done");
            }
            catch (Exception ex)
            {
                context.LogError(methodName != null
                    ? $"Error on executing {methodName} - {ex.Message}"
                    : $"Error - {ex.Message}");
            }
        }
    }
}