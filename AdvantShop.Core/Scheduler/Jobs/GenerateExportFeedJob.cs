//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    public class GenerateExportFeedJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!context.CanStart()) 
                return;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;

            if (jobData?.DataMap == null || !int.TryParse(jobData.DataMap.ToString(), out var exportFeedId))
                return;

            var exportFeed = ExportFeedService.GetExportFeed(exportFeedId);
            if (exportFeed == null)
            {
                TaskManager.TaskManagerInstance().RemoveTask(jobData.GetUniqueName(), TaskManager.TaskGroup);
                context.LogInformation($" ExportFeed: {exportFeedId} not found");
                return;
            }

            //var exportFeedSettings = ExportFeedSettingsProvider.GetSettings(exportFeed.Id);
            //if (exportFeedSettings == null)
            //    return;
            
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.ExportFeedsAutoUpdate &&
                (exportFeed.FeedType == EExportFeedType.YandexMarket ||
                 exportFeed.FeedType == EExportFeedType.YandexDirect ||
                 exportFeed.FeedType == EExportFeedType.YandexWebmaster ||
                 exportFeed.FeedType == EExportFeedType.GoogleMerchentCenter ||
                 exportFeed.FeedType == EExportFeedType.Avito))
            {
                context.LogInformation($" ExportFeed: {exportFeed.Id}-{exportFeed.FeedType} not run because of ExportFeedsAutoUpdate feature is off");
                return;
            }

            try
            {
                //var type = ReflectionExt.GetTypeByAttributeValue<ExportFeedKeyAttribute>(typeof(IExportFeed), atr => atr.Value, exportFeed.FeedType.ToString()) ??
                //           ReflectionExt.GetTypeByAttributeValue<ExportFeedKeyAttribute>(typeof(IExportFeed), atr => atr.Value, exportFeed.Type);
                //var currentExportFeed = (IExportFeed)Activator.CreateInstance(type, exportFeed.Id, false);
                var currentExportFeed = ExportFeedService.GetExportFeedInstance(exportFeed.Type, exportFeed.Id, false);

                //var filePath = exportFeedSettings.FileFullPath;
                //var directory = filePath.Substring(0, filePath.LastIndexOf('\\'));

                //if (!string.IsNullOrEmpty(directory))
                //    FileHelpers.CreateDirectory(directory);

                currentExportFeed.Export();

                //exportFeed.LastExport = DateTime.Now;
                //exportFeed.LastExportFileFullName = exportFeedSettings.FileFullName;

                //ExportFeedService.UpdateExportFeed(exportFeed);

                if (exportFeed.FeedType != EExportFeedType.None)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_ExportAuto, exportFeed.Type.ToString());
            }
            catch (Exception ex)
            {
                context.LogError(ex.Message + $" Exportfeed: {exportFeed.Id} {exportFeed.Type}");
                Debug.Log.Error($"GenerateExportFeedJob id {exportFeed.Id} {exportFeed.Type}", ex);
            }

            context.WriteLastRun();
        }
    }
}