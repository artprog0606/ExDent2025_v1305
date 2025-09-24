using AdvantShop.MobileApp;
using System.Collections.Generic;

namespace AdvantShop.Core.Modules.Interfaces
{
    public interface INotificationService
    {
        string SendNotification(Notification notification);
        (bool Result, List<string> Messages) AllowSendNotification();
    }
}
