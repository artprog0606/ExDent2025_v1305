using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping.RussianPost.Api;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.RussianPost
{
    public class RussianPostOption : BaseShippingOption
    {
        public RussianPostCalculateOption CalculateOption { get; set; }

        public RussianPostOption() { }
        public RussianPostOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public EnMailCategory CashMailCategory { get; set; }
        public EnMailCategory MailCategory { get; set; }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = string.Empty,
                PickPointAddress = string.Empty,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
            {
                Rate = PriceCash;
                CalculateOption.MailCategory = CashMailCategory;
            }
            else
            {
                Rate = BasePrice;
                CalculateOption.MailCategory = MailCategory;
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
