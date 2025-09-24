using AdvantShop.Areas.Api.Models.Orders;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Customers;
using AdvantShop.Handlers.MyAccount;
using AdvantShop.Orders;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Orders
{
    public class AddOrderReview : AbstractCommandHandler<AddOrderReviewResponse>
    {
        private readonly int _orderId;
        private readonly Customer _customer;
        private readonly OrderReview _review;

        public AddOrderReview(int orderId, OrderReview review) 
                    : this(orderId, CustomerContext.CurrentCustomer, review)
        {
        }

        public AddOrderReview(int orderId, Customer customer, OrderReview review)
        {
            _orderId = orderId;
            _customer = customer;
            _review = review;
        }

        protected override void Validate()
        {
            if (!SettingsCheckout.AllowAddOrderReview)
                throw new BlException("Оценка заказов недоступна");
            if (SettingsCheckout.ReviewOnlyPaidOrder && !OrderService.IsPaidOrder(_orderId))
                throw new BlException("Нельзя оставить отзыв к неоплаченному заказу");
        }

        protected override AddOrderReviewResponse Handle()
        {
            var review = new AddOrderReviewHandler(_orderId, _review.Ratio, _review.Text, _customer.Id).Execute();
            if (review == null)
                throw new BlException("Ошибка при добавлении отзыва");
            
            return new AddOrderReviewResponse();
        }
    }
}