using AdvantShop.Core.Services.Bonuses.Notification;

namespace AdvantShop.Core.Services.Bonuses.Model
{
    public class NotificationTemplate
    {
        public ENotifcationType NotificationTypeId { get; set; }
        public string NotificationBody { get; set; }
        public EBonusNotificationMethod NotificationMethod { get; set; }

        public int NotificationTemplateId { get; set; }
    }
}
