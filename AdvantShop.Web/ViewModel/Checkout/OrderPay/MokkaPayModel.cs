using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Payment;

namespace AdvantShop.ViewModel.Checkout.OrderPay
{
    [PaymentOrderPayModel("Mokka")]
    public class MokkaPayModel: OrderPayModel
    {
        public override string ViewPath => "~/Views/Checkout/OrderPay/_Mokka.cshtml";
    }
}