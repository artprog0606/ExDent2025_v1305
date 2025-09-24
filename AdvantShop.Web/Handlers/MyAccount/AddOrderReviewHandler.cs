using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.AdminPushNotification;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.MyAccount
{
    public class AddOrderReviewHandler : AbstractCommandHandler<OrderReview>
    {
        private readonly Order _order;
        private readonly float _ratio;
        private readonly string _text;
        private readonly Guid _customerId;

        public AddOrderReviewHandler(int orderId, float ratio, string text, Guid customerId) : this(ratio, text, customerId)
        {
            _order = OrderService.GetOrder(orderId);
        }
        
        public AddOrderReviewHandler(string orderNumber, float ratio, string text, Guid customerId) : this(ratio, text, customerId)
        {
            _order = OrderService.GetOrderByNumber(orderNumber);
        }
        
        public AddOrderReviewHandler(float ratio, string text, Guid customerId)
        {
            _ratio = ratio;
            _text = text;
            _customerId = customerId;
        }

        protected override void Validate()
        {
            if (_order == null)
                throw new BlException("Заказ не найден");
            
            if (_order.OrderCustomer != null && _order.OrderCustomer.CustomerID != _customerId)
                throw new BlException("Заказ не найден для этого покупателя");
            
            if (_order.OrderReview != null)
                throw new BlException("Отзыв уже добавлен");
            
            if (string.IsNullOrWhiteSpace(_text) && _ratio == 0)
                throw new BlException("Укажите оценку или комментарий");
            
            if (_ratio != 0 && (_ratio < 1 || _ratio > 5))
                throw new BlException("Оценка должна быть от 1 до 5");
        }

        protected override OrderReview Handle()
        {
            var orderReview = new OrderReview
            {
                OrderId = _order.OrderID,
                Ratio = _ratio,
                Text = HttpUtility.HtmlEncode(_text)
            };
            
            try
            {
                OrderService.AddOrderReview(orderReview);
                
                var mail = new NewOrderReviewMailTemplate(orderReview, _order.Number);
                MailService.SendMailNow(SettingsMail.EmailForProductDiscuss, mail);

                AdminPushNotificationService.NewOrderReview(_order);
            } 
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                throw new BlException("Ошибка при добавлении отзыва");
            }
            
            return orderReview;
        }
    }
}