using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Payment;

namespace AdvantShop.Shipping.Edost
{
    public class EdostCashOnDeliveryOption : EdostOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public float Transfer { get; set; }

        public EdostCashOnDeliveryOption()
        {
        }

        public EdostCashOnDeliveryOption(ShippingMethod method, float preCost, EdostTarif tarif)
            : base(method, preCost, tarif)
        {
            BasePrice = tarif.Price;
            PriceCash = tarif.PriceCash ?? 0f;
            Transfer = tarif.PriceTransfer ?? 0f;

            Rate = BasePrice;

            IsAvailablePaymentCashOnDelivery = true;
        }

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

        //todo research
        public override string GetDescriptionForPayment()
        {
            var diff = PriceCash - BasePrice;
            if (diff <= 0)
                return string.Empty;

            string transferMessage = Transfer > 0 ? 
                string.Format(LocalizationService.GetResource("Edost.CachOnDelivery.Transfer"), 
                        Transfer.RoundPrice().FormatPrice(), (diff + Transfer).RoundPrice().FormatPrice()) 
                : string.Empty;

            return string.Format(LocalizationService.GetResource("Edost.CachOnDelivery.Sum"), diff.RoundPrice().FormatPrice(), transferMessage);
        }
    }
}