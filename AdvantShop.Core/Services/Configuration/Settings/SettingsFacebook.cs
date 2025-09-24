using System;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Configuration
{
    public class SettingsFacebook
    {
        public static string ClientId
        {
            get => SettingProvider.Items["SettingsFacebook.ClientId"];
            set => SettingProvider.Items["SettingsFacebook.ClientId"] = value;
        }

        public static string ClientSecret
        {
            get => SettingProvider.Items["SettingsFacebook.ClientSecret"];
            set => SettingProvider.Items["SettingsFacebook.ClientSecret"] = value;
        }

        /// <summary>
        /// Токен с правами только на юзера (сначала нужно получить группы, а уже потом авторизоваться от имени группы)
        /// </summary>
        public static string UserToken
        {
            get => SettingProvider.Items["SettingsFacebook.TokenUser"];
            set => SettingProvider.Items["SettingsFacebook.TokenUser"] = value;
        }

        
        public static string GroupId
        {
            get => SettingProvider.Items["SettingsFacebook.GroupId"];
            set => SettingProvider.Items["SettingsFacebook.GroupId"] = value;
        }

        public static string GroupName
        {
            get => SettingProvider.Items["SettingsFacebook.GroupName"];
            set => SettingProvider.Items["SettingsFacebook.GroupName"] = value;
        }

        /// <summary>
        /// Токен с правами на группу
        /// </summary>
        public static string GroupToken
        {
            get => SettingProvider.Items["SettingsFacebook.GroupToken"];
            set => SettingProvider.Items["SettingsFacebook.GroupToken"] = value;
        }

        public static string GroupPerms
        {
            get => SettingProvider.Items["SettingsFacebook.GroupPerms"];
            set => SettingProvider.Items["SettingsFacebook.GroupPerms"] = value;
        }


        public static int TokenUserErrorCount
        {
            get => Convert.ToInt32(SettingProvider.Items["SettingsFacebook.TokenGroupUserCount"]);
            set => SettingProvider.Items["SettingsFacebook.TokenGroupUserCount"] = value.ToString();
        }

        public static int TokenGroupErrorCount
        {
            get => Convert.ToInt32(SettingProvider.Items["SettingsFacebook.TokenGroupErrorCount"]);
            set => SettingProvider.Items["SettingsFacebook.TokenGroupErrorCount"] = value.ToString();
        }

        public static bool IsDataLoaded
        {
            get => Convert.ToBoolean(SettingProvider.Items["SettingsFacebook.IsDataLoaded"]);
            set => SettingProvider.Items["SettingsFacebook.IsDataLoaded"] = value.ToString();
        }
        

        /// <summary>
        /// Время последнего запроса, когда получили ошибку с ограничением к API
        /// https://developers.facebook.com/docs/graph-api/advanced/rate-limiting 
        /// </summary>
        public static DateTime StopTime
        {
            get
            {
                var str = SettingProvider.Items["SettingsFacebook.StopTime"];
                return !string.IsNullOrEmpty(str) ? str.TryParseDateTime() : DateTime.MinValue;
            }
            set => SettingProvider.Items["SettingsFacebook.StopTime"] = value.ToString();
        }


        public static string VerifyToken
        {
            get
            {
                var t = SettingProvider.Items["SettingsFacebook.VerifyToken"];
                if (string.IsNullOrEmpty(t))
                    t = VerifyToken = Guid.NewGuid().ToString("N");
                return t;
            }
            set => SettingProvider.Items["SettingsFacebook.VerifyToken"] = value;
        }

        /// <summary>
        /// Создавать лид из личных сообщений
        /// </summary>
        public static bool CreateLeadFromMessages
        {
            get
            {
                var value = SettingProvider.Items["SettingsFacebook.CreateLeadFromMessages"];
                return value == null || Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsFacebook.CreateLeadFromMessages"] = value.ToString();
        }

        /// <summary>
        /// Создавать лид из комментариев
        /// </summary>
        public static bool CreateLeadFromComments
        {
            get
            {
                var value = SettingProvider.Items["SettingsFacebook.CreateLeadFromComments"];
                return Convert.ToBoolean(value);
            }
            set => SettingProvider.Items["SettingsFacebook.CreateLeadFromComments"] = value.ToString();
        }
    }
}