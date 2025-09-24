using System;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Services.Crm.Facebook
{
    [DisallowConcurrentExecution]
    public class FacebookJob : IJob
    {
        private readonly FacebookApiService _fbService = new FacebookApiService();

        public void Execute(IJobExecutionContext context)
        {
            // Job останавливает сам себя если не настроен или произошла ошибка. 
            // После перенастройки запускается снова.
            if (!CanRunJob())
            {
                StopJob();
                return;
            }
            
            var stopTime = SettingsFacebook.StopTime;

            if (stopTime != DateTime.MinValue && stopTime.AddMinutes(30) > DateTime.Now)
                return;

            if (FacebookJobState.IsRun)
                return;

            try
            {
                FacebookJobState.IsRun = true;

                _fbService.GetLastMessages();
            }
            catch (BlException ex)
            {
                Debug.Log.Error(ex);
                StopJob();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            finally
            {
                FacebookJobState.IsRun = false;
            }
        }


        private bool CanRunJob()
        {
            return _fbService.IsActive();
        }

        private void StopJob()
        {
            SettingsFacebook.GroupToken = null;

            TaskManager.TaskManagerInstance().RemoveTask(nameof(FacebookJob), TaskManager.WebConfigGroup);
        }
    }
}
