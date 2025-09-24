using System.Collections.Generic;
using System.Linq;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketDeliveryMapOption : BaseShippingOption, Shipping.PointDelivery.IPointDeliveryMapOption
    {
        public OzonRocketDeliveryMapOption()
        {
        }

        public OzonRocketDeliveryMapOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public Shipping.PointDelivery.MapParams MapParams { get; set; }
        public Shipping.PointDelivery.PointParams PointParams { get; set; }
        public int YaSelectedPoint { get; set; }
        public string PickpointId { get; set; }
        [JsonIgnore]
        public List<OzonRocketShippingPoint> CurrentPoints { get; set; }
        public OzonRocketCalculateOption CalculateOption { get; set; }

        public override string TemplateName
        {
            get { return "PointDeliveryMapOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as OzonRocketDeliveryMapOption;
            if (opt != null && opt.Id == this.Id)
            {
                var selectedPoint = this.CurrentPoints?.FirstOrDefault(x => x.Id == opt.PickpointId);
                if (selectedPoint != null)
                {
                    this.PickpointId = opt.PickpointId;
                    this.YaSelectedPoint = opt.YaSelectedPoint;
                    this.SelectedPoint = selectedPoint;
                    this.IsAvailablePaymentCashOnDelivery &=
                        selectedPoint.AvailableCashOnDelivery is true || selectedPoint.AvailableCardOnDelivery is true;
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
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
        }
    }
}