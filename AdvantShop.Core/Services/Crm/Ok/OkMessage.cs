using AdvantShop.Core.Services.Crm.OK.Domain;
using System;
using System.Linq;

namespace AdvantShop.Core.Services.Crm.Ok
{
    public class OkMessage
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string FromUser { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public OkMessage() { }

        public OkMessage(OkWebhookNotification notification)
        {
            this.MessageId = notification.Message.MessageID.Split('.').Last();
            this.CreatedDate = new DateTime(1970, 1, 1, 3, 0, 0).AddMilliseconds(notification.Timestemp);
            this.ChatId = notification.Recipient.ChatID.Split(':').Last();
            this.UserId = AdvantShop.Configuration.SettingsOk.GroupId ?? "";
            this.FromUser = notification.Sender.UserID.Split(':').Last();
            this.Text = notification.Message.Text;
        }
    }

    public class OkUserMessage : OkMessage
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid CustomerId { get; set; }
        public string Photo { get; set; }
    }
}