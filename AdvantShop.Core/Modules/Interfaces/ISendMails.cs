//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Customers;

namespace AdvantShop.Core.Modules.Interfaces
{ 
    public interface ISendMails : IModule
    {
        bool SendMails(string title, string message, Services.Mails.EMailRecipientType recipientType);
        void SubscribeEmail(ISubscriber subscriber);
        void UnsubscribeEmail(string email);

        bool SendMails(string title, string message, Services.Mails.EMailRecipientType recipientType, string objectId);
        void SubscribeEmail(ISubscriber subscriber, string objectId);
        void UnsubscribeEmail(string email, string objectId);
    }
}