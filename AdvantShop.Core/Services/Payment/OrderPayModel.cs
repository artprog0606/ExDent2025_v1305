using AdvantShop.Orders;
using AdvantShop.Payment;

namespace AdvantShop.Core.Services.Payment
{
    public class OrderPayModel
    {
        public Order Order { get; set; }
        public string PaymentType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PageWithPaymentButton PageWithPaymentButton { get; set; }
        public string Javascript { get; set; }
        public string ButtonHref { get; set; }
        public string ButtonOnClick { get; set; }
        public PaymentForm PaymentForm { get; set; }
        
        public virtual string ViewPath => null;

        public bool ValidationDisabled { get; set; }
    }
}