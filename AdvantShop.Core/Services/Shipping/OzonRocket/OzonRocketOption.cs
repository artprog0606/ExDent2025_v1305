using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.OzonRocket
{
    public class OzonRocketOption : BaseShippingOption
    {
        public OzonRocketCalculateOption CalculateOption { get; set; }

        public List<OzonRocketShippingPoint> ShippingPoints { get; set; }
    
        public OzonRocketOption()
        {
        }

        public OzonRocketOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }
 
        public override string TemplateName
        {
            get { return "OzonRocketOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            if (option is OzonRocketOption opt && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints?.FirstOrDefault();
                if (this.SelectedPoint != null)
                    this.IsAvailablePaymentCashOnDelivery &=
                        this.SelectedPoint.AvailableCashOnDelivery is true || this.SelectedPoint.AvailableCardOnDelivery is true;
            }
        }
        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint == null ? string.Empty : SelectedPoint.Id,
                PickPointAddress = SelectedPoint == null ? string.Empty : SelectedPoint.Address,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
        }  
    }
}