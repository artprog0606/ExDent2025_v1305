using System;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using InstaSharper.Classes;
using Newtonsoft.Json;

namespace AdvantShop.Configuration
{
    public class SettingsInstagram
    {
        public static string Login
        {
            get => SettingProvider.Items["SettingsInstagram.Login"];
            set
            {
                SettingProvider.Items["SettingsInstagram.Login"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        public static string Password
        {
            get => SettingProvider.Items["SettingsInstagram.Password"];
            set
            {
                SettingProvider.Items["SettingsInstagram.Password"] = value;
                JobActivationManager.SettingUpdated();
            }
        }

        /// <summary>
        /// Имя текущего пользователя
        /// </summary>
        public static string UserName
        {
            get => SettingProvider.Items["SettingsInstagram.UserName"];
            set => SettingProvider.Items["SettingsInstagram.UserName"] = value;
        }

        /// <summary>
        /// Id текущего пользователя
        /// </summary>
        public static string UserPk
        {
            get => SettingProvider.Items["SettingsInstagram.UserPk"];
            set => SettingProvider.Items["SettingsInstagram.UserPk"] = value;
        }

        public static int ErrorCount
        {
            get => Convert.ToInt32(SettingProvider.Items["SettingsInstagram.ErrorCount"]);
            set => SettingProvider.Items["SettingsInstagram.ErrorCount"] = value.ToString();
        }


        public static DateTime LastTheadActivity
        {
            get => SettingProvider.Items["SettingsInstagram.LastTheadActivity"].TryParseDateTime();
            set => SettingProvider.Items["SettingsInstagram.LastTheadActivity"] = value.ToString();
        }

        public static DateTime LastMediaActivity
        {
            get => SettingProvider.Items["SettingsInstagram.LastMediaActivity"].TryParseDateTime();
            set => SettingProvider.Items["SettingsInstagram.LastMediaActivity"] = value.ToString();
        }

        /// <summary>
        /// Создавать лид из сообщений в direct
        /// </summary>
        public static bool CreateLeadFromDirectMessages
        {
            get
            {
                var value = SettingProvider.Items["SettingsInstagram.CreateLeadFromDirectMessages"];
                return value == null || Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsInstagram.CreateLeadFromDirectMessages"] = value.ToString();
        }

        /// <summary>
        /// Создавать лид из комментариев
        /// </summary>
        public static bool CreateLeadFromComments
        {
            get
            {
                var value = SettingProvider.Items["SettingsInstagram.CreateLeadFromComments"];
                return Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsInstagram.CreateLeadFromComments"] = value.ToString();
        }
        

        public static InstaApiData InstaApiData
        {
            get
            {
                var json = SettingProvider.Items["SettingsInstagram.InstaApiData"];
                if (string.IsNullOrEmpty(json))
                    return new InstaApiData();

                var data = JsonConvert.DeserializeObject<InstaApiData>(json);

                return data ?? new InstaApiData();
            }
            set =>
                SettingProvider.Items["SettingsInstagram.InstaApiData"] = value != null
                    ? JsonConvert.SerializeObject(value)
                    : "";
        }
    }
}