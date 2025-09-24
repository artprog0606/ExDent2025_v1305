using AdvantShop.Configuration;
using AdvantShop.Web.Admin.Models.Settings;

namespace AdvantShop.Web.Admin.Handlers.Settings.Tasks
{
    public class LoadSaveTasksSettings
    {
        private TasksSettingsModel _model;

        public LoadSaveTasksSettings(TasksSettingsModel model = null)
        {
            _model = model;
        }

        public TasksSettingsModel Load()
        {
            _model = new TasksSettingsModel()
            {
                WebNotificationInNewTab = SettingsNotifications.WebNotificationInNewTab,
                TasksActive = SettingsTasks.TasksActive,
                ReminderActive = SettingsTasks.ReminderActive
            };

            return _model;
        }

        public void Save()
        {
            SettingsNotifications.WebNotificationInNewTab = _model.WebNotificationInNewTab;
            SettingsTasks.TasksActive = _model.TasksActive;
            SettingsTasks.ReminderActive = _model.ReminderActive;
        }
    }
}
