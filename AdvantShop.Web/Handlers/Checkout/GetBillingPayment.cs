using System;
using System.Linq;
using AdvantShop.Handlers.MyAccount;
using AdvantShop.Models.Checkout;
using AdvantShop.Orders;
using AdvantShop.Payment;

namespace AdvantShop.Handlers.Checkout
{
    public class GetBillingPayment
    {
        private readonly Order _order;

        public GetBillingPayment(Order order)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
        }

        public GetBillingPaymentResponse Execute()
        {
            var options = new GetPaymentsOfOrderForCustomer(_order).Get();
            BasePaymentOption selectedPayment = null;
            if (options != null)
                selectedPayment = options.FirstOrDefault(x => x.Id == _order.PaymentMethodId)
                                  ?? options.FirstOrDefault();

            return new GetBillingPaymentResponse
            {
                selectPayment = selectedPayment,
                option = options
            };
        }
    }
}