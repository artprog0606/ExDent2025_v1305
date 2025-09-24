using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Shipping.Grastin;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Shipping.Grastin
{
    public class GrastinPointOption : BaseShippingOption
    {
        public GrastinEventWidgetData PickpointAdditionalData { get; set; }

        public GrastinPointOption() { }
        public GrastinPointOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
            HideAddressBlock = true;
            IsAvailablePaymentPickPoint = true;
        }
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public List<GrastinPoint> ShippingPoints { get; set; }

        public override string TemplateName
        {
            get { return "GrastinOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as GrastinPointOption;
            if (opt != null && opt.Id == this.Id)
            {
                var selectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = selectedPoint;
                if (selectedPoint != null)
                {
                    PickpointAdditionalData.PickPointId = selectedPoint.Id;
                    PickpointAdditionalData.Partner = selectedPoint.TypePoint;
                }
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint != null ? SelectedPoint.Id : string.Empty,
                PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : string.Empty,
                AdditionalData = JsonConvert.SerializeObject(PickpointAdditionalData)
            };
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsPickPointPayment is true)
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
                SelectedPoint = ShippingPoints?.FirstOrDefault(x => x.Id == orderPickPoint.PickPointId);
        }
    }
}
