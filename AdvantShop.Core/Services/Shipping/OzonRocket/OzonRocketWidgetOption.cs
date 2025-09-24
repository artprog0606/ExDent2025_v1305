using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketWidgetOption : BaseShippingOption
    {
        public OzonRocketWidgetOption()
        {
            HideAddressBlock = true;
        }

        public OzonRocketWidgetOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
        }

        public Dictionary<string, string> WidgetConfigData { get; set; }
        public string PointId { get; set; }
        public string PickpointAddress { get; set; }
        public string PickpointAdditionalData { get; set; }
        
        public OzonRocketCalculateOption CalculateOption { get; set; }
        
        [JsonIgnore]
        public List<OzonRocketShippingPoint> CurrentPoints { get; set; }

        public override string TemplateName => "OzonRocketWidgetOption.html";

        public override void Update(BaseShippingOption option)
        {
            if (option is OzonRocketWidgetOption opt && opt.Id == this.Id)
            {
                var selectedPoint = this.CurrentPoints?.FirstOrDefault(x => x.Id == opt.PointId);
                SelectedPoint = selectedPoint;
                if (this.CurrentPoints == null || selectedPoint != null)
                {
                    this.PointId = opt.PointId;
                    this.PickpointAddress = opt.PickpointAddress;
                    this.PickpointAdditionalData = opt.PickpointAdditionalData;
                }
                else
                {
                    this.PointId = null;
                    this.PickpointAddress = null;
                    this.PickpointAdditionalData = null;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return !string.IsNullOrEmpty(PointId)
                ? new OrderPickPoint
                {
                    PickPointId = PointId,
                    PickPointAddress = PickpointAddress ?? string.Empty,
                    AdditionalData = JsonConvert.SerializeObject(CalculateOption)
                }
                : null;
        }
    }
}