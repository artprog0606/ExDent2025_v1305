using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Diagnostics;
using System;

namespace AdvantShop.MobileApp
{
    public class NotificationService
    {
        public static INotificationService GetActiveModule()
        {
            foreach (var moduleType in AttachedModules.GetModules<INotificationService>())
            {
                var module = (INotificationService)Activator.CreateInstance(moduleType, null);
                if (module != null)
                    return module;
            }

            return null;
        }

        public static string SendNotification(Notification notification)
        {
            if (notification.Title.IsNullOrEmpty() && notification.Body.IsNullOrEmpty())
                return "Не заполнен заголовок и тело уведомления";

            try
            {
                var module = GetActiveModule();
                if (module == null)
                    return "Нет активных модулей для отправки Push уведомлений";
                return module.SendNotification(notification);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return ex.Message;
            }
        }
    }
}
