using System.Collections.Generic;
using AdvantShop.Core.Scheduler;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl.Matchers;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class GetSchedulerJobs
    {
        public List<SchedulerJobItemModel> Execute()
        {
            var items = new List<SchedulerJobItemModel>();

            var scheduler = TaskManager.TaskManagerInstance().Scheduler;

            foreach (string group in scheduler.GetJobGroupNames())
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);

                foreach (var jobKey in scheduler.GetJobKeys(groupMatcher))
                {
                    var detail = scheduler.GetJobDetail(jobKey);

                    foreach (var trigger in scheduler.GetTriggersOfJob(jobKey))
                    {
                        var nextFireTime = trigger.GetNextFireTimeUtc();
                        var previousFireTime = trigger.GetPreviousFireTimeUtc();

                        var item = new SchedulerJobItemModel()
                        {
                            Group = group,
                            JobName = jobKey.Name,
                            JobDescription = detail.Description,
                            JobDataMap = JsonConvert.SerializeObject(detail.JobDataMap),
                            TriggerName = trigger.Key.Name,
                            TriggerGroup = trigger.Key.Group,
                            TriggerType = trigger.GetType().Name,
                            TriggerState = scheduler.GetTriggerState(trigger.Key).ToString(),
                            NextFireTime = nextFireTime.HasValue
                                ? nextFireTime.Value.LocalDateTime.ToString()
                                : null,
                            PreviousFireTime = previousFireTime.HasValue
                                ? previousFireTime.Value.LocalDateTime.ToString()
                                : "not start",
                        };
                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }

    public class SchedulerJobItemModel
    {
        public string Group { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string JobDataMap { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string TriggerType { get; set; }
        public string TriggerState { get; set; }
        public string NextFireTime { get; set; }
        public string PreviousFireTime { get; set; }
    }
}
