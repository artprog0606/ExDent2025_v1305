using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Services.Crm.Instagram
{
    [DisallowConcurrentExecution]
    public class InstagramJob : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
            if (InstagramJobState.IsRun)
            {
                context.LogWarning("InstagramJobState.IsRun is still true");
                return;
            }

            InstagramJobState.IsRun = true;

            try
            {
                await Instagram.Instance.GetLastMessages();
            }
            catch (BlException ex)
            {
                SettingsInstagram.ErrorCount += 1;

                Debug.Log.Error(ex);
                context.LogError(ex.Message);
                StopJob();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                context.LogError(ex.Message);
            }
            finally
            {
                InstagramJobState.IsRun = false;
            }
        }

        private void StopJob()
        {
            // сбрасываем только пароль, чтобы не выкачивать все сообщения заново
            SettingsInstagram.Password = null;
            TaskManager.TaskManagerInstance().RemoveTask(nameof(InstagramJob), TaskManager.WebConfigGroup);
        }
    }
}
