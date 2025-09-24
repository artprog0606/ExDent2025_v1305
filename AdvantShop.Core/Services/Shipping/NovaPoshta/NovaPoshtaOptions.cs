using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.NovaPoshta
{
    public class NovaPoshtaOptions : BaseShippingOption
    {
        public List<NovaPoshtaPoint> ShippingPoints { get; set; }

        public NovaPoshtaOptions()
        {
        }

        public NovaPoshtaOptions(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            IsAvailablePaymentCashOnDelivery = true;
        }

        public override string TemplateName
        {
            get { return "SdekOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as NovaPoshtaOptions;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                // this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }
        public override OrderPickPoint GetOrderPickPoint()
        {
            if (SelectedPoint == null) return null;
            var temp = new OrderPickPoint
            {
                PickPointId = SelectedPoint.Id,
                PickPointAddress = SelectedPoint.Address,
                //AdditionalData = JsonConvert.SerializeObject(CalculateOption)
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
