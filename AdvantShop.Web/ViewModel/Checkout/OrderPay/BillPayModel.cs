using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.ViewModel.Checkout.OrderPay
{
    [PaymentOrderPayModel("Bill")]
    public class BillPayModel: OrderPayModel
    {
        public string StampImageName => (PaymentMethod as Bill)?.StampImageName;
        public override string ViewPath => "~/Views/Checkout/OrderPay/_Bill.cshtml";
    }
}