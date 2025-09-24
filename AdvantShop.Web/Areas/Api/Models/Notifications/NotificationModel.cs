using System;

namespace AdvantShop.Areas.Api.Models.Notifications
{
    public class NotificationModel
    {
        public Guid CustomerId { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
    }
}