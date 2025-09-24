using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Shipping.Pec
{
    public class PecOption : BaseShippingOption
    {
        public float BasePrice { get; set; }
        public float PriceCash { get; set; }

        public List<PecPoint> ShippingPoints { get; set; }
        public PecCalculateOption CalculateOption { get; set; }

        public PecOption()
        {
        }

        public PecOption(ShippingMethod method, float preCost)
            : base(method, preCost)
        {
        }

        public override string TemplateName
        {
            get { return "PecOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as PecOption;

            if (opt != null && opt.Id == this.Id && opt.ShippingPoints != null)
            {
                this.SelectedPoint = opt.SelectedPoint != null && this.ShippingPoints != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints?.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = SelectedPoint != null ? SelectedPoint.Id : string.Empty,
                PickPointAddress = SelectedPoint != null ? SelectedPoint.Address : string.Empty,
                AdditionalData = JsonConvert.SerializeObject(CalculateOption)
            };
        }

        public override bool ApplyPay(BasePaymentOption payOption)
        {
            if (payOption?.GetDetails()?.IsCashOnDeliveryPayment is true)
                Rate = PriceCash;
            else
                Rate = BasePrice;
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
