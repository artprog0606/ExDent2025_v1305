using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PickPoint
{
    public class PickPointDeliveryMapOption : BaseShippingOption, PointDelivery.IPointDeliveryMapOption
    {
        public PickPointDeliveryMapOption()
        {
        }

        public PickPointDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public PointDelivery.MapParams MapParams { get; set; }
        public PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        [JsonIgnore]
        public List<PickPointShippingPoint> CurrentPoints { get; set; }

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PickPointDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                if (this.CurrentPoints != null && this.CurrentPoints.Any(x => x.Id == opt.PickpointId))
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = this.CurrentPoints.FirstOrDefault(x => x.Id == opt.PickpointId);
                    if (this.SelectedPoint != null)
                        this.IsAvailablePaymentCashOnDelivery &=
                            this.SelectedPoint.AvailableCashOnDelivery is true || SelectedPoint.AvailableCardOnDelivery is true;
                }
                else
                {
                    this.PickpointId = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PickpointId)
                ? new OrderPickPoint
                {
                    PickPointId = PickpointId,
                    PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : null,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
                }
                : null;
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
    }
}
