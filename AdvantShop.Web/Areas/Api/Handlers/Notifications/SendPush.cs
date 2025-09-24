 using AdvantShop.Areas.Api.Models.Notifications;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.MobileApp;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Notifications
{
    public class SendPush : AbstractCommandHandler<SendPushResponse>
    {
        private readonly NotificationModel _notification;
        
        public SendPush(NotificationModel notification)
        {
            _notification = notification;
        }
        
        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_notification.Title) || string.IsNullOrWhiteSpace(_notification.Body))
                throw new BlException("Укажите заголовок и сообщение уведомления");

            var customer = CustomerService.GetCustomer(_notification.CustomerId);
            if (customer == null)
                throw new BlException("Пользователь не найден");
        }
        
        
        protected override SendPushResponse Handle()
        {
            var result = NotificationService.SendNotification(new Notification()
            {
                CustomerId = _notification.CustomerId,
                Title = _notification.Title,
                Body = _notification.Body
            });

            if (!string.IsNullOrEmpty(result))
                throw new BlException(result);
            
            return new SendPushResponse();
        }
    }
}