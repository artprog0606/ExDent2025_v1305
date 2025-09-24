using System.Linq;
using System.Collections.Generic;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Shipping.LPost.Api;
using AdvantShop.Payment;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Shipping.PointDelivery;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.LPost
{
    public class LPostDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public string PickpointId { get; set; }
        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        [JsonIgnore]
        public List<LPostPoint> CurrentPoints { get; set; }
        public LPostOptionParams CalculateOption { get; set; }

        public float BasePrice { get; set; }

        public float PriceCash { get; set; }

        public LPostDeliveryMapOption() { }

        public LPostDeliveryMapOption(ShippingMethod method) : base(method) { }

        public LPostDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        //public override string Id
        //{
        //    get { return MethodId + "_" + MethodId.GetHashCode(); }
        //}


        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as LPostDeliveryMapOption;
            if (opt != null && opt.Id == Id)
            {
                if (CurrentPoints != null && CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    PickpointId = opt.PickpointId;
                    YaSelectedPoint = opt.YaSelectedPoint;
                    SelectedPoint = CurrentPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
                    if (SelectedPoint != null && SelectedPoint is LPostPoint selectedPoint)
                        IsAvailablePaymentCashOnDelivery &= selectedPoint.AvailableCardOnDelivery is true || selectedPoint.AvailableCashOnDelivery is true;
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
                    YaSelectedPoint = SelectedPoint.Id.TryParseInt(SelectedPoint.Id.GetHashCode());
            }
        }
    }
}
