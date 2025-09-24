using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.ViewModel.Checkout.OrderPay
{
    [PaymentOrderPayModel("BillBy")]
    public class BillByPayModel: OrderPayModel
    {
        public string StampImageName => (PaymentMethod as BillBy)?.StampImageName;
        public override string ViewPath => "~/Views/Checkout/OrderPay/_BillBy.cshtml";
    }
}