using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.PickPoint
{
    public class PickPointOption : BaseShippingOption
    {
        public List<PickPointShippingPoint> ShippingPoints { get; set; }

        public PickPointOption()
        {
        }

        public PickPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string TemplateName
        {
            get { return "PickPointOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PickPointOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
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
            if (SelectedPoint == null) return null;
            var temp = new OrderPickPoint
            {
                PickPointId = SelectedPoint.Id,
                PickPointAddress = SelectedPoint.Address,
                AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
            };
            return temp;
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
                SelectedPoint = ShippingPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
        }
    }
}
