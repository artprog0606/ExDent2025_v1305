using System.Collections.Generic;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Shipping.Measoft.Api;
using AdvantShop.Payment;
using AdvantShop.Core.Services.Catalog;
using System;
using AdvantShop.Core.Common.Extensions;
using System.Linq;

namespace AdvantShop.Shipping.Measoft
{
    public class MeasoftPointOption : BaseShippingOption
    {
        public MeasoftPointOption() { }

        public MeasoftPointOption(ShippingMethod method) : base(method) 
        {
            HideAddressBlock = true;
        }

        public MeasoftPointOption(ShippingMethod method, float preCost) : base(method, preCost) 
        {
            HideAddressBlock = true;
        }

        public override string Id => MethodId + "_PickPoint_" + (Name + MethodId + DeliveryId).GetHashCode();

        public MeasoftPoint SelectedPoint { get; set; }

        public List<MeasoftPoint> ShippingPoints { get; set; }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        [JsonIgnore]
        public MeasoftCalcOption CalculateOption { get; set; }

        public int? PaymentCodCardId { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }

        public override string TemplateName
        {
            get { return "MeasoftOption.html"; }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint != null ? SelectedPoint.Id : string.Empty,
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

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
                SelectedPoint = ShippingPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
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
