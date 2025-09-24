using System.Collections.Generic;
using AdvantShop.Payment;

namespace AdvantShop.Models.Checkout
{
    public class GetBillingPaymentResponse
    {
        public BasePaymentOption selectPayment { get; set; }
        public List<BasePaymentOption> option { get; set; }
    }
}