using AdvantShop.Payment;

namespace AdvantShop.Models.Checkout
{
    public class OrderPayModel
    {
        public string OrderCode { get; set; }

        // public int? OrderId { get; set; }
        public int? PaymentMethodId { get; set; }
        public PageWithPaymentButton PageWithPaymentButton { get; set; }

        public bool ValidationDisabled { get; set; }
    }
}