using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.LPost
{
    public class LPostOption : BaseShippingOption
    {
        public LPostOption() { }

        public LPostOption(ShippingMethod method) : base(method) { }

        public LPostOption(ShippingMethod method, float preCost)
            : base(method, preCost)  { }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
                Rate = PriceCash;
            else
            {
                Rate = BasePrice;
            }
            return true;
        }

        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            return string.Format("Стоимость доставки увеличится на {0}", diff.RoundPrice().FormatPrice());
        }
    }
}
