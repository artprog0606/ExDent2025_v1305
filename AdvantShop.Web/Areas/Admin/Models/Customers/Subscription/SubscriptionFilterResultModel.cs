using System;
using AdvantShop.Core.Services.Mails;

namespace AdvantShop.Web.Admin.Models.Customers.Subscription
{
    public class SubscriptionFilterResultModel
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public EMailRecipientType CustomerType { get; set; }

        public bool Enabled { get; set; }
        public DateTime? SubscribeDate { get; set; }
        public string SubscribeDateStr => SubscribeDate.HasValue ? SubscribeDate.Value.ToString("dd.MM.yyyy HH:mm") : string.Empty;
        public string SubscribeFromPage { get; set; }
        public string SubscribeFromIp { get; set; }
        public DateTime? UnsubscribeDate { get; set; }
        public string UnsubscribeDateStr => UnsubscribeDate.HasValue ? UnsubscribeDate.Value.ToString("dd.MM.yyyy HH:mm"): string.Empty;
    }
}
