//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using Quartz;

namespace AdvantShop.Core.Scheduler
{
    public class TaskSettings : List<TaskSetting>
    {
        public static TaskSettings Settings
        {
            get
            {
                var fromDb = SettingProvider.Items["TaskSqlSettings"];
                if (fromDb == null)
                    return new TaskSettings();

                var temp = JsonConvert.DeserializeObject<TaskSettings>(SQLDataHelper.GetString(fromDb));
                if (temp == null)
                    return new TaskSettings();

                return temp;
            }
            set => SettingProvider.Items["TaskSqlSettings"] = JsonConvert.SerializeObject(value);
        }
    }

    public class TaskSetting
    {
        public string JobType { get; set; }
        public bool Enabled { get; set; }
        public int TimeInterval { get; set; }
        public int TimeHours { get; set; }
        public int TimeMinutes { get; set; }
        public TimeIntervalType TimeType { get; set; }

        public object DataMap { get; set; }

        public string GetUniqueName()
        {
            return JobType + (DataMap != null
                ? "_" + DataMap.GetHashCode()
                : string.Empty);
        }
    }

    public static class JobExt
    {
        /// <summary>
        /// Может ли job запуститься. Проверка работает только для задач TaskSetting
        /// </summary>
        public static bool CanStart(this IJobExecutionContext obj)
        {
            var dataMap = obj.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;

            if (jobData == null)
            {
                obj.LogWarning("Job can't start because of dataMap is not TaskSetting");
                return false;
            }

            var dir = SettingsGeneral.AbsolutePath + "App_Data/Jobs/";
            FileHelpers.CreateDirectory(dir);

            var file = dir + jobData.GetUniqueName() + ".txt";
            var lastTime = System.IO.File.Exists(file)
                ? System.IO.File.ReadAllText(file).TryParseDateTime(true)
                : null;
            var currentTime = DateTime.Now;

            if (!lastTime.HasValue)
                return true;

            bool result;
            DateTime lastDateTime;
            switch (jobData.TimeType)
            {
                case TimeIntervalType.Days:
                    lastDateTime = new DateTime(lastTime.Value.Year, lastTime.Value.Month, lastTime.Value.Day, 0, 0, 0);
                    result = (currentTime - lastDateTime).TotalDays >= jobData.TimeInterval;
                    break;
                case TimeIntervalType.Hours:
                    lastDateTime = new DateTime(lastTime.Value.Year, lastTime.Value.Month, lastTime.Value.Day, lastTime.Value.Hour, 0, 0);
                    result = (currentTime - lastDateTime).TotalHours >= jobData.TimeInterval;
                    break;
                case TimeIntervalType.Minutes:
                    result = (currentTime - lastTime.Value).TotalMinutes >= jobData.TimeInterval;
                    break;
                case TimeIntervalType.None:
                default:
                    result = true;
                    break;
            }

            if (!result)
                obj.LogInformation($"Job can't start because of TimeIntervalType is {jobData.TimeType.ToString()} with Interval {jobData.TimeInterval} [last job run was at {lastTime.Value:dd/MM/yyyy HH:mm:ss:fff z} (time passed => {currentTime - lastTime.Value})]");
            return result;
        }

        public static void WriteLastRun(this IJobExecutionContext obj)
        {
            var dataMap = obj.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;

            var fileName = jobData == null
                ? obj.JobDetail.Key.Name
                : jobData.GetUniqueName();

            if (string.IsNullOrEmpty(fileName))
                return;

            var dir = SettingsGeneral.AbsolutePath + "App_Data/Jobs/";
            FileHelpers.CreateDirectory(dir);

            var file = dir + fileName + ".txt";
            System.IO.File.WriteAllText(file, DateTime.Now.ToString());
        }
    }
}
