using AdvantShop.Core.Modules;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Smses;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class NotifyStatusChanged
    {
        private readonly int _orderId;
        private readonly string _type;

        public NotifyStatusChanged(int orderId, string type)
        {
            _orderId = orderId;
            _type = type;
        }

        public bool Exectute()
        {
            var order = OrderService.GetOrder(_orderId);

            if (order == null || order.OrderStatus.Hidden)
                return false;

            switch (_type)
            {
                case "sms":
                    NotifyBySms(order);
                    ModulesExecuter.SendNotificationsOnOrderChangeStatus(order);
                    break;

                default:
                    NotifyByEmail(order);
                    break;
            }

            return true;
        }

        private void NotifyByEmail(Order order)
        {
            var mail = new OrderStatusMailTemplate(order);
            MailService.SendMailNow(order.OrderCustomer.CustomerID, order.OrderCustomer.Email, mail);
        }

        private void NotifyBySms(Order order)
        {
            SmsNotifier.SendSmsOnOrderStatusChanging(order);
        }

    }
}
