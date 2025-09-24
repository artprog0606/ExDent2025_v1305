using System;
using System.Data;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    internal class FileStorageRecalculateJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            RecalcAttachmentsSizeJob(context);

            context.TryRun(() => {
                SettingsMain.CurrentDbSize = (long)DataBaseService.CalcDbSize().Bytes;
            });

            Debug.Log.Info($"FilesStorageService.RecalcAttachmentsSize took {SettingsMain.CurrentFilesStorageSwTime} milliseconds");
        }

        private static void RecalcAttachmentsSizeJob(IJobExecutionContext context, bool retry = false)
        {
            var result = FilesStorageService.RecalcAttachmentsSize();

            if (retry || result)
                return;

            context.LogInformation($"{nameof(FilesStorageService.RecalcAttachmentsSize)} was already running");

            Thread.Sleep(TimeSpan.FromMinutes(new Random().Next(1, 5)));
            RecalcAttachmentsSizeJob(context, true);
        }
    }
}
