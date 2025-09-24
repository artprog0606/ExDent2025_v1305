using AdvantShop.Orders;
using AdvantShop.Shipping.Measoft.Api;
using Newtonsoft.Json;
using AdvantShop.Payment;
using AdvantShop.Core.Services.Catalog;
using System;

namespace AdvantShop.Shipping.Measoft
{
    public class MeasoftShippingOption : BaseShippingOption
    {
        public MeasoftShippingOption() { }

        public MeasoftShippingOption(ShippingMethod method) : base(method) { }

        public MeasoftShippingOption(ShippingMethod method, float preCost)
            : base(method, preCost) { }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public MeasoftCalcOption CalculateOption { get; set; }

        public int? PaymentCodCardId { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }

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

        public override bool AvailablePayment(BasePaymentOption payOption)
        {
            if (payOption.GetDetails()?.IsCashOnDeliveryPayment is true)
                return IsAvailablePaymentCashOnDelivery &&
                    (PaymentCodCardId != payOption.Id || CashOnDeliveryCardAvailable);

            return base.AvailablePayment(payOption);
        }
    }
}
