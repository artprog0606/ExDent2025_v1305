//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Diagnostics;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace AdvantShop.Core.Scheduler
{
    //pattern singleton 
    public class TaskManager
    {
        public readonly IScheduler Scheduler;
        public const string DataMap = "data";
        public const string TaskGroup = "TaskGroup";
        public const string ModuleGroup = "ModuleGroup";

        private static readonly TaskManager taskManager = new TaskManager();

        public const string WebConfigGroup = "WebConfigGroup";
        public const string LicJob = "LicJob";
        public const string SiteMapJobName = "SiteMapJob";

        private readonly Random _random = new Random();

        private readonly ConcurrentQueue<Action> _queueAction;
        private Task _currentTask;

        private readonly object _syncObject = new object();

        private TaskManager()
        {
            _queueAction = new ConcurrentQueue<Action>();
            var properties = new NameValueCollection();

            var dbThreadCount = Configuration.SettingProvider.GetInternalSetting("quartzThreadCount");
            properties["quartz.threadPool.threadCount"] =
                dbThreadCount.IsNotEmpty()
                    ? dbThreadCount
                    : Saas.SaasDataService.IsSaasEnabled
                        ? "3"
                        : "10";

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            Scheduler = schedulerFactory.GetScheduler();
            Scheduler.ListenerManager.AddJobListener(new BaseQuartzJobListener());
        }

        public static TaskManager TaskManagerInstance()
        {
            return taskManager;
        }

        public void Init()
        {
            InitWebConfigJobs();

            if (!Trial.TrialService.IsTrialEnabled)
            {
                if (!Scheduler.CheckExists(new JobKey(LicJob, WebConfigGroup)))
                {
                    var tType = typeof(LicJob);
                    var jDetail = new JobDetailImpl(LicJob, WebConfigGroup, tType);
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(LicJob, WebConfigGroup)
                        .WithCronSchedule(RandomizeCron("0 0 0 1/1 * ?", 59, 59, 4))
                        .ForJob(LicJob, WebConfigGroup)
                        .Build();

                    Scheduler.ScheduleJob(jDetail, trigger);
                }
            }
        }

        public void InitWebConfigJobs()
        {
            lock (_syncObject)
            {
                var config = (List<XmlNode>)ConfigurationManager.GetSection("TasksConfig");

                if (config == null) return;
                foreach (var nodeItem in config)
                {
                    if (nodeItem.NodeType == XmlNodeType.Comment)
                        continue;

                    if (nodeItem.Attributes == null ||
                        string.IsNullOrEmpty(nodeItem.Attributes["name"].Value) ||
                        string.IsNullOrEmpty(nodeItem.Attributes["type"].Value) ||
                        string.IsNullOrEmpty(nodeItem.Attributes["cronExpression"].Value))
                    {
                        return;
                    }

                    var jobName = nodeItem.Attributes["name"].Value;
                    var jobType = nodeItem.Attributes["type"].Value;
                    var cronExpression = nodeItem.Attributes["cronExpression"].Value;

                    var enabled = nodeItem.Attributes["enabled"].Value.TryParseBool();

                    if (!enabled)
                        continue;

                    var activationKey = nodeItem.Attributes["activationKey"]?.Value;
                    if (activationKey.IsNotEmpty() && JobActivationManager.GetActivityStatus(activationKey) is false)
                    {
                        RemoveTask(jobName, WebConfigGroup);
                        continue;
                    }

                    if (Scheduler.CheckExists(new JobKey(jobName, WebConfigGroup)))
                        continue;

                    var taskType = Type.GetType(jobType);
                    if (taskType == null)
                        continue;

                    // construct job info
                    var jobDetail = new JobDetailImpl(jobName, WebConfigGroup, taskType)
                    {
                        JobDataMap =
                        {
                            // каждый класс сам обработает хmlNode для себя
                            [DataMap] = nodeItem,
                        }
                    };

                    var rndSec = nodeItem.Attributes["rndSec"]?.Value.TryParseInt();
                    var rndMin = nodeItem.Attributes["rndMin"]?.Value.TryParseInt();
                    var rndHour = nodeItem.Attributes["rndHour"]?.Value.TryParseInt();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(jobName, WebConfigGroup)
                        .WithCronSchedule(RandomizeCron(cronExpression, rndSec, rndMin, rndHour))
                        .ForJob(jobName, WebConfigGroup)
                        .Build();

                    Scheduler.ScheduleJob(jobDetail, trigger);
                }
            }
        }

        public void Start()
        {
            Scheduler.Start();
        }

        public void ManagedTask(TaskSettings settings)
        {
            foreach (var setting in settings)
                AddUpdateTask(setting, TaskGroup);

            ReloadModulesTasks();
        }

        public void AddUpdateTask(TaskSetting setting, string taskGroup)
        {
            var name = setting.GetUniqueName();

            if (Scheduler.CheckExists(new JobKey(name, taskGroup)))
                Scheduler.DeleteJob(new JobKey(name, taskGroup));

            if (!setting.Enabled)
                return;

            if (string.IsNullOrEmpty(setting.JobType))
                return;

            var taskType = Type.GetType(setting.JobType);
            if (taskType == null)
                return;

            var jobDetail = new JobDetailImpl(name, taskGroup, taskType)
            {
                JobDataMap =
                {
                    [DataMap] = setting
                }
            };

            var cronExpression = GetCronString(setting);
            if (string.IsNullOrEmpty(cronExpression))
                return;

            var trigger = TriggerBuilder.Create()
                .WithIdentity(name, taskGroup)
                .WithCronSchedule(cronExpression)
                .ForJob(name, taskGroup)
                .Build();
            Scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void RemoveTask(string taskName, string group)
        {
            if (!Scheduler.CheckExists(new JobKey(taskName, group)))
                return;

            Scheduler.DeleteJob(new JobKey(taskName, group));
        }

        public void ReloadModulesTasks()
        {
            var modules = AttachedModules.GetModuleInstances<IModuleTask>();
            if (modules != null && modules.Count != 0)
            {
                foreach (var module in modules)
                {
                    var tasks = module.GetTasks();
                    if (tasks != null && tasks.Count != 0)
                    {
                        foreach (var task in tasks)
                            AddUpdateTask(task, ModuleGroup);
                    }
                }
            }
        }

        public void RemoveModuleTask(TaskSetting task)
        {
            if (!Scheduler.CheckExists(new JobKey(task.GetUniqueName(), ModuleGroup)))
                return;

            Scheduler.DeleteJob(new JobKey(task.GetUniqueName(), ModuleGroup));
        }

        public string GetTasks()
        {
            var res = (from jobGroupName in Scheduler.GetJobGroupNames()
                from jobKey in Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroupName))
                select Scheduler.GetJobDetail(jobKey)).ToList();

            return res.Aggregate(string.Empty, (current, item) => current + (";" + item.JobType));
        }

        public void AddTask(Action action)
        {
            if (!_queueAction.Contains(action))
                _queueAction.Enqueue(action);

            if (_currentTask != null && _currentTask.Status != TaskStatus.Created &&
                (_currentTask.IsCompleted == false
                 || _currentTask.Status == TaskStatus.Running
                 || _currentTask.Status == TaskStatus.WaitingToRun
                 || _currentTask.Status == TaskStatus.WaitingForActivation))
            {
                return;
            }

            _currentTask = Task.Factory.StartNew(DoJobs, TaskCreationOptions.LongRunning);
        }

        private void DoJobs()
        {
            while (_queueAction.TryDequeue(out var item))
            {
                if (item is null)
                    continue;

                try
                {
                    item();
                }
                catch (Exception e)
                {
                    Debug.Log.Error(e);
                }
            }
        }

        private string GetCronString(TaskSetting item)
        {
            switch (item.TimeType)
            {
                case TimeIntervalType.Days:
                    return RandomizeCron(string.Format("0 {1} {0} 1/1 * ?", item.TimeHours, item.TimeMinutes), 59);
                case TimeIntervalType.Hours:
                    return RandomizeCron("0 0 0/1 * * ?", 59, 59);
                case TimeIntervalType.Minutes:
                    return RandomizeCron("0 0/1 * * * ?", 59);
            }
            return string.Empty;
        }

        private string RandomizeCron(string cron, int? maxRandomSecond = null, int? maxRandomMinute = null, int? maxRandomHour = null)
        {
            var split = cron.Split(' ');

            if (maxRandomSecond.HasValue)
            {
                if (maxRandomSecond < 0 || maxRandomSecond > 59)
                    maxRandomSecond = 59;

                split[0] = _random.Next(maxRandomSecond.Value).ToString();
            }
            if (maxRandomMinute.HasValue)
            {
                if (maxRandomMinute < 0 || maxRandomMinute > 59)
                    maxRandomMinute = 59;
                split[1] = _random.Next(maxRandomMinute.Value).ToString();
            }
            if (maxRandomHour.HasValue)
            {
                if (maxRandomHour < 0 || maxRandomHour > 23)
                    maxRandomHour = 23;

                split[2] = _random.Next(maxRandomHour.Value).ToString();
            }

            return string.Join(" ", split);
        }
    }
}
