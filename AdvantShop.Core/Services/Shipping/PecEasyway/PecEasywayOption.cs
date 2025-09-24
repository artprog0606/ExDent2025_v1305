using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PecEasyway
{
    public class PecEasywayOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public PecEasywayCalculateOption CalculateOption { get; set; }

        public PecEasywayOption()
        {
        }

        public PecEasywayOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            var temp = new OrderPickPoint
            {
                PickPointId = string.Empty,
                PickPointAddress = string.Empty,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
            return temp;
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
                Rate = PriceCash;
            else
                Rate = BasePrice;
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
