using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Customers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.MyAccount
{
    public sealed class CancelOrder : AbstractCommandHandler
    {
        private readonly string _orderNumber;
        private Order _order;

        public CancelOrder(string orderNumber)
        {
            _orderNumber = orderNumber;
        }
        
        public CancelOrder(Order order)
        {
            _order = order;
        }

        protected override void Validate()
        {
            if (!CustomerContext.CurrentCustomer.RegistredUser)
                throw new BlException("Покупатель не зарегистрирован");
            
            if (_order == null && _orderNumber.IsNullOrEmpty())
                throw new BlException("Заказ не найден");
            
            if (_order == null)
                _order = OrderService.GetOrderByNumber(_orderNumber);
            
            if (_order == null)
                throw new BlException("Заказ не найден");
            
            if (_order.OrderStatus.CancelForbidden)
                throw new BlException("Заказ нельзя отменить");
            
            if (_order.OrderCustomer == null || _order.OrderCustomer.CustomerID != CustomerContext.CurrentCustomer.Id)
                throw new BlException("Покупатель не найден");
        }

        protected override void Handle()
        {
            OrderService.CancelOrder(_order.OrderID);

            var order = OrderService.GetOrder(_order.OrderID);

            var mail = new OrderStatusMailTemplate(order);

            MailService.SendMailNow(order.OrderCustomer.CustomerID, order.OrderCustomer.Email, mail);
            MailService.SendMailNow(SettingsMail.EmailForOrders, mail);
        }
    }
}