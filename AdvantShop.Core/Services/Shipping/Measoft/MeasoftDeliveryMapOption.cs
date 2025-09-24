using System.Linq;
using System.Collections.Generic;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Shipping.Measoft.Api;
using AdvantShop.Payment;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Shipping.PointDelivery;

namespace AdvantShop.Shipping.Measoft
{
    public class MeasoftDeliveryMapOption : BaseShippingOption, IPointDeliveryMapOption
    {
        public string PickpointId { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        [JsonIgnore]
        public List<MeasoftPoint> CurrentPoints { get; set; }
        [JsonIgnore]
        public MeasoftCalcOption CalculateOption { get; set; }

        public MeasoftDeliveryMapOption() { }

        public MeasoftDeliveryMapOption(ShippingMethod method) : base(method) { }

        public MeasoftDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            Name = method.Name;
            ShippingType = method.ShippingType;
        }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public int? PaymentCodCardId { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }

        public override string Id => MethodId + "_PickPoint_" + (Name + MethodId + DeliveryId).GetHashCode();

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as MeasoftDeliveryMapOption;
            if (opt != null && opt.Id == Id)
            {
                var selectedPoint = CurrentPoints?.FirstOrDefault(x => x.Id == opt.PickpointId);
                if (selectedPoint != null)
                {
                    PickpointId = opt.PickpointId;
                    YaSelectedPoint = opt.YaSelectedPoint;
                    SelectedPoint = selectedPoint;
                    if (SelectedPoint != null)
                        IsAvailablePaymentCashOnDelivery &=
                            selectedPoint.AvailableCardOnDelivery is true || selectedPoint.AvailableCashOnDelivery is true;
                }
                else
                    PickpointId = null;
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : null,
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
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
            {
                PickpointId = orderPickPoint.PickPointId;
                SelectedPoint = CurrentPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
                if (SelectedPoint != null)
                    YaSelectedPoint = SelectedPoint.Id.GetHashCode();
            }
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
