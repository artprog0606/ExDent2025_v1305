using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop.Payment;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.DDelivery
{
    public class DDeliveryPointOption : BaseShippingOption
    {
        public DDeliveryPointOption() { }

        public DDeliveryPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentCashOnDelivery = true;
        }

        public DDeliveryPoint SelectedPoint { get; set; }

        public List<DDeliveryPoint> ShippingPoints { get; set; }

        public int DeliveryCompanyId { get; set; }
        public int DeliveryTypeId { get; set; }

        public override string TemplateName
        {
            get { return "DDeliveryOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as DDeliveryPointOption;
            if (opt != null && opt.Id == this.Id)
            {
                this.SelectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                var selectedPoint = this.SelectedPoint;
                if (selectedPoint != null)
                {
                    this.Rate = selectedPoint.Rate;
                    this.DeliveryCompanyId = selectedPoint.DeliveryCompanyId;
                    this.DeliveryTypeId = selectedPoint.DeliveryTypeId;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint?.Id,
                PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : string.Empty,
                AdditionalData = SelectedPoint != null ? JsonConvert.SerializeObject(SelectedPoint) : null
            };
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.AdditionalData.IsNotEmpty())
                SelectedPoint = JsonConvert.DeserializeObject<DDeliveryPoint>(orderPickPoint.AdditionalData);
        }
    }
}