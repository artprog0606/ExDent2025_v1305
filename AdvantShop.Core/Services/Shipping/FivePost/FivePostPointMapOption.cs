using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping.FivePost.CalculateCost;
using AdvantShop.Shipping.PointDelivery;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.FivePost
{
    public class FivePostPointMapOption : BaseShippingOption, IPointDeliveryMapOption
    {
        public string PickpointId { get; set; }
        public MapParams MapParams { get; set; }
        public PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }

        [JsonIgnore]
        public List<FivePostPoint> CurrentPoints { get; set; }

        public float BasePrice { get; set; }
        public float PriceCash { get; set; }
        public int? PaymentCodCardId { get; set; }
        public bool CashOnDeliveryCardAvailable { get; set; }

        public FivePostCalculationParams CalculateOption { get; set; }

        public FivePostPointMapOption() { }

        public FivePostPointMapOption(ShippingMethod method) : base(method) 
        {
            HideAddressBlock = true;
        }

        public FivePostPointMapOption(ShippingMethod method, float preCost) : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public override string TemplateName => "PointDeliveryMapOption.html";

        public override void Update(BaseShippingOption option)
        {
            var opt = option as FivePostPointMapOption;
            if (opt != null && opt.Id == Id)
            {
                if (CurrentPoints != null && CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    PickpointId = opt.PickpointId;
                    YaSelectedPoint = opt.YaSelectedPoint;
                    SelectedPoint = CurrentPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
                    if (SelectedPoint != null && SelectedPoint is FivePostPoint selectedPoint)
                        IsAvailablePaymentCashOnDelivery &= selectedPoint.AvailableCashOnDelivery is true || selectedPoint.AvailableCardOnDelivery is true;
                }
                else
                    PickpointId = null;
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return PickpointId.IsNotEmpty()
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
